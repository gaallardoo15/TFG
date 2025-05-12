import {useTranslation} from "react-i18next";

import {AuthStore} from "@/utils/stores/Auth";

export const useFileViewer = () => {
    const {t} = useTranslation();

    const viewFile = async (url) => {
        try {
            const response = await fetch(url, {
                method: "GET",
                headers: {
                    "Content-Type": "application/octet-stream",
                    "Authorization": `Bearer ${AuthStore.getToken()}`,
                },
            });

            if (!response.ok) {
                const errorContent = await response.json();
                // console.log(errorContent);
                // throw new Error(errorContent);
                console.error(t("Hubo un error al abrir el archivo:"), errorContent);
                return errorContent;
            }

            const data = await response.blob();
            const viewUrl = window.URL.createObjectURL(data);
            window.open(viewUrl, "_blank"); // Abre el archivo en una nueva pestaña
        } catch (error) {
            console.error(t("Hubo un error al abrir el archivo:"), error);
            return error;
        }
    };

    return {viewFile};
};
