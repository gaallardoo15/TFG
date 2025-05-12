import {MaterialesOrden} from "@/components/correctivos/MaterialesOrden";

export const MaterialesFormModal = ({
    onChange: handleInputChange,
    required,
    formData,
    actualizarAdjuntosOrden,
    activeTab,
    isVisible = true,
    isLoading = false,
    ...rest
}) => {
    return (
        <div>
            <MaterialesOrden
                onChange={handleInputChange}
                required={required}
                formData={formData}
                actualizarAdjuntosOrden={actualizarAdjuntosOrden}
                isVisible={isVisible}
                isLoading={isLoading}
                activeTab={activeTab}
                {...rest}
            />
        </div>
    );
};
