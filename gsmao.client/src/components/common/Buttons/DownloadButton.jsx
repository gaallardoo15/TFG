import {Button} from "react-bootstrap";

import {LoadingSpinner} from "../loading/LoadingSpinner";
import {Icon} from "./Icon";

import {useFileDownloader} from "@/hooks/useFileDownloader";

export const DownloadButton = ({pathFile, url, iconName, ...rest}) => {
    const {isLoading, fileDownload} = useFileDownloader();

    return (
        <Button disabled={isLoading} {...rest} onClick={() => fileDownload(url, pathFile)}>
            {isLoading ? <LoadingSpinner /> : <Icon name={iconName || "fa-solid fa-download"} />}
        </Button>
    );
};
