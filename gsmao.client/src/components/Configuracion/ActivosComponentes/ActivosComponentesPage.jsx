import {useState} from "react";
import {useTranslation} from "react-i18next";

import {CustomTabs} from "../../common/CustomTabs";
import {ListaActivos} from "./ListaActivos";
import {ListaComponentes} from "./ListaComponentes";

export const ActivosComponentesPage = () => {
    const {t} = useTranslation();
    const [activeTab, setActiveTab] = useState("tabActivos"); // Tab predeterminado
    return (
        <>
            <CustomTabs defaultActiveKey="tabActivos" onSelect={(key) => setActiveTab(key)}>
                <ListaActivos eventKey="tabActivos" title={t("Activos")} />
                <ListaComponentes
                    eventKey="tabComponentes"
                    title={t("Componentes")}
                    activeTab={activeTab === "tabComponentes"}
                />
            </CustomTabs>
        </>
    );
};
