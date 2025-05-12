import {useEffect, useState} from "react";
import {Button, Form, Modal, Row} from "react-bootstrap";
import {useTranslation} from "react-i18next";

import {DateInput} from "@/components/common/formInputs/DateInput";
import {ListCheckBox} from "@/components/common/formInputs/ListCheckBox";
import {SelectInput} from "@/components/common/formInputs/SelectInput";
import {LoadingSpinner} from "@/components/common/loading/LoadingSpinner";
import {BaseModal} from "@/components/common/modales/BaseModal";
import {CustomModalHeader} from "@/components/common/modales/CustomModalHeader";
import {MostrarErrores} from "@/components/common/MostrarErrores";
import {activosService} from "@/services/ActivoService";
import {centrosCostesService} from "@/services/CentroCosteService";
import {ordenService} from "@/services/OrdenService";

export const ParametersSelectionsModal = ({
    show: showModal,
    onClose: handleClose,
    onSubmit,
    tipoEstudio,
    selectedFilters: init,
}) => {
    const defaultFilters = {
        years: [new Date().getFullYear(), new Date().getFullYear() - 1, new Date().getFullYear() - 2],
        meses: [],
        idCriticidad: 0,
        idCentroCoste: 0,
        idActivo: 0,
        idTipoEstudio: tipoEstudio || 0,
        fechaDesde: "",
        fechaHasta: "",
    };
    const defaultOpcionesFiltros = {
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
        tiposEstudiosComparativos: [
            {id: 1, name: "Indicadores OT"},
            {id: 2, name: "Indicadores Confiabilidad"},
        ],
        fechaDesde: "",
        fechaHasta: "",
    };

    const {t} = useTranslation();
    const [filters, setFilters] = useState(defaultFilters);
    const [opcionesFiltros, setOpcionesFiltros] = useState(defaultOpcionesFiltros);
    const [filteredOpcionesFiltros, setFilteredOpcionesFiltros] = useState(defaultOpcionesFiltros);
    const [errors, setErrors] = useState({});
    const [isLoading, setIsLoading] = useState(false);

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

    useEffect(() => {
        fetchYears();
        fetchCriticidades();
        fetchActivos();
        fetchCentrosCoste();
        setErrors({});
        console.log(init);
        if (init) {
            let init2 = Object.keys(init).length !== 0 ? {...init} : {};
            setFilters({...defaultFilters, ...init2});
        }

        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [showModal]);

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
        }

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
        }

        setFilters(newValue);
        setErrors({});
    };

    const handleYearsChange = (value, checked) => {
        setErrors({});
        const valueInt = parseInt(value); // Aseguramos que sea un entero
        let updatedYears = [];
        setFilters((prev) => {
            const currentYears = prev.years || []; // Asegura que sea un array
            updatedYears = checked
                ? [...currentYears, valueInt] // Agrega el nuevo valor
                : currentYears.filter((year) => year !== valueInt); // Elimina el valor

            return {
                ...prev,
                years: updatedYears, // Actualiza el array correctamente
            };
        });
    };
    const handleMesesChange = (value, checked) => {
        setErrors({});
        const valueInt = parseInt(value); // Aseguramos que sea un entero
        setFilters((prev) => {
            const currentMeses = prev.meses || []; // Asegura que sea un array
            const updatedMeses = checked
                ? [...currentMeses, valueInt] // Agrega el nuevo valor
                : currentMeses.filter((mes) => mes !== valueInt); // Elimina el valor

            return {
                ...prev,
                meses: updatedMeses, // Actualiza el array correctamente
            };
        });
    };

    const handleSubmit = (event) => {
        event.preventDefault();
        event.stopPropagation();

        onSubmit(filters, opcionesFiltros, setErrors);
    };

    return (
        <BaseModal show={showModal} onHide={() => handleClose({shouldRefetch: false})} isLoading={isLoading} size="xl">
            <CustomModalHeader title={t("Seleccionar Parámetros")} />

            <Form onSubmit={handleSubmit}>
                <Modal.Body>
                    <Row>
                        <div className="d-flex justify-content-between gap-5">
                            <ListCheckBox
                                textLabel={t("Años")}
                                options={opcionesFiltros.years}
                                onChange={(value, checked) => handleYearsChange(value, checked)}
                                selectedValues={filters.years}
                                maxSelectable={3}
                            />
                            <ListCheckBox
                                textLabel={t("Meses")}
                                options={opcionesFiltros.meses}
                                onChange={(value, checked) => handleMesesChange(value, checked)}
                                selectedValues={filters.meses}
                                maxSelectable={3}
                            />
                        </div>
                        <SelectInput
                            label={t("Estudio Comparativo")}
                            required
                            name="idTipoEstudio"
                            value={filters.idTipoEstudio}
                            onChange={handleInputChange}
                            options={filteredOpcionesFiltros.tiposEstudiosComparativos.map((te) => ({
                                value: te.id,
                                label: t(te.name),
                            }))}
                            disabled={isLoading}
                            md="6"
                        />
                        <SelectInput
                            label={t("Centro de Coste")}
                            name="idCentroCoste"
                            value={filters.idCentroCoste}
                            onChange={handleInputChange}
                            options={filteredOpcionesFiltros.centrosCoste.map((c) => ({
                                value: c.id,
                                label: c.centroCosteSAP + " - " + t(c.descripcionES),
                            }))}
                            disabled={isLoading}
                            md="6"
                        />
                        <SelectInput
                            label={t("Criticidad")}
                            name="idCriticidad"
                            value={filters.idCriticidad}
                            onChange={handleInputChange}
                            options={filteredOpcionesFiltros.criticidades.map((c) => ({
                                value: c.id,
                                label: c.descripcion + " (" + t(c.siglas) + ")",
                            }))}
                            disabled={isLoading}
                            md="6"
                        />
                        <SelectInput
                            label={t("Activo")}
                            name="idActivo"
                            value={filters.idActivo}
                            onChange={handleInputChange}
                            options={filteredOpcionesFiltros.activos.map((a) => ({
                                value: a.id,
                                label: a.id + " - " + t(a.descripcionES),
                            }))}
                            disabled={isLoading}
                            md="6"
                        />
                        <DateInput
                            label={t("Desde")}
                            name="fechaDesde"
                            value={filters.fechaDesde}
                            onChange={handleInputChange}
                            md="6"
                        />
                        <DateInput
                            label={t("Hasta")}
                            name="fechaHasta"
                            value={filters.fechaHasta}
                            onChange={handleInputChange}
                            md="6"
                        />

                        <MostrarErrores errors={errors} />
                    </Row>
                </Modal.Body>
                <Modal.Footer>
                    <Button
                        variant="secondary"
                        onClick={() => handleClose({shouldRefetch: false})}
                        disabled={isLoading}>
                        {t("Cancelar")}
                    </Button>
                    <Button
                        variant="primary"
                        type="submit"
                        disabled={isLoading || filters.years.length < 2 || !filters.idTipoEstudio}>
                        {t("Realizar Estudio")}
                        <LoadingSpinner isLoading={isLoading} />
                    </Button>
                </Modal.Footer>
            </Form>
        </BaseModal>
    );
};
const dataAux = [
    {
        Anio: 2025,
        TotalOrdenesCorrectivasCerradas: 36,
        DisponibilidadTotal: 99,
        ConfiabilidadTotal: 48,
        MTBFTotal: 116,
        MTTRTotal: 1,
        IndicadoresPeriodos: [
            {
                Periodo: 0,
                NTotalIncidenciasOrdenes: 1,
                NombrePeriodo: "Semana 0",
                NOrdenes: 1,
                ActivosCriticos: [9221],
                ActivosNoCriticos: [],
                TiempoTotalReparacionOrdenes: 0,
                MTBF: 168,
                MTTR: 0,
                Disponibilidad: 100,
                Confiabilidad: 62,
                TiempoFuncionamiento: 168,
                TiempoPeriodo: 80,
            },
            {
                Periodo: 1,
                NTotalIncidenciasOrdenes: 28,
                NombrePeriodo: "Semana 1",
                NOrdenes: 23,
                ActivosCriticos: [9226, 9260, 9277, 9254, 9056, 9270, 9158, 10137, 2668],
                ActivosNoCriticos: [1056, 9405, 9696, 9684, 9951, 9190, 9154, 9686, 9565, 9230],
                TiempoTotalReparacionOrdenes: 35,
                MTBF: 99,
                MTTR: 1,
                Disponibilidad: 99,
                Confiabilidad: 45,
                TiempoFuncionamiento: 2312,
                TiempoPeriodo: 80,
            },
            {
                Periodo: 2,
                NTotalIncidenciasOrdenes: 13,
                NombrePeriodo: "Semana 2",
                NOrdenes: 12,
                ActivosCriticos: [9254, 9221, 9252],
                ActivosNoCriticos: [10471, 10467, 9652, 9563, 10468, 9686],
                TiempoTotalReparacionOrdenes: 26,
                MTBF: 80,
                MTTR: 2,
                Disponibilidad: 98,
                Confiabilidad: 37,
                TiempoFuncionamiento: 984,
                TiempoPeriodo: 80,
            },
            {
                Periodo: 3,
                NTotalIncidenciasOrdenes: 0,
                NombrePeriodo: "Semana 3",
                NOrdenes: 0,
                ActivosCriticos: [],
                ActivosNoCriticos: [],
                TiempoTotalReparacionOrdenes: 0,
                MTBF: 0,
                MTTR: 0,
                Disponibilidad: 0,
                Confiabilidad: 100,
                TiempoFuncionamiento: 0,
                TiempoPeriodo: 0,
            },
            {
                Periodo: 4,
                NTotalIncidenciasOrdenes: 0,
                NombrePeriodo: "Semana 4",
                NOrdenes: 0,
                ActivosCriticos: [],
                ActivosNoCriticos: [],
                TiempoTotalReparacionOrdenes: 0,
                MTBF: 0,
                MTTR: 0,
                Disponibilidad: 0,
                Confiabilidad: 100,
                TiempoFuncionamiento: 0,
                TiempoPeriodo: 0,
            },
        ],
    },
    {
        Anio: 2024,
        TotalOrdenesCorrectivasCerradas: 192,
        DisponibilidadTotal: 35,
        ConfiabilidadTotal: 1506,
        MTBFTotal: 34,
        MTTRTotal: 62,
        IndicadoresPeriodos: [
            {
                Periodo: 0,
                NTotalIncidenciasOrdenes: 3,
                NombrePeriodo: "Semana 0",
                NOrdenes: 3,
                ActivosCriticos: [9981, 10126, 2668],
                ActivosNoCriticos: [],
                TiempoTotalReparacionOrdenes: 13,
                MTBF: 164,
                MTTR: 4,
                Disponibilidad: 98,
                Confiabilidad: 61,
                TiempoFuncionamiento: 504,
                TiempoPeriodo: 80,
            },
            {
                Periodo: 1,
                NTotalIncidenciasOrdenes: 64,
                NombrePeriodo: "Semana 1",
                NOrdenes: 55,
                ActivosCriticos: [
                    9056, 9270, 9254, 9578, 9408, 2669, 9279, 9137, 10399, 9341, 10353, 9159, 10265, 9969, 9141, 10136,
                    2633, 9292,
                ],
                ActivosNoCriticos: [
                    9675, 9613, 9283, 9203, 9156, 9378, 9702, 9804, 9403, 10175, 9364, 9652, 10291, 9062, 9978, 10286,
                    10509, 2637, 4190,
                ],
                TiempoTotalReparacionOrdenes: 5646,
                MTBF: -20,
                MTTR: 88,
                Disponibilidad: 0,
                Confiabilidad: 5460,
                TiempoFuncionamiento: 4544,
                TiempoPeriodo: 80,
            },
            {
                Periodo: 2,
                NTotalIncidenciasOrdenes: 63,
                NombrePeriodo: "Semana 2",
                NOrdenes: 58,
                ActivosCriticos: [
                    9056, 9254, 9869, 2670, 9221, 9279, 10262, 9340, 10265, 9703, 9233, 10137, 2671, 9248, 9158, 9700,
                    10263, 9683, 10399, 9981, 9982, 10126, 9140, 9225, 9591, 10353,
                ],
                ActivosNoCriticos: [
                    1058, 9686, 10468, 9062, 9136, 10013, 9804, 2697, 10120, 9190, 10494, 9239, 9006, 9378, 9117, 9203,
                    9651, 10275, 10114,
                ],
                TiempoTotalReparacionOrdenes: 1668,
                MTBF: 73,
                MTTR: 26,
                Disponibilidad: 74,
                Confiabilidad: 33,
                TiempoFuncionamiento: 5888,
                TiempoPeriodo: 80,
            },
            {
                Periodo: 3,
                NTotalIncidenciasOrdenes: 45,
                NombrePeriodo: "Semana 3",
                NOrdenes: 44,
                ActivosCriticos: [2624, 9578, 9254, 9056, 9270, 9252, 10263, 9233, 9058, 9137, 10398, 9079, 9683, 9232],
                ActivosNoCriticos: [
                    9686, 9871, 10468, 10120, 9613, 10118, 9507, 9062, 9006, 10134, 10494, 9403, 9114, 9065, 2510, 9457,
                    9652, 9001, 10144, 10222, 10389, 9506,
                ],
                TiempoTotalReparacionOrdenes: 5279,
                MTBF: -27,
                MTTR: 117,
                Disponibilidad: 0,
                Confiabilidad: 1936,
                TiempoFuncionamiento: 4112,
                TiempoPeriodo: 80,
            },
            {
                Periodo: 4,
                NTotalIncidenciasOrdenes: 32,
                NombrePeriodo: "Semana 4",
                NOrdenes: 32,
                ActivosCriticos: [2668, 2669, 9056, 9254, 9983, 9281, 9700, 9059, 10264, 10262, 9703],
                ActivosNoCriticos: [10479, 10215, 9491, 9665, 9970, 9506, 10512, 9783, 9978, 10195, 9679, 9361],
                TiempoTotalReparacionOrdenes: 167,
                MTBF: 83,
                MTTR: 5,
                Disponibilidad: 94,
                Confiabilidad: 38,
                TiempoFuncionamiento: 2808,
                TiempoPeriodo: 80,
            },
        ],
    },
];
