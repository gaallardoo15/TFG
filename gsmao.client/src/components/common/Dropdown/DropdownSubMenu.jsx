import {Dropdown} from "react-bootstrap";

export const DropdownSubMenu = ({title, children, autoclose, ...rest}) => (
    <Dropdown drop="start" className="dropdown-submenu" autoClose={autoclose}>
        <Dropdown.Toggle as="div" className="dropdown-item">
            {title}
        </Dropdown.Toggle>
        <Dropdown.Menu className="contenedorMenuDropdownMultiFiltro" {...rest}>
            {children}
        </Dropdown.Menu>
    </Dropdown>
);
