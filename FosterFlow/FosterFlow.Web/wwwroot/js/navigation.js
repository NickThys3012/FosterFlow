// Navigation interop for Blazor WASM
window.navigation = window.navigation || {};

window.navigation.goBack = function () {
    if (window.history.length > 1) {
        window.history.back();
        return true;
    }
    return false;
};
