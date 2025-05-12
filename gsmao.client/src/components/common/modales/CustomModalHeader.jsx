import {Button, Modal} from "react-bootstrap";
import {useTranslation} from "react-i18next";

export const CustomModalHeader = ({title, variant = "primary", ...rest}) => {
    const {t} = useTranslation();
    return (
        <Modal.Header>
            <Button className="w-100 btnHeaderModal" variant={variant} {...rest}>
                <Modal.Title className="tamanoLetraTituloModal">{t(title)}</Modal.Title>
            </Button>
        </Modal.Header>
    );
};
