import { useTranslation } from "react-i18next";
import { CustomTabs } from "../../common/CustomTabs"
import { ListaCentrosCostes } from "./ListaCentrosCostes"
import { ListaLocalizaciones} from "./ListaLocalizaciones"


export const CentrosCostesLocalizacionesPage = () => {
    const {t} = useTranslation();
  return (
    <>
        <CustomTabs defaultActiveKey="tabLocalizaciones">
            <ListaLocalizaciones
                eventKey="tabLocalizaciones" 
                title={t("Localizaciones")}
            />
            <ListaCentrosCostes
                eventKey="tabCentrosCostes" 
                title={t("Centros de Costes")}
            />

        </CustomTabs>
    
    </>
  )
}
