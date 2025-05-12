import {Dropdown} from "react-bootstrap";
import {Button} from "react-bootstrap";
import {useTranslation} from "react-i18next";

import {CheckInput} from "../formInputs/CheckInput";

export const DropdownMenuCheckboxes = ({opciones, onclickItem, onclickButton, checked, name, type = "checkbox", ...rest}) => {
    const {t} = useTranslation();

    const handleCheckboxChange = (value) => {
        onclickItem(value);
    };

    return (
        <div>
            <Dropdown.Item as="div" className="d-flex align-items-center">
                <CheckInput
                    type="radio"
                    label={t("Todos")}
                    value={0}
                    name={name}
                    checked={checked(0)} // Controlar el estado del check
                    onChange={() => handleCheckboxChange(0)}
                />
            </Dropdown.Item>
            <Dropdown.Divider />
            <div {...rest}>
            {opciones.map((opcion) => (
                <Dropdown.Item key={opcion.id || opcion} as="div" className="d-flex align-items-center">
                    <CheckInput
                        type={type}
                        label={opcion.name || opcion.siglas || opcion}
                        value={opcion.id || opcion}
                        name={opcion.name || opcion.siglas || opcion}
                        checked={checked(opcion.id || opcion)} // Controlar el estado del check
                        onChange={() => handleCheckboxChange(opcion.id || opcion)}
                    />
                </Dropdown.Item>
            ))}
            </div>
            
            {onclickButton && (
                <div>
                    <Dropdown.Divider />
                    <Dropdown.Item as="div" className="text-center">
                        <Button variant="primary" size="sm" className="w-100" onClick={onclickButton}>
                            Aplicar
                        </Button>
                    </Dropdown.Item>
                </div>
            )}
        </div>
    );
};
