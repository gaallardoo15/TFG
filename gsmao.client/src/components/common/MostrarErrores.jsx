import {useEffect, useState} from "react";
import {useTranslation} from "react-i18next";
import PropTypes from "prop-types";

import {Icono} from "./Icono";

export const MostrarErrores = ({errors, ...rest}) => {
    const [errorList, setErrorList] = useState([]);
    const {t} = useTranslation();

    useEffect(() => {
        if (!errors) {
            return;
        } else if (typeof errors === "string" || errors instanceof String) {
            setErrorList([errors]);
        } else if (typeof errors === "object") {
            if (Array.isArray(errors)) {
                setErrorList(errors);
            } else {
                let tempErrors;
                if (Object.hasOwn(errors, "errors")) {
                    tempErrors = errors.errors;
                } else if (Object.hasOwn(errors, "title")) {
                    tempErrors = [errors.title + (errors.status ? ` (${errors.status})` : "")];
                } else {
                    tempErrors = errors;
                }
                let values = Object.values(tempErrors);
                let temp = [];
                for (const value of values) {
                    if (Array.isArray(value)) {
                        temp = temp.concat(value);
                    } else {
                        temp.push(value);
                    }
                }
                setErrorList(temp);
            }
        }
    }, [errors]);

    return (
        errorList.length !== 0 && (
            <div className="text-danger" {...rest}>
                {errorList.map((error, index) => (
                    <div key={`${index}${error}`}>
                        <Icono name="fa-solid fa-circle-exclamation" />
                        &nbsp;
                        {error}
                    </div>
                ))}
            </div>
        )
    );
};

MostrarErrores.propTypes = {
    errors: PropTypes.oneOfType([PropTypes.string, PropTypes.array, PropTypes.object]),
};
