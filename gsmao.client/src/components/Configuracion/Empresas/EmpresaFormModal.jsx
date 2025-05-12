import {useEffect, useState} from "react";
import {Button, Form, Modal, Row} from "react-bootstrap";
import {useTranslation} from "react-i18next";

import {TextInput} from "../../common/formInputs/TextInput";
import {LoadingSpinner} from "../../common/loading/LoadingSpinner";
import {BaseModal} from "../../common/modales/BaseModal";
import {CustomModalHeader} from "../../common/modales/CustomModalHeader";
import {MostrarErrores} from "../../common/MostrarErrores";

import {empresasService} from "@/services/EmpresaService";

export const EmpresaFormModal = ({show: showModal, onClose: handleClose, initialData: init}) => {
    const defaultData = {
        id: null,
        descripcion: "",
    };

    const {t} = useTranslation();
    const [formData, setFormData] = useState(defaultData);
    const [errors, setErrors] = useState({});
    const [isLoading, setIsLoading] = useState(false);

    useEffect(() => {
        setFormData({...defaultData, ...init});
        setErrors({});
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [showModal, init]);

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
        setErrors({});
        setIsLoading(true);

        empresasService.save(formData).then((response) => {
            if (!response.is_error) {
                handleClose({shouldRefetch: true});
            } else {
                setErrors(response.error_content);
            }
            setIsLoading(false);
        });
    };

    const isEdit = !!init?.id;

    return (
        <BaseModal show={showModal} onHide={() => handleClose({shouldRefetch: false})} isLoading={isLoading}>
            <CustomModalHeader
                title={isEdit ? "Editar Empresa" : "Crear Empresa"}
                variant={isEdit ? "primary" : "success"}
            />

            <Form onSubmit={handleSubmit}>
                <Modal.Body>
                    <Row className="">
                        <TextInput
                            label={t("Descipción")}
                            placeholder={t("Ingresa la descripción")}
                            required
                            name="descripcion"
                            value={formData.descripcion}
                            onChange={handleInputChange}
                            disabled={isLoading}
                            md="12"
                            autoFocus={true}
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
                    <Button variant={isEdit ? "primary" : "success"} type="submit" disabled={isLoading}>
                        {t("Guardar")}
                        <LoadingSpinner isLoading={isLoading} />
                    </Button>
                </Modal.Footer>
            </Form>
        </BaseModal>
    );
};
