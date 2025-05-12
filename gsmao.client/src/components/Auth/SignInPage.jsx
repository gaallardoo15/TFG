import {useEffect, useState} from "react";
import {useTranslation} from "react-i18next";
import {useLocation, useNavigate} from "react-router-dom";

import {SignInForm} from "./SignInForm";

import {redirections} from "@/components/App";
import {AuthService} from "@/services/AuthService";

export const SignInPage = () => {
    const {t} = useTranslation();
    const navigate = useNavigate();
    const location = useLocation();
    const [initialLoadContent, setInitialLoadContent] = useState();

    useEffect(() => {
        // Comprobamos si el usuario está autenticado aun con el token y si lo está redirijo a donde estuviera
        if (AuthService.isSignedIn()) {
            // Redireccionar si el usuario ya está autenticado.
            console.log("Autenticado.");
        }
    }, []);

    useEffect(() => {
        if (location.state && location.state.signedOut) {
            setInitialLoadContent(
                // <div className="alert alert-info" role="alert">
                <div className="alert alert-danger" role="alert">
                    <strong>{t("Has Cerrado Sesión")}</strong>
                </div>,
            );
        }
    }, [t, location.state, location.search]);

    const handleLogin = () => {
        //Obtenemos el rol y en función del rol dirigimos a una página u a otra
        redirect();
    };

    const redirect = () => {
        if (location.state?.next) {
            navigate(location.state.next);
        }

        //Obtenemos el rol y en función del rol dirigimos a una página u a otra
        const userRol = AuthService.getUserRole();
        navigate(redirections[userRol]);
    };

    return (
        <>
            <SignInForm onLogin={handleLogin} initialLoadContent={initialLoadContent} modalBody={false} />
        </>
    );
};
