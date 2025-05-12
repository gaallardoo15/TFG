import {useEffect, useState} from "react";
import {Button, Form, Modal, Row} from "react-bootstrap";
import {useTranslation} from "react-i18next";

import {TextInput} from "../../common/formInputs/TextInput";
import {LoadingSpinner} from "../../common/loading/LoadingSpinner";
import {BaseModal} from "../../common/modales/BaseModal";
import {CustomModalHeader} from "../../common/modales/CustomModalHeader";
import {MostrarErrores} from "../../common/MostrarErrores";

import {SmartSelectInput} from "@/components/common/formInputs/SmartSelectInput";
import {empresasService} from "@/services/EmpresaService";
import {plantasService} from "@/services/PlantaService";

export const PlantaFormModal = ({show: showModal, onClose: handleClose, initialData: init}) => {
    const defaultData = {
        id: null,
        descripcion: "",
        latitud: "",
        longitud: "",
        stmpConfig: "",
        idEmpresa: "",
    };

    const {t} = useTranslation();
    const [formData, setFormData] = useState(defaultData);
    const [errors, setErrors] = useState({});
    const [isLoading, setIsLoading] = useState(false);

    useEffect(() => {
        let init2 = Object.keys(init).length !== 0 ? {...init, idEmpresa: init.empresa?.id} : {};

        setFormData({...defaultData, ...init2});
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

        plantasService.save(formData).then((response) => {
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
                title={isEdit ? "Editar Planta" : "Crear Planta"}
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
                        <TextInput
                            label={t("Latitud")}
                            placeholder={t("Ingresa la latitud")}
                            name="latitud"
                            value={formData.latitud}
                            onChange={handleInputChange}
                            disabled={isLoading}
                            md="6"
                        />
                        <TextInput
                            label={t("Longitud")}
                            placeholder={t("Ingresa la longitud")}
                            name="longitud"
                            value={formData.longitud}
                            onChange={handleInputChange}
                            disabled={isLoading}
                            md="6"
                        />
                        <TextInput
                            label={t("Stmp Config")}
                            placeholder={t("Ingresa la configuración stmp")}
                            name="stmpConfig"
                            value={formData.stmpConfig}
                            onChange={handleInputChange}
                            disabled={isLoading}
                            md="6"
                        />
                        <SmartSelectInput
                            label={t("Empresa")}
                            required
                            name="idEmpresa"
                            value={formData.idEmpresa}
                            fetcher={empresasService.fetchAll}
                            valueKey="id"
                            labelKey="descripcion"
                            onChange={handleInputChange}
                            disabled={isLoading}
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
                    <Button variant={isEdit ? "primary" : "success"} type="submit" disabled={isLoading}>
                        {t("Guardar")}
                        <LoadingSpinner isLoading={isLoading} />
                    </Button>
                </Modal.Footer>
            </Form>
        </BaseModal>
    );
};
