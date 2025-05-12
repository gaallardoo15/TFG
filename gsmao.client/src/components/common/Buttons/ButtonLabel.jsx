import {FormLabel} from "react-bootstrap";

import {CustomButtonIconText} from "./CustomButtonIconText";

export const ButtonLabel = ({titulo, texto, icono, textLabel, badge = false, badgeValue, ...rest}) => {
    return (
        <>
            <FormLabel className="fw-semibold">{textLabel}</FormLabel>
            <CustomButtonIconText
                titulo={titulo}
                texto={texto}
                icono={icono}
                {...rest}
                badge={badge}
                badgeValue={badgeValue}
            />
        </>
    );
};
