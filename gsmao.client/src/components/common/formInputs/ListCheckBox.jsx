import {FormLabel, ListGroup, ListGroupItem} from "react-bootstrap";

import {CheckInput} from "./CheckInput";

export const ListCheckBox = ({textLabel, options = [], onChange, selectedValues, maxSelectable = 0}) => {
    const handleCheckboxChange = (event) => {
        const {value, checked} = event.target;
        onChange(value, checked);
    };
    const handleContainerClick = (event, optionId) => {
        // Evita interferir si el clic ocurre en el label o el checkbox directamente
        if (event.target.tagName === "INPUT" || event.target.tagName === "LABEL") {
            return;
        }

        // Busca el checkbox en el contenedor y dispara su clic
        const checkbox = event.currentTarget.querySelector(`input[value="${optionId}"]`);
        if (checkbox) {
            checkbox.click();
        }
    };

    return (
        <div className="w-50 mt-3">
            <FormLabel className="fw-semibold">{textLabel}</FormLabel>
            <ListGroup className="scrollable-list">
                {options.map((option, index) => {
                    const isChecked = selectedValues?.includes(option.id || option);
                    const isDisabled =
                        maxSelectable > 0 ? !isChecked && selectedValues?.length >= maxSelectable : false;

                    return (
                        <ListGroupItem
                            key={option.id}
                            onClick={(e) => handleContainerClick(e, option.id || option)}
                            style={{cursor: isDisabled ? "not-allowed" : "pointer"}}
                            disabled={isDisabled}>
                            <CheckInput
                                label={option.descripcionES || option.name || option}
                                value={option.id || option}
                                name={option.descripcionES || option.name || option}
                                onChange={handleCheckboxChange}
                                disabled={isDisabled}
                                {...(isChecked !== undefined ? {checked: isChecked} : {})} // Propiedad condicional
                            />
                        </ListGroupItem>
                    );
                })}
            </ListGroup>
        </div>
    );
};
