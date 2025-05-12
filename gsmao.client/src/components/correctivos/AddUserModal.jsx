import {useEffect, useState} from "react";
import {Button, Form, Modal, Row} from "react-bootstrap";
import {useTranslation} from "react-i18next";

import {SmartSelectInput} from "../common/formInputs/SmartSelectInput";
import {LoadingSpinner} from "../common/loading/LoadingSpinner";
import {BaseModal} from "../common/modales/BaseModal";
import {CustomModalHeader} from "../common/modales/CustomModalHeader";
import {MostrarErrores} from "../common/MostrarErrores";

import {ordenService} from "@/services/OrdenService";

export const AddUserModal = ({show: showModal, onClose: handleClose, idOrden}) => {
    const defaultData = {
        idOrden: null,
        idUsuario: "",
    };

    const {t} = useTranslation();
    const [formData, setFormData] = useState(defaultData);
    const [errors, setErrors] = useState({});
    const [isLoading, setIsLoading] = useState(false);

    useEffect(() => {
        setFormData({...defaultData, idOrden});
        setErrors({});
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [showModal]);

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

        ordenService.asignarUsuarioOrden(formData.idOrden, formData.idUsuario).then((response) => {
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
            <CustomModalHeader title={t("Asignar Usuario")} variant="success" />

            <Form onSubmit={handleSubmit}>
                <Modal.Body>
                    <Row className="">
                        <SmartSelectInput
                            label={t("Usuario: ")}
                            required
                            name="idUsuario"
                            value={formData.idUsuario}
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
