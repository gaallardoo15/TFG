import {useEffect, useState} from "react";
import {Badge, ButtonGroup, Dropdown} from "react-bootstrap";
import DatePicker from "react-datepicker";
import {useTranslation} from "react-i18next";
import {toast} from "react-toastify";
import {es} from "date-fns/locale";

import {Icono} from "../common/Icono";

import "react-datepicker/dist/react-datepicker.css";
import {CustomButtonIconText} from "@/components/common/Buttons/CustomButtonIconText";
import {DatatableSelectFilter} from "@/components/common/DatatableSelectFilter";
import {SelectInput} from "@/components/common/formInputs/SelectInput";
import {OptimisticProgressBar} from "@/components/common/loading/OptimisticProgressBar";
import {TableComponent} from "@/components/common/TableComponent";
import {useFileDownloader} from "@/hooks/useFileDownloader";
import {activosService} from "@/services/ActivoService";
import {informesService} from "@/services/InformesService";

export const InformeOrdenesPage = () => {
    const defaultData = {
        idActivo: 0,
        fechaDesde: "",
        fechaHasta: "",
    };

    const {t} = useTranslation();
    const {isLoading: isLoadingInforme, fileDownload} = useFileDownloader();
    const [formData, setFormData] = useState(defaultData);
    const [activos, setActivos] = useState([]);
    const [ordenes, setOrdenes] = useState([]);
    const [arrayActivos, setArrayActivos] = useState([]);
    const [filteredActivos, setFilteredActivos] = useState([]);
    const [filteredOrdenes, setFilteredOrdenes] = useState([]);
    const [filteredIncidencias, setFilteredIncidencias] = useState([]);
    const [ordenesUnicas, setOrdenesUnicas] = useState([]);
    const [selectedOrderId, setSelectedOrderId] = useState(null);
    const [isLoading, setIsLoading] = useState(true);

    const fetchActivos = () => {
        activosService.fetchAll().then((response) => {
            if (!response.is_error) {
                const filteredActivos = response.content.filter((activo) => activo.estadoActivo.name == "Activo");
                setActivos(filteredActivos);
            } else {
                console.error(response.error_content);
                return [];
            }
        });
    };
    const formatLocalDate = (date) => {
        if (!date) {
            return "";
        }
        const year = date.getFullYear();
        const month = String(date.getMonth() + 1).padStart(2, "0");
        const day = String(date.getDate()).padStart(2, "0");
        return `${year}-${month}-${day}`;
    };

    const fetchData = () => {
        setIsLoading(true);

        const params = new URLSearchParams({
            idActivo: formData.idActivo,
            fechaDesde: formData.fechaDesde ? formatLocalDate(formData.fechaDesde) : "",
            fechaHasta: formData.fechaHasta ? formatLocalDate(formData.fechaHasta) : "",
        });

        informesService.getInforme(params).then((response) => {
            if (!response.is_error) {
                if (response.content.length == 0) {
                    toast.error("No se encontraron órdenes para los filtros seleccionados");
                    setIsLoading(false);
                    return;
                }
                setOrdenes(response.content || []);
            } else {
                console.error(response.error_content);
                setIsLoading(false);
            }
        });
    };

    useEffect(() => {
        fetchActivos();
    }, []);

    useEffect(() => {
        fetchData();
    }, [formData]);

    useEffect(() => {
        setFilteredOrdenes(ordenes);
    }, [ordenes]);

    useEffect(() => {
        if (filteredOrdenes.length > 0) {
            setIsLoading(true);
            let array = [];

            array = filteredOrdenes.filter(
                (value, index, self) => index === self.findIndex((t) => t.idActivo === value.idActivo),
            );

            setArrayActivos(array);
            setFilteredActivos(array);

            // Filtramos las órdenes únicas por IdOrden
            const ordenesUnicas = filteredOrdenes.filter(
                (value, index, self) => index === self.findIndex((t) => t.id === value.id),
            );
            setOrdenesUnicas(ordenesUnicas);
            setFilteredIncidencias(filteredOrdenes);
            setIsLoading(false);
        } else {
            setArrayActivos([]);
            setFilteredActivos([]);
            setOrdenesUnicas([]);
            setFilteredIncidencias([]);
        }

        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [filteredOrdenes]);

    const handleClickRowTable = (event) => {
        if (selectedOrderId != event.data.id) {
            setSelectedOrderId(event.data.id);

            const incidenciasFiltradas = filteredOrdenes.filter((orden) => orden.id == event.data.id);
            setFilteredIncidencias(incidenciasFiltradas);
            setFilteredActivos(arrayActivos.filter((activo) => activo.idActivo == event.data.idActivo));
        } else {
            setFilteredOrdenes(ordenes);
            setFilteredIncidencias(ordenes);
            setFilteredActivos(arrayActivos);
            setSelectedOrderId(null);
        }
        // Guarda el id de la fila seleccionada
    };

    const handleInputChange = (event) => {
        const {name, value, type, checked} = event.target;

        let newValue = {
            ...formData,
            [name]: type === "checkbox" ? checked : value,
        };

        setFormData(newValue);
        // setErrors({});
        filterOrdenes(newValue);
    };

    const handleDeleteFilters = () => {
        if (formData.idActivo != 0 || formData.fechaDesde != "" || formData.fechaHasta != "") {
            setFormData({
                idActivo: 0,
                fechaDesde: "",
                fechaHasta: "",
            });
            setFilteredOrdenes(ordenes);
            setFilteredIncidencias(ordenes);
            setFilteredActivos(arrayActivos);
        } else {
            setFilteredOrdenes(ordenes);
            setFilteredIncidencias(ordenes);
            setFilteredActivos(arrayActivos);
        }
        setSelectedOrderId(null);
    };

    const descargarInforme = async (antiguo) => {
        const params = new URLSearchParams({
            idActivo: formData.idActivo,
            fechaDesde: formData.fechaDesde ? formatLocalDate(formData.fechaDesde) : "",
            fechaHasta: formData.fechaHasta ? formatLocalDate(formData.fechaHasta) : "",
            antiguo,
        });

        toast.info(
            t(
                "La exportación del informe se está realizando en base a los filtros: fecha desde, fecha hasta y activo.",
            ),
            {
                position: "top-right",
                autoClose: 10000,
                closeButton: true,
            },
        );

        const error = await fileDownload(`/api/informes/descargar?${params.toString()}`, "InformeOrdenes");
        if (error != undefined) {
            toast.error(`${error}`);
        }
    };

    // Función para filtrar las órdenes
    const filterOrdenes = (filters) => {
        const {idActivo, fechaDesde, fechaHasta} = filters;

        const filtered = ordenes.filter((orden) => {
            // Filtro dinámico: solo aplica el filtro si el valor de la propiedad no es el de `defaultDat
            const matchesIdActivo = idActivo === null || orden.idAtivo === idActivo;
            const matchesFechaDesde =
                fechaDesde === "" || new Date(FormatearFecha(orden.fechaApertura)) >= new Date(fechaDesde);
            const matchesFechaHasta =
                fechaHasta === "" || new Date(FormatearFecha(orden.fechaApertura)) <= new Date(fechaHasta);

            return matchesIdActivo && matchesFechaDesde && matchesFechaHasta;
        });
        setFilteredOrdenes(filtered); // Actualiza el estado filtrado
    };

    const FormatearFecha = (fecha) => {
        if (fecha) {
            const [dia, mes, year] = fecha.split("/");

            return `${year}-${mes}-${dia}`;
        }
        return;
    };
    const FormatearFechaTablas = (fecha) => {
        if (fecha) {
            // Separar la fecha y la hora
            const partesFecha = fecha.split("T");

            const date = partesFecha[0];
            const hora = partesFecha[1];
            // Convertir la fecha al formato yyyy/mm/dd
            const [year, month, day] = date.split("-");
            const fechaFormateada = `${day}-${month}-${year} ${hora}`;
            return fechaFormateada;
        }
        return;
    };

    const getUniqueUsuarios = () => {
        // Obtener un array con todos los usuarios de cada objeto
        const usuariosArray = ordenesUnicas.flatMap((obj) => obj.usuarios || []);
        const uniqueUsuarios = new Set();

        usuariosArray.forEach((row) => {
            if (row) {
                const usuariosSplit = row.split(", ");
                usuariosSplit.forEach((user) => {
                    uniqueUsuarios.add(user);
                });
            }
        });

        return Array.from(uniqueUsuarios);
    };

    return (
        <div className="d-flex flex-column gap-2">
            <div id="toolbarInformesOrdenes" className="d-flex justify-content-between align-items-end w-100">
                <div id="filtrosInformesOrdenes" className="d-flex gap-3 align-items-end">
                    <div className="d-flex gap-2 w-100" id="contenedorFiltrosFechas">
                        <div className="">
                            <label className="fw-semibold form-label">{t("Desde")}</label>
                            <DatePicker
                                selected={formData.fechaDesde}
                                onChange={(date) => setFormData((prev) => ({...prev, fechaDesde: date}))}
                                dateFormat="dd-MM-yyyy"
                                locale={es} // Aquí se establece el idioma español
                                disabled={isLoading}
                                className="form-control"
                                placeholderText="dd-mm-aaaa"
                            />
                        </div>
                        <div className="">
                            <label className="fw-semibold form-label">{t("Hasta")}</label>
                            <DatePicker
                                selected={formData.fechaHasta}
                                onChange={(date) => setFormData((prev) => ({...prev, fechaHasta: date}))}
                                dateFormat="dd-MM-yyyy"
                                locale={es} // Aquí se establece el idioma español
                                disabled={isLoading}
                                className="form-control"
                                placeholderText="dd-mm-aaaa"
                            />
                        </div>
                    </div>
                    <div id="contenedorSelectActivoInformeOrdenes" className="ps-3 w-100">
                        <SelectInput
                            id="selectActivoInformeOrdenes"
                            key={formData.idActivo}
                            label={t("Activo")}
                            name="idActivo"
                            value={formData.idActivo}
                            onChange={handleInputChange}
                            options={activos.map((a) => ({
                                value: a.id,
                                label: a.id + " - " + t(a.descripcionES),
                            }))}
                            disabled={isLoading}
                            className="mb-0"
                        />
                    </div>
                    <CustomButtonIconText
                        id="btnEliminarFiltrosInformeOrdenes"
                        titulo={t("Eliminar Filtros")}
                        icono="fa-solid fa-filter-circle-xmark"
                        variant="danger"
                        className="h-100"
                        onClick={handleDeleteFilters}
                    />
                </div>
                <div id="contenedorBtnDescargarInformeOrdenes" className="d-flex gap-3 h-50">
                    <CustomButtonIconText
                        id="btnDescargarInformeOrdenes"
                        titulo={t("Descargar Informe")}
                        texto={t("Descargar Informe")}
                        icono="fa-solid fa-download"
                        variant="success"
                        onClick={() => descargarInforme(false)}
                        disabled={isLoadingInforme || ordenes.length == 0}
                        isLoading={isLoadingInforme}
                    />
                </div>
            </div>
            <div>
                {isLoadingInforme && (
                    <OptimisticProgressBar
                        isLoading={isLoadingInforme}
                        estimatedDuration={formData.idActivo > 0 ? 3000 : 8000}
                        isUploadComplete={!isLoadingInforme}
                    />
                )}
            </div>
            {/*Tabla Datos Activo */}
            <div className="mt-0" id="tablaActivoInformeOrdenes">
                <TableComponent
                    isLoading={isLoading}
                    data={filteredActivos}
                    pagination={filteredActivos.length > 1}
                    tableHeight={0.3}
                    loadingRows={2}
                    tableId="tablaActivosInforme"
                    theme={"quartz-dark"}
                    columnDefs={[
                        {
                            field: "idActivo",
                            headerName: t("ID Activo"),
                            flex: 1,
                            minWidth: 80,
                            editable: false,
                        },
                        {
                            field: "descripcionES",
                            headerName: t("Descripción"),
                            flex: 7,
                            minWidth: 225,
                            valueGetter: (e) => e.data?.activo || "-",
                            editable: false,
                        },
                        {
                            field: "criticidad",
                            headerName: t("Criticidad"),
                            flex: 2,
                            minWidth: 100,
                            valueGetter: (e) => e.data?.criticidad || "SC",
                            editable: false,
                            filter: DatatableSelectFilter,
                        },
                    ]}
                />
            </div>
            {/* Tabla Datos principales Órdenes */}
            <div className="contenedorPlantillaComponente mt-1" id="tablaOrdenesInformeIncidenciasOrdenes">
                <TableComponent
                    isLoading={isLoading}
                    data={ordenesUnicas}
                    pagination={true}
                    tableHeight={0.4}
                    tableId="tablaOrdenesInforme"
                    onRowClicked={handleClickRowTable}
                    getRowClass={(params) => (params.data.id === selectedOrderId ? "selected-row" : "")}
                    columnDefs={[
                        {
                            field: "id",
                            headerName: t("ID Orden"),
                            flex: 1.5,
                            minWidth: 90,
                            editable: false,
                            cellRenderer: (e) => {
                                return (
                                    <Badge
                                        pill
                                        style={{
                                            backgroundColor: `var(--primario)`,
                                            letterSpacing: "0.5px",
                                            padding: "4px 12px 4px 12px",
                                        }}>
                                        {" "}
                                        {t(e.value)}
                                    </Badge>
                                );
                            },
                        },
                        {
                            field: "tipoOrden",
                            headerName: t("Tipo Orden"),
                            flex: 1,
                            minWidth: 90,
                            valueGetter: (e) => e.data?.tipoOrden || "-",
                            editable: false,
                            filter: DatatableSelectFilter,
                        },
                        {
                            field: "fechaApertura",
                            headerName: t("Fecha Inicio"),
                            valueGetter: (e) => e.data?.fechaApertura,
                            valueFormatter: (e) => FormatearFechaTablas(e.data?.fechaApertura),
                            flex: 2,
                            minWidth: 100,
                            editable: false,
                        },
                        {
                            field: "fechaCierre",
                            headerName: t("Fecha Cierre"),
                            flex: 2,
                            minWidth: 100,
                            valueGetter: (e) => e.data?.fechaCierre || "",
                            valueFormatter: (e) => FormatearFechaTablas(e.data?.fechaCierre) || "-",
                            editable: false,
                        },
                        {
                            field: "estadoOrden",
                            headerName: t("Estado"),
                            flex: 2,
                            minWidth: 150,
                            valueGetter: (e) => e.data?.estadoOrden || "-",
                            cellRenderer: (e) => {
                                let badgeColor = "warning"; // Color por defecto

                                // Asigna colores según el estado
                                if (e.value === "Abierta") {
                                    badgeColor = "var(--estadoAbierta)";
                                } else if (e.value === "Cerrada") {
                                    badgeColor = "var(--estadoCerrada)"; // Puedes ajustar a cualquier color de tu preferencia
                                } else if (
                                    e.value == "Cerrada: Pendiente Material" ||
                                    e.value == "Abierta: Pendiente Material"
                                ) {
                                    badgeColor = "var(--estadoEsperandoMaterial)";
                                } else if (
                                    e.value == "Abierta: Material Gestionado" ||
                                    e.value == "Cerrada: Material Gestionado"
                                ) {
                                    badgeColor = "var(--estadoMaterialGestionado)"; // Puedes ajustar a cualquier color de tu preferencia
                                } else if (e.value == "Anulada") {
                                    badgeColor = "var(--estadoAnulada)";
                                } else if (e.value == "En Curso") {
                                    badgeColor = "var(--estadoEnCurso)";
                                }

                                return (
                                    <Badge
                                        pill
                                        style={{
                                            backgroundColor: `${badgeColor} !important`,
                                            letterSpacing: "0.5px",
                                            padding: "4px 12px 4px 12px",
                                        }}>
                                        {" "}
                                        {t(e.value)}
                                    </Badge>
                                );
                            },
                            wrapText: true,
                            autoHeight: true,
                            editable: false,
                            filter: DatatableSelectFilter,
                        },
                        {
                            field: "usuarios",
                            headerName: t("Usuarios Asignados"),
                            flex: 2.5,
                            minWidth: 150,
                            valueGetter: (e) => e.data?.usuarios || "-",
                            editable: false,
                            filter: DatatableSelectFilter,
                            filterParams: {customUniqueValues: getUniqueUsuarios()},
                        },
                    ]}
                />
            </div>
            {/*Tabla Datos Incidencias */}
            <div className="contenedorPlantillaComponente mt-0">
                <TableComponent
                    isLoading={isLoading}
                    data={filteredIncidencias}
                    tableId="tablaIncidenciasInforme"
                    tableHeight={0.4}
                    columnDefs={[
                        {
                            field: "id",
                            headerName: t("ID Orden"),
                            flex: 1,
                            minWidth: 100,
                            editable: false,
                            cellRenderer: (e) => {
                                return (
                                    <Badge
                                        pill
                                        style={{
                                            backgroundColor: `var(--primario)`,
                                            letterSpacing: "0.5px",
                                            padding: "4px 12px 4px 12px",
                                        }}>
                                        {" "}
                                        {t(e.value)}
                                    </Badge>
                                );
                            },
                        },
                        {
                            field: "kksComponente",
                            headerName: t("Denominación"),
                            flex: 6,
                            minWidth: 250,
                            valueGetter: (e) => e.data?.kksComponente + " - " + e.data?.componente || "-",
                            editable: false,
                        },
                        {
                            field: "mecanismoDeFallo",
                            headerName: t("Tipo de Incidencia"),
                            flex: 4,
                            minWidth: 150,
                            valueGetter: (e) => e.data?.mecanismoDeFallo || "-",
                            editable: false,
                            filter: DatatableSelectFilter,
                        },
                        {
                            field: "incidencia",
                            headerName: t("Incidencia"),
                            flex: 3,
                            minWidth: 120,
                            valueGetter: (e) => e.data?.incidencia || "-",
                            editable: false,
                            filter: DatatableSelectFilter,
                        },
                        {
                            field: "resolucion",
                            headerName: t("Resolución"),
                            flex: 3,
                            minWidth: 120,
                            valueFormatter: (e) => e.data?.resolucion || "-",
                            editable: false,
                            filter: DatatableSelectFilter,
                        },
                        {
                            field: "fechaDeteccion",
                            headerName: t("Fecha Detección"),
                            flex: 4,
                            minWidth: 150,
                            valueGetter: (e) => e.data?.fechaDeteccion,
                            valueFormatter: (e) => FormatearFechaTablas(e.data?.fechaDeteccion),
                            editable: false,
                        },
                        {
                            field: "fechaResolucion",
                            headerName: t("Fecha Resolución"),
                            flex: 4,
                            minWidth: 150,
                            valueGetter: (e) => e.data?.fechaResolucion || "",
                            valueFormatter: (e) => FormatearFechaTablas(e.data?.fechaResolucion) || "-",
                            editable: false,
                        },
                    ]}
                />
            </div>
        </div>
    );
};
