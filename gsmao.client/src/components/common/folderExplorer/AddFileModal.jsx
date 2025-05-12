import {useEffect, useState} from "react";
import {Button, Form, Modal, Row} from "react-bootstrap";
import {useTranslation} from "react-i18next";

import {FileUploadInput} from "../formInputs/FileUploadInput";
import {LoadingSpinner} from "../loading/LoadingSpinner";
import {BaseModal} from "../modales/BaseModal";
import {CustomModalHeader} from "../modales/CustomModalHeader";
import {MostrarErrores} from "../MostrarErrores";
import imageCompression from "browser-image-compression";

export const AddFileModal = ({
    show: showModal,
    onClose: handleClose,
    initialData: init,
    handleAction,
    existingFiles = [],
    errors,
}) => {
    const {t} = useTranslation();
    const [file, setFile] = useState();
    const [modalErrors, setModalErrors] = useState({});
    const [isLoading, setIsLoading] = useState(false);

    useEffect(() => {
        setModalErrors(errors);
    }, [errors]);

    const handleFileChange = async (event) => {
        const archivo = event.target.files[0];
        if (!archivo) return;
    
        console.log(archivo)
        // Solo comprimir si es una imagen
        if (archivo.type.startsWith("image/")) {
            try {
                const options = {
                    maxSizeMB: 0.5, // Máximo tamaño del archivo en MB (ajustar según necesidad)
                    maxWidthOrHeight: 1024, // Reducir dimensiones si es mayor a 1024px
                    useWebWorker: true, // Mejora el rendimiento
                };

                const archivoComprimido = await imageCompression(archivo, options);

                setFile(archivoComprimido);

            } catch (error) {
                console.error("Error al comprimir la imagen:", error);
            }
        } else {
            setFile(archivo);
        }
    
        setModalErrors({});
    };

    const handleSubmit = async (event) => {
        event.preventDefault();
        event.stopPropagation();
        setModalErrors({});
        setIsLoading(true);

        if (!file) {
            setModalErrors(t("Debe seleccionar un archivo."));
            setIsLoading(false);
            return;
        }

        // Creamos un FormData para enviar el archivo

        const formDataToSend = new FormData();
        formDataToSend.append("file", file, file.name);

        const parentKey = init || "";
        const fileName = formDataToSend.get("file").name;
        const newFileKey = parentKey + fileName;

        if (existingFiles.some((existingFile) => existingFile.key === newFileKey)) {
            setModalErrors(t("El archivo ya existe."));
            setIsLoading(false);
            return;
        }

        // Llamamos a la función de creación de archivo pasando el FormData
        handleAction(parentKey, formDataToSend, setIsLoading);
    };

    return (
        <BaseModal show={showModal} onHide={() => handleClose({shouldRefetch: false})} isLoading={isLoading} size="xs">
            <CustomModalHeader title={t("Adjuntar Archivo")} variant="primary" />

            <Form onSubmit={handleSubmit}>
                <Modal.Body>
                    <Row className="">
                        <FileUploadInput
                            label={t("Seleccione el archivo deseado:")}
                            required
                            onChange={handleFileChange}
                            disabled={isLoading}
                            md="12"
                        />
                        <MostrarErrores errors={modalErrors} />
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
