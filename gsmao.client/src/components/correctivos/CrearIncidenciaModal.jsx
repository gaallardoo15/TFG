import {useEffect, useState} from "react";
import {Button, Form, Modal, Row} from "react-bootstrap";
import {useTranslation} from "react-i18next";

import {CheckInput} from "../common/formInputs/CheckInput";
import {DateInput} from "../common/formInputs/DateInput";
import {DynamicSmartSelectInputs} from "../common/formInputs/DynamicSmartSelectInputs";
import {ListCheckBox} from "../common/formInputs/ListCheckBox";
import {TimeInput} from "../common/formInputs/TimeInput";
import {LoadingSpinner} from "../common/loading/LoadingSpinner";
import {BaseModal} from "../common/modales/BaseModal";
import {CustomModalHeader} from "../common/modales/CustomModalHeader";
import {MostrarErrores} from "../common/MostrarErrores";

import {componentesService} from "@/services/ComponenteService";
import {incidenciasService} from "@/services/IncidenciaService";
import {mecanismosFallosService} from "@/services/MecanismoFalloService";
import {ordenService} from "@/services/OrdenService";

export const CrearIncidenciaModal = ({show: showModal, onClose: handleClose, idActivo, idOrden}) => {
    const defaultData = {
        idOrden: null,
        fechaDeteccion: new Date().toISOString().split("T")[0],
        horaDeteccion: new Date().toTimeString().split(" ")[0].substring(0, 5),
        idComponente: 0,
        incidencias: [],
        cambioPieza: false,
        afectaProduccion: false,
        paroMaquina: false,
    };

    const {t} = useTranslation();
    const [formData, setFormData] = useState(defaultData);
    const [errors, setErrors] = useState({});
    const [mecanismosFalloSelected, setMecanismosFalloSelected] = useState([]);
    const [incidencias, setIncidencias] = useState([]);
    const [mecanismosFallo, setMecanismosFallo] = useState([]);
    const [componentes, setComponentes] = useState([]);
    const [componentesBaseLevel, setComponentesBaseLevel] = useState([]);
    const [isLoading, setIsLoading] = useState(false);

    const fetchIncidencias = () => {
        incidenciasService.fetchAll().then((response) => {
            if (!response.is_error) {
                setIncidencias(response.content);
            } else {
                setErrors(response.error_content);
            }
            setIsLoading(false);
        });
    };
    const fetchMecanismosFallo = () => {
        mecanismosFallosService.fetchAll().then((response) => {
            if (!response.is_error) {
                setMecanismosFallo(response.content);
            } else {
                setErrors(response.error_content);
            }
            setIsLoading(false);
        });
    };

    const fetchComponentes = (idActivo, idComponente) => {
        componentesService.getComponentes(idActivo, idComponente).then((response) => {
            if (!response.is_error) {
                const components = (response.content || []).sort((a, b) => a.id - b.id);
                setComponentes(components);
                setComponentesBaseLevel(components);
            } else {
                console.log(response.error_content);
                setErrors(response.error_content);
            }
        });
    };

    useEffect(() => {
        if (idActivo > 0) {
            fetchComponentes(idActivo, 0); // Cargar los componentes para el activo seleccionado
        }
        setFormData({...defaultData, idOrden});
        fetchMecanismosFallo();
        fetchIncidencias();
        setErrors({});
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [showModal, idOrden]);

    //Inputs para inputs normales
    const handleInputChange = (event) => {
        const {name, value, type, checked} = event.target;
        let newValue = {
            ...formData,
            [name]: type === "checkbox" ? checked : value,
        };

        setFormData(newValue);
        setErrors({});
    };

    const handleMecanismoChange = (value, checked) => {
        setErrors({});
        setMecanismosFalloSelected((prevMecanismos) => {
            const valueInt = parseInt(value);
            if (checked) {
                // Añadir mecanismo seleccionado si no está ya en la lista
                return [...prevMecanismos, valueInt];
            } else {
                // Eliminar mecanismo si se deselecciona
                return prevMecanismos.filter((mecanismo) => mecanismo !== valueInt);
            }
        });
    };

    const handleIncidenciaChange = (name, checked) => {
        setErrors({});
        setFormData((prevData) => {
            const updatedIncidencias = checked
                ? [...prevData.incidencias, name] // Añadir incidencia seleccionada
                : prevData.incidencias.filter((incidencia) => incidencia !== name); // Eliminar incidencia desmarcada

            return {
                ...prevData,
                incidencias: updatedIncidencias,
            };
        });
    };

    const handleComponenteChange = (selectedComponent, children) => {
        setErrors({});
        let newValue = {
            ...formData,
        };
        newValue.idComponente = selectedComponent.id;
        setFormData(newValue);
        setComponentes(children);
    };

    const handleSubmit = (event) => {
        event.preventDefault();
        event.stopPropagation();
        setErrors({});
        setIsLoading(true);

        const newFechaDeteccion = formData.fechaDeteccion && formData.fechaDeteccion + " " + formData.horaDeteccion;

        if (!comprobarFechaValida(newFechaDeteccion)) {
            setErrors({fechaDeteccion: "Compruebe la fecha de detección, no puede ser posterior a la fecha actual."});
            setIsLoading(false);
            return;
        }
        if (!formData.incidencias.length > 0) {
            setErrors({fechaDeteccion: "Debe seleccionar al menos 1 incidencia del listado de incidencias posibles."});
            setIsLoading(false);
            return;
        }
        if (!formData.idComponente > 0) {
            setErrors({fechaDeteccion: "Debe seleccionar al menos 1 nivel de componente."});
            setIsLoading(false);
            return;
        }

        const dataToSend = {
            ...formData,
            fechaDeteccion: newFechaDeteccion,
        };

        ordenService.addNuevasIncidencias(dataToSend).then((response) => {
            if (!response.is_error) {
                handleClose({shouldRefetch: true});
            } else {
                setErrors(response.error_content);
            }
            setIsLoading(false);
        });
    };

    const filteredIncidencias =
        mecanismosFalloSelected.length > 0
            ? incidencias.filter((incidencia) => mecanismosFalloSelected.includes(incidencia.mecanismoDeFallo.id))
            : incidencias;

    const comprobarFechaValida = (fechaAux) => {
        const fecha = new Date(fechaAux);
        const fechaActual = new Date(); // Fecha y hora actual
        // Validar si la fecha de detección es superior a la actual
        if (fecha > fechaActual) {
            // setErrors({ fechaDeteccion: "La fecha de detección no puede ser posterior a la fecha y hora actuales." });
            // setIsLoading(false);
            return false;
        }
        return true;
    };

    return (
        <BaseModal show={showModal} onHide={() => handleClose({shouldRefetch: false})} isLoading={false}>
            <CustomModalHeader
                title={t("Nueva Incidencia")}
                variant="primary"
                style={{backgroundColor: "var(--identificativoCliente) !important"}}
            />
            <Form onSubmit={handleSubmit}>
                <Modal.Body style={{minHeight: "55vh"}}>
                    <Row>
                        <DateInput
                            label={t("Fecha Detección")}
                            required
                            name="fechaDeteccion"
                            value={formData.fechaDeteccion}
                            onChange={handleInputChange}
                            md="6"
                        />
                        <TimeInput
                            label={t("Hora Detección")}
                            required
                            name="horaDeteccion"
                            value={formData.horaDeteccion}
                            onChange={handleInputChange}
                            md="6"
                        />
                        <DynamicSmartSelectInputs
                            onChange={handleComponenteChange}
                            md="col-auto"
                            idActivo={idActivo}
                            componentesBase={componentesBaseLevel}
                        />
                        <div className="d-flex gap-4">
                            <CheckInput
                                label={t("Cambio Pieza")}
                                name="cambioPieza"
                                value={formData.cambioPieza}
                                onChange={handleInputChange}
                                md="4"
                                type="switch"
                            />
                            <CheckInput
                                label={t("Afecta producción")}
                                name="afectaProduccion"
                                value={formData.afectaProduccion}
                                onChange={handleInputChange}
                                md="4"
                                type="switch"
                            />
                            <CheckInput
                                label={t("Paro máquina")}
                                name="paroMaquina"
                                value={formData.paroMaquina}
                                onChange={handleInputChange}
                                md="4"
                                type="switch"
                            />
                        </div>
                        <div className="d-flex justify-content-between gap-5">
                            <ListCheckBox
                                textLabel={t("Mecanismos de Fallo")}
                                options={mecanismosFallo}
                                onChange={(value, checked) => handleMecanismoChange(value, checked)}
                            />
                            <ListCheckBox
                                textLabel={t("Incidencias")}
                                options={filteredIncidencias}
                                onChange={(value, checked) => handleIncidenciaChange(value, checked)}
                            />
                        </div>

                        <MostrarErrores errors={errors} />
                    </Row>
                </Modal.Body>
                <Modal.Footer>
                    <Button variant="secondary" onClick={() => handleClose({shouldRefetch: false})} disabled={false}>
                        {t("Cerrar")}
                    </Button>
                    <Button
                        variant="primary"
                        style={{backgroundColor: "var(--identificativoCliente) !important"}}
                        type="submit"
                        disabled={isLoading}>
                        {t("Guardar")}
                        <LoadingSpinner isLoading={isLoading} />
                    </Button>
                </Modal.Footer>
            </Form>
        </BaseModal>
    );
};
