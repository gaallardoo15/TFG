import {useEffect, useState} from "react";
import {NavLink, useNavigate} from "react-router-dom";

import {CLIENT, CONFIG} from "../../../../config";
import {OffCanvas} from "./OffCanvas";

import {redirections, RoutePaths} from "@/components/App";
import {Icono} from "@/components/common/Icono";
import {AuthService} from "@/services/AuthService";

const authService = new AuthService();

export const Header = () => {
    const navigate = useNavigate();
    const [navegacion, setNavegacion] = useState("#");

    useEffect(() => {
        const userRol = AuthService.getUserRole();
        setNavegacion(redirections[userRol]);
    }, []);

    const signOut = () => {
        authService.signOut();
        navigate(RoutePaths.SignIn.path, {replace: true, state: {signedOut: true}});
    };

    return (
        <header>
            <nav className="menuTop navbar navbar-expand-lg fixed-top mb-5">
                <div className="containerMenuTop">
                    <div className="containerBtnMenuLateralHeader">
                        <OffCanvas />
                    </div>
                    <div className="containerLogoHeader">
                        <NavLink to={navegacion} className="tituloMenuTop">
                            {CONFIG[CLIENT].tituloPrincipal}
                            {/* GSMAO */}
                        </NavLink>
                    </div>
                    <div className="containerLangLogOutHeader">
                        {/* <LanguageSelect clases="languageSelect" /> */}
                        <button className="btn btnCerrarSesion" type="button" onClick={signOut}>
                            <Icono name="fa-solid fa-arrow-right-from-bracket" style={{color: "white"}} />
                        </button>
                    </div>
                </div>
            </nav>
        </header>
    );
};
