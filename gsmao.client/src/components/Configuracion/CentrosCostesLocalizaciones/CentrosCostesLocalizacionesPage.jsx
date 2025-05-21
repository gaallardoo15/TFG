import {Col, Row} from "react-bootstrap";
import {useTranslation} from "react-i18next";

import {LocalizacionFormModal} from "./LocalizacionFormModal";

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
import {localizacionesService} from "@/services/LocalizacionService";
import {intersect} from "@/utils/intersect";

export const CentrosCostesLocalizacionesPage = () => {
    const {t} = useTranslation();
    const {
        items: localizaciones,
        isLoading,
        isRefetching,
        fetchData,
    } = useFetchListData(localizacionesService.fetchAll);

    const {modalState: modalState, openModal, closeModal} = useModalState(fetchData);

    //LOCALIZACIONES
    const handleAction = (actionType, id = null) => {
        const target = id ? localizaciones.find((u) => u.id === id) : {};
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
            RESPONSABLE_TALLER: [],
            RESPONSABLE: [],
            RESPONSABLE_MATERIALES: [],
        };

        const state = "Activo";
        const availableActions = intersect(actionByState[state], actionByRole[AuthService.getUserRole()]);

        const buttonByAction = {
            edit: (
                <BtnTabla
                    key="edit"
                    icono="fa-solid fa-pen"
                    title={t("Editar localización")}
                    onClick={() => handleAction("form", props.value)}
                    disabled={isRefetching}
                />
            ),
            delete: (
                <BtnTabla
                    key="delete"
                    icono="fa-solid fa-trash"
                    title={t("Eliminar localización")}
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
                        <BtnCrearElemento elemento={t("Localización")} onClick={() => handleAction("form")} />
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
                                    <LocalizacionFormModal
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
                                        action={() => localizacionesService.delete(modalState.target.id)}
                                        stringAction={t("eliminar")}
                                        description={modalState.target.descripcionES}
                                        modelName={t("Localización")}
                                        variant="danger"
                                    />
                                );
                            default:
                                return null;
                        }
                    })()}
            </div>
            <div>
                <LocalizacionesTable items={localizaciones} isLoading={isLoading} ActionsComponent={ActionsComponent} />
            </div>
        </>
    );
};

export const LocalizacionesTable = ({items, isLoading, ActionsComponent, ...rest}) => {
    const {t} = useTranslation();
    const columns = [
        {
            field: "descripcionES",
            headerName: t("Localizacion"),
            flex: 2,
            minWidth: 100,
        },
        {
            field: "latitud",
            headerName: t("Latitud"),
            valueFormatter: (e) => e.data?.latitud || "-",
            flex: 1,
            minWidth: 60,
        },
        {
            field: "longitud",
            headerName: t("Longitud"),
            valueFormatter: (e) => e.data?.longitud || "-",
            flex: 1,
            minWidth: 75,
        },
        {
            field: "planta",
            headerName: t("Planta"),
            valueGetter: (e) => e.data?.planta.descripcion,
            valueFormatter: (e) => t(e.value),
            flex: 1.5,
            minWidth: 130,
            filter: DatatableSelectFilter,
        },
        {
            field: "id",
            headerName: "",
            cellRenderer: ActionsComponent,
            flex: 1,
            cellClass: "celdaBotonesTabla",
            minWidth: 80,
            suppressHeaderMenuButton: true,
            floatingFilter: false, // Deshabilitar el filtro flotante
            filter: false, // Deshabilitar el filtro completo para esta columna
            sortable: false,
            hide: !ActionsComponent,
        },
    ];

    return <TableComponent isLoading={isLoading} data={items} columnDefs={columns} tableId="tablaLocalizaciones" {...rest} />;
};
