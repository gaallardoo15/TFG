import {useEffect, useState} from "react";
import {useTranslation} from "react-i18next";
import {useIdleTimer} from "react-idle-timer";
import {useLocation, useNavigate} from "react-router-dom";

import {Footer} from "./Footer.jsx";
import {Header} from "./Header.jsx";

import {RoutePaths} from "@/components/App.jsx";
import {SingInModal} from "@/components/Auth/SignInModal.jsx";
import {TituloTopPagina} from "@/components/common/TituloTopPagina.jsx";
import {AuthService} from "@/services/AuthService.js";

export const DefaultLayout = ({
    component: Component,
    datos,
    claseContenedorPrincipal = "contenedorPlantillaComponente",
    mostrarTitulo = true,
    ...rest
}) => {
    const {t} = useTranslation();
    const navigate = useNavigate();
    const location = useLocation();
    const [modal, setModal] = useState(null);

    const [isIdle, setIsIdle] = useState(false);
    const handleOnIdle = () => setIsIdle(true);
    const handleOnActive = () => setIsIdle(false);

    useIdleTimer({
        timeout: 1000 * 60 * 60, // 1 hora
        onIdle: handleOnIdle,
        onActive: handleOnActive,
    });

    useEffect(() => {
        setModal(<SingInModal isIdle={isIdle} />);
    }, [isIdle]);

    useEffect(() => {
        if (!AuthService.isSignedIn()) {
            // Redireccionar al SignIn si el usuario no está autenticado
            navigate(RoutePaths.SignIn.path, {state: {next: location.pathname}});
        }
        // eslint-disable-next-line no-constant-binary-expression
        setModal(<SingInModal />);
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, []);
    // Si el usuario está autenticado, mostrar el layout con el componente deseado
    return (
        <div className="bodyLayout">
            <Header />
            {modal}
            <div id={datos.elemento}>
                <div className="bodyContent">
                    {mostrarTitulo && <TituloTopPagina titulo={t(datos.title)} />}
                    <div className={claseContenedorPrincipal}>
                        <Component elemento={t(datos.elemento)} {...rest} />
                    </div>
                </div>
            </div>

            <Footer />
        </div>
    );
};
