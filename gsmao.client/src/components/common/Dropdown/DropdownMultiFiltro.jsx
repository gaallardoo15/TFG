import {Dropdown} from "react-bootstrap";

import {Icono} from "../Icono";

export const DropdownMultiFiltro = ({variant, customIcono = "fa-solid fa-filter", children, autoclose, ...rest}) => {
    return (
        <Dropdown autoClose={autoclose}>
            <Dropdown.Toggle variant={variant} {...rest} className="dropdownMultiFiltro m-0">
                <Icono name={customIcono} style={{color: "#777777", fontSize: "18px"}} />
            </Dropdown.Toggle>

            <Dropdown.Menu className="contenedorMenuDropdownMultiFiltro">{children}</Dropdown.Menu>
        </Dropdown>
    );
};
