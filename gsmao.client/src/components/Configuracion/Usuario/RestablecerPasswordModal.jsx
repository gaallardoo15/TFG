import {useEffect, useState} from "react";
import {Button, Col, Form, FormGroup, Modal, Row} from "react-bootstrap";
import {t} from "i18next";

import {CustomModalHeader} from "../../common/modales/CustomModalHeader";

import {TextInput} from "@/components/common/formInputs/TextInput";
import {LoadingSpinner} from "@/components/common/loading/LoadingSpinner";
import {BaseModal} from "@/components/common/modales/BaseModal";
import {MostrarErrores} from "@/components/common/MostrarErrores";
import {userService} from "@/services/UserService";

export const RestablecerPasswordModal = ({show: showModal, onClose: handleClose, initialData: init}) => {
    console.log(init);
    const defaultData = {
        id: init.id,
        newPassword: "",
        repeatPassword: "",
    };

    const [errors, setErrors] = useState({});
    const [formData, setFormData] = useState(false);
    const [showPassword, setShowPassword] = useState(false);
    const [isLoading, setIsLoading] = useState(false);

    useEffect(() => {
        let init2 = {...init};
        setFormData({...defaultData, ...init2});
        setErrors({});
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [showModal, init]);

    const handleInputChange = (event) => {
        setErrors({});
        const {name, value, type, checked} = event.target;
        setFormData({
            ...formData,
            [name]: type === "checkbox" ? checked : value,
        });
    };

    const handleSubmit = (event) => {
        event.preventDefault();
        setErrors({});
        let passwordValida = checkPassword();

        if (passwordValida) {
            setIsLoading(true);
            userService.resetPassword(formData).then((response) => {
                if (!response.is_error) {
                    handleClose({shouldRefetch: false});
                } else {
                    setErrors(response.error_content);
                }
                setIsLoading(false);
            });
        } else {
            setErrors(t("Las constraseñas no coinciden"));
        }
    };

    const checkPassword = () => {
        if (formData.newPassword === formData.repeatPassword) {
            return true;
        }
        return false;
    };
    return (
        <BaseModal show={showModal} onHide={() => handleClose({shouldRefetch: false})} size="xs" isLoading={isLoading}>
            <CustomModalHeader title="Restablecer Contraseña" />
            <Form onSubmit={handleSubmit}>
                <Modal.Body>
                    <Row className="">
                        <TextInput
                            label={t("Contraseña")}
                            placeholder={t("Nueva contraseña")}
                            required
                            name="newPassword"
                            type={showPassword ? "text" : "password"}
                            value={formData.newPassword}
                            onChange={handleInputChange}
                            disabled={isLoading}
                            md="12"
                        />

                        <TextInput
                            label={t("Repetir Contraseña")}
                            required
                            name="repeatPassword"
                            type={showPassword ? "text" : "password"}
                            value={formData.repeatPassword}
                            onChange={handleInputChange}
                            disabled={isLoading}
                            md="12"
                        />
                    </Row>
                    <Row>
                        <FormGroup as={Col}>
                            <Form.Check
                                label={t("Mostrar contraseña")}
                                name="showPassword"
                                type="checkbox"
                                id="showPassword"
                                onChange={() => setShowPassword(!showPassword)}
                                disabled={isLoading}
                            />
                        </FormGroup>

                        <MostrarErrores errors={errors} />
                    </Row>
                </Modal.Body>
                <Modal.Footer>
                    <Button variant="secondary" onClick={handleClose} disabled={isLoading}>
                        {t("Cancelar")}
                    </Button>
                    <Button variant="primary" type="submit" disabled={isLoading}>
                        {t("Restablecer")}
                        <LoadingSpinner isLoading={isLoading} />
                    </Button>
                </Modal.Footer>
            </Form>
        </BaseModal>
    );
};
