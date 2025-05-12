import {useEffect, useState} from "react";
import {Button, Form, Modal, Row} from "react-bootstrap";
import {useTranslation} from "react-i18next";

import {CLIENT, CONFIG} from "../../../config";
import {CheckInput} from "../common/formInputs/CheckInput";
import {DateInput} from "../common/formInputs/DateInput";
import {SelectInput} from "../common/formInputs/SelectInput";
import {SmartSelectInput} from "../common/formInputs/SmartSelectInput";
import {TextInput} from "../common/formInputs/TextInput";
import {TimeInput} from "../common/formInputs/TimeInput";
import {LoadingSpinner} from "../common/loading/LoadingSpinner";
import {BaseModal} from "../common/modales/BaseModal";
import {CustomModalHeader} from "../common/modales/CustomModalHeader";
import {MostrarErrores} from "../common/MostrarErrores";

import {AuthService} from "@/services/AuthService";
import {incidenciasService} from "@/services/IncidenciaService";
import {mecanismosFallosService} from "@/services/MecanismoFalloService";
import {ordenService} from "@/services/OrdenService";

export const EditarIncidenciaOrdenModal = ({
    show: showModal,
    onClose: handleClose,
    initialData: init,
    isVisible,
    datosOrden,
}) => {
    const defaultData = {
        id: null,
        idOrden: null,
        fechaDeteccion: "",
        horaDeteccion: "",
        idComponente: null,
        idIncidencia: null,
        idMecanismoFallo: null,
        cambioPieza: false,
        afectaProduccion: false,
        paroMaquina: false,
    };

    const {t} = useTranslation();
    const [formData, setFormData] = useState(defaultData);
    const [errors, setErrors] = useState({});
    const [incidencias, setIncidencias] = useState([]);
    const [filteredIncidencias, setFilteredIncidencias] = useState([]);
    const [isLoading, setIsLoading] = useState(false);
    const [permitirModificacion, setPermitirModificacion] = useState(isVisible);
    //no se si funciona me esta poniendo q es acastro ya

    const isUserAuthorizedToEditFechas = CONFIG[
        CLIENT
    ].logicaDelNegocio.UsuariosPermitidosModificarFechasOrdenCerrada.includes(AuthService.getUserData().UserName)
        ? true
        : false;

    const fetchIncidencias = () => {
        incidenciasService.fetchAll().then((response) => {
            if (!response.is_error) {
                const incidenciasAux = response.content;
                setIncidencias(incidenciasAux);
                //filtrarIncidencias según el mecanismo de fallo escogido
                setFilteredIncidencias(
                    init.incidencia?.mecanismoDeFallo?.id > 0
                        ? incidenciasAux.filter(
                              (incidencia) => incidencia.mecanismoDeFallo.id == init.incidencia?.mecanismoDeFallo?.id,
                          )
                        : incidenciasAux,
                );
            } else {
                setErrors(response.error_content);
            }
            setIsLoading(false);
        });
    };

    useEffect(() => {
        fetchIncidencias();
        let init2 =
            Object.keys(init).length !== 0
                ? {
                      ...init,
                      idOrden: datosOrden?.id,
                      idIncidencia: init.incidencia?.id,
                      idMecanismoFallo: init.incidencia.mecanismoDeFallo?.id,
                      idComponente: init.componente?.id,
                  }
                : {};
        setFormData({...defaultData, ...init2});
        setErrors({});
        //Variable para poder cambiar datosd de la orden si la orden estaba cerrada pero se acaba de marcar el estado a alguno de los estados de abierta
        //datosOrden?.estadoOrden?.id es el estado rescatado de la base de datos
        //datosOrden?.idEstadoOrden es el estado que se ha marcado en la orden en la pantalla de edición de ordenes (selector de estado)
        if (
            datosOrden?.estadoOrden?.id == 2 &&
            ![2, 3, 5, 8].includes(datosOrden?.idEstadoOrden) &&
            isUserAuthorizedToEditFechas
        ) {
            setPermitirModificacion(true);
        }
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [showModal, init]);

    //Inputs para inputs normales
    const handleInputChange = (event) => {
        const {name, value, type, checked} = event.target;
        let newValue = {
            ...formData,
            [name]: type === "checkbox" ? checked : value,
        };

        if (name == "idMecanismoFallo") {
            console.log(incidencias);
            setFilteredIncidencias(
                newValue.idMecanismoFallo > 0
                    ? incidencias.filter((incidencia) => incidencia.mecanismoDeFallo.id == newValue.idMecanismoFallo)
                    : incidencias,
            );
        }

        setFormData(newValue);
        setErrors({});
    };

    const handleSubmit = (event) => {
        event.preventDefault();
        event.stopPropagation();
        setErrors({});
        setIsLoading(true);

        const newFechaDeteccion = formData.fechaDeteccion + " " + formData.horaDeteccion;

        if (!comprobarFechaValida(newFechaDeteccion)) {
            setErrors({fechaDeteccion: "La fecha de detección no puede ser posterior a la fecha y hora actual."});
            setIsLoading(false);
            return;
        }

        const dataToSend = {
            ...formData,
            fechaDeteccion: newFechaDeteccion,
        };

        ordenService.editarIncidenciaOrden(dataToSend).then((response) => {
            if (!response.is_error) {
                handleClose({shouldRefetch: true});
            } else {
                setErrors(response.error_content);
            }
            setIsLoading(false);
        });
    };

    const comprobarFechaValida = (fecha) => {
        const fechaDeteccion = new Date(fecha);
        const fechaActual = new Date(); // Fecha y hora actual
        // Validar si la fecha de detección es superior a la actual
        if (fechaDeteccion > fechaActual) {
            // setErrors({ fechaDeteccion: "La fecha de detección no puede ser posterior a la fecha y hora actuales." });
            // setIsLoading(false);
            return false;
        }
        return true;
    };
    function FormatearFechaEspañol(fecha) {
        if (fecha) {
            // Separar la fecha y la hora
            const partesFecha = fecha.split("T");

            const date = partesFecha[0];
            const hora = partesFecha[1].split(":")[0] + ":" + partesFecha[1].split(":")[1];
            // Convertir la fecha al formato yyyy/mm/dd
            const [year, month, day] = date.split("-");
            const fechaFormateada = `${day}-${month}-${year} ${hora}`;

            return fechaFormateada;
        }
        return;
    }
    return (
        <BaseModal show={showModal} onHide={() => handleClose({shouldRefetch: false})} isLoading={false}>
            <CustomModalHeader title={t("Editar Incidencia")} variant="primary" />
            <Form onSubmit={handleSubmit}>
                <Modal.Body>
                    <Row>
                        <TextInput
                            label={t("ID Orden")}
                            name="idOrden"
                            value={formData.idOrden}
                            onChange={handleInputChange}
                            disabled
                            md="2"
                        />
                        <TextInput
                            label={t("Componente")}
                            name="componente"
                            value={init.componente?.denominacion + " - " + init.componente?.descripcionES}
                            onChange={handleInputChange}
                            disabled
                            md="10"
                        />
                        <DateInput
                            label={t(
                                `Fecha Detección ${init.fechaDeteccion ? "(Antigua: " + FormatearFechaEspañol(init.fechaDeteccion).split(" ")?.[0] + ")" : ""}`,
                            )}
                            required
                            name="fechaDeteccion"
                            value={formData.fechaDeteccion}
                            onChange={handleInputChange}
                            disabled={!permitirModificacion || isLoading}
                            md="6"
                        />
                        <TimeInput
                            label={t(
                                `Hora Detección ${init.fechaDeteccion ? "(Antigua: " + FormatearFechaEspañol(init.fechaDeteccion).split(" ")?.[1] + ")" : ""}`,
                            )}
                            required
                            name="horaDeteccion"
                            value={formData.horaDeteccion}
                            onChange={handleInputChange}
                            disabled={!permitirModificacion || isLoading}
                            md="6"
                        />
                        <SmartSelectInput
                            label={t("Mecanismo de Fallo")}
                            required
                            name="idMecanismoFallo"
                            value={formData.idMecanismoFallo}
                            fetcher={mecanismosFallosService.fetchAll}
                            valueKey="id"
                            labelKey="descripcionES"
                            onChange={handleInputChange}
                            disabled={!isVisible || isLoading}
                            md="6"
                        />
                        <SelectInput
                            key={formData.idIncidencia}
                            required
                            label={t("Incidencia")}
                            name="idIncidencia"
                            value={formData.idIncidencia}
                            onChange={handleInputChange}
                            options={filteredIncidencias.map((cp) => ({value: cp.id, label: t(cp.descripcionES)}))}
                            disabled={!isVisible || isLoading}
                            md="6"
                        />
                        <div className="d-flex gap-4">
                            <CheckInput
                                label={t("Cambio Pieza")}
                                name="cambioPieza"
                                checked={formData.cambioPieza}
                                onChange={handleInputChange}
                                md="4"
                                type="switch"
                                disabled={!isVisible || isLoading}
                            />
                            <CheckInput
                                label={t("Afecta producción")}
                                name="afectaProduccion"
                                checked={formData.afectaProduccion}
                                onChange={handleInputChange}
                                md="4"
                                type="switch"
                                disabled={!isVisible || isLoading}
                            />
                            <CheckInput
                                label={t("Paro máquina")}
                                name="paroMaquina"
                                checked={formData.paroMaquina}
                                onChange={handleInputChange}
                                md="4"
                                type="switch"
                                disabled={!isVisible || isLoading}
                            />
                        </div>

                        <MostrarErrores className="text-danger mt-3" errors={errors} />
                    </Row>
                </Modal.Body>
                <Modal.Footer>
                    <Button variant="secondary" onClick={() => handleClose({shouldRefetch: false})} disabled={false}>
                        {t("Cerrar")}
                    </Button>
                    <Button variant="primary" type="submit" disabled={!permitirModificacion || isLoading}>
                        {t("Guardar")}
                        <LoadingSpinner isLoading={isLoading} />
                    </Button>
                </Modal.Footer>
            </Form>
        </BaseModal>
    );
};
