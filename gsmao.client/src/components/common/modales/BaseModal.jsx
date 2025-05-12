import {Modal} from "react-bootstrap";
import {motion} from "framer-motion";

export function BaseModal({onHide, isLoading, children, show, dialogClassName = "", ...rest}) {
    return (
        <Modal
            show={show}
            size="lg"
            aria-labelledby="contained-modal-title-vcenter"
            centered
            className="modal-dialog-centered"
            backdrop="static"
            onHide={isLoading ? () => {} : onHide} // prevent closing while loading
            dialogClassName={dialogClassName}
            {...rest}>
            {/* Aquí envolvemos el contenido en motion.div para animar solo al abrir */}
            <motion.div initial={{opacity: 0}} animate={{y: 0, opacity: 1}} transition={{duration: 0.5, delay: 0.5}}>
                <div className="custom-content-scroll">{children}</div>
            </motion.div>
        </Modal>
    );
}
