import {useCallback, useEffect, useMemo, useRef, useState} from "react";
import {useTranslation} from "react-i18next";
import Skeleton from "react-loading-skeleton";
import {useLocation} from "react-router-dom";
import {AgGridReact} from "ag-grid-react";
import {motion} from "framer-motion";

import {CustomButtonIconText} from "./Buttons/CustomButtonIconText";
import {TextInput} from "./formInputs/TextInput";

import "react-loading-skeleton/dist/skeleton.css";
import "ag-grid-community/styles/ag-grid.css";
import "ag-grid-community/styles/ag-theme-quartz.css";
import "ag-grid-community/styles/ag-theme-alpine.css";
import {AG_GRID_STRINGS} from "@/utils/i18n/tableLocalisation";
import {AuthStore} from "@/utils/stores/Auth";

export const TableComponent = ({
    data: rowData,
    columnDefs,
    extraStyle,
    isLoading,
    theme = "quartz",
    hideFilters,
    sortable = true,
    pinnedTopRowData,
    tableHeight = 0.62,
    loadingRows = 10,
    globalQuickFilter = true,
    rowHeight = 38,
    // AGREGA UNA PROP PARA IDENTIFICAR LA TABLA:
    tableId = null,
    //Función para descargar datos tabla
    onDownload,
    ...rest
}) => {
    // Usamos useRef para almacenar la API del grid
    const gridRef = useRef();
    const {t, i18n} = useTranslation();
    const [pageSize, setPageSize] = useState(10); // Tamaño de página predeterminado
    const headerHeightValue = 50;
    const rowHeightValue = rowHeight;
    const [searchText, setSearchText] = useState("");
    const marginGlobalSearch = theme == "quartz-dark" ? "15px" : "5px";

    // Función para calcular el número de filas que caben en la pantalla disponible
    const calculatePageSize = useCallback(() => {
        const rowHeight = rowHeightValue; // Altura estimada por fila (ajustar según tu configuración)
        const headerHeight = headerHeightValue; // Altura de la cabecera
        const totalHeight = window.innerHeight * tableHeight; // 65vh de la tabla
        const availableHeight = totalHeight - headerHeight;
        let rowsThatFit = Math.floor(availableHeight / rowHeight);
        if (pinnedTopRowData != undefined) {
            rowsThatFit -= 1; //Para los casos que tengan titulo la tabla, le tengo que restar una unidad más para que no salga scroll
        }
        return rowsThatFit - 1;
    }, []);

    // Recalcula el tamaño de la página en la primera carga y al cambiar el tamaño de la ventana
    useEffect(() => {
        const updatePageSize = () => {
            const newPageSize = calculatePageSize();
            setPageSize(newPageSize);
        };

        updatePageSize(); // Llamar la primera vez
        window.addEventListener("resize", updatePageSize); // Recalcular al cambiar el tamaño de la ventana

        return () => {
            window.removeEventListener("resize", updatePageSize);
        };
    }, [calculatePageSize]);

    const defaultColDef = useMemo(() => {
        return {
            filter: !hideFilters ? "agTextColumnFilter" : true,

            // suppressHeaderMenuButton: false,
            wrapHeaderText: false,
            autoHeaderHeight: true,
            resizable: false,
            sortable: sortable,
            suppressMovable: false,
            wrapText: false,
            autoHeight: true,
            minWidth: 60,
            editable: false,
            tooltipValueGetter: (p) => p.value,
        };
    }, [hideFilters, sortable]);

    const actualColumnDefs = isLoading
        ? columnDefs.map((column) => ({
              ...column,
              cellRenderer: Skeleton,
              tooltipValueGetter: null,
              tooltipField: null,
          }))
        : columnDefs;

    const actualRowData = isLoading
        ? Array.from({length: loadingRows}, () => columnDefs.reduce((acc, {field}) => ({...acc, [field]: "-"}), {}))
        : rowData;

    const [destroyed, setDestroyed] = useState(false);

    useEffect(() => {
        setDestroyed(true);
        gridRef.current = null; // Reiniciamos la referencia al API
        setTimeout(() => setDestroyed(false), 100);
    }, [i18n.language]);

    // -- FILTROS VIA QUERY PARAMS --
    const location = useLocation();
    const queryParams = new URLSearchParams(location.search);
    const filters = {};
    queryParams.forEach((value, key) => {
        filters[key] = value;
    });

    // ------------------------------------------------------------------------
    // FUNCIONES PARA GUARDAR Y RESTAURAR ESTADO (FILTROS, SORT) EN LOCALSTORAGE
    // ------------------------------------------------------------------------

    const restoreGridState = useCallback(
        (api) => {
            if (!tableId) {
                return;
            }
            if (!api) {
                return;
            }
            // Leer estado guardado en localStorage (AuthStore)
            const savedState = AuthStore.getSetting(`tableState_${tableId}`);

            if (savedState) {
                const {filterModel, sortModel} = savedState;
                // Aplica los filtros guardados
                api.setFilterModel(filterModel);
                // Aplica la ordenación guardada
                api.applyColumnState({state: sortModel, applyOrder: true});
            }

            const savedGlobalFilter = AuthStore.getSetting(`globalFilterText_${tableId}`);
            if (savedGlobalFilter && gridRef.current) {
                setSearchText(savedGlobalFilter); // Establece el texto en el estado
                api.setGridOption("quickFilterText", savedGlobalFilter);
            }
        },
        [tableId],
    );

    const saveGridState = useCallback(
        (api) => {
            if (!tableId) {
                return;
            }
            if (!api) {
                return;
            }
            const filterModel = api.getFilterModel();
            // getColumnState() retorna array con la config de cada columna, incluyendo 'sort'
            const columnState = api.getColumnState();
            // filtra las columnas que tengan una propiedad de sort para no "ensuciar" la info
            const sortModel = columnState.filter((col) => col.sort);

            // Guarda el objeto en localStorage (via AuthStore)
            AuthStore.updateSetting(`tableState_${tableId}`, {
                filterModel,
                sortModel,
            });
        },
        [tableId],
    );

    // Aplica los filtros iniciales
    const onGridReady = useCallback((params) => {
        // 1) Restaurar estado previo desde localStorage
        restoreGridState(params.api);

        if (Object.keys(filters).length > 0) {
            Object.keys(filters).forEach((key) => {
                params.api.getFilterInstance(key, function (filterInstance) {
                    filterInstance.setModel({
                        type: "contains",
                        filter: filters[key],
                    });
                    params.api.onFilterChanged();
                });
            });
        }

        gridRef.current = params.api;
    }, []);

    // Cada vez que cambien los filtros o se ordene, guardamos en localStorage
    // Callback que se ejecuta cada vez que cambian los filtros internos de ag‑grid
    // Se comprueba el número de filas mostradas; si es 0, se muestra el overlay de “No Rows”
    const onFilterChanged = useCallback(
        (params) => {
            saveGridState(params.api);
            if (gridRef.current) {
                if (gridRef.current.getDisplayedRowCount() === 0) {
                    gridRef.current.showNoRowsOverlay();
                } else {
                    gridRef.current.hideOverlay();
                }
            }
        },
        [saveGridState],
    );

    const onSortChanged = useCallback(
        (params) => {
            saveGridState(params.api);
        },
        [saveGridState],
    );

    // Función para aplicar el filtro a todas las columnas
    const onFilterTextBoxChanged = useCallback((event) => {
        const value = event.target.value;
        setSearchText(value);

        if (tableId) {
            AuthStore.updateSetting(`globalFilterText_${tableId}`, value);
        }

        if (gridRef.current) {
            gridRef.current.setGridOption("quickFilterText", value);
        } else {
            console.error("Error: gridRef.current no está disponible");
        }
    }, []);

    // Función para limpiar (desactivar) todos los filtros
    const clearFilters = useCallback(() => {
        if (gridRef.current) {
            gridRef.current.setFilterModel(null);
            gridRef.current.onFilterChanged();
            // 2. Limpia la ordenación de las columnas
            gridRef.current.applyColumnState({
                state: gridRef.current.getColumnState().map((col) => ({
                    colId: col.colId,
                    sort: null, // Elimina cualquier orden aplicado
                })),
                applyOrder: true,
            });

            // 3. Resetea el filtro global
            gridRef.current.setGridOption("quickFilterText", "");
            setSearchText(""); // Limpiar el estado del input

            if (tableId) {
                AuthStore.updateSetting(`tableState_${tableId}`, {
                    filterModel: null,
                    sortModel: [],
                });
                AuthStore.updateSetting(`globalFilterText_${tableId}`, ""); // También limpia el filtro global
            }
        } else {
            if (tableId) {
                AuthStore.updateSetting(`tableState_${tableId}`, {
                    filterModel: null,
                    sortModel: [],
                });
            }
            AuthStore.updateSetting(`globalFilterText_${tableId}`, ""); // También limpia el filtro global
        }
    }, [tableId]);

    return (
        <motion.div
            className={`ag-theme-${theme}`}
            style={{...extraStyle}}
            initial={{y: 0, opacity: 0}}
            animate={{y: 0, opacity: 1}}
            transition={{duration: 0.2, delay: 0}}
            key={isLoading} // Permite re-renderizar y animar en cada carga
        >
            {/* Buscador */}
            {/* Input de búsqueda: se deshabilita hasta que la grid API esté lista */}

            <div className="d-flex justify-content-end gap-3" style={{marginBottom: `${marginGlobalSearch}`}}>
                {onDownload && (
                    <div>
                        <CustomButtonIconText
                            titulo={t("Exportar datos a excel")}
                            texto={t("")}
                            icono="fa-solid fa-download"
                            variant="success"
                            onClick={onDownload}
                            disabled={isLoading}
                            className="pt-2"
                        />
                    </div>
                )}
                {globalQuickFilter && (
                    <>
                        <div>
                            <CustomButtonIconText
                                titulo={t("Eliminar los filtros de las columnas")}
                                texto={t("")}
                                icono="fa-solid fa-filter-circle-xmark"
                                variant="danger"
                                onClick={clearFilters}
                                disabled={isLoading}
                                className="pt-2"
                            />
                        </div>

                        <TextInput
                            className="tableQuickFilter"
                            type="text"
                            placeholder={t("Buscar...")}
                            value={searchText}
                            onChange={onFilterTextBoxChanged}
                            disabled={isLoading} // Deshabilita hasta que la API esté lista
                            md={"none"}
                        />
                    </>
                )}

                {/* Botón para limpiar todos los filtros */}
            </div>

            <AgGridReact
                ref={gridRef}
                rowData={actualRowData}
                columnDefs={actualColumnDefs}
                pinnedTopRowData={pinnedTopRowData}
                defaultColDef={defaultColDef}
                rowSelection="multiple"
                suppressRowClickSelection={true}
                pagination={true}
                paginationPageSize={pageSize > 13 ? pageSize : pageSize}
                paginationPageSizeSelector={[pageSize > 13 ? pageSize : pageSize, 20, 50, 200]}
                suppressDragLeaveHidesColumns={false}
                tooltipShowDelay={300} // milliseconds
                readOnlyEdit={true}
                tooltipInteraction={true}
                headerHeight={headerHeightValue}
                rowHeight={rowHeightValue}
                onGridReady={onGridReady}
                onFilterChanged={onFilterChanged}
                onSortChanged={onSortChanged}
                // Aseguramos que se use el modelo de filas del cliente
                rowModelType="clientSide"
                getLocaleText={({key}) => t(AG_GRID_STRINGS[key])}
                suppressHorizontalScroll={true} // Deshabilitar el scroll horizontal
                domLayout="autoHeight" // Altura automática según contenido
                floatingFilter={false}
                filter={true}
                // floatingFiltersHeight={20}
                {...rest}
            />
        </motion.div>
    );
};
