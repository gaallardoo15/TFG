import {Badge, Button} from "react-bootstrap";

import {Icono} from "../Icono";
import {LoadingSpinner} from "../loading/LoadingSpinner";

export const CustomButtonIconText = ({
    titulo,
    texto,
    icono,
    type = "button",
    isLoading = false,
    badge = false,
    badgeValue,
    badgeColor = "secondary",
    ...rest
}) => {
    return (
        <>
            <Button
                type={type}
                title={titulo}
                className="w-100 fw-semibold d-flex justify-content-center align-items-center gap-3"
                {...rest}>
                {icono && <Icono name={icono} />}
                {texto}
                {badge && (
                    <Badge pill bg={badgeColor}>
                        {badgeValue}
                    </Badge>
                )}
                {isLoading && <LoadingSpinner isLoading={isLoading} />}
            </Button>
        </>
    );
};
