import {useEffect, useState} from "react";
import {Button, Modal} from "react-bootstrap";
import {useTranslation} from "react-i18next";

import {MaterialesOrden} from "../MaterialesOrden";

import {BaseModal} from "@/components/common/modales/BaseModal";
import {CustomModalHeader} from "@/components/common/modales/CustomModalHeader";
import {MostrarErrores} from "@/components/common/MostrarErrores";

export const MaterialesModal = ({
    show: showModal,
    onClose: handleClose,
    onChange: handleInputChange,
    required,
    formData,
    actualizarAdjuntosOrden,
    ...rest
}) => {
    const {t} = useTranslation();
    const [errors, setErrors] = useState({});
    useEffect(() => {
        setErrors({});
    }, []);

    return (
        <BaseModal show={showModal} onHide={() => handleClose({shouldRefetch: false})} size="lg" backdrop="static">
            <CustomModalHeader title={t("Materiales")} />
            <Modal.Body style={{minHeight: "45vh"}}>
                <MaterialesOrden
                    onChange={handleInputChange}
                    required={required}
                    formData={formData}
                    actualizarAdjuntosOrden={actualizarAdjuntosOrden}
                    {...rest}
                />
                <MostrarErrores errors={errors} />
            </Modal.Body>
            <Modal.Footer>
                <Button variant="secondary" onClick={handleClose}>
                    {t("Cerrar")}
                </Button>
            </Modal.Footer>
        </BaseModal>
    );
};
