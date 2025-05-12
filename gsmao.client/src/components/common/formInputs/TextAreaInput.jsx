import {Col, Form} from "react-bootstrap";

import {RequiredSpan} from "./RequiredSpan";

export const TextAreaInput = ({
    value,
    onChange,
    id,
    name,
    label,
    placeholder,
    required,
    disabled,
    rows = 2,
    ...rest
}) => (
    <Form.Group as={Col} className="mb-3" controlId={id || name} {...rest}>
        <Form.Label className="fw-semibold">
            {label}
            {!!required && <RequiredSpan />}
        </Form.Label>
        <Form.Control
            as="textarea"
            rows={rows}
            required={!!required}
            disabled={disabled}
            name={name}
            value={value}
            onChange={onChange}
            placeholder={placeholder || label}
        />
    </Form.Group>
);

// TextAreaInput.defaultProps = {rows: 2};
