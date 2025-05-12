import {Col, Form} from "react-bootstrap";

import {RequiredSpan} from "./RequiredSpan";

export const FileUploadInput = ({onChange, id, label, required, disabled = false, ...rest}) => (
    <Form.Group as={Col} md="12" controlId={id || label} className="mb-3" {...rest}>
        <Form.Label className="fw-semibold">
            {label}
            {!!required && <RequiredSpan />}
        </Form.Label>
        <Form.Control type="file" onChange={onChange} required={!!required} disabled={!!disabled} />
    </Form.Group>
);
