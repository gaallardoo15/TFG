import {Button, Modal} from "react-bootstrap";
import {useTranslation} from "react-i18next";

import {BaseModal} from "../../common/modales/BaseModal";
import {CustomModalHeader} from "../../common/modales/CustomModalHeader";
import {ListaUsuarios} from "../ListaUsuarios";
export const UsuariosModal = ({
    show: showModal,
    onClose: handleClose,
    formData,
    registroOrdenes,
    actualizarUsuariosOrden,
}) => {
    const {t} = useTranslation();

    return (
        <BaseModal show={showModal} onHide={() => handleClose({shouldRefetch: false})} isLoading={false}>
            <CustomModalHeader title={t("Usuarios Orden")} variant="primary" />
            <Modal.Body style={{minHeight: "35vh"}}>
                <ListaUsuarios
                    formData={formData}
                    creador={formData?.creador}
                    registroOrdenes={registroOrdenes}
                    actualizarUsuariosOrden={actualizarUsuariosOrden}
                />
            </Modal.Body>
            <Modal.Footer>
                <Button variant="secondary" onClick={() => handleClose({shouldRefetch: false})} disabled={false}>
                    {t("Cerrar")}
                </Button>
            </Modal.Footer>
        </BaseModal>
    );
};
