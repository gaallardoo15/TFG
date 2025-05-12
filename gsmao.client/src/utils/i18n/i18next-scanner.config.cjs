// eslint-disable-next-line no-undef
module.exports = {
    options: {
        func: {
            list: ["t", "i18next.t", "i18n.t", "NotFound", "BadRequest", "InvalidOperationException"],
            extensions: [".js", ".jsx", ".ts", ".tsx", ".cs"],
        },
        lngs: ["en", "es", "zh"],
        defaultLng: "en",
        resource: {
            loadPath: "public/traducciones/{{lng}}/{{ns}}.json",
            savePath: "public/traducciones/{{lng}}/{{ns}}.json",
            jsonIndent: 2,
            lineEnding: "\n",
        },
        nsSeparator: false,
        keySeparator: false,
        interpolation: {
            prefix: "{{",
            suffix: "}}",
        },
    },
};
