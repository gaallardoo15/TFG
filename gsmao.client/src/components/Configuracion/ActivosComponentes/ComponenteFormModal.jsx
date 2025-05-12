import {useEffect, useState} from "react";
import {Button, Form, Modal, Row} from "react-bootstrap";
import {useTranslation} from "react-i18next";

import {SelectInput} from "@/components/common/formInputs/SelectInput";
import {SmartSelectInput} from "@/components/common/formInputs/SmartSelectInput";
import {TextInput} from "@/components/common/formInputs/TextInput";
import {LoadingSpinner} from "@/components/common/loading/LoadingSpinner";
import {BaseModal} from "@/components/common/modales/BaseModal";
import {CustomModalHeader} from "@/components/common/modales/CustomModalHeader";
import {MostrarErrores} from "@/components/common/MostrarErrores";
import {useFetchItemData} from "@/hooks/useFetchData";
import {activosService} from "@/services/ActivoService";
import {componentesService} from "@/services/ComponenteService";

export const ComponenteFormModal = ({show: showModal, onClose: handleClose, initialData: init}) => {
    const defaultData = {
        id: null,
        denominacion: "",
        descripcionES: "",
        descripcionEN: "",
        idActivo: null,
        idComponentePadre: null,
    };

    const {t} = useTranslation();
    const [formData, setFormData] = useState(defaultData);
    const [componentesPadre, setComponentesPadre] = useState([]);
    const [errors, setErrors] = useState({});
    const {item, isLoadingDetails} = useFetchItemData(componentesService, init?.id || 0);
    const [isLoading, setIsLoading] = useState(false);

    const isEdit = !!init?.id;

    const fetchComponentesActivo = (idActivo) => {
        if (idActivo) {
            componentesService.getComponentesActivo(idActivo).then((response) => {
                if (!response.is_error) {
                    setComponentesPadre(response.content || []);
                } else {
                    console.error(response.error_content);
                    return [];
                }
            });
        }
    };

    useEffect(() => {
        let item2 =
            Object.keys(init).length !== 0
                ? {...item, idActivo: item.activo?.id, idComponentePadre: item?.componentePadre?.id}
                : {};
        setFormData({...defaultData, ...item2});
        setErrors({});
        fetchComponentesActivo(item.activo?.id);
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [showModal, item]);

    const handleInputChange = (event) => {
        const {name, value, type, checked} = event.target;

        let newValue = {
            ...formData,
            [name]: type === "checkbox" ? checked : value,
        };

        if (name == "idActivo") {
            newValue.idComponentePadre = null;
            setComponentesPadre([]);
            fetchComponentesActivo(value);
        }
        setFormData(newValue);
        setErrors({});
    };

    const handleSubmit = (event) => {
        event.preventDefault();
        setErrors({});
        setIsLoading(true);

        const actionFunction = isEdit ? componentesService.update : componentesService.create;

        actionFunction(formData).then((response) => {
            if (!response.is_error) {
                handleClose({shouldRefetch: true});
            } else {
                setErrors(response.error_content);
            }
            setIsLoading(false);
        });
    };

    return (
        <BaseModal show={showModal} onHide={() => handleClose({shouldRefetch: false})} isLoading={isLoading}>
            <CustomModalHeader
                title={isEdit ? "Editar Componente" : "Crear Componente"}
                variant={isEdit ? "primary" : "success"}
            />

            <Form onSubmit={handleSubmit}>
                <Modal.Body>
                    <Row className="">
                        <TextInput
                            label={t("KKS Componente")}
                            placeholder={t("denominación")}
                            // required
                            name="denominacion"
                            value={formData.denominacion}
                            onChange={handleInputChange}
                            disabled={isLoading}
                            md="4"
                            autoFocus={true}
                        />
                        <SmartSelectInput
                            label={t("Activo")}
                            required={!isEdit}
                            name="idActivo"
                            value={formData.idActivo}
                            fetcher={activosService.fetchAll}
                            valueKey="id"
                            labelComponent={(a) => `${a.id} - ${a.descripcionES}`}
                            onChange={handleInputChange}
                            disabled={isLoading || isEdit}
                            md="8"
                        />
                        <TextInput
                            label={t("Descipción ES")}
                            placeholder={t("Descripción en español")}
                            name="descripcionES"
                            value={formData.descripcionES}
                            onChange={handleInputChange}
                            disabled={isLoading}
                            required
                            md="6"
                        />
                        <TextInput
                            label={t("Descipción EN")}
                            placeholder={t("Descripción en inglés")}
                            name="descripcionEN"
                            value={formData.descripcionEN}
                            onChange={handleInputChange}
                            disabled={isLoading}
                            md="6"
                        />

                        <SelectInput
                            key={formData.idComponentePadre}
                            label={t("Componente Padre")}
                            name="idComponentePadre"
                            value={formData.idComponentePadre}
                            onChange={handleInputChange}
                            options={componentesPadre.map((cp) => ({
                                value: cp.id,
                                label: cp.denominacion + " - " + t(cp.descripcionES),
                            }))}
                            // disabled={isLoading || !formData.idActivo}
                            disabled={isLoading || !formData.idActivo || isEdit}
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
                    <Button variant={isEdit ? "primary" : "success"} type="submit" disabled={isLoading}>
                        {t("Guardar")}
                        <LoadingSpinner isLoading={isLoading} />
                    </Button>
                </Modal.Footer>
            </Form>
        </BaseModal>
    );
};
