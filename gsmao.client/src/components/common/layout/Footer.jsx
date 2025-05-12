import {useTranslation} from "react-i18next";

import {CLIENT, CONFIG} from "../../../../config";

export const Footer = () => {
    const {t} = useTranslation();
    return (
        <footer className="footerPrincipal">
            <hr />
            <p>
                &copy; {new Date().getFullYear()} - <b>{CONFIG[CLIENT].tituloPrincipal}</b>.{" "}
                {t("Todos los derechos reservados.")}
                {/* &copy; {new Date().getFullYear()} - <b>GSMAO</b>. {t("Todos los derechos reservados.")} */}
            </p>
        </footer>
    );
};
