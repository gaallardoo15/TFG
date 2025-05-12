import {useEffect, useState} from "react";
import {Button, Modal} from "react-bootstrap";
import {useTranslation} from "react-i18next";

import {FoldersExplorer} from "@/components/common/folderExplorer/FoldersExplorer";
import {BaseModal} from "@/components/common/modales/BaseModal";
import {CustomModalHeader} from "@/components/common/modales/CustomModalHeader";
import {MostrarErrores} from "@/components/common/MostrarErrores";

export const DocumentsModal = ({idActivo, onClose: handleClose, show: showModal, ...rest}) => {
    const {t} = useTranslation();
    const [errors, setErrors] = useState({});

    useEffect(() => {
        setErrors({});
    }, []);

    return (
        <BaseModal show={showModal} onHide={() => handleClose({shouldRefetch: false})} size="lg" backdrop="static">
            <CustomModalHeader title={t("Documentación")} />
            <Modal.Body style={{minHeight: "35vh"}}>
                <FoldersExplorer id={idActivo} gestor="activos" {...rest} />

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
