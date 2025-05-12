import {useState} from "react";
import {Button, Form, Modal} from "react-bootstrap";
import {useTranslation} from "react-i18next";

import {CustomModalHeader} from "./CustomModalHeader";

import {Html} from "@/components/common/Html";
import {LoadingSpinner} from "@/components/common/loading/LoadingSpinner";
import {BaseModal} from "@/components/common/modales/BaseModal";
import {MostrarErrores} from "@/components/common/MostrarErrores";

export function ModalChangeSelectConfirmation({
    show: showModal,
    onClose: handleClose,
    onConfirm,
    message,
    nextDescription,
    label,
}) {
    const {t} = useTranslation();
    const [errors, setErrors] = useState({});
    const [isLoading, setIsLoading] = useState(false);

    // useEffect(() => {
    //     setErrors({});
    // }, []);

    // Para prevenir que se propague el evento submit, se usa el método preventDefault() y stopPropagation()
    const handleSubmit = (event) => {
        event.preventDefault();
        event.stopPropagation();

        onConfirm(true);
    };

    return (
        <BaseModal
            show={showModal}
            onHide={() => handleClose({shouldRefetch: false})}
            size="xs"
            isLoading={isLoading}
            backdrop="static">
            <CustomModalHeader title={t("Confirmar modificación de {{label}}", {label})} />
            <Form onSubmit={handleSubmit}>
                <Modal.Body>
                    <Html>
                        {t(
                            `Se va a modificar el estado de la orden a estado <span class="fw-semibold">{{nextDescription}}</span>?
                            <br/>
                            Al asignar este estado a la orden se asignará automáticamente a los usuarios Responsable de Taller, Responsable de Material y Personal Royse.
                            <span class="fw-semibold">¿Estás de acuerdo?</span>
                            `,
                            {
                                nextDescription,
                            },
                        )}
                    </Html>
                    <MostrarErrores errors={errors} />
                </Modal.Body>
                <Modal.Footer>
                    <Button
                        variant="secondary"
                        onClick={() => {
                            onConfirm(false);
                        }}
                        disabled={isLoading}>
                        {t("No")}
                    </Button>
                    <Button type="submit" disabled={isLoading}>
                        {t("Confirmar")}
                        <LoadingSpinner isLoading={isLoading} />
                    </Button>
                </Modal.Footer>
            </Form>
        </BaseModal>
    );
}
