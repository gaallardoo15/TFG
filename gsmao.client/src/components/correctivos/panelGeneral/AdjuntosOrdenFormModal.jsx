import {FoldersExplorer} from "@/components/common/folderExplorer/FoldersExplorer";

export const AdjuntosOrdenFormModal = ({
    isLoading,
    formData,
    activeTab,
    isVisible,
    actualizarAdjuntosOrden,
    style,
    ...rest
}) => {
    return (
        <div>
            <FoldersExplorer
                id={formData.id}
                gestor="ordenes"
                isVisible={isVisible}
                actualizarAdjuntosOrden={actualizarAdjuntosOrden}
                style={style}
                activeTab={activeTab}
            />
        </div>
    );
};
