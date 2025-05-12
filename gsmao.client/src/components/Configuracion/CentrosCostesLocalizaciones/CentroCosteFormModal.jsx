import {useEffect, useState} from "react";
import {Button, Form, Modal, Row} from "react-bootstrap";
import {useTranslation} from "react-i18next";

import {SmartSelectInput} from "../../common/formInputs/SmartSelectInput";
import {TextInput} from "../../common/formInputs/TextInput";
import {LoadingSpinner} from "../../common/loading/LoadingSpinner";
import {BaseModal} from "../../common/modales/BaseModal";
import {CustomModalHeader} from "../../common/modales/CustomModalHeader";
import {MostrarErrores} from "../../common/MostrarErrores";

import {centrosCostesService} from "@/services/CentroCosteService";
import {plantasService} from "@/services/PlantaService";

export const CentroCosteFormModal = ({show: showModal, onClose: handleClose, initialData: init}) => {
    const defaultData = {
        id: null,
        descripcionES: "",
        descripcionEN: "",
        centroCosteSAP: "",
        idPlanta: "",
    };

    const {t} = useTranslation();
    const [formData, setFormData] = useState(defaultData);
    const [errors, setErrors] = useState({});
    const [isLoading, setIsLoading] = useState(false);

    useEffect(() => {
        let init2 = Object.keys(init).length !== 0 ? {...init, idPlanta: init.planta?.id} : {};

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

        centrosCostesService.save(formData).then((response) => {
            if (!response.is_error) {
                handleClose({shouldRefetch: true});
            } else {
                if (response.statusCode == 409) {
                    setErrors(t("Ya existe un centro de coste con el SAP introducido."));
                } else {
                    setErrors(response.error_content);
                }
            }
            setIsLoading(false);
        });
    };

    const isEdit = !!init?.id;

    return (
        <BaseModal show={showModal} onHide={() => handleClose({shouldRefetch: false})} isLoading={isLoading}>
            <CustomModalHeader
                title={isEdit ? "Editar Centro de Coste" : "Crear Centro de Coste"}
                variant={isEdit ? "primary" : "success"}
            />

            <Form onSubmit={handleSubmit}>
                <Modal.Body>
                    <Row className="">
                        <TextInput
                            label={t("Descipción ES")}
                            placeholder={t("Ingresa la descripción en español")}
                            required
                            name="descripcionES"
                            value={formData.descripcionES}
                            onChange={handleInputChange}
                            disabled={isLoading}
                            md="12"
                            autoFocus={true}
                        />
                        <TextInput
                            label={t("Descipción EN")}
                            placeholder={t("Ingresa la descripción en inglés")}
                            name="descripcionEN"
                            value={formData.descripcionEN}
                            onChange={handleInputChange}
                            disabled={isLoading}
                            md="12"
                        />
                        <TextInput
                            label={t("Centro de Coste SAP")}
                            placeholder={t("Ingresa el id SAP del centro de coste")}
                            required
                            name="centroCosteSAP"
                            value={formData.centroCosteSAP}
                            onChange={handleInputChange}
                            disabled={isLoading}
                            md="6"
                        />
                        <SmartSelectInput
                            label={t("Planta")}
                            required
                            name="idPlanta"
                            value={formData.idPlanta}
                            fetcher={plantasService.fetchAll}
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
