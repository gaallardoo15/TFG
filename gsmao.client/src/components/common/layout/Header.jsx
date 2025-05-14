// import {useEffect, useState} from "react";
// import {NavLink, useNavigate} from "react-router-dom";

// import {CLIENT, CONFIG} from "../../../../config";
// import {OffCanvas} from "./OffCanvas";

// import {redirections, RoutePaths} from "@/components/App";
// import {Icono} from "@/components/common/Icono";
// import {AuthService} from "@/services/AuthService";

// const authService = new AuthService();

// export const Header = () => {
//     const navigate = useNavigate();
//     const [navegacion, setNavegacion] = useState("#");

//     useEffect(() => {
//         const userRol = AuthService.getUserRole();
//         setNavegacion(redirections[userRol]);
//     }, []);

//     const signOut = () => {
//         authService.signOut();
//         navigate(RoutePaths.SignIn.path, {replace: true, state: {signedOut: true}});
//     };

//     return (
//         <header>
//             <nav className="menuTop navbar navbar-expand-lg fixed-top mb-5">
//                 <div className="containerMenuTop">
//                     <div className="containerBtnMenuLateralHeader">
//                         <OffCanvas />
//                     </div>
//                     <div className="containerLogoHeader">
//                         <NavLink to={navegacion} className="tituloMenuTop">
//                             {CONFIG[CLIENT].tituloPrincipal}
//                             {/* GSMAO */}
//                         </NavLink>
//                     </div>
//                     <div className="containerLangLogOutHeader">
//                         {/* <LanguageSelect clases="languageSelect" /> */}
//                         <button className="btn btnCerrarSesion" type="button" onClick={signOut}>
//                             <Icono name="fa-solid fa-arrow-right-from-bracket" style={{color: "white"}} />
//                         </button>
//                     </div>
//                 </div>
//             </nav>
//         </header>
//     );
// };

import { useEffect, useState } from "react";
import { NavLink, useNavigate } from "react-router-dom";
import { Dropdown } from "react-bootstrap";

import { CLIENT, CONFIG } from "../../../../config";
import { redirections, RoutePaths, canSee, NavRoutes } from "@/components/App";
import { Icono } from "@/components/common/Icono";
import { AuthService } from "@/services/AuthService";

export const Header = () => {
    const navigate = useNavigate();
    const [navegacion, setNavegacion] = useState("#");
    const [userRol, setUserRol] = useState(null);

    useEffect(() => {
        const rol = AuthService.getUserRole();
        setUserRol(rol);
        setNavegacion(redirections[rol]);
    }, []);

    const signOut = () => {
        AuthService.signOut();
        navigate(RoutePaths.SignIn.path, { replace: true, state: { signedOut: true } });
    };

    const renderDropdownMenus = () => {
        return Object.values(NavRoutes).map((route, index) => {
            if (!canSee(userRol, route.navName)) return null;

            if (route.subRoutes && Object.keys(route.subRoutes).length > 0) {
                return (
                    <Dropdown key={index} className="mx-2">
                        <Dropdown.Toggle variant="link" className="text-white dropdown-toggle-custom">
                            
                            {route.navName}
                        </Dropdown.Toggle>
                        <Dropdown.Menu>
                            {Object.values(route.subRoutes).map((subRoute, subIndex) => {
                                if (!canSee(userRol, subRoute.navName)) return null;
                                return (
                                    <Dropdown.Item key={subIndex} as={NavLink} to={subRoute.path}>
                                        
                                        {subRoute.navName}
                                    </Dropdown.Item>
                                );
                            })}
                        </Dropdown.Menu>
                    </Dropdown>
                );
            } else {
                return (
                    <NavLink key={index} to={route.path} className="nav-link text-white mx-2">
                        
                        {route.navName}
                    </NavLink>
                );
            }
        });
    };

    return (
        <header>
            <nav className="menuTop navbar navbar-expand-lg fixed-top mb-5">
                <div className="container-fluid justify-content-between align-items-center px-4">
                    {/* Izquierda: Título */}
                    <NavLink to={navegacion} className="tituloMenuTop">
                        {CONFIG[CLIENT].tituloPrincipal}
                    </NavLink>

                    {/* Centro: Menús desplegables */}
                    <div className="d-flex justify-content-center flex-grow-1">
                        {renderDropdownMenus()}
                    </div>

                    {/* Derecha: Botón cerrar sesión */}
                    <div>
                        <button className="btn btnCerrarSesion" onClick={signOut}>
                            <Icono name="fa-solid fa-arrow-right-from-bracket" style={{ color: "white" }} />
                        </button>
                    </div>
                </div>
            </nav>
        </header>
    );
};

