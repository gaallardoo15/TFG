import {useState} from "react";
import {useTranslation} from "react-i18next";

import {CrearIncidenciaModal} from "../CrearIncidenciaModal";
import {TablaIncidenciasOrden} from "../TablaIncidenciasOrden";

import {BtnCrearElemento} from "@/components/common/Buttons/BtnCrearElemento";
import {SimpleLoadingMessage} from "@/components/common/loading/SimpleLoadingMessage";
import {useModalState} from "@/hooks/useModalState";

export const IncidenciasOrdenFormModal = ({formdata, activeTab, isVisible, actualizarIncidencias}) => {
    const {t} = useTranslation();
    const {modalState: modalState, openModal, closeModal} = useModalState();
    const [isLoading, setIsLoading] = useState(false);
    const [lastUpdate, setLastUpdate] = useState(new Date().toISOString());

    const handleAction = (actionType, id = null) => {
        openModal(actionType, id);
    };

    const handleClose = ({shouldRefetch}) => {
        closeModal();
        if (shouldRefetch) {
            setLastUpdate(new Date().toISOString());
        }
    };

    return (
        <div>
            <div>
                <div className="d-flex justify-content-between">
                    <div>
                        <BtnCrearElemento
                            elemento={t("Incidencia")}
                            onClick={() => handleAction("form")}
                            disabled={!isVisible}
                        />
                    </div>
                    <div className="d-flex align-items-center gap-3">
                        {isLoading && <SimpleLoadingMessage message={t("Recargando")} />}
                    </div>
                </div>
                {modalState.show &&
                    (() => {
                        switch (modalState.type) {
                            case "form":
                                return (
                                    <CrearIncidenciaModal
                                        show={true}
                                        onClose={handleClose}
                                        idActivo={formdata.activo.id}
                                        idOrden={formdata.id}
                                    />
                                );
                            default:
                                return null;
                        }
                    })()}
            </div>
            <div>
                <TablaIncidenciasOrden
                    formdata={formdata}
                    isVisible={isVisible}
                    id="tablaIncidenciasPanelGeneral"
                    key={lastUpdate}
                    actualizarIncidencias={actualizarIncidencias}
                />
            </div>
        </div>
    );
};
