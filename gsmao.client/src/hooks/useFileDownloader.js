import {useState} from "react";
import {useTranslation} from "react-i18next";

import {AuthStore} from "@/utils/stores/Auth";

export const useFileDownloader = () => {
    const [isLoading, setIsLoading] = useState(false);
    const {t} = useTranslation();

    const fileDownload = async (url, path = "") => {
        setIsLoading(true);
        try {
            const response = await fetch(url, {
                method: "GET",
                headers: {
                    "Content-Type": "application/octet-stream",
                    "Authorization": `Bearer ${AuthStore.getToken()}`,
                },
            });

            console.log("is_error: ", response);
            if (!response.ok) {
                const errorContent = await response.text();
                console.error(t("Hubo un error al abrir el archivo:"), errorContent);
                setIsLoading(false);
                return errorContent;
            }

            const data = await response.blob();
            const downloadUrl = window.URL.createObjectURL(data);
            const link = document.createElement("a");
            link.href = downloadUrl;
            link.setAttribute("download", path);
            document.body.appendChild(link);
            link.click();
            link.remove();
        } catch (error) {
            console.error(t("Hubo un error al descargar el archivo:"), error);
            setIsLoading(false);
            return error;
        }
        setIsLoading(false);
    };

    return {isLoading, fileDownload};
};
