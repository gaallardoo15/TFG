import {useEffect, useState} from "react";
import {Button, Form, Modal, Row} from "react-bootstrap";
import {useTranslation} from "react-i18next";

import {TextInput} from "../formInputs/TextInput";
import {LoadingSpinner} from "../loading/LoadingSpinner";
import {BaseModal} from "../modales/BaseModal";
import {CustomModalHeader} from "../modales/CustomModalHeader";
import {MostrarErrores} from "../MostrarErrores";

export const AddFolderModal = ({
    show: showModal,
    onClose: handleClose,
    initialData: init,
    handleAction,
    isRename,
    existingFiles = [],
    errors,
}) => {
    const defaultData = {
        folderName: "",
        ruta: "",
        rutaNueva: "",
        materiales: false,
    };

    const {t} = useTranslation();
    const [formData, setFormData] = useState(defaultData);
    const [modalErrors, setModalErrors] = useState({});
    const [isLoading, setIsLoading] = useState(false);

    useEffect(() => {
        setModalErrors(errors);
    }, [errors]);

    useEffect(() => {
        let init2 = Object.keys(init).length !== 0 ? {folderName: init.name} : {};
        setFormData({...defaultData, ...init2});
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [showModal, init]);

    const handleInputChange = (event) => {
        const {name, value, type, checked} = event.target;

        let newValue = {
            ...formData,
            [name]: type === "checkbox" ? checked : value,
        };

        setFormData(newValue);
        setModalErrors({});
    };

    const handleSubmit = (event) => {
        event.preventDefault();
        event.stopPropagation();

        setIsLoading(true);

        const parentKey = init || ""; // Usamos la key del elemento actual si es un renombrado
        let rutaCarpetaNueva = "";

        if (!isRename) {
            rutaCarpetaNueva = parentKey + formData.folderName + "/";
        }

        //La barra es un parche para que funcione
        if (existingFiles.some((existingFile) => existingFile.key === rutaCarpetaNueva)) {
            setModalErrors(t("La carpeta ya existe."));
            setIsLoading(false);
            return;
        }

        const formDataToSend = {
            ...formData,
            folderName: formData.folderName.trimEnd(),
        };

        handleAction(parentKey, formDataToSend, setIsLoading);
    };

    return (
        <BaseModal show={showModal} onHide={() => handleClose({shouldRefetch: false})} isLoading={isLoading} size="xs">
            <CustomModalHeader title={isRename ? "Renombrar Carpeta" : "Crear Carpeta"} variant="primary" />

            <Form onSubmit={handleSubmit}>
                <Modal.Body>
                    <Row className="">
                        <TextInput
                            label={t("Introduzca el nombre de la carpeta:")}
                            required
                            onChange={handleInputChange}
                            disabled={isLoading}
                            value={formData.folderName}
                            name="folderName"
                            placeholder={t("Nombre carpeta")}
                            md="12"
                        />
                        <MostrarErrores errors={modalErrors} />
                    </Row>
                </Modal.Body>
                <Modal.Footer>
                    <Button variant="secondary" onClick={() => handleClose()} disabled={isLoading}>
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
