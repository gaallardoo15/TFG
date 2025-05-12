// import PropTypes from 'prop-types'
import {Dropdown, DropdownButton} from "react-bootstrap";
import {useTranslation} from "react-i18next";

export function LanguageSelect({props, clases}) {
    const {t, i18n} = useTranslation();

    const changeLanguage = (language) => {
        console.log(language);
        i18n.changeLanguage(language);
    };

    const languages = [
        {code: "es", name: "Español", tranlsatedName: t("Español")},
        {code: "en", name: "English", tranlsatedName: t("English")},
        {code: "fr", name: "Français", tranlsatedName: t("Français")},
    ];

    return (
        <DropdownButton
            id="selectorLanguage"
            variant="transparent"
            title={i18n.language}
            size="sm"
            className={"d-inline-block me-2 " + clases}
            {...props}>
            {languages.map((lang) => (
                <Dropdown.Item
                    active={i18n.language == lang.code}
                    onClick={() => changeLanguage(lang.code)}
                    title={t("Cambiar idioma a {{language}}", {language: lang.tranlsatedName})}
                    key={lang.code}>
                    {lang.name}
                </Dropdown.Item>
            ))}
        </DropdownButton>
    );
}

LanguageSelect.propTypes = {};
