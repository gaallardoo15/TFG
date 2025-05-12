import {Spinner} from "react-bootstrap";

export const LoadingSpinner = ({isLoading = true, ...rest}) =>
    isLoading && (
        <>
            {" "}
            <Spinner size="sm" {...rest} />
        </>
    );
