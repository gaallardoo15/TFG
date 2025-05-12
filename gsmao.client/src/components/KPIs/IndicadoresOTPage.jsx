import {useEffect, useState} from "react";
import {useTranslation} from "react-i18next";
import {useNavigate} from "react-router";
import {toast} from "react-toastify";

import {RoutePaths} from "../App";
import {CustomButtonIconText} from "../common/Buttons/CustomButtonIconText";
import {DropdownMenuCheckboxes} from "../common/Dropdown/DropdownMenuCheckboxes";
import {DropdownMultiFiltro} from "../common/Dropdown/DropdownMultiFiltro";
import {DropdownSubMenu} from "../common/Dropdown/DropdownSubMenu";
import {DateInput} from "../common/formInputs/DateInput";
import {SelectInput} from "../common/formInputs/SelectInput";
import {TableComponent} from "../common/TableComponent";
import {InformadorFiltrosSeleccionados} from "./InformadorFiltrosSeleccionados";
import {PlantillaGraficaAreaMultiple} from "./PlantillaGraficaAreaMultiple";
import {PlantillaGraficaSimple} from "./PlantillaGraficaSimple";
import {PlantillaIndicador} from "./PlantillaIndicador";

import {useFileDownloader} from "@/hooks/useFileDownloader";
import {activosService} from "@/services/ActivoService";
import {centrosCostesService} from "@/services/CentroCosteService";
import {kpisService} from "@/services/KpisService";
import {ordenService} from "@/services/OrdenService";

export const IndicadoresOTPage = () => {
    const defaultFilters = {
        year: new Date().getFullYear(),
        idCriticidad: 0,
        mes: 0,
        idCentroCoste: 0,
        idActivo: 0,
        fechaDesde: "",
        fechaHasta: "",
    };

    const defaultOpciopnesFiltros = {
        criticidades: [],
        years: [],
        meses: [
            {id: 1, name: "Enero"},
            {id: 2, name: "Febrero"},
            {id: 3, name: "Marzo"},
            {id: 4, name: "Abril"},
            {id: 5, name: "Mayo"},
            {id: 6, name: "Junio"},
            {id: 7, name: "Julio"},
            {id: 8, name: "Agosto"},
            {id: 9, name: "Septiembre"},
            {id: 10, name: "Octubre"},
            {id: 11, name: "Noviembre"},
            {id: 12, name: "Diciembre"},
        ],
        activos: [],
        centrosCoste: [],
        fechaDesde: "",
        fechaHasta: "",
    };
    const defaultCompleteFilters = {
        year: new Date().getFullYear(),
        mes: 0,
        centroCoste: 0,
        criticidad: 0,
        activo: 0,
        fechaDesde: "",
        fechaHasta: "",
    };
    const {t} = useTranslation();
    const navigate = useNavigate();
    const [filters, setFilters] = useState(defaultFilters);
    const [completeFilters, setCompleteFilters] = useState(defaultCompleteFilters);
    const [data, setData] = useState({});
    const [seriesMultiples, setSeriesMultiples] = useState([]);
    const [opcionesFiltros, setOpcionesFiltros] = useState(defaultOpciopnesFiltros);
    const [filteredOpcionesFiltros, setFilteredOpcionesFiltros] = useState(defaultOpciopnesFiltros);
    const [isLoading, setIsLoading] = useState();
    const {isLoading: isLoadingInforme, fileDownload} = useFileDownloader();

    const fetchCriticidades = () => {
        ordenService.getCriticidades().then((response) => {
            if (!response.is_error) {
                const criticidades = response.content || [];
                setOpcionesFiltros((prev) => ({
                    ...prev,
                    criticidades: criticidades,
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
                    years: years,
                }));
            } else {
                console.error(response.error_content);
            }
        });
    };
    const fetchActivos = () => {
        activosService.fetchAll().then((response) => {
            if (!response.is_error) {
                let activos = response.content || [];
                activos = activos.filter((a) => a.estadoActivo?.name == "Activo");
                setOpcionesFiltros((prev) => ({
                    ...prev,
                    activos: activos,
                }));
            } else {
                console.error(response.error_content);
            }
        });
    };
    const fetchCentrosCoste = () => {
        centrosCostesService.fetchAll().then((response) => {
            if (!response.is_error) {
                const centrosCoste = response.content || [];
                setOpcionesFiltros((prev) => ({
                    ...prev,
                    centrosCoste: centrosCoste,
                }));
            } else {
                console.error(response.error_content);
            }
        });
    };

    const fetchData = () => {
        setIsLoading(true);
        const params = new URLSearchParams({
            year: filters.year,
            idCriticidad: filters.idCriticidad,
            mes: filters.mes,
            idCentroCoste: filters.idCentroCoste,
            idActivo: filters.idActivo,
            fechaDesde: filters.fechaDesde,
            fechaHasta: filters.fechaHasta,
        });

        kpisService.getIndicadoresOTs(params).then((response) => {
            if (!response.is_error) {
                setData(response.content || []);
            } else {
                console.error(response.error_content);
            }
            setIsLoading(false);
        });
    };

    useEffect(() => {
        fetchYears();
        fetchCriticidades();
        fetchActivos();
        fetchCentrosCoste();
        // fetchMeses();
    }, []);

    useEffect(() => {
        fetchData();
    }, [filters]);

    // Uso en el useEffect
    useEffect(() => {
        if (data?.mantenimientoGeneral) {
            setSeriesMultiples(
                generateSeries(data.mantenimientoGeneral, [
                    {name: "Correctivas", key: "porcentajeCorrectivas", color: "var(--graficaMantenimientoCorrectivo)"},
                    {name: "Preventivas", key: "porcentajePreventivas", color: "var(--graficaMantenimientoPreventivo)"},
                    {name: "Mejoras", key: "porcentajeMejoras", color: "var(--graficaMantenimientoMejora)"},
                    {
                        name: "Falla Humana",
                        key: "porcentajeFallaHumana",
                        color: "var(--graficaMantenimientoFiabilidadHumana)",
                    },
                ]),
            );
        }
    }, [data]);

    useEffect(() => {
        setFilteredOpcionesFiltros(opcionesFiltros);
    }, [opcionesFiltros]);

    const handleInputChange = (event) => {
        const {name, value, type, checked} = event.target;

        let newValue = {
            ...filters,
            [name]: type === "checkbox" ? checked : value,
        };

        if (name === "idCentroCoste") {
            if (value == "") {
                newValue.idCentroCoste = 0;
            }

            let activosAux =
                newValue.idCentroCoste > 0
                    ? opcionesFiltros.activos.filter((a) => a.centroCoste.id == newValue.idCentroCoste)
                    : opcionesFiltros.activos;
            let criticidades = Array.from(
                new Map(activosAux.map((activo) => [activo.criticidad.id, activo.criticidad])).values(),
            );
            setFilteredOpcionesFiltros((prev) => ({
                ...prev,
                activos: activosAux,
                criticidades: criticidades,
            }));

            setCompleteFilters((prev) => ({
                ...prev,
                centroCoste:
                    value != 0
                        ? opcionesFiltros.centrosCoste?.find((c) => c.id == value)?.centroCosteSAP +
                          " - " +
                          opcionesFiltros.centrosCoste?.find((c) => c.id == value)?.descripcionES
                        : 0,
            }));
        }

        if (name == "idActivo") {
            setCompleteFilters((prev) => ({
                ...prev,
                activo:
                    value != 0
                        ? opcionesFiltros.activos?.find((a) => a.id == value)?.id +
                          " - " +
                          opcionesFiltros.activos?.find((a) => a.id == value)?.descripcionES
                        : 0,
            }));
        }
        if (name == "fechaDesde" || name == "fechaHasta") {
            setCompleteFilters((prev) => ({
                ...prev,
                mes: 0,
                year: 0,
                fechaDesde: newValue.fechaDesde,
                fechaHasta: newValue.fechaHasta,
            }));

            newValue.mes = 0;
            newValue.year = 0;
        }

        setFilters(newValue);
    };

    const generateSeries = (data, properties) => {
        return properties.map((prop) => ({
            name: prop.name,
            data: data?.map((item) => item[prop.key] && Math.round(item[prop.key] * 100) / 100) || [],
            color: prop.color, // Asignar el color aquí
        }));
    };

    const toggleCheckbox = (name, value) => {
        if (name === "idCriticidad") {
            let activosCentroCoste =
                filters.idCentroCoste > 0
                    ? opcionesFiltros.activos.filter((a) => a.centroCoste.id == filters.idCentroCoste)
                    : opcionesFiltros.activos;
            let activosAux =
                value > 0
                    ? activosCentroCoste.filter((a) => a.criticidad.id == value)
                    : opcionesFiltros.activos.filter((a) => a.centroCoste.id == filters.idCentroCoste);

            setFilteredOpcionesFiltros((prev) => ({
                ...prev,
                activos: activosAux,
            }));

            setCompleteFilters((prev) => ({
                ...prev,
                criticidad: value != 0 ? opcionesFiltros.criticidades?.find((c) => c.id == value)?.siglas : 0,
            }));

            setFilters((prevFilters) => {
                return {
                    ...prevFilters,
                    [name]: value, // Almacena solo el valor seleccionado en ese momento
                };
            });
        }

        if (name === "year") {
            setCompleteFilters((prev) => ({
                ...prev,
                fechaDesde: "",
                fechaHasta: "",
                year: value != 0 ? value : 0,
            }));
        }
        if (name === "mes") {
            setCompleteFilters((prev) => ({
                ...prev,
                fechaDesde: "",
                fechaHasta: "",
                mes: value != 0 ? opcionesFiltros.meses?.find((m) => m.id == value)?.name : 0,
            }));
        }

        if (name == "year" || name == "mes") {
            setFilters((prevFilters) => {
                return {
                    ...prevFilters,
                    fechaDesde: "",
                    fechaHasta: "",
                    [name]: value, // Almacena solo el valor seleccionado en ese momento
                };
            });
        }
    };

    const exportTable = async () => {
        const params = new URLSearchParams({
            year: filters.year,
            idCriticidad: filters.idCriticidad,
            mes: filters.mes,
            idCentroCoste: filters.idCentroCoste,
            idActivo: filters.idActivo,
            fechaDesde: filters.fechaDesde,
            fechaHasta: filters.fechaHasta,
        });

        toast.info(t("La exportación del informe se está realizando en base a los filtros seleccionados"), {
            position: "top-right",
            autoClose: 10000,
            closeButton: true,
        });

        const error = await fileDownload(
            `/api/kpis/ordenes/descargar-tabla?${params.toString()}`,
            "Desglose_Por_Activos_KPIS_OT",
        );
        if (error != undefined) {
            toast.error(`${error}`);
        }
    };
    return (
        <>
            {!(data && Object.keys(data).length > 0) ? (
                <div>
                    <div className="contenedorSpinnerGenericoDashboard">
                        <div className="spinner-border spinnerGenericoDashboard" role="status"></div>
                    </div>
                </div>
            ) : (
                <div className="d-flex flex-column gap-3 mt-5">
                    <section className="indicadoresSection">
                        <PlantillaIndicador
                            valorIndicador={data?.totalOrdenes}
                            tituloIndicador={"Total"}
                            id="indicadorTotal"
                        />
                        <PlantillaIndicador
                            valorIndicador={
                                (data?.porcentajeCompletadas && Math.round(data?.porcentajeCompletadas * 100) / 100) ||
                                0
                            }
                            tituloIndicador={t("Cumplimiento")}
                            barraPorcentaje={true}
                            variant="success"
                            unidad="%"
                        />
                        <PlantillaIndicador
                            valorIndicador={
                                (data?.porcentajePendientes && Math.round(data?.porcentajePendientes * 100) / 100) || 0
                            }
                            tituloIndicador={t("Pendientes")}
                            barraPorcentaje={true}
                            variant="warning"
                            unidad="%"
                        />
                        <PlantillaIndicador
                            valorIndicador={
                                (data?.porcentajeMaterial && Math.round(data?.porcentajeMaterial * 100) / 100) || 0
                            }
                            tituloIndicador={t("Material")}
                            barraPorcentaje={true}
                            variant="info"
                            unidad="%"
                        />
                        <div className="contenedorFiltros">
                            <DropdownMultiFiltro variant="primary" autoclose="outside">
                                <DropdownSubMenu title={t("Año")} autoclose="outside">
                                    <DropdownMenuCheckboxes
                                        type="radio"
                                        opciones={filteredOpcionesFiltros.years}
                                        checked={(year) => filters.year == year}
                                        onclickItem={(value) => toggleCheckbox("year", value)} // Aquí pasamos el nombre y el valor
                                        name="year"
                                    />
                                </DropdownSubMenu>
                                <DropdownSubMenu title={t("Mes")} autoclose="outside">
                                    <DropdownMenuCheckboxes
                                        type="radio"
                                        opciones={filteredOpcionesFiltros.meses}
                                        checked={(mes) => filters.mes == mes}
                                        onclickItem={(value) => toggleCheckbox("mes", value)}
                                        name="mes"
                                    />
                                </DropdownSubMenu>
                                <DropdownSubMenu
                                    title={t("Centro de Coste")}
                                    autoclose="outside"
                                    style={{width: "30vh"}}>
                                    <div
                                        className="ms-3 me-3"
                                        onClick={(e) => e.stopPropagation()}
                                        onKeyDown={(e) => {
                                            if (e.key === "Enter" || e.key === " ") {
                                                e.stopPropagation();
                                            }
                                        }}
                                        role="button"
                                        tabIndex={0}>
                                        <SelectInput
                                            key={filters.idCentroCoste}
                                            label={t("Centros de Coste")}
                                            name="idCentroCoste"
                                            value={filters.idCentroCoste}
                                            onChange={handleInputChange}
                                            options={filteredOpcionesFiltros.centrosCoste.map((c) => ({
                                                value: c.id,
                                                label: c.centroCosteSAP + " - " + t(c.descripcionES),
                                            }))}
                                            disabled={isLoading}
                                            md="12"
                                        />
                                    </div>
                                </DropdownSubMenu>
                                <DropdownSubMenu title={t("Criticidad")} autoclose="outside">
                                    <DropdownMenuCheckboxes
                                        type="radio"
                                        opciones={filteredOpcionesFiltros.criticidades}
                                        checked={(criticidad) => filters.idCriticidad == criticidad}
                                        onclickItem={(value) => toggleCheckbox("idCriticidad", value)}
                                        name="idCriticidad"
                                    />
                                </DropdownSubMenu>
                                <DropdownSubMenu title={t("Activo")} autoclose="outside" style={{width: "30vh"}}>
                                    <div>
                                        <div
                                            className="ms-3 me-3"
                                            onClick={(e) => e.stopPropagation()}
                                            onKeyDown={(e) => {
                                                if (e.key === "Enter" || e.key === " ") {
                                                    e.stopPropagation();
                                                }
                                            }}
                                            role="button"
                                            tabIndex={0}>
                                            <SelectInput
                                                key={filters.idActivo}
                                                label={t("Activos")}
                                                name="idActivo"
                                                value={filters.idActivo}
                                                onChange={handleInputChange}
                                                options={filteredOpcionesFiltros.activos.map((c) => ({
                                                    value: c.id,
                                                    label: c.id + " - " + t(c.descripcionES),
                                                }))}
                                                disabled={isLoading}
                                                md="12"
                                            />
                                        </div>
                                    </div>
                                </DropdownSubMenu>
                                <DropdownSubMenu title={t("Período")} autoclose="outside">
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
                                </DropdownSubMenu>
                            </DropdownMultiFiltro>
                        </div>
                    </section>
                    <section className="d-flex justify-content-between gap-3 mt-4">
                        <InformadorFiltrosSeleccionados filtros={completeFilters} isLoading={isLoading} />
                        <div>
                            <CustomButtonIconText
                                titulo={t("Estudio Comparativo")}
                                texto={t("Estudio Comparativo")}
                                icono="fa-solid fa-magnifying-glass-chart"
                                disabled={isLoading}
                                onClick={() =>
                                    navigate(RoutePaths.EstudioComparativo.path, {state: {idTipoEstudio: 1}})
                                }
                            />
                        </div>
                    </section>
                    {Object.keys(data).length > 0 && (
                        <section className="containerGraficosKPIS">
                            <div className="row">
                                <PlantillaGraficaAreaMultiple
                                    md={12}
                                    titulo={t("Mantenimiento General")}
                                    color="var(--graficaMantenimientoPreventivo)"
                                    data={data?.mantenimientoGeneral}
                                    series={seriesMultiples}
                                />
                            </div>

                            <div className="row">
                                <PlantillaGraficaSimple
                                    md={6}
                                    titulo={t("Mantenimiento Preventivo")}
                                    color="var(--graficaMantenimientoPreventivo)"
                                    data={data?.preventivas}
                                />
                                <PlantillaGraficaSimple
                                    md={6}
                                    titulo={t("Mantenimiento Correctivo")}
                                    color="var(--graficaMantenimientoCorrectivo)"
                                    data={data?.correctivas}
                                />
                                <PlantillaGraficaSimple
                                    md={6}
                                    titulo={t("Mejora")}
                                    color="var(--graficaMantenimientoMejora)"
                                    data={data?.mejoras}
                                />
                                <PlantillaGraficaSimple
                                    md={6}
                                    titulo={t("Falla Humana")}
                                    color="var(--graficaMantenimientoFiabilidadHumana)"
                                    data={data?.fallaHumana}
                                />
                            </div>
                        </section>
                    )}
                    <div className="mt-3">
                        <h2 className="tituloTopPagina" style={{fontSize: "23px"}}>
                            {t("Desglose por Activos")}
                        </h2>
                        <div className="contenedorPlantillaCuadrada mt-3">
                            <TableComponent
                                isLoading={isLoading}
                                data={data.desglosePorActivos}
                                pagination={true}
                                onDownload={exportTable}
                                columnDefs={[
                                    {
                                        field: "idActivo",
                                        headerName: t("Activo"),
                                        // valueGetter: (e) =>
                                        //     e.data?.activo?.id + " - " + e.data?.activo?.descripcionES || "-",
                                        flex: 2.5,
                                        minWidth: 230,
                                        editable: true,
                                    },
                                    {
                                        field: "totalOrdenes",
                                        headerName: t("Total Órdenes"),
                                        valueGetter: (e) => e.data?.indicadores?.totalOrdenes || "-",
                                        flex: 1.5,
                                        minWidth: 200,
                                        editable: true,
                                    },
                                    {
                                        field: "porcentajeCompletadas",
                                        headerName: t("% Cumplimiento"),
                                        valueGetter: (e) => e.data?.indicadores?.porcentajeCompletadas + "%" || "-",
                                        flex: 1.5,
                                        minWidth: 120,
                                        editable: false,
                                    },
                                    {
                                        field: "porcentajePendientes",
                                        headerName: t("% Pendientes"),
                                        valueGetter: (e) => e.data?.indicadores?.porcentajePendientes + "%" || "-",
                                        flex: 1.5,
                                        minWidth: 120,
                                        editable: false,
                                    },
                                    {
                                        field: "porcentajeMaterial",
                                        headerName: t("% Material"),
                                        valueGetter: (e) => e.data?.indicadores?.porcentajeMaterial + "%" || "-",
                                        flex: 1.5,
                                        minWidth: 120,
                                        editable: false,
                                    },
                                ]}
                            />
                        </div>
                    </div>
                </div>
            )}
        </>
    );
};
