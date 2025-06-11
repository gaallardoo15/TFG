import {useEffect, useState} from "react";
import {Badge, Button, Dropdown} from "react-bootstrap";
import {useTranslation} from "react-i18next";
import {useNavigate} from "react-router-dom";

import {EditOrdenFormModal} from "./EditOrdenFormModal";
import {GraficoAreaOrdenesDiarias} from "./GraficoAreaOrdenesDiarias";
import {GraficoCircularSemaforo} from "./GraficoCircularSemaforo";
import {OrdenHistoryModal} from "./OrdenHistoryModal";

import {RoutePaths} from "@/components/App";
import {BtnCrearElemento} from "@/components/common/Buttons/BtnCrearElemento";
import {BtnTabla} from "@/components/common/Buttons/BtnTabla";
import {CeldaBotonesTabla} from "@/components/common/CeldaBotonesTabla";
import {DatatableSelectFilter} from "@/components/common/DatatableSelectFilter";
import {DropdownMenuCheckboxes} from "@/components/common/Dropdown/DropdownMenuCheckboxes";
import {DropdownMultiFiltro} from "@/components/common/Dropdown/DropdownMultiFiltro";
import {DropdownSubMenu} from "@/components/common/Dropdown/DropdownSubMenu";
import {DateInput} from "@/components/common/formInputs/DateInput";
import {TableComponent} from "@/components/common/TableComponent";
import {useModalState} from "@/hooks/useModalState";
import {AuthService} from "@/services/AuthService";
import {ordenService} from "@/services/OrdenService";
import {panelGeneralService} from "@/services/PanelGeneralService";

const limitesSemaforoTiposOrdenes = [
    [5, 20],
    [10, 30],
    [5, 20],
    [5, 20],
];
const limitesSemaforoEstados = [
    [6, 10],
    [11, 20],
    [11, 20],
    [50, 79],
];
const revertDirectionsSemaforoEstados = [false, false, false, true];

const itemsDefault = {
    table: [],
    grafica: [],
    semaforoEstados: [],
    semaforoTipos: [],
};

const ObtenerSeries = (datos) => {
    // Calcular el total de órdenes
    const totalOrdenes = datos.reduce((total, item) => total + item.nOrdenesOpcion, 0);

    // Calcular el ángulo de cada tipo de orden en función de sus órdenes
    const series = datos.map((item) => {
        const proporcion = item.nOrdenesOpcion / totalOrdenes;
        return Math.round(proporcion * 270); // Calcula el ángulo basado en 270°
    });

    return series;
    //Obtener el total
};
const FormatearFecha = (fecha) => {
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

export const PanelGeneralPage = () => {
    const defaultFilters = {
        years: [new Date().getFullYear()],
        tiposOrdenes: [],
        criticidades: [],
        fechaDesde: "",
        fechaHasta: "",
    };

    const fetchPanelGeneral = () => {
        setIsLoading(true);

        // Función para transformar los arrays
        const transformFilter = (filterArray) => {
            return filterArray.length === 1 && filterArray[0] === 0 ? [] : filterArray;
        };

        const params = new URLSearchParams({
            years: JSON.stringify(transformFilter(filters.years)), // Serializa arrays como JSON
            tiposOrdenes: JSON.stringify(transformFilter(filters.tiposOrdenes)),
            criticidades: JSON.stringify(transformFilter(filters.criticidades)),
            fechaDesde: filters.fechaDesde,
            fechaHasta: filters.fechaHasta,
        });

        panelGeneralService.getPanelGeneral(params).then((response) => {
            if (!response.is_error) {
                setItems(response.content || {});
            } else {
                console.error(response.error_content);
            }
            setIsLoading(false);
        });
    };

    const {t} = useTranslation();
    const navigate = useNavigate();
    const [filters, setFilters] = useState(defaultFilters);
    const [opcionesFiltros, setOpcionesFiltros] = useState(defaultFilters);
    const [items, setItems] = useState(itemsDefault);
    const [datosGrafica, setDatosGrafica] = useState([]);
    const [seriesGraficaEstados, setSeriesGraficaEstados] = useState([]);
    const [seriesGraficaTiposOrdenes, setSeriesGraficaTiposOrdenes] = useState([]);
    const [discriminantSeriesEstados, setDiscriminantSeriesEstados] = useState([]);
    const [discriminantSeriesTiposOrdenes, setDiscriminantSeriesTiposOrdenes] = useState([]);
    const {modalState, openModal, closeModal} = useModalState(fetchPanelGeneral);
    const [isLoading, setIsLoading] = useState(false);
    const [errors, setErrors] = useState(false);

    const fetchTiposOrdenes = () => {
        ordenService.getTiposOrdenes().then((response) => {
            if (!response.is_error) {
                const tiposOrdenes = response.content || [];
                setOpcionesFiltros((prev) => ({
                    ...prev,
                    tiposOrdenes,
                }));
            } else {
                console.error(response.error_content);
            }
        });
    };
    const fetchCriticidades = () => {
        ordenService.getCriticidades().then((response) => {
            if (!response.is_error) {
                const criticidades = response.content || [];
                setOpcionesFiltros((prev) => ({
                    ...prev,
                    criticidades,
                }));
            } else {
                console.error(response.error_content);
            }
        });
    };
    const fetchYears = () => {
        ordenService.getYears().then((response) => {
            if (!response.is_error) {
                const years = response.content?.reverse() || [];
                setOpcionesFiltros((prev) => ({
                    ...prev,
                    years,
                }));
            } else {
                console.error(response.error_content);
            }
        });
    };

    useEffect(() => {
        fetchPanelGeneral();
        fetchYears();
        fetchTiposOrdenes();
        fetchCriticidades();
    }, []);

    useEffect(() => {
        if (items.grafica) {
            setDatosGrafica(items.grafica);
        }
        if (items.semaforoEstados) {
            // Define el orden deseado
            const ordenPersonalizado = ["En Curso", "Abierta", "Material", "Cerrada"];

            // Función de comparación para el orden personalizado
            items.semaforoEstados.sort((a, b) => {
                return (
                    ordenPersonalizado.indexOf(a.descripcionOpcion) - ordenPersonalizado.indexOf(b.descripcionOpcion)
                );
            });
            //Preparo los datos para las gráficas semáforos:
            setSeriesGraficaTiposOrdenes(ObtenerSeries(items.semaforoTipos));
            setSeriesGraficaEstados(ObtenerSeries(items.semaforoEstados));
            setDiscriminantSeriesTiposOrdenes(items.semaforoTipos.map((objeto) => objeto.nOrdenesOpcion));
            setDiscriminantSeriesEstados(
                items.semaforoEstados
                    .slice(0, -1)
                    .map((objeto) => objeto.nOrdenesOpcion)
                    .concat(items.semaforoEstados.slice(-1).map((objeto) => objeto.porcentajeOpcion)),
            );
        }
    }, [items]);

    const navigateToOrderRegistration = () => {
        navigate(RoutePaths.RegistroOrdenes.path); // Asegúrate de que esta ruta coincida con la definida en tu configuración de rutas
    };

    const handleAction = (actionType, id = null) => {
        let targetOrden = id ? items.table.find((o) => o.id === id) : {};

        openModal(actionType, targetOrden);
    };

    const handleInputChange = (event) => {
        const {name, value, type, checked} = event.target;
        let newValue = {
            ...filters,
            [name]: type === "checkbox" ? checked : value,
        };

        setFilters(newValue);
        setErrors({});
    };

    // Manejador del botón "Aplicar"
    const applyFilters = (event) => {
        event.preventDefault();
        event.stopPropagation();
        fetchPanelGeneral();
    };

    const toggleCheckbox = (name, value) => {
        setFilters((prevFilters) => {
            const currentValues = prevFilters[name]; // Obtén la propiedad actual (array)
            let updatedValues;

            if (value === 0) {
                // Si es "Todos", selecciona solo este y desmarca los demás.
                updatedValues = [0];
            } else {
                // Si no es "Todos", elimina "Todos" y actualiza las opciones seleccionadas.
                updatedValues = currentValues.includes(value)
                    ? currentValues.filter((v) => v !== value) // Elimina si ya está seleccionado
                    : [...currentValues.filter((v) => v !== 0), value]; // Añade valor y elimina "Todos"
            }

            return {...prevFilters, [name]: updatedValues}; // Actualiza solo la propiedad correspondiente
        });
    };

    const ActionsComponent = (props) => {
        const actionByRole = {
            OPERARIO: ["edit", "history"],
            SUPER_ADMINISTRADOR: ["edit", "history"],
            ADMINISTRADOR: ["edit", "history"],
            JEFE_MANTENIMIENTO: ["edit", "history"],
            RESPONSABLE_TALLER: ["edit", "history"],
            RESPONSABLE: ["edit", "history"],
            RESPONSABLE_MATERIALES: ["edit", "history"],
        };
        const availableActions = actionByRole[AuthService.getUserRole()];

        const buttonByAction = {
            edit: (
                <BtnTabla
                    key="edit"
                    icono="fa-solid fa-pen"
                    title={t("Editar Orden")}
                    onClick={() => handleAction("form", props.value)}
                    disabled={isLoading}
                />
            ),
            history: (
                <BtnTabla
                    key="history"
                    icono="fa-solid fa-user-clock"
                    title={t("Ver historial modificaciones orden")}
                    onClick={() => handleAction("history", props.value)}
                    disabled={isLoading}
                    variant="info"
                />
            ),
        };

        return <CeldaBotonesTabla availableActions={availableActions} buttonByAction={buttonByAction} />;
    };

    const getUniqueUsuarios = () => {
        // Obtener un array con todos los usuarios de cada objeto
        const usuariosArray = items.table.flatMap((obj) => obj.usuarios || []);

        const uniqueUsuarios = new Set();

        usuariosArray.forEach((row) => {
            if (row) {
                uniqueUsuarios.add(`${row.nombre} ${row.apellidos}`);
            }
        });

        return Array.from(uniqueUsuarios);
    };
    const getUniqueMecanismos = () => {
        // Obtener un array con todos los usuarios de cada objeto
        const mecanismos = items.table.flatMap((obj) => obj.mecanismosDeFallos || []);

        const uniqueMecanismos = new Set();

        mecanismos.forEach((row) => {
            if (row) {
                uniqueMecanismos.add(`${row.descripcionES}`);
            }
        });

        return Array.from(uniqueMecanismos);
    };
    const getUniqueIncidencias = () => {
        // Obtener un array con todos los usuarios de cada objeto
        const incidencias = items.table.flatMap((obj) => obj.incidencias || []);

        const uniqueIncidencias = new Set();

        incidencias.forEach((row) => {
            if (row) {
                uniqueIncidencias.add(`${row.descripcionES}`);
            }
        });

        return Array.from(uniqueIncidencias);
    };

    return (
        <div className="contenedorPanelGeneral">
            <div className="seccionGraficasPanelGeneral">
                <div className="divGraficaOrdenesDiarias mb-2">
                    <GraficoAreaOrdenesDiarias data={datosGrafica} />
                </div>
                <div className="divGraficasCirculares">
                    <div className="graficaSemaforo">
                        <GraficoCircularSemaforo
                            series={seriesGraficaTiposOrdenes}
                            datos={items.semaforoTipos}
                            discriminantSeries={discriminantSeriesTiposOrdenes}
                            limitesSemaforo={limitesSemaforoTiposOrdenes}
                        />
                    </div>
                    <div className="graficaSemaforo">
                        <GraficoCircularSemaforo
                            series={seriesGraficaEstados}
                            datos={items.semaforoEstados}
                            discriminantSeries={discriminantSeriesEstados}
                            limitesSemaforo={limitesSemaforoEstados}
                            revertDirections={revertDirectionsSemaforoEstados}
                        />
                    </div>
                </div>
            </div>
            <div className="d-flex justify-content-between">
                <div>
                    <BtnCrearElemento
                        customText={t("Registrar Nueva Orden")}
                        className="boton btnCrearElemento"
                        onClick={navigateToOrderRegistration}
                    />
                </div>
                <div>
                    <DropdownMultiFiltro variant="primary" autoclose="outside">
                        <DropdownSubMenu title={t("Años")} autoclose="outside">
                            <DropdownMenuCheckboxes
                                className="scrollable-list-dropdown"
                                opciones={opcionesFiltros.years}
                                checked={(year) => filters.years.includes(year)}
                                onclickItem={(value) => toggleCheckbox("years", value)} // Aquí pasamos el nombre y el valor
                                onclickButton={applyFilters}
                                name="years"
                            />
                        </DropdownSubMenu>
                        <DropdownSubMenu title={t("Tipos de Órdenes")} autoclose="outside">
                            <DropdownMenuCheckboxes
                                opciones={opcionesFiltros.tiposOrdenes}
                                checked={(tipo) => filters.tiposOrdenes.includes(tipo)}
                                onclickItem={(value) => toggleCheckbox("tiposOrdenes", value)}
                                onclickButton={applyFilters}
                                name="tiposOrdenes"
                            />
                        </DropdownSubMenu>
                        <DropdownSubMenu title={t("Criticidades")} autoclose="outside">
                            <DropdownMenuCheckboxes
                                opciones={opcionesFiltros.criticidades}
                                checked={(criticidad) => filters.criticidades.includes(criticidad)}
                                onclickItem={(value) => toggleCheckbox("criticidades", value)}
                                onclickButton={applyFilters}
                                name="criticidades"
                            />
                        </DropdownSubMenu>
                        <DropdownSubMenu title={t("Período")} autoclose="outside">
                            <div>
                                <div className="ms-3 me-3">
                                    <DateInput
                                        label={t("Desde")}
                                        name="fechaDesde"
                                        value={filters.fechaDesde}
                                        onChange={handleInputChange}
                                        className="mb-1"
                                    />
                                    <DateInput
                                        label={t("Hasta")}
                                        name="fechaHasta"
                                        value={filters.fechaHasta}
                                        onChange={handleInputChange}
                                    />
                                </div>

                                <Dropdown.Divider />
                                <Dropdown.Item as="div" className="text-center">
                                    <Button variant="primary" size="sm" className="w-100" onClick={applyFilters}>
                                        Aplicar
                                    </Button>
                                </Dropdown.Item>
                            </div>
                        </DropdownSubMenu>
                    </DropdownMultiFiltro>
                </div>
            </div>
            <div className="contenedorPlantillaComponente mt-0" id="tablaPanelGeneral" style={{minHeight: "48vh"}}>
                <TableComponent
                    isLoading={isLoading}
                    data={items.table}
                    tableHeight={0.41}
                    tableId="tablaPanelGeneral"
                    rowHeight={35}
                    columnDefs={[
                        {
                            field: "id",
                            headerName: t("Id"),
                            flex: 1,
                            minWidth: 80,
                            tooltipValueGetter: (e) => e.data?.comentarioOrden,
                            editable: true,
                            filter: true,
                        },
                        {
                            field: "estado",
                            headerName: t("Estado"),
                            valueGetter: (e) => e.data?.estado?.name,
                            cellRenderer: (e) => {
                                let badgeColor = "warning"; // Color por defecto

                                // Asigna colores según el estado
                                if (e.value === "Abierta") {
                                    badgeColor = "var(--estadoAbierta)";
                                } else if (e.value === "Cerrada") {
                                    badgeColor = "var(--estadoCerrada)"; // Puedes ajustar a cualquier color de tu preferencia
                                } else if (
                                    e.value === "Cerrada: Pendiente Material" ||
                                    e.value === "Abierta: Pendiente Material"
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
                            minWidth: 210,
                            flex: 3,
                            filter: DatatableSelectFilter,
                        },
                        {
                            field: "usuarios",
                            headerName: t("Usuarios"),
                            valueGetter: (e) => {
                                let usuarios = e.data?.usuarios;
                                if (usuarios === "-") {
                                    usuarios = [];
                                }
                                const usuariosConcatenados = usuarios
                                    .map((val) => `${val.nombre} ${val.apellidos}`)
                                    .join(", ");

                                return usuariosConcatenados;
                            },
                            valueFormatter: (e) => {
                                const usuarios = e.data?.usuarios || [];
                                if (usuarios.length === 0) {
                                    return "-";
                                } // Si no hay usuarios, devuelve "-"

                                const primerUsuario = `${usuarios[0]?.nombre} ${usuarios[0]?.apellidos}`;
                                const extraUsuarios = usuarios.length > 1 ? ` (+${usuarios.length - 1})` : "";

                                return primerUsuario + extraUsuarios;
                            },
                            flex: 5,
                            minWidth: 200,
                            editable: true,
                            filter: DatatableSelectFilter,
                            filterParams: {customUniqueValues: getUniqueUsuarios()},
                        },
                        {
                            field: "activo",
                            headerName: t("Activo"),
                            valueGetter: (e) => e.data?.activo?.id + " - " + e.data?.activo?.descripcionES || "-",
                            valueFormatter: (e) => e.data?.activo?.id || "-",
                            tooltipValueGetter: (e) => e.data?.activo?.descripcionES,
                            flex: 1,
                            minWidth: 100,
                            editable: false,
                            filter: DatatableSelectFilter,
                        },
                        {
                            field: "fechaApertura",
                            headerName: t("Fecha Creación"),
                            valueGetter: (e) => e.data?.fechaApertura,
                            valueFormatter: (e) => FormatearFecha(e.data?.fechaApertura),
                            flex: 4,
                            minWidth: 160,
                            editable: false,
                        },
                        {
                            field: "fechaCierre",
                            headerName: t("Fecha Cierre"),
                            valueGetter: (e) => e.data?.fechaCierre || "",
                            valueFormatter: (e) => FormatearFecha(e.data?.fechaCierre) || "-",
                            flex: 4,
                            minWidth: 160,
                        },
                        {
                            field: "mecanismosDeFallos",
                            headerName: t("Tipo de Incidencia"),
                            valueGetter: (e) => {
                                let mecanismos = e.data?.mecanismosDeFallos || [];
                                if (mecanismos === "-") {
                                    mecanismos = [];
                                }

                                // Asegurar que mecanismos siempre sea un array válido
                                if (!Array.isArray(mecanismos)) {
                                    mecanismos = [];
                                }
                                // Filtrar valores únicos
                                const mecanismosUnicos = Array.from(
                                    new Set(mecanismos.map((val) => val.descripcionES)),
                                );

                                return mecanismosUnicos.join(", ");
                            },
                            valueFormatter: (e) => {
                                const mecanismos = e.data?.mecanismosDeFallos || [];
                                if (!Array.isArray(mecanismos) || mecanismos.length === 0) {
                                    return "-"; // Si no hay datos, devuelve "-"
                                }

                                // Filtrar valores únicos
                                const mecanismosUnicos = Array.from(
                                    new Set(mecanismos.map((val) => val.descripcionES)),
                                );

                                const primerMecanismo = mecanismosUnicos[0];
                                const extraMecanismos =
                                    mecanismosUnicos.length > 1 ? ` (+${mecanismosUnicos.length - 1})` : "";

                                return primerMecanismo + extraMecanismos;
                            },
                            flex: 4,
                            minWidth: 190,
                            editable: false,
                            filter: DatatableSelectFilter,
                            filterParams: {customUniqueValues: getUniqueMecanismos()},
                        },
                        {
                            field: "incidencias",
                            headerName: t("Incidencias"),
                            valueGetter: (e) => {
                                let incidencias = e.data?.incidencias || [];
                                if (incidencias === "-") {
                                    incidencias = [];
                                }

                                // Asegurar que mecanismos siempre sea un array válido
                                if (!Array.isArray(incidencias)) {
                                    incidencias = [];
                                }
                                // Filtrar valores únicos
                                const incidenciasConcatenadas = incidencias.map((val) => val.descripcionES);

                                return incidenciasConcatenadas.join(", ");
                            },
                            flex: 4,
                            valueFormatter: (e) =>
                                e.data?.incidencias[0]?.descripcionES + ` (${e.data?.incidencias.length})` || "-",
                            minWidth: 140,
                            editable: false,
                            filter: DatatableSelectFilter,
                            filterParams: {customUniqueValues: getUniqueIncidencias()},
                        },

                        {
                            field: "id",
                            headerName: "",
                            cellRenderer: ActionsComponent,
                            flex: 1.5,
                            minWidth: 80,
                            floatingFilter: false, // Deshabilitar el filtro flotante
                            filter: false, // Deshabilitar el filtro completo para esta columna
                            sortable: false,
                            tooltipValueGetter: () => null,
                        },
                    ]}
                />
                {modalState.show &&
                    (() => {
                        switch (modalState.type) {
                            case "form":
                                return (
                                    <EditOrdenFormModal
                                        show={true}
                                        onClose={closeModal}
                                        initialData={modalState.target}
                                    />
                                );
                            case "history":
                                return (
                                    <OrdenHistoryModal
                                        show={true}
                                        onClose={closeModal}
                                        initialData={modalState.target}
                                    />
                                );

                            default:
                                return null;
                        }
                    })()}
            </div>
        </div>
    );
};
