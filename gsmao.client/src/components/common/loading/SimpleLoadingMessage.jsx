import {t} from "i18next";

import {LoadingSpinner} from "./LoadingSpinner";

export const SimpleLoadingMessage = ({message}) => (
    <div style={{display: "inline-block", verticalAlign: "middle"}}>
        <i>{message || t("Cargando")}</i>
        <LoadingSpinner />
    </div>
);
