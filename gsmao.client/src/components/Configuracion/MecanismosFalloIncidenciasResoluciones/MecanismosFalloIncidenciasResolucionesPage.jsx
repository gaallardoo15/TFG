import {useTranslation} from "react-i18next";

import {CustomTabs} from "../../common/CustomTabs";
import {ListaIncidencias} from "./ListaIncidencias";
import {ListaMecanismosFallo} from "./ListaMecanismosFallo";
import {ListaResoluciones} from "./ListaResoluciones";

export const MecanismosFalloIncidenciasResolucionesPage = () => {
    const {t} = useTranslation();
    return (
        <>
            <CustomTabs defaultActiveKey="tabMecanismosFallo">
                <ListaMecanismosFallo eventKey="tabMecanismosFallo" title={t("Tipos de Incidencia")} />
                <ListaIncidencias eventKey="tabIncidencias" title={t("Incidencias")} />
                <ListaResoluciones eventKey="tabResoluciones" title={t("Resoluciones")} />
            </CustomTabs>
        </>
    );
};
