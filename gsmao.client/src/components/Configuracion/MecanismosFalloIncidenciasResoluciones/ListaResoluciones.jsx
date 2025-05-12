import {Col, Row} from "react-bootstrap";
import {useTranslation} from "react-i18next";

import {BtnCrearElemento} from "../../common/Buttons/BtnCrearElemento";
import {BtnTabla} from "../../common/Buttons/BtnTabla";
import {SimpleLoadingMessage} from "../../common/loading/SimpleLoadingMessage";
import {BasicActionModal} from "../../common/modales/BasicActionModal";
import {TableComponent} from "../../common/TableComponent";
import {ResolucionFormModal} from "./ResolucionFormModal";

import {CeldaBotonesTabla} from "@/components/common/CeldaBotonesTabla";
import {useFetchListData} from "@/hooks/useFetchData";
import {useModalState} from "@/hooks/useModalState";
import {AuthService} from "@/services/AuthService";
import {resolucionesService} from "@/services/ResolucionService";
import {intersect} from "@/utils/intersect";

export const ListaResoluciones = () => {
    const {t} = useTranslation();
    const {items, isLoading, isRefetching, fetchData} = useFetchListData(resolucionesService.fetchAll);

    const {modalState: modalState, openModal, closeModal} = useModalState(fetchData);

    //LOCALIZACIONES
    const handleAction = (actionType, id = null) => {
        const target = id ? items.find((u) => u.id === id) : {};
        openModal(actionType, target);
    };

    const ActionsComponent = (props) => {
        let actionByState = {
            Activo: ["edit", "delete"],
        };
        const actionByRole = {
            OPERARIO: [],
            SUPER_ADMINISTRADOR: ["edit", "delete"],
            ADMINISTRADOR: ["edit", "delete"],
            JEFE_MANTENIMIENTO: ["edit", "delete"],
            RESPONSABLE_TALLER: ["edit", "delete"],
            RESPONSABLE: ["edit", "delete"],
            RESPONSABLE_MATERIALES: ["edit", "delete"],
        };

        const state = "Activo";
        const availableActions = intersect(actionByState[state], actionByRole[AuthService.getUserRole()]);

        const buttonByAction = {
            edit: (
                <BtnTabla
                    key="edit"
                    icono="fa-solid fa-pen"
                    title={t("Editar resolución")}
                    onClick={() => handleAction("form", props.value)}
                    disabled={isRefetching}
                />
            ),
            delete: (
                <BtnTabla
                    key="delete"
                    icono="fa-solid fa-trash"
                    title={t("Eliminar resolución")}
                    variant="danger"
                    onClick={() => handleAction("delete", props.value)}
                    disabled={isRefetching}
                />
            ),
        };

        return <CeldaBotonesTabla availableActions={availableActions} buttonByAction={buttonByAction} />;
    };
    return (
        <>
            <div>
                <Row>
                    <Col>
                        <BtnCrearElemento elemento={t("Resolución")} onClick={() => handleAction("form")} />
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
                                    <ResolucionFormModal
                                        show={true}
                                        onClose={closeModal}
                                        initialData={modalState.target}
                                    />
                                );
                            case "delete":
                                return (
                                    <BasicActionModal
                                        show={true}
                                        onClose={closeModal}
                                        action={() => resolucionesService.delete(modalState.target.id)}
                                        stringAction={t("eliminar")}
                                        description={modalState.target.descripcionES}
                                        modelName={t("Resolución")}
                                        variant="danger"
                                    />
                                );
                            default:
                                return null;
                        }
                    })()}
            </div>
            <div>
                <ResolucionesTable items={items} isLoading={isLoading} ActionsComponent={ActionsComponent} />
            </div>
        </>
    );
};

export const ResolucionesTable = ({items, isLoading, ActionsComponent, ...rest}) => {
    const {t} = useTranslation();
    const columns = [
        {
            field: "descripcionES",
            headerName: t("Resolución"),
            flex: 6,
            minWidth: 300,
        },
        {
            field: "id",
            headerName: "",
            cellRenderer: ActionsComponent,
            flex: 1,
            cellClass: "celdaBotonesTabla",
            minWidth: 85,
            suppressHeaderMenuButton: true,
            floatingFilter: false, // Deshabilitar el filtro flotante
            filter: false, // Deshabilitar el filtro completo para esta columna
            sortable: false,
            hide: !ActionsComponent,
        },
    ];

    return <TableComponent isLoading={isLoading} data={items} columnDefs={columns} tableId="tablaResoluciones" {...rest} />;
};
