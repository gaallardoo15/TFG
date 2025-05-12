import {AuthStore} from "@/utils/stores/Auth";

export class RestUtilities {
    static get(url) {
        return RestUtilities.request("GET", url);
    }

    static delete(url) {
        return RestUtilities.request("DELETE", url);
    }

    static put(url, data) {
        return RestUtilities.request("PUT", url, data);
    }

    static post(url, data) {
        return RestUtilities.request("POST", url, data);
    }

    static request(method, url, data = null, authHeader = null) {
        // eslint-disable-next-line no-unused-vars
        let isJsonResponse = false;
        let isBadRequest = false;
        let statusCode = false;
        let body = data;
        let headers = new Headers();

        if (authHeader) {
            headers.set(...authHeader);
        } else if (AuthStore.getToken()) {
            headers.set("Authorization", `Bearer ${AuthStore.getToken()}`);
        }
        headers.set("Accept", "application/json");

        if (data && !(data instanceof FormData)) {
            if (typeof data === "object") {
                body = JSON.stringify(data);
            }
            headers.set("Content-Type", "application/json");
        }

        return fetch(url, {
            method: method,
            headers: headers,
            body: body,
        })
            .then((response) => {
                statusCode = response.status;
                if (response.status == 401) {
                    // Unauthorized; redirect to sign-in
                    AuthStore.removeToken();
                    window.location.replace(`/?expired=1`);
                }

                isBadRequest = !response.ok;

                let responseContentType = response.headers.get("content-type");
                if (responseContentType && responseContentType.indexOf("application/json") !== -1) {
                    isJsonResponse = true;
                    return response.json();
                } else {
                    return response.text();
                }
            })
            .then((responseContent) => {
                let response = {
                    is_error: isBadRequest,
                    error_content: isBadRequest ? responseContent : `"Error ${statusCode}"`,
                    content: isBadRequest ? null : responseContent,
                    statusCode,
                };
                return response;
            });
    }
}
