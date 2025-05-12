import {Button, OverlayTrigger, Popover} from "react-bootstrap";
import {useTranslation} from "react-i18next";

import {Icono} from "./Icono";

export const CustomPopover = ({body, icono = "fa-solid fa-question", size = 13}) => {
    const {t} = useTranslation();

    const popover = (
        <Popover id="popover-basic">
            <Popover.Body>
                {t(body)
                    .split("\n")
                    .map((line, index) => (
                        <span key={index}>
                            {line}
                            <br />
                        </span>
                    ))}
            </Popover.Body>
        </Popover>
    );

    return (
        <OverlayTrigger trigger="click" placement="left" overlay={popover}>
            <Button id="customPopover">
                <Icono name={icono} style={{fontSize: `${size}px`}} />
            </Button>
        </OverlayTrigger>
    );
};
