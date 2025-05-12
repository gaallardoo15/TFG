export class AuthStore {
    static STORAGE_KEY = "tokenGSMAO";
    static getToken() {
        return window.localStorage.getItem(AuthStore.STORAGE_KEY);
    }
    static setToken(token) {
        window.localStorage.setItem(AuthStore.STORAGE_KEY, token);
    }
    static removeToken() {
        window.localStorage.removeItem(AuthStore.STORAGE_KEY);
    }

    static SETTINGS_STORAGE_KEY = "userSettingsAM";
    static getSettings() {
        return JSON.parse(window.localStorage.getItem(AuthStore.SETTINGS_STORAGE_KEY));
    }
    static setSettings(settings) {
        window.localStorage.setItem(AuthStore.SETTINGS_STORAGE_KEY, JSON.stringify(settings));
    }
    static removeSettings() {
        window.localStorage.removeItem(AuthStore.SETTINGS_STORAGE_KEY);
    }

    static getSetting(settingKey) {
        return AuthStore.getSettings()?.[settingKey];
    }
    static updateSetting(settingKey, settingValue) {
        const current = AuthStore.getSettings();
        AuthStore.setSettings({...current, [settingKey]: settingValue});
    }

    static getDebugRole() {
        return AuthStore.getSettings()?.debugRole;
    }
}
