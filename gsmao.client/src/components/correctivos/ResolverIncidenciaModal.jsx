import {useEffect, useState} from "react";
import {Button, Form, Modal, Row} from "react-bootstrap";
import {useTranslation} from "react-i18next";

import {CLIENT, CONFIG} from "../../../config";
import {DateInput} from "../common/formInputs/DateInput";
import {SmartSelectInput} from "../common/formInputs/SmartSelectInput";
import {TimeInput} from "../common/formInputs/TimeInput";
import {LoadingSpinner} from "../common/loading/LoadingSpinner";
import {BaseModal} from "../common/modales/BaseModal";
import {CustomModalHeader} from "../common/modales/CustomModalHeader";
import {MostrarErrores} from "../common/MostrarErrores";

import {AuthService} from "@/services/AuthService";
import {ordenService} from "@/services/OrdenService";
import {resolucionesService} from "@/services/ResolucionService";

export const ResolverIncidenciaModal = ({
    show: showModal,
    onClose: handleClose,
    initialData: init,
    datosOrden,
    isVisible,
}) => {
    const defaultData = {
        idOrden: null,
        id: null,
        fechaResolucion: "",
        horaResolucion: "",
        fechaDeteccion: "",
        horaDeteccion: "",
        idResolucion: 0,
    };

    const {t} = useTranslation();
    const [formData, setFormData] = useState(defaultData);
    const [errors, setErrors] = useState({});
    const [isLoading, setIsLoading] = useState(false);
    const [permitirModificacion, setPermitirModificacion] = useState(isVisible);

    const isUserAuthorizedToEditFechas = CONFIG[
        CLIENT
    ].logicaDelNegocio.UsuariosPermitidosModificarFechasOrdenCerrada.includes(AuthService.getUserData().UserName)
        ? true
        : false;

    useEffect(() => {
        let init2 =
            Object.keys(init).length !== 0
                ? {
                      ...init,
                      idOrden: datosOrden.id,
                      id: init.id,
                      idResolucion: init.resolucion?.id,
                      fechaResolucion: init.fechaResolucion?.split("T")[0] || new Date().toISOString().split("T")[0],
                      horaResolucion:
                          init.fechaResolucion?.split("T")[1] ||
                          new Date().toTimeString().split(" ")[0].substring(0, 5),
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

        if (name == "fechaResolucion" && value == "") {
            newValue.horaResolucion = "";
            newValue.idResolucion = null;
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
        const newFechaResolucion = formData.fechaResolucion && formData.fechaResolucion + " " + formData.horaResolucion;

        if (!comprobarFechaValida(newFechaDeteccion) || !comprobarFechaValida(newFechaResolucion)) {
            setErrors({
                fechaDeteccion:
                    "Compruebe la fecha de detección o la fecha de resolución, no pueden ser posterior a la fecha actual.",
            });
            setIsLoading(false);
            return;
        }

        // if (!formData.fechaDeteccionVerificada) {
        //     setIsLoading(false);
        //     setErrors(t("Debe aceptar que ha verificado o ajustado la fecha de detección de la incidencia"));
        //     return;
        // }

        const dataToSend = {
            ...formData,
            fechaDeteccion: newFechaDeteccion,
            fechaResolucion: newFechaResolucion,
        };

        ordenService.editarResolucionOrden(dataToSend).then((response) => {
            if (!response.is_error) {
                handleClose({shouldRefetch: true});
            } else {
                setErrors(response.error_content);
            }
            setIsLoading(false);
        });
    };

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
            <CustomModalHeader
                title={formData.idResolucion ? t("Editar Resolución") : t("Resolver Incidencia")}
                variant="success"
            />
            <Form onSubmit={handleSubmit}>
                <Modal.Body>
                    <Row>
                        <DateInput
                            label={t(
                                `Fecha Detección ${init.fechaDeteccion ? "(Antigua: " + FormatearFechaEspañol(init.fechaDeteccion).split(" ")?.[0] + ")" : ""}`,
                            )}
                            required
                            name="fechaDeteccion"
                            value={formData.fechaDeteccion}
                            onChange={handleInputChange}
                            md="6"
                            disabled={!permitirModificacion || isLoading}
                        />
                        <TimeInput
                            label={t(
                                `Hora Detección ${init.fechaDeteccion ? "(Antigua: " + FormatearFechaEspañol(init.fechaDeteccion).split(" ")?.[1] + ")" : ""}`,
                            )}
                            required
                            name="horaDeteccion"
                            value={formData.horaDeteccion}
                            onChange={handleInputChange}
                            md="6"
                            disabled={!permitirModificacion || isLoading}
                        />
                        <DateInput
                            label={t("Fecha Resolución")}
                            required={formData.idResolucion > 0 || formData.fechaResolucion}
                            name="fechaResolucion"
                            value={formData.fechaResolucion}
                            onChange={handleInputChange}
                            md="6"
                            disabled={!permitirModificacion || isLoading}
                        />
                        <TimeInput
                            label={t("Hora Resolución")}
                            required={formData.idResolucion > 0 || formData.fechaResolucion}
                            name="horaResolucion"
                            value={formData.horaResolucion}
                            onChange={handleInputChange}
                            md="6"
                            disabled={!permitirModificacion || isLoading}
                        />
                        <SmartSelectInput
                            label={t("Resolución")}
                            required={formData.fechaResolucion && formData.horaResolucion}
                            name="idResolucion"
                            value={formData.idResolucion}
                            fetcher={resolucionesService.fetchAll}
                            valueKey="id"
                            labelKey="descripcionES"
                            onChange={handleInputChange}
                            disabled={!isVisible || isLoading}
                            md="12"
                        />
                        {/* <CheckInput
                            label={
                                <span className="fw-semibold">
                                    {t("He verificado o ajustado la fecha de detección de la incidencia")}
                                </span>
                            }
                            name="fechaDeteccionVerificada"
                            value={formData.fechaDeteccionVerificada}
                            onChange={handleInputChange}
                            md="12"
                            disabled={!isVisible || isLoading}
                        /> */}

                        <MostrarErrores className="text-danger mt-3" errors={errors} />
                    </Row>
                </Modal.Body>
                <Modal.Footer>
                    <Button variant="secondary" onClick={() => handleClose({shouldRefetch: false})} disabled={false}>
                        {t("Cerrar")}
                    </Button>
                    <Button variant="success" type="submit" disabled={!isVisible || isLoading}>
                        {t("Guardar")}
                        <LoadingSpinner isLoading={isLoading} />
                    </Button>
                </Modal.Footer>
            </Form>
        </BaseModal>
    );
};
