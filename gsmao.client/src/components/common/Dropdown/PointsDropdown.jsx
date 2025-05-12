import {Dropdown} from "react-bootstrap";

import {Icono} from "../Icono";
import {LoadingSpinner} from "../loading/LoadingSpinner";

export const PointsDropdown = ({
    variant,
    actions,
    customIcono = "fa-solid fa-ellipsis-vertical",
    isLoading,
    ...rest
}) => {
    return (
        <Dropdown>
            <Dropdown.Toggle variant={variant} {...rest} className="btnMenuOpciones">
                <Icono name={customIcono} style={{color: "#777777", fontSize: "18px"}} />
            </Dropdown.Toggle>

            <Dropdown.Menu className="contenedorMenuOpciones">
                {actions.map((action, index) => (
                    <Dropdown.Item
                        key={index}
                        onClick={action.onClick}
                        className={action.clases}
                        disabled={action.disabled}>
                        <div className="d-flex justify-content-between gap-2 align-items-center">
                            <div>{action.label}</div>
                            {isLoading && <LoadingSpinner />}
                        </div>
                    </Dropdown.Item>
                ))}
            </Dropdown.Menu>
        </Dropdown>
    );
};
