import {Button} from "react-bootstrap";
import {useTranslation} from "react-i18next";

import {Icono} from "../Icono";

export const BtnCrearElemento = ({elemento, customText, customIcon, ...rest}) => {
    const {t} = useTranslation();
    return (
        <Button className="btnCrearElemento mb-2" {...rest}>
            <Icono name={`fa-solid fa-${customIcon || "plus"}`} className="icono" />{" "}
            {customText || t("Crear {{elemento}}", {elemento})}
        </Button>
    );
};
