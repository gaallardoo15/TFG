import {useEffect} from "react";
import {useTranslation} from "react-i18next";

import {BtnCrearElemento} from "../common/Buttons/BtnCrearElemento";
import {BtnTabla} from "../common/Buttons/BtnTabla";
import {CeldaBotonesTabla} from "../common/CeldaBotonesTabla";
import {SimpleLoadingMessage} from "../common/loading/SimpleLoadingMessage";
import {BasicActionModal} from "../common/modales/BasicActionModal";
import {TableComponent} from "../common/TableComponent";
import {AddUserModal} from "./AddUserModal";
import {ReasignarUsuarioModal} from "./panelGeneral/ReasignarUsuarioModal";

import {useFetchListData} from "@/hooks/useFetchData";
import {useModalState} from "@/hooks/useModalState";
import {ordenService} from "@/services/OrdenService";
import {intersect} from "@/utils/intersect";

export const ListaUsuarios = ({
    formData,
    creador,
    isVisible = true,
    registroOrdenes = false,
    actualizarUsuariosOrden,
}) => {
    const {t} = useTranslation();
    const {
        items: usuarios,
        isLoading,
        isRefetching,
        fetchData,
    } = useFetchListData(() =>
        formData.id
            ? ordenService.getUsuariosOrden(formData.id)
            : new Promise((resolve) => {
                  resolve({response: {content: []}});
              }),
    );
    const {modalState: modalState, openModal, closeModal} = useModalState(fetchData);

    //LOCALIZACIONES
    const handleAction = (actionType, id = null) => {
        const target = id ? usuarios.find((u) => u.id === id) : {};
        openModal(actionType, target);
    };

    useEffect(() => {
        fetchData();
    }, [formData.id]);

    useEffect(() => {
        actualizarUsuariosOrden(usuarios);
    }, [usuarios]);

    const ActionsComponent = (props) => {
        let actionByState = {
            "Abierta": ["reassign", "delete"],
            "Cerrada": [],
            "Anulada": [],
            "Abierta: Pendiente Material": ["reassign", "delete"],
            "Cerrada: Pendiente Material": [],
            "Abierta: Material Gestionado": ["reassign", "delete"],
            "Cerrada: Material Gestionado": [],
            "En Curso": ["reassign", "delete"],
            "": ["reassign", "delete"],
        };

        //Si solo hay un usuario, no podremos borrarlo puesto que la orden tiene que tener mínimo un usuario asignado.
        const actionByUnicoUsuario = {
            true: ["reassign"],
            false: ["reassign", "delete"],
        };
        let availableActions = ["delete"];

        if (!registroOrdenes) {
            const state = formData?.estadoOrden?.name || "";
            availableActions = intersect(actionByState[state], actionByUnicoUsuario[usuarios?.length === 1]);
        }

        const buttonByAction = {
            delete: (
                <BtnTabla
                    key="delete"
                    icono="fa-solid fa-trash"
                    title={t("Eliminar usuario")}
                    variant="danger"
                    onClick={() => handleAction("delete", props.value)}
                    disabled={isRefetching}
                />
            ),
            reassign: (
                <BtnTabla
                    key="reassign"
                    icono="fa-solid fa-people-arrows"
                    title={t("Reasignar usuario")}
                    variant="warning"
                    onClick={() => handleAction("reassign", props.value)}
                    disabled={isRefetching}
                />
            ),
        };
        return <CeldaBotonesTabla availableActions={availableActions} buttonByAction={buttonByAction} />;
    };

    return (
        <>
            <div>
                <div className="d-flex justify-content-between">
                    <div>
                        <BtnCrearElemento
                            customText={t("Asignar Usuario")}
                            elemento={t("Usuario")}
                            onClick={() => handleAction("form")}
                            disabled={!isVisible}
                        />
                    </div>
                    <div className="d-flex align-items-center gap-3">
                        {isRefetching && <SimpleLoadingMessage message={t("Recargando")} />}
                        <div>
                            <span className="px-2 py-1 fw-semibold text-primary-emphasis bg-primary-subtle border border-primary-subtle rounded-2">
                                Creador: {creador?.nombre + " " + creador?.apellidos}
                            </span>
                        </div>
                    </div>
                </div>

                {modalState.show &&
                    (() => {
                        switch (modalState.type) {
                            case "form":
                                return <AddUserModal show={true} onClose={closeModal} idOrden={formData.id} />;
                            case "reassign":
                                return (
                                    <ReasignarUsuarioModal
                                        show={true}
                                        onClose={closeModal}
                                        usuarioOrigen={modalState.target}
                                        idOrden={formData.id}
                                    />
                                );
                            case "delete":
                                return (
                                    <BasicActionModal
                                        show={true}
                                        onClose={closeModal}
                                        action={() =>
                                            ordenService.eliminarUsuarioOrden(formData.id, modalState.target.id)
                                        }
                                        stringAction={t("eliminar")}
                                        description={modalState.target.nombre + " " + modalState.target.apellidos}
                                        modelName={t("Usuario")}
                                        variant="danger"
                                    />
                                );
                            default:
                                return null;
                        }
                    })()}
            </div>
            <div>
                <UsuariosOrdenTable
                    items={usuarios}
                    isLoading={isLoading}
                    ActionsComponent={ActionsComponent}
                    isVisible={isVisible}
                />
            </div>
        </>
    );
};

export const UsuariosOrdenTable = ({items, isLoading, ActionsComponent, isVisible, ...rest}) => {
    const {t} = useTranslation();
    const columns = [
        {
            field: "usuario",
            headerName: t("Usuario"),
            valueGetter: (e) => e.data?.nombre + " " + e.data?.apellidos,
            flex: 5,
            filter: true,
            floatingFilter: false,
        },
        {
            field: "id",
            headerName: "",
            cellRenderer: ActionsComponent,
            flex: 1,
            cellClass: "celdaBotonesTabla",
            minWidth: 160,
            suppressHeaderMenuButton: true,
            sortable: false,
            hide: !isVisible,
            tooltipValueGetter: null,
            filter: false,
        },
    ];

    return (
        <div>
            <TableComponent isLoading={isLoading} data={items} columnDefs={columns} pagination={false} globalQuickFilter={false} {...rest} />
        </div>
    );
};
