import {Col, Row} from "react-bootstrap";
import {useTranslation} from "react-i18next";

import {IncidenciaFormModal} from "./IncidenciaFormModal";

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
import {incidenciasService} from "@/services/IncidenciaService";
import {intersect} from "@/utils/intersect";

export const ListaIncidencias = () => {
    const {t} = useTranslation();
    const {items, isLoading, isRefetching, fetchData} = useFetchListData(incidenciasService.fetchAll);

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
                    title={t("Editar incidencia")}
                    onClick={() => handleAction("form", props.value)}
                    disabled={isRefetching}
                />
            ),
            delete: (
                <BtnTabla
                    key="delete"
                    icono="fa-solid fa-trash"
                    title={t("Eliminar incidencia")}
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
                        <BtnCrearElemento elemento={t("Incidencia")} onClick={() => handleAction("form")} />
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
                                    <IncidenciaFormModal
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
                                        action={() => incidenciasService.delete(modalState.target.id)}
                                        stringAction={t("eliminar")}
                                        description={modalState.target.descripcionES}
                                        modelName={t("Incidencia")}
                                        variant="danger"
                                    />
                                );
                            default:
                                return null;
                        }
                    })()}
            </div>
            <div>
                <IncidenciasTable items={items} isLoading={isLoading} ActionsComponent={ActionsComponent} />
            </div>
        </>
    );
};

export const IncidenciasTable = ({items, isLoading, ActionsComponent, ...rest}) => {
    const {t} = useTranslation();
    const columns = [
        {
            field: "descripcionES",
            headerName: t("Incidencia"),
            flex: 2.5,
            minWidth: 150,
        },
        {
            field: "mecanismoFallo",
            headerName: t("Mecanismo de Fallo"),
            valueGetter: (e) => e.data?.mecanismoDeFallo?.descripcionES,
            valueFormatter: (e) => t(e.value),
            flex: 2.5,
            minWidth: 175,
            filter: DatatableSelectFilter,
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

    return <TableComponent isLoading={isLoading} data={items} columnDefs={columns} tableId="tablaIncidencias" {...rest} />;
};
