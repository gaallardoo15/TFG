import {useEffect, useState} from "react";
import {Button, Form, Modal} from "react-bootstrap";
import {useTranslation} from "react-i18next";

import {Html} from "../Html";
import {CustomModalHeader} from "../modales/CustomModalHeader";

import {LoadingSpinner} from "@/components/common/loading/LoadingSpinner";
import {BaseModal} from "@/components/common/modales/BaseModal";
import {MostrarErrores} from "@/components/common/MostrarErrores";

export function DeleteModal({
    modelName,
    onClose: handleClose,
    show: showModal,
    initialData: init,
    handleAction,
    isFolder,
    errors,
}) {
    const {t} = useTranslation();
    const [modalErrors, setModalErrors] = useState({});
    const [isLoading, setIsLoading] = useState(false);

    useEffect(() => {
        setModalErrors(errors);
    }, [errors]);

    const handleSubmit = (event) => {
        event.preventDefault();
        event.stopPropagation();
        setIsLoading(true);
        // Usamos la key del elemento actual si es un renombrado
        const formdata = {
            ruta: init || "",
            materiales: false,
        };
        handleAction(formdata, setIsLoading);
    };

    const nombreElemento = init;
    return (
        <BaseModal show={showModal} onHide={() => handleClose()} size="xs" isLoading={isLoading}>
            <CustomModalHeader title={t("Eliminar {{modelName}}", {modelName})} variant="danger" />
            <Form onSubmit={handleSubmit}>
                <Modal.Body>
                    {!isFolder ? (
                        <Html>
                            {t("¿Está seguro de que desea eliminar el archivo <i>{{nombreElemento}}</i>?", {
                                nombreElemento,
                            })}
                        </Html>
                    ) : (
                        <Html>{t("Al confirmar se perderá todo el contenido de esta carpeta. ¿Desea eliminar?")}</Html>
                    )}

                    <MostrarErrores errors={modalErrors} />
                </Modal.Body>
                <Modal.Footer>
                    <Button variant="secondary" onClick={handleClose} disabled={isLoading}>
                        {t("Cancelar")}
                    </Button>
                    <Button variant="danger" type="submit" disabled={isLoading}>
                        {t("Confirmar")}
                        <LoadingSpinner isLoading={isLoading} />
                    </Button>
                </Modal.Footer>
            </Form>
        </BaseModal>
    );
}
