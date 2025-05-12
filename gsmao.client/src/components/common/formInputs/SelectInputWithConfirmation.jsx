import {useState} from "react";

import {SelectInput} from "./SelectInput";

import {ModalChangeSelectConfirmation} from "@/components/common/modales/ModalChangeSelectConfirmation";
import {useModalState} from "@/hooks/useModalState";

export const SelectInputWithConfirmation = ({
    name,
    label,
    onChange,
    valuesToBeConfirmed = [4, 5],
    message,
    ...rest
}) => {
    const [nextValue, setNextValue] = useState({});
    const [nextDescription, setNextDescription] = useState("descripción del próximo valor (sin inicializar)");
    const {modalState, openModal, closeModal} = useModalState();

    const handleConfirm = (confirm) => {
        console.log("handleConfirm, confirm, nextValue", confirm, nextValue, name);

        if (confirm) {
            onChange({
                target: {
                    name,
                    value: nextValue,
                },
            });
        }
        closeModal();
    };

    const handleInputChange = (event) => {
        const {value, description} = event.target;

        // Si este nuevo valor no necesita confirmación:
        if (!valuesToBeConfirmed.includes(value)) {
            onChange(event);
            return;
        }

        setNextValue(value);
        setNextDescription(description);
        openModal();
    };

    return (
        <>
            <SelectInput name={name} label={label} onChange={handleInputChange} {...rest} />
            {modalState.show && (
                <ModalChangeSelectConfirmation
                    show={true}
                    onConfirm={handleConfirm}
                    onClose={closeModal}
                    label={label}
                    message={message}
                    nextDescription={nextDescription}
                />
            )}
        </>
    );
};
