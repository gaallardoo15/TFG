import {forwardRef, useRef} from "react";
import {Form} from "react-bootstrap";

import {Icono} from "../Icono";
import {RequiredSpan} from "./RequiredSpan";

// eslint-disable-next-line react/display-name
export const CheckInput = forwardRef(
    (
        {
            label,
            type = "checkbox",
            value,
            name,
            onChange,
            required = false,
            disabled = false,
            controlProps,
            warning,
            checked,
            ...rest
        },
        ref,
    ) => {
        // eslint-disable-next-line react-hooks/rules-of-hooks
        const checkboxRef = ref || useRef();

        return (
            <div>
                <div className="d-flex" style={{cursor: "pointer"}}>
                    <Form.Check // prettier-ignore
                        type={type}
                        id={name}
                        name={name}
                        value={value || ""}
                        onChange={onChange}
                        disabled={disabled}
                        checked={checked}
                        ref={checkboxRef}
                        {...controlProps}
                        {...rest}
                    />
                    <Form.Label htmlFor={name} className="mb-0 ms-2" style={{cursor: "pointer"}}>
                        {label}
                    </Form.Label>
                    {!!required && <RequiredSpan className="ms-2 text-danger" />}
                </div>
                {!!warning && (
                    <div className="text-warning mb-2">
                        <div>
                            <Icono name="fa-solid fa-circle-exclamation" />
                            &nbsp;
                            {warning}
                        </div>
                    </div>
                )}
            </div>
        );
    },
);
