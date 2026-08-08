window.performIqTheme = {
    storageKey: "performiq.theme",
    getStoredTheme: function () {
        return localStorage.getItem(this.storageKey) || "";
    },
    setStoredTheme: function (theme) {
        localStorage.setItem(this.storageKey, theme);
    },
    prefersDark: function () {
        return window.matchMedia && window.matchMedia("(prefers-color-scheme: dark)").matches;
    }
};
