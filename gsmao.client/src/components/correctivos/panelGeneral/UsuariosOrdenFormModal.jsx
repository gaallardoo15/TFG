import {ListaUsuarios} from "../ListaUsuarios";

export const UsuariosOrdenFormModal = ({activeTab, formData, creador, isVisible, actualizarUsuariosOrden}) => {
    return (
        <div>
            <ListaUsuarios
                formData={formData}
                creador={creador}
                isVisible={isVisible}
                actualizarUsuariosOrden={actualizarUsuariosOrden}
            />
        </div>
    );
};
