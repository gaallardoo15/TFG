import {Col, Row} from "react-bootstrap";
import {useTranslation} from "react-i18next";

import {BtnCrearElemento} from "../../common/Buttons/BtnCrearElemento";
import {BtnTabla} from "../../common/Buttons/BtnTabla";
import {SimpleLoadingMessage} from "../../common/loading/SimpleLoadingMessage";
import {TableComponent} from "../../common/TableComponent";
import {EmpresaFormModal} from "./EmpresaFormModal";

import {CeldaBotonesTabla} from "@/components/common/CeldaBotonesTabla";
import {useFetchListData} from "@/hooks/useFetchData";
import {useModalState} from "@/hooks/useModalState";
import {AuthService} from "@/services/AuthService";
import {empresasService} from "@/services/EmpresaService";

export const EmpresasPage = () => {
    const {t} = useTranslation();

    const {items: empresas, isLoading, isRefetching, fetchData} = useFetchListData(empresasService.fetchAll);
    const {modalState, openModal, closeModal} = useModalState(fetchData);

    const handleAction = (actionType, id = null) => {
        const targetEmpresa = id ? empresas.find((u) => u.id === id) : {};
        openModal(actionType, targetEmpresa);
    };

    const ActionsComponent = (props) => {
        const actionByRole = {
            OPERARIO: [],
            RESPONSABLE_MATERIALES: [],
            RESPONSABLE_TALLER: [],
            RESPONSABLE: [],
            JEFE_MANTENIMIENTO: [],
            ADMINISTRADOR: [],
            SUPER_ADMINISTRADOR: ["edit"],
        };

        const availableActions = actionByRole[AuthService.getUserRole()];

        const buttonByAction = {
            edit: (
                <BtnTabla
                    key="edit"
                    icono="fa-solid fa-pen"
                    title={t("Editar Empresa")}
                    onClick={() => handleAction("form", props.value)}
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
                        <BtnCrearElemento elemento={t("Empresa")} onClick={() => handleAction("form")} />
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
                                    <EmpresaFormModal
                                        show={true}
                                        onClose={closeModal}
                                        initialData={modalState.target}
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
                    data={empresas}
                    tableId="tablaEmpresas"
                    columnDefs={[
                        {
                            field: "descripcion",
                            headerName: t("Descripción"),
                            flex: 6,
                            minWidth: 110,
                            editable: false,
                            floatingFilter: false,
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
