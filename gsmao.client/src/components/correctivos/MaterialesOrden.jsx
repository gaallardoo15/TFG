import {Row} from "react-bootstrap";
import {useTranslation} from "react-i18next";

import {FoldersExplorer} from "../common/folderExplorer/FoldersExplorer";
import {TextAreaInput} from "../common/formInputs/TextAreaInput";
import { useEffect, useState } from "react";

export const MaterialesOrden = ({
    formData,
    actualizarAdjuntosOrden,
    onChange: handleInputChange,
    required,
    isVisible = true,
    isLoading,
    style,
    activeTab,
    ...rest
}) => {
    const {t} = useTranslation();
    const estadosNoDisabled=[1, 4, 5,6, 7, 8]; 
    return (
        <div className="d-flex flex-column justify-content-between">
            <FoldersExplorer
                id={formData.id}
                gestor="ordenes"
                actualizarAdjuntosOrden={actualizarAdjuntosOrden}
                isVisible={estadosNoDisabled.includes(formData?.idEstadoOrden)}
                style={style}
                activeTab={activeTab}
                materiales={true}
            />
            <Row>
                <TextAreaInput
                    label={t("Materiales")}
                    name="materiales"
                    value={formData.materiales}
                    onChange={handleInputChange}
                    md="12"
                    rows={2}
                    disabled={!estadosNoDisabled.includes(formData?.idEstadoOrden) ||  isLoading}
                    placeholder={t("Descripción detallada de los materiales")}
                />
            </Row>
        </div>
    );
};
