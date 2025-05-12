import {useEffect, useState} from "react";
import {useTranslation} from "react-i18next";
import {useLocation} from "react-router";

import {CustomButtonIconText} from "../../common/Buttons/CustomButtonIconText";
import {Icono} from "../../common/Icono";
import {InformadorFiltrosSeleccionados} from "../InformadorFiltrosSeleccionados";
import {PlantillaGraficaAreaMultiple} from "../PlantillaGraficaAreaMultiple";
import {ParametersSelectionsModal} from "./ParametersSelectionsModal";
import {PlantillaGraficaBarrasMultiple} from "./PlantillaGraficaBarrasMultiple";
import {PlantillaIndicadorMultiple} from "./PlantillaIndicadorMultiple";

import {useModalState} from "@/hooks/useModalState";
import {kpisService} from "@/services/KpisService";

export const EstudioComparativoPage = () => {
    const defaultCompleteFilters = {
        years: [],
        meses: [],
        centroCoste: 0,
        criticidad: 0,
        activo: 0,
        fechaDesde: "",
        fechaHasta: "",
    };
    const {t} = useTranslation();
    const location = useLocation();
    const [completeFilters, setCompleteFilters] = useState(defaultCompleteFilters);
    const [filters, setFilters] = useState({});
    const [tipoEstudioComparativo, setTipoEstudioComparativo] = useState(0);
    const [data, setData] = useState([]);
    const [datosIndicadores, setDatosIndicadores] = useState([]);
    const [datosGraficas, setDatosGraficas] = useState([]);
    const [isLoading, setIsLoading] = useState();
    const {modalState, openModal, closeModal} = useModalState();
    const colores = [
        "var(--graficaMantenimientoCorrectivo)",
        "var(--graficaMantenimientoPreventivo)",
        "var(--graficaMantenimientoMejora)",
        "var(--graficaMantenimientoFiabilidadHumana)",
    ];

    useEffect(() => {
        const {idTipoEstudio} = location.state || 0;

        setTipoEstudioComparativo(idTipoEstudio);
        openModal("parametersSelection");
    }, []);

    useEffect(() => {
        if (data.length > 0) {
            console.log(tipoEstudioComparativo);
            if (tipoEstudioComparativo === 1) {
                prepararDatosIndicadoresOT();
            } else if (tipoEstudioComparativo === 2) {
                prepararDatosConfiabilidad();
            }
        }
    }, [data]);

    const prepararDatosConfiabilidad = () => {
        setDatosIndicadores(() => [
            {
                titulo: t("Total Órdenes"),
                tooltip: t("Total órdenes"),
                data: data.map(({year, totalOrdenesCerradas}) => ({
                    year: year,
                    valor: totalOrdenesCerradas,
                })),
                unidad: "",
                id: "indicadorTotalEstudioComparativo",
            },
            {
                titulo: t("MTBF"),
                tooltip: t("Tiempo medio entre averías"),
                data: data.map(({year, totalMTBF}) => ({year: year, valor: totalMTBF})),
                unidad: "H",
                id: "",
            },
            {
                titulo: t("MTTR"),
                tooltip: t("Tiempo medio de Reparación"),
                data: data.map(({year, totalMTTR}) => ({year: year, valor: totalMTTR})),
                unidad: "H",
                id: "",
            },
            {
                titulo: t("Disponibilidad"),
                tooltip: t("Disponibilidad"),
                data: data.map(({year, totalDisponibilidad}) => ({year: year, valor: totalDisponibilidad})),
                unidad: "%",
                id: "",
            },
            {
                titulo: t("Confiabilidad"),
                tooltip: t("Confiabilidad"),
                data: data.map(({year, totalConfiabilidad}) => ({year: year, valor: totalConfiabilidad})),
                unidad: "%",
                id: "",
            },
        ]);

        setDatosGraficas(() => [
            {
                titulo: t("Total Correctivos"),
                data: data.map(({year, graficasPeriodos}) => ({
                    year: year,
                    datosGrafico: graficasPeriodos.map(({nombrePeriodo, ordenesCerradas}) => ({
                        nombrePeriodo: nombrePeriodo,
                        valor: ordenesCerradas,
                    })),
                })),
                unidad: "",
                id: "indicadorTotalEstudioComparativo",
                tooltip: false,
            },
            {
                titulo: t("Confiabilidad"),
                data: data.map(({year, graficasPeriodos}) => ({
                    year: year,
                    datosGrafico: graficasPeriodos.map(({nombrePeriodo, confiabilidad}) => ({
                        nombrePeriodo: nombrePeriodo,
                        valor: confiabilidad,
                    })),
                })),
                unidad: "%",
                id: "",
                tooltip: false,
            },
            {
                titulo: t("MTBF (Tiempo medio entre averías)"),
                data: data.map(({year, graficasPeriodos}) => ({
                    year: year,
                    datosGrafico: graficasPeriodos.map(({nombrePeriodo, mtbf}) => ({
                        nombrePeriodo: nombrePeriodo,
                        valor: mtbf,
                    })),
                })),
                unidad: "H",
                id: "",
                tooltip: false,
            },
            {
                titulo: t("MTTR (Tiempo medio de Reparación)"),
                data: data.map(({year, graficasPeriodos}) => ({
                    year: year,
                    datosGrafico: graficasPeriodos.map(({nombrePeriodo, mttr}) => ({
                        nombrePeriodo: nombrePeriodo,
                        valor: mttr,
                    })),
                })),
                unidad: t("H"),
                id: "",
                tooltip: false,
            },
            {
                titulo: t("Disponibilidad"),
                data: data.map(({year, graficasPeriodos}) => ({
                    year: year,
                    datosGrafico: graficasPeriodos.map(({nombrePeriodo, disponibilidad}) => ({
                        nombrePeriodo: nombrePeriodo,
                        valor: disponibilidad,
                    })),
                })),
                unidad: "%",
                id: "",
                tooltip: false,
            },
        ]);
    };
    const prepararDatosIndicadoresOT = () => {
        setDatosIndicadores(() => [
            {
                titulo: t("Total Órdenes"),
                data: data.map(({year, totalOrdenes}) => ({year: year, valor: totalOrdenes})),
                unidad: "",
                id: "indicadorTotalEstudioComparativo",
            },
            {
                titulo: t("Cumplimiento"),
                data: data.map(({year, porcentajeCompletadas}) => ({year: year, valor: porcentajeCompletadas})),
                unidad: "%",
                id: "",
            },
            {
                titulo: t("Pendientes"),
                data: data.map(({year, porcentajePendientes}) => ({year: year, valor: porcentajePendientes})),
                unidad: "%",
                id: "",
            },
            {
                titulo: t("Material"),
                data: data.map(({year, porcentajeMaterial}) => ({year: year, valor: porcentajeMaterial})),
                unidad: "%",
                id: "",
            },
        ]);
        setDatosGraficas(() => [
            {
                titulo: t("Mantenimiento Preventivo"),
                data: data.map(({year, preventivas}) => ({year: year, datosGrafico: preventivas})),
                unidad: "",
                id: "indicadorTotalEstudioComparativo",
                tooltip: true,
            },
            {
                titulo: t("Mantenimiento Correctivo"),
                data: data.map(({year, correctivas}) => ({year: year, datosGrafico: correctivas})),
                unidad: "%",
                id: "",
                tooltip: true,
            },
            {
                titulo: t("Mejora"),
                data: data.map(({year, mejoras}) => ({year: year, datosGrafico: mejoras})),
                unidad: "%",
                id: "",
                tooltip: true,
            },
            {
                titulo: t("Falla Humana"),
                data: data.map(({year, fallaHumana}) => ({year: year, datosGrafico: fallaHumana})),
                unidad: "%",
                id: "",
                tooltip: true,
            },
            {
                titulo: t("Mantenimiento General"),
                data: data.map(({year, mantenimientoGeneral}) => ({year: year, datosGrafico: mantenimientoGeneral})),
                unidad: "",
                id: "indicadorTotalEstudioComparativo",
                tooltip: true,
            },
        ]);
    };

    const convertirSeriesMultiplesConfiabilidad = (datosGrafica) => {
        const resultado = datosGrafica.data.map(({year, datosGrafico}, index) => ({
            name: String(year), // Nombre será el año como string
            data: datosGrafico.map(({valor}) => Math.round(valor * 100) / 100), // Extrae todos los valores
            color: colores[index % colores.length], // Asigna un color basado en el índice
        }));
        return resultado;
    };
    const convertirSeriesMultiplesOT = (datosGrafica) => {
        const resultado = generateSeries(datosGrafica.datosGrafico, [
            {name: "Correctivas", key: "porcentajeCorrectivas", color: "var(--graficaMantenimientoCorrectivo)"},
            {name: "Preventivas", key: "porcentajePreventivas", color: "var(--graficaMantenimientoPreventivo)"},
            {name: "Mejoras", key: "porcentajeMejoras", color: "var(--graficaMantenimientoMejora)"},
            {
                name: "Falla Humana",
                key: "porcentajeFallaHumana",
                color: "var(--graficaMantenimientoFiabilidadHumana)",
            },
        ]);
        return resultado;
    };

    const generateSeries = (data, properties) => {
        return properties.map((prop) => ({
            name: prop.name,
            data: data?.map((item) => item[prop.key] && Math.round(item[prop.key] * 100) / 100) || [],
            color: prop.color, // Asignar el color aquí
        }));
    };

    const handleAction = (actionType) => {
        openModal(actionType);
    };

    const handleSubmit = (filters, opcionesFiltros, setErrors) => {
        setFilters(filters);
        setTipoEstudioComparativo(filters.idTipoEstudio);
        setIsLoading(true);

        const transformFilter = (filterArray) => {
            return filterArray.length === 1 && filterArray[0] === 0 ? [] : filterArray;
        };

        const params = new URLSearchParams({
            yearsString: JSON.stringify(transformFilter(filters.years)),
            mesesString: JSON.stringify(transformFilter(filters.meses)),
            idCriticidad: filters.idCriticidad,
            idCentroCoste: filters.idCentroCoste,
            idActivo: filters.idActivo,
            fechaDesde: filters.fechaDesde,
            fechaHasta: filters.fechaHasta,
        });
        let fetcher;
        if (filters.idTipoEstudio == 1) {
            fetcher = kpisService.getEstudioComparativoIndicadoresOT;
        } else if (filters.idTipoEstudio == 2) {
            fetcher = kpisService.getEstudioComparativoConfiabilidad;
        }

        fetcher(params).then((response) => {
            if (!response.is_error) {
                setData(response.content || []);

                //Rellenamos el informador de filtros
                setCompleteFilters(() => ({
                    years: filters.years,
                    meses: opcionesFiltros.meses
                        .filter((m) => filters.meses.includes(m.id)) // Filtra los meses
                        .map((m) => m.name),
                    centroCoste:
                        filters.idCentroCoste != 0
                            ? opcionesFiltros.centrosCoste?.find((c) => c.id == filters.idCentroCoste)?.centroCosteSAP +
                              " - " +
                              opcionesFiltros.centrosCoste?.find((c) => c.id == filters.idCentroCoste)?.descripcionES
                            : 0,
                    criticidad:
                        filters.idCriticidad != 0
                            ? opcionesFiltros.criticidades?.find((c) => c.id == filters.idCriticidad)?.siglas
                            : 0,
                    activo:
                        filters.idActivo != 0
                            ? opcionesFiltros.activos?.find((a) => a.id == filters.idActivo)?.id +
                              " - " +
                              opcionesFiltros.activos?.find((a) => a.id == filters.idActivo)?.descripcionES
                            : 0,
                    fechaDesde: filters.fechaDesde,
                    fechaHasta: filters.fechaHasta,
                }));

                //Cerramos el modal de selección de parámetros
                closeModal();
            } else {
                console.error(response.error_content);
                setErrors(response.error_content);
            }
            setIsLoading(false);
        });
    };
    return (
        <>
            <section hidden={data && !Object.keys(data).length == 0}>
                <div className="d-flex justify-content-end mt-4">
                    <div>
                        <CustomButtonIconText
                            titulo={t("Seleccionar parámetros")}
                            texto={t("Seleccionar Parámetros")}
                            icono="fa-solid fa-list-ul"
                            disabled={isLoading}
                            onClick={() => handleAction("parametersSelection")}
                        />
                    </div>
                </div>
                <div className="noFiltrosSeleccionadosEstudioComparativo" id="noFiltrosSeleccionadosEstudioComparativo">
                    <Icono name="fa-solid fa-magnifying-glass-chart" style={{fontSize: "150px", color: "#adb5bd"}} />
                    <p className="text-center fw-semibold text-info-emphasis" style={{fontSize: "18 px"}}>
                        {t("No se ha seleccionado ningún parámetro para realizar el estudio comparativo")}
                    </p>
                </div>
            </section>
            <section className="mt-5" hidden={data && Object.keys(data).length == 0}>
                {/* INDICADORES */}
                <div className="indicadoresSection">
                    {datosIndicadores.map((item) => (
                        <PlantillaIndicadorMultiple
                            key={item.titulo}
                            tooltip={item.tooltip || item.titulo}
                            titulo={item.titulo}
                            data={item.data}
                            colores={colores}
                            unidad={item.unidad}
                            style={{width: "100%"}}
                        />
                    ))}
                </div>
                {/* FILTROS SELECCIONADOS */}
                <div className="d-flex justify-content-between gap-3 mt-4">
                    <InformadorFiltrosSeleccionados filtros={completeFilters} isLoading={isLoading} />
                    <div>
                        <CustomButtonIconText
                            titulo={t("Seleccionar parámetros")}
                            texto={t("Seleccionar Parámetros")}
                            icono="fa-solid fa-list-ul"
                            disabled={isLoading}
                            onClick={() => handleAction("parametersSelection")}
                        />
                    </div>
                </div>
                {/* GRÁFICOS */}
                <div className="containerGraficosKPIS">
                    <div className="row mt-2">
                        {datosGraficas
                            .filter(
                                (item) =>
                                    item.titulo == "Disponibilidad" || item.titulo.includes("Mantenimiento General"), // Excluye títulos que contengan "Mantenimiento General"
                            )
                            .map((item) =>
                                tipoEstudioComparativo == 2 ? (
                                    <PlantillaGraficaAreaMultiple
                                        key={item.titulo}
                                        md={12}
                                        titulo={t(item.titulo)}
                                        data={item.data}
                                        series={convertirSeriesMultiplesConfiabilidad(item)}
                                    />
                                ) : (
                                    item.data.map((i) => (
                                        <PlantillaGraficaAreaMultiple
                                            key={i.year}
                                            md={12}
                                            titulo={t(item.titulo + " " + i.year)}
                                            data={i.datosGrafico}
                                            series={convertirSeriesMultiplesOT(i)}
                                        />
                                    ))
                                ),
                            )}
                    </div>
                    <div className="row">
                        {datosGraficas
                            .filter(
                                (item) =>
                                    item.titulo !== "Disponibilidad" && !item.titulo.includes("Mantenimiento General"), // Excluye títulos que contengan "Mantenimiento General"
                            )
                            .map((item) => (
                                <PlantillaGraficaBarrasMultiple
                                    key={item.titulo}
                                    titulo={item.titulo}
                                    data={item.data}
                                    colores={[
                                        "var(--graficaMantenimientoCorrectivo)",
                                        "var(--graficaMantenimientoPreventivo)",
                                        "var(--graficaMantenimientoMejora)",
                                        "var(--graficaMantenimientoFiabilidadHumana)",
                                    ]}
                                    unidad={item.unidad}
                                    tooltip={item.tooltip}
                                    md={6}
                                />
                            ))}
                    </div>
                </div>
            </section>

            {modalState.show &&
                (() => {
                    switch (modalState.type) {
                        case "parametersSelection":
                            return (
                                <ParametersSelectionsModal
                                    show={true}
                                    onClose={closeModal}
                                    onSubmit={handleSubmit}
                                    tipoEstudio={tipoEstudioComparativo}
                                    selectedFilters={filters}
                                />
                            );

                        default:
                            return null;
                    }
                })()}
        </>
    );
};
