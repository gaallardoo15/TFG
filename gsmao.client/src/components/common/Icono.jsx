import {FontAwesomeIcon} from "@fortawesome/react-fontawesome";

export const Icono = ({name, ...rest}) => {
    return <FontAwesomeIcon icon={name} {...rest} />;
};
