import {useTranslation} from "react-i18next";
import PropTypes from "prop-types";

export const RequiredSpan = ({requiredCause, ...rest}) => {
    const {t} = useTranslation();
    return (
        <>
            {" "}
            <span
                title={requiredCause ? t("Este campo es requerido: ") + requiredCause : t("Este campo es requerido")}
                className="text-danger"
                style={{cursor: "help"}}
                {...rest}
            >
                *
            </span>
        </>
    );
};

RequiredSpan.propTypes = {
    requiredCause: PropTypes.string,
};
