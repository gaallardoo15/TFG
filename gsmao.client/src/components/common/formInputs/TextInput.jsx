import {Col, Form} from "react-bootstrap";

import {RequiredSpan} from "./RequiredSpan";

import {Icono} from "@/components/common/Icono";

export const TextInput = ({
    value,
    onChange,
    id,
    name,
    label,
    placeholder,
    type = "text",
    required = false,
    disabled = false,
    controlProps,
    warning,
    ...rest
}) => (
    <Form.Group as={Col} className="mb-3" controlId={id || name} {...rest}>
        {label && (
            <Form.Label className="fw-semibold">
                {label}
                {!!required && <RequiredSpan />}
            </Form.Label>
        )}
        <Form.Control
            required={!!required}
            disabled={disabled}
            type={type}
            name={name}
            value={value || ""}
            onChange={onChange}
            placeholder={placeholder || label}
            {...controlProps}
        />
        {!!warning && (
            <div className="text-warning mt-1">
                <div>
                    <Icono name="fa-solid fa-circle-exclamation" />
                    &nbsp;
                    {warning}
                </div>
            </div>
        )}
    </Form.Group>
);

// TextInput.defaultProps = {type: "text", disabled: false, required: false};
