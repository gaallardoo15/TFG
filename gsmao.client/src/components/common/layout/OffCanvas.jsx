import {useState} from "react";
import {Accordion, Nav} from "react-bootstrap";
import Offcanvas from "react-bootstrap/Offcanvas";
import {useTranslation} from "react-i18next";
import {NavLink} from "react-router-dom";
import {faCircleUser} from "@fortawesome/free-solid-svg-icons";
import {faBars} from "@fortawesome/free-solid-svg-icons";
import {FontAwesomeIcon} from "@fortawesome/react-fontawesome";

import {canSee, NavRoutes} from "@/components/App";
import {Icono} from "@/components/common/Icono";
import {AuthService} from "@/services/AuthService";

export const OffCanvas = () => {
    const {t} = useTranslation();
    const [show, setShow] = useState(false);
    const handleClose = () => setShow(false);
    const handleShow = () => setShow(true);
    const userData = AuthService.getUserData();
    const userRol = AuthService.getUserRole();

    const renderNavItems = () => {
        return Object.values(NavRoutes).map((route, index) => {
            // Verifica si el usuario tiene permiso para ver este elemento del menú
            if (canSee(userRol, route.navName)) {
                return (
                    <Accordion key={index} className="accordionMenu">
                        {route.subRoutes && Object.keys(route.subRoutes).length > 0 ? (
                            <>
                                <Accordion.Item className="navItem" eventKey={index.toString()}>
                                    <Accordion.Header>
                                        <Icono name={route.icon} className="iconoNavLat" />
                                        &nbsp;{t(route.navName)}
                                    </Accordion.Header>
                                    <Accordion.Body className="p-0">
                                        {Object.values(route.subRoutes).map((subRoute, subIndex) => {
                                            if (canSee(userRol, subRoute.navName)) {
                                                return (
                                                    <NavLink
                                                        key={subIndex}
                                                        className="subNavItem"
                                                        to={subRoute.path}
                                                        onClick={handleClose}>
                                                        <Icono
                                                            name={subRoute.icon}
                                                            className="iconoNavLat iconoNavLatSubmenu"
                                                        />
                                                        &nbsp;{t(subRoute.navName)}
                                                    </NavLink>
                                                );
                                            }
                                        })}
                                    </Accordion.Body>
                                </Accordion.Item>
                            </>
                        ) : (
                            <NavLink className="navItemSinSubMenu" to={route.path} onClick={handleClose}>
                                <Icono name={route.icon} className="iconoNavLat" />
                                &nbsp;{t(route.navName)}
                            </NavLink>
                        )}
                    </Accordion>
                );
            }
            return null;
        });
    };
    // Función para verificar si el usuario tiene permiso para ver un elemento de menú específico
    const checkUserRole = (role, navName) => {
        let bandera = true;
        switch (role) {
            case "ADMINISTRADOR":
                if (navName == "Plantas" || navName == "Empresas") {
                    bandera = false;
                }
                break;
            // case "Álvaro Moreno":
            //     if (navName == "Usuarios" || navName == "Empresas") {
            //         bandera = false;
            //     }
            //     break;
            case "SUPER_ADMINISTRADOR":
                bandera = true;
                break;
            default:
                bandera = false;
                break;
        }

        return bandera;
    };

    return (
        <>
            <button className="btn btnOffCanvas" onClick={handleShow}>
                <FontAwesomeIcon style={{color: "white"}} icon={faBars} />
            </button>

            <Offcanvas show={show} onHide={handleClose} className="contenedorMenuLateral">
                <Offcanvas.Header className="headerMenuLateral sombra" closeButton>
                    <div className="containerTituloBienvenida">
                        <FontAwesomeIcon icon={faCircleUser} style={{fontSize: 50}} />
                        <div className="">
                            <p className="tituloBienvenida">{t("Bienvenido")}</p>
                            <p className="tituloBienvenida">{`${userData.name} ${userData.lastname || ""}`}</p>
                        </div>
                    </div>
                </Offcanvas.Header>
                <Offcanvas.Body className="p-0">
                    <Nav variant="pills" className="navMenuLateral flex-column">
                        {renderNavItems()}
                    </Nav>
                </Offcanvas.Body>
            </Offcanvas>
        </>
    );
};
