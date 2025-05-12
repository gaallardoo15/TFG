import {initReactI18next} from "react-i18next";
import i18n from "i18next";
import LanguageDetector from "i18next-browser-languagedetector";
import HttpBackend from "i18next-http-backend";

i18n.use(HttpBackend)
    .use(LanguageDetector)
    .use(initReactI18next)
    .init({
        fallbackLng: "es",
        debug: false,
        returnEmptyString: false,
        detection: {
            order: ["queryString", "cookie", "localStorage", "sessionStorage", "navigator", "htmlTag"],
            caches: ["localStorage", "cookie"],
        },
        backend: {
            loadPath: "/traducciones/{{lng}}/{{ns}}.json",
        },
    });

export default i18n;
