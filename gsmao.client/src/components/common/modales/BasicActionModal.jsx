import {useEffect, useState} from "react";
import {Button, Form, Modal} from "react-bootstrap";
import {useTranslation} from "react-i18next";
import PropTypes from "prop-types";

import {Html} from "../Html";
import {CustomModalHeader} from "./CustomModalHeader";

import {LoadingSpinner} from "@/components/common/loading/LoadingSpinner";
import {BaseModal} from "@/components/common/modales/BaseModal";
import {MostrarErrores} from "@/components/common/MostrarErrores";

export function BasicActionModal({
    description,
    modelName,
    action,
    stringAction,
    onClose: handleClose,
    show: showModal,
    cursive = true,
    variant = "primary",
}) {
    const {t} = useTranslation();
    const [errors, setErrors] = useState({});
    const [isLoading, setIsLoading] = useState(false);

    useEffect(() => {
        setErrors({});
    }, []);

    const handleSubmit = (event) => {
        event.preventDefault();
        event.stopPropagation();
        setErrors({});
        setIsLoading(true);
        action().then((response) => {
            if (!response.is_error) {
                handleClose({shouldRefetch: true});
            } else {
                setErrors(response.error_content);
            }
            setIsLoading(false);
        });
    };
    return (
        <BaseModal
            show={showModal}
            onHide={() => handleClose({shouldRefetch: false})}
            size="xs"
            isLoading={isLoading}
            backdrop="static">
            <CustomModalHeader
                title={t("{{stringAction}} {{modelName}}", {stringAction, modelName})}
                variant={variant}
            />
            <Form onSubmit={handleSubmit}>
                <Modal.Body>
                    <Html>
                        {cursive
                            ? t("¿Está seguro de que desea {{stringAction}} <i>{{description}}</i>?", {
                                  stringAction,
                                  description,
                              })
                            : t("¿Está seguro de que desea {{stringAction}} {{description}}?", {
                                  stringAction,
                                  description,
                              })}
                    </Html>
                    <MostrarErrores errors={errors} />
                </Modal.Body>
                <Modal.Footer>
                    <Button variant="secondary" onClick={handleClose} disabled={isLoading}>
                        {t("Cancelar")}
                    </Button>
                    <Button variant={variant} type="submit" disabled={isLoading}>
                        {t("Confirmar")}
                        <LoadingSpinner isLoading={isLoading} />
                    </Button>
                </Modal.Footer>
            </Form>
        </BaseModal>
    );
}

BasicActionModal.propTypes = {
    description: PropTypes.string.isRequired,
    modelName: PropTypes.string.isRequired,
    action: PropTypes.func.isRequired,
    stringAction: PropTypes.string.isRequired,
    onClose: PropTypes.func.isRequired,
    show: PropTypes.bool.isRequired,
    variant: PropTypes.string,
};
