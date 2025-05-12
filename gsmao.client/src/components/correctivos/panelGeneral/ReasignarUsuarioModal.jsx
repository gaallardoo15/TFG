import {useEffect, useState} from "react";
import {Button, Form, Modal, Row} from "react-bootstrap";
import {useTranslation} from "react-i18next";

import {SmartSelectInput} from "@/components/common/formInputs/SmartSelectInput";
import {LoadingSpinner} from "@/components/common/loading/LoadingSpinner";
import {BaseModal} from "@/components/common/modales/BaseModal";
import {CustomModalHeader} from "@/components/common/modales/CustomModalHeader";
import {MostrarErrores} from "@/components/common/MostrarErrores";
import {ordenService} from "@/services/OrdenService";
import {panelGeneralService} from "@/services/PanelGeneralService";

export const ReasignarUsuarioModal = ({show: showModal, onClose: handleClose, usuarioOrigen, idOrden}) => {
    const defaultData = {
        idOrden: null,
        idUsuarioOrigen: null,
        idUsuarioDestino: null,
    };
    console.log(usuarioOrigen);

    const {t} = useTranslation();
    const [formData, setFormData] = useState(defaultData);
    const [errors, setErrors] = useState({});
    const [isLoading, setIsLoading] = useState(false);

    useEffect(() => {
        let init2 =
            Object.keys(usuarioOrigen).length !== 0
                ? {
                      ...usuarioOrigen,
                      idUsuarioOrigen: usuarioOrigen.id,
                      idOrden: idOrden,
                  }
                : {};
        setFormData({...defaultData, ...init2});
        setErrors({});
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [showModal, usuarioOrigen]);

    const handleInputChange = (event) => {
        const {name, value, type, checked} = event.target;

        let newValue = {
            ...formData,
            [name]: type === "checkbox" ? checked : value,
        };

        setFormData(newValue);
        setErrors({});
    };

    const handleSubmit = (event) => {
        event.preventDefault();
        event.stopPropagation();
        setErrors({});
        setIsLoading(true);

        panelGeneralService.reasignarOrden(formData).then((response) => {
            if (!response.is_error) {
                handleClose({shouldRefetch: true});
            } else {
                setErrors(response.error_content);
            }
            setIsLoading(false);
        });
    };

    console.log(formData);

    return (
        <BaseModal show={showModal} onHide={() => handleClose({shouldRefetch: false})} isLoading={isLoading} size="xs">
            <CustomModalHeader title={t("Reasignar Usuario")} />

            <Form onSubmit={handleSubmit}>
                <Modal.Body>
                    <Row>
                        <p className="fw-semibold">
                            Usuario Actual:{" "}
                            <span className="fw-normal">{usuarioOrigen.nombre + " " + usuarioOrigen.apellidos}</span>
                        </p>
                    </Row>
                    <Row className="">
                        <SmartSelectInput
                            label={t("Nuevo Usuario: ")}
                            required
                            name="idUsuarioDestino"
                            value={formData.idUsuarioDestino}
                            fetcher={() => ordenService.getUsuariosDisponiblesOrden(idOrden)}
                            valueKey="id"
                            labelComponent={(u) => `${u.nombre}  ${u.apellidos}`}
                            onChange={handleInputChange}
                            disabled={isLoading}
                            md="12"
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
                    <Button variant="primary" type="submit" disabled={isLoading}>
                        {t("Guardar")}
                        <LoadingSpinner isLoading={isLoading} />
                    </Button>
                </Modal.Footer>
            </Form>
        </BaseModal>
    );
};
