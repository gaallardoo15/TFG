import {Col, Row} from "react-bootstrap";
import {useTranslation} from "react-i18next";

import {PlantaFormModal} from "./PlantaFormModal";

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
import {plantasService} from "@/services/PlantaService";

export const PlantasPage = () => {
    const {t} = useTranslation();

    const {items: plantas, isLoading, isRefetching, fetchData} = useFetchListData(plantasService.fetchAll);
    const {modalState, openModal, closeModal} = useModalState(fetchData);

    const handleAction = (actionType, id = null) => {
        const targetPlantas = id ? plantas.find((u) => u.id === id) : {};
        openModal(actionType, targetPlantas);
    };

    const ActionsComponent = (props) => {
        const actionByRole = {
            OPERARIO: [],
            RESPONSABLE_MATERIALES: [],
            RESPONSABLE_TALLER: [],
            RESPONSABLE: [],
            JEFE_MANTENIMIENTO: [],
            ADMINISTRADOR: [],
            SUPER_ADMINISTRADOR: ["edit", "delete"],
        };

        const availableActions = actionByRole[AuthService.getUserRole()];

        const buttonByAction = {
            edit: (
                <BtnTabla
                    key="edit"
                    icono="fa-solid fa-pen"
                    title={t("Editar Planta")}
                    onClick={() => handleAction("form", props.value)}
                    disabled={isRefetching}
                />
            ),
            delete: (
                <BtnTabla
                    key="delete"
                    icono="fa-solid fa-trash"
                    title={t("Eliminar planta")}
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
                        <BtnCrearElemento elemento={t("Planta")} onClick={() => handleAction("form")} />
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
                                    <PlantaFormModal show={true} onClose={closeModal} initialData={modalState.target} />
                                );
                            case "delete":
                                return (
                                    <BasicActionModal
                                        show={true}
                                        onClose={closeModal}
                                        action={() => plantasService.delete(modalState.target.id)}
                                        stringAction={t("eliminar")}
                                        description={modalState.target.descripcion}
                                        modelName={t("Planta")}
                                        variant="danger"
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
                    data={plantas}
                    columnDefs={[
                        {
                            field: "descripcion",
                            headerName: t("Descripción"),
                            flex: 6,
                            minWidth: 110,
                            editable: false,
                        },
                        {
                            field: "latitud",
                            headerName: t("Latitud"),
                            flex: 4,
                            minWidth: 110,
                            valueGetter: (e) => e.data?.latitud || "-",
                            editable: false,
                        },
                        {
                            field: "longitud",
                            headerName: t("longitud"),
                            flex: 4,
                            minWidth: 110,
                            valueGetter: (e) => e.data?.longitud || "-",
                            editable: false,
                        },
                        {
                            field: "empresa",
                            headerName: t("Empresa"),
                            flex: 4,
                            minWidth: 110,
                            valueGetter: (e) => e.data?.empresa?.descripcion,
                            editable: false,
                            filter: DatatableSelectFilter,
                        },
                        {
                            field: "id",
                            headerName: "",
                            cellRenderer: ActionsComponent,
                            flex: 1,
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
