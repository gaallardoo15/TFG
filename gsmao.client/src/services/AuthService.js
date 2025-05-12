import {RestUtilities} from "./RestUtilities";

import {parseJwt} from "@/utils/parseJwt";
import {AuthStore} from "@/utils/stores/Auth";

export class AuthService {
    static isSignedIn() {
        return !!AuthStore.getToken();
    }

    static getUserData() {
        return AuthStore.getToken()
            ? Object.entries(parseJwt(AuthStore.getToken())).reduce((acc, [key, value]) => {
                  if (key.startsWith("user.")) {
                      acc[key.slice(5)] = value;
                  } else if (key.startsWith("http://schemas.microsoft.com/ws/2008/06/identity/claims/role")) {
                      acc.role = value;
                  }
                  return acc;
              }, {})
            : {};
    }

    static isDebug() {
        return AuthService.getUserData().role == "Superadministrador";
    }

    static getUserRole() {
        return AuthService.isDebug()
            ? AuthStore.getDebugRole() || AuthService.getUserData().role
            : AuthService.getUserData().role;
    }

    static getUserNotify() {
        return ["True", "true", "1", "enabled", "Enabled"].includes(AuthService.getUserData()?.notify);
    }

    signInOrRegister(email, password, isRegister = false) {
        return RestUtilities.post(`/api/Login/${isRegister ? "register" : "login"}`, {email: email, password}).then(
            (response) => {
                if (!response.is_error) {
                    AuthStore.setToken(response.content.token);
                }
                return response;
            },
        );
    }

    signIn(email, password) {
        return this.signInOrRegister(email, password, false);
    }

    register(email, password) {
        return this.signInOrRegister(email, password, true);
    }

    confirm(token) {
        return RestUtilities.post("/api/login/confirm", {token: token})
            .then(() => {
                return true;
            })
            .catch((err) => {
                console.error(err);
                return false;
            });
    }

    signOut() {
        AuthStore.removeToken();
    }
}
