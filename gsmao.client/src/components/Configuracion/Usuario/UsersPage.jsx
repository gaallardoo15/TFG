import {Badge, Col, Row} from "react-bootstrap";
import {useTranslation} from "react-i18next";

import {RestablecerPasswordModal} from "./RestablecerPasswordModal";
import {UserFormModal} from "./UserFormModal";

import {BtnCrearElemento} from "@/components/common/Buttons/BtnCrearElemento";
import {BtnTabla} from "@/components/common/Buttons/BtnTabla";
import {CeldaBotonesTabla} from "@/components/common/CeldaBotonesTabla";
import {DatatableSelectFilter} from "@/components/common/DatatableSelectFilter";
import {SimpleLoadingMessage} from "@/components/common/loading/SimpleLoadingMessage";
import {BasicActionModal} from "@/components/common/modales/BasicActionModal";
import {TableComponent} from "@/components/common/TableComponent";
import {useFetchListData} from "@/hooks/useFetchData";
import {useModalState} from "@/hooks/useModalState";
import {AuthService} from "@/services/AuthService";
import {userService} from "@/services/UserService";
import {intersect} from "@/utils/intersect";

export const UsersPage = () => {
    const {t} = useTranslation();

    const {items: users, isLoading, isRefetching, fetchData} = useFetchListData(userService.fetchAll);
    const {modalState, openModal, closeModal} = useModalState(fetchData);

    const handleAction = (actionType, id = null) => {
        const targetUser = id ? users.find((u) => u.id === id) : {};
        openModal(actionType, targetUser);
    };

    const ActionsComponent = (props) => {
        let actionByState = {
            Activo: ["edit", "delete", "resetPassword", "deactivate"],
            Inactivo: ["edit", "delete", "documents", "reactivate"],
            Borrado: ["restore"],
        };
        const actionByRole = {
            OPERARIO: [],
            SUPER_ADMINISTRADOR: ["reactivate", "deactivate", "restore", "edit", "resetPassword", "delete"],
            ADMINISTRADOR: ["reactivate", "deactivate", "restore", "edit", "resetPassword", "delete"],
        };

        const user = users.filter((u) => u.id == props.value)[0];
        const state = user?.estadoUsuario?.name;
        const availableActions = intersect(actionByState[state], actionByRole[AuthService.getUserRole()]);

        const buttonByAction = {
            restore: (
                <BtnTabla
                    key="restore"
                    icono="fa-solid fa-trash-can-arrow-up"
                    title={t("Restaurar usuario")}
                    variant="success"
                    onClick={() => handleAction("restore", props.value)}
                    disabled={isRefetching}
                />
            ),
            reactivate: (
                <BtnTabla
                    key="reactivate"
                    icono="fa-solid fa-arrow-rotate-right"
                    title={t("Reactivar usuario")}
                    onClick={() => handleAction("reactivate", props.value)}
                    disabled={isRefetching}
                    variant="success"
                />
            ),
            deactivate: (
                <BtnTabla
                    key="deactivate"
                    icono="fa-solid fa-user-clock"
                    title={t("Dar de baja temporalmente")}
                    onClick={() => handleAction("deactivate", props.value)}
                    disabled={isRefetching}
                    variant="dark"
                />
            ),
            edit: (
                <BtnTabla
                    key="edit"
                    icono="fa-solid fa-pen"
                    title={t("Editar usuario")}
                    onClick={() => handleAction("form", props.value)}
                    disabled={isRefetching}
                />
            ),
            resetPassword: (
                <BtnTabla
                    key="resetPassword"
                    icono="fa-solid fa-key"
                    title={t("Restablecer Contraseña")}
                    clases={"btnRestablecerPassword"}
                    onClick={() => handleAction("resetPassword", props.value)}
                    disabled={isRefetching}
                />
            ),
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
        };

        return <CeldaBotonesTabla availableActions={availableActions} buttonByAction={buttonByAction} />;
    };

    return (
        <div className="contenedorComponente">
            <div>
                <Row>
                    <Col md={4}>
                        <BtnCrearElemento elemento={t("Usuario")} onClick={() => handleAction("form")} />
                    </Col>
                    <Col md={4} className="d-flex flex-column justify-content-center align-items-center">
                        {isRefetching && <SimpleLoadingMessage message={t("Recargando")} />}
                    </Col>
                </Row>

                {modalState.show &&
                    (() => {
                        switch (modalState.type) {
                            case "form":
                                return (
                                    <UserFormModal show={true} onClose={closeModal} initialData={modalState.target} />
                                );
                            case "delete":
                                return (
                                    <BasicActionModal
                                        show={true}
                                        onClose={closeModal}
                                        action={() => userService.delete(modalState.target.id)}
                                        stringAction={t("eliminar")}
                                        description={modalState.target.nombre + " " + modalState.target.apellidos}
                                        modelName={t("Usuario")}
                                        variant="danger"
                                    />
                                );
                            case "resetPassword":
                                return (
                                    <RestablecerPasswordModal
                                        show={true}
                                        onClose={closeModal}
                                        initialData={modalState.target}
                                    />
                                );
                            case "restore":
                                return (
                                    <BasicActionModal
                                        show={true}
                                        onClose={closeModal}
                                        action={() => userService.changeState(modalState.target.id, "Activo")}
                                        stringAction={t("restaurar")}
                                        description={modalState.target.nombre + " " + modalState.target.apellidos}
                                        modelName={t("Usuario")}
                                        variant="success"
                                    />
                                );
                            case "reactivate":
                                return (
                                    <BasicActionModal
                                        show={true}
                                        onClose={closeModal}
                                        action={() => userService.changeState(modalState.target.id, "Activo")}
                                        stringAction={t("reactivar")}
                                        description={modalState.target.nombre + " " + modalState.target.apellidos}
                                        modelName={t("Usuario")}
                                    />
                                );
                            case "deactivate":
                                return (
                                    <BasicActionModal
                                        show={true}
                                        onClose={closeModal}
                                        action={() => userService.changeState(modalState.target.id, "Inactivo")}
                                        stringAction={t("dar de baja temporalmente")}
                                        description={modalState.target.nombre + " " + modalState.target.apellidos}
                                        modelName={t("Usuario")}
                                        variant="dark"
                                    />
                                );
                            default:
                                return null;
                        }
                    })()}
            </div>
            <div>
                <TableComponent
                    isLoading={isLoading}
                    data={users}
                    tableId="tablaUsuarios"
                    columnDefs={[
                        {
                            field: "estadoUsuario",
                            headerName: t("Estado"),
                            valueGetter: (e) => e.data?.estadoUsuario?.name,
                            valueFormatter: (e) => t(e.value),
                            cellRenderer: (e) => {
                                let badgeColor = "warning"; // Color por defecto

                                // Asigna colores según el estado
                                if (e.value === "Activo") {
                                    badgeColor = "success";
                                } else if (e.value === "Inactivo") {
                                    badgeColor = "secondary"; // Puedes ajustar a cualquier color de tu preferencia
                                } else if (e.value === "Borrado") {
                                    badgeColor = "danger"; // Puedes ajustar a cualquier color de tu preferencia
                                }

                                return <Badge bg={badgeColor}> {t(e.value)}</Badge>;
                            },
                            minWidth: 80,
                            flex: 0.9,
                            filter: DatatableSelectFilter,
                        },
                        {
                            field: "nombre",
                            headerName: t("Nombre"),
                            flex: 1.2,
                            minWidth: 110,
                            editable: true,
                        },
                        {
                            field: "apellidos",
                            headerName: t("Apellidos"),
                            valueGetter: (e) => e.data?.apellidos || "-",
                            flex: 2,
                            minWidth: 145,
                            editable: true,
                        },
                        {
                            field: "email",
                            headerName: t("Email"),
                            flex: 2,
                            minWidth: 200,
                            editable: true,
                        },
                        {
                            field: "empresa",
                            headerName: t("Empresa"),
                            valueGetter: (e) => e.data?.empresa?.descripcion || "-",
                            flex: 1,
                            minWidth: 120,
                            editable: false,
                            filter: DatatableSelectFilter,
                        },
                        {
                            field: "planta",
                            headerName: t("Planta"),
                            valueGetter: (e) => e.data?.planta?.descripcion || "-",
                            flex: 1.1,
                            minWidth: 150,
                            editable: false,
                            filter: DatatableSelectFilter,
                        },
                        {
                            field: "rol",
                            headerName: t("Rol"),
                            valueGetter: (e) => e.data?.rol?.name,
                            valueFormatter: (e) => t(e.value),
                            flex: 1.5,
                            minWidth: 200,
                            filter: DatatableSelectFilter,
                        },
                        {
                            field: "id",
                            headerName: "",
                            cellRenderer: ActionsComponent,
                            flex: 1.5,
                            minWidth: 150,
                            floatingFilter: false, // Deshabilitar el filtro flotante
                            filter: false, // Deshabilitar el filtro completo para esta columna
                            sortable: false,
                            tooltipValueGetter: () => null,
                        },
                    ]}
                />
            </div>
        </div>
    );
};
