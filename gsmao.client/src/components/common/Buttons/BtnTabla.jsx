import {Button} from "react-bootstrap";

import {Icono} from "../Icono";

export const BtnTabla = ({icono, clases, nombre, title, variant = "primary", ...rest}) => {
    return (
        <Button title={title} variant={variant} className={`btnTabla btn-sm ${clases}`} {...rest}>
            <Icono name={icono} className="iconoBotonTabla" /> {nombre}
        </Button>
    );
};
