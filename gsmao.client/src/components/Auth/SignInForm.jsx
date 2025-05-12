import {useRef, useState} from "react";
import {Button} from "react-bootstrap";

import {CLIENT, CONFIG} from "../../../config";
import {LoadingSpinner} from "../common/loading/LoadingSpinner";

import {AuthService} from "@/services/AuthService";

const authService = new AuthService();

export const SignInForm = ({initialLoadContent, onLogin, formClassName, modalBody = false}) => {
    const emailRef = useRef(null);
    const passwordRef = useRef(null);
    const [initialLoad, setInitialLoad] = useState(true);
    const [error, setError] = useState(null);
    const [isLoading, setIsLoading] = useState(false);

    const handleSubmit = (event) => {
        setIsLoading(true);
        event.preventDefault();
        //Petición para iniciar sesión
        authService.signIn(emailRef.current.value, passwordRef.current.value).then((response) => {
            if (!response.is_error) {
                onLogin();
            } else {
                setError(response.error_content.error_description);
                setInitialLoad(false);
            }
            setIsLoading(false);
        });
    };

    return (
        <div className={formClassName || "bodyLogin"} id={modalBody ? "modalLogin" : ""}>
            <div className="backgroundImg">
                {/* <div style={{position: "absolute", top: 0, right: 0, margin: "1em"}}>
                    <LanguageSelect variant="outline-dark" />
                </div> */}
                <section className="left-form">
                    {/* <img src="/fondoLogin.png" alt="logoLogin" className="imgFondoLogin" /> */}
                    {CLIENT == "suitelec" && <h1>GSMAO</h1>}
                </section>
                <section className="right-form">
                    <div className="wrapperLogoLogin">
                        {CLIENT == "hitachi" && (
                            <img src="/logoHitachi.png" alt="logoHitachi" className="imgLogoHitachi" />
                        )}
                        {CLIENT == "suitelec" && (
                            <img
                                src="/suitelec_energia_comunicacion_control.svg"
                                alt="logoHitachi"
                                className="imgLogoSuitelec"
                            />
                        )}
                    </div>
                    <div className="formularioLogin">
                        <form onSubmit={handleSubmit}>
                            <h2>INICIAR SESIÓN</h2>

                            {initialLoad && initialLoadContent}
                            {error && (
                                <div className="alert alert-danger" role="alert" style={{whiteSpace: "pre-line"}}>
                                    {error}
                                </div>
                            )}

                            <label htmlFor="inputEmail">Email</label>
                            <input
                                required
                                disabled={isLoading}
                                id="inputEmail"
                                type="email"
                                ref={emailRef}
                                placeholder="Email"
                            />

                            <label htmlFor="inputPassword">Contraseña</label>
                            <input
                                required
                                disabled={isLoading}
                                id="inputPassword"
                                type="password"
                                ref={passwordRef}
                                placeholder="Contraseña"
                            />

                            <Button size="lg" className="btn btnLogin mt-4 w-100" type="submit" disabled={isLoading}>
                                Iniciar sesión
                                <LoadingSpinner isLoading={isLoading} />
                            </Button>
                        </form>
                    </div>
                    <footer className="footerLogin">
                        <p>
                            &copy; {new Date().getFullYear()} - <b>{CONFIG[CLIENT].tituloPrincipal}</b>. Todos los
                            derecho reservados.
                            {/* &copy; {new Date().getFullYear()} - <b>GSMAO</b>. Todos los derecho reservados. */}
                        </p>
                    </footer>
                </section>
            </div>
        </div>
    );
};
