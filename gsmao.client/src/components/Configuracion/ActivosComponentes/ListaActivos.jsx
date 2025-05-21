import {Badge, Col, Row} from "react-bootstrap";
import {useTranslation} from "react-i18next";

import {ActivoFormModal} from "./ActivoFormModal";
import {DocumentsModal} from "./DocumentsModal";

import {BtnCrearElemento} from "@/components/common/Buttons/BtnCrearElemento";
import {BtnTabla} from "@/components/common/Buttons/BtnTabla";
import {DatatableSelectFilter} from "@/components/common/DatatableSelectFilter";
import {SimpleLoadingMessage} from "@/components/common/loading/SimpleLoadingMessage";
import {BasicActionModal} from "@/components/common/modales/BasicActionModal";
import {TableComponent} from "@/components/common/TableComponent";
import {useFetchListData} from "@/hooks/useFetchData";
import {useModalState} from "@/hooks/useModalState";
import {activosService} from "@/services/ActivoService";
import {AuthService} from "@/services/AuthService";
import {intersect} from "@/utils/intersect";

export const ListaActivos = () => {
    const {t} = useTranslation();
    const {items, isLoading, isRefetching, fetchData} = useFetchListData(activosService.fetchAll);
    const {modalState: modalState, openModal, closeModal} = useModalState(fetchData);

    //LOCALIZACIONES
    const handleAction = (actionType, id = null) => {
        const target = id ? items.find((u) => u.id === id) : {};
        openModal(actionType, target);
    };

    const ActionsComponent = (props) => {
        if (!props.value) {
            return;
        }

        let actionByState = {
            Activo: ["edit", "delete", "documents", "deactivate"],
            Inactivo: ["edit", "delete", "documents", "reactivate"],
            Borrado: ["restore"],
        };

        const actionByRole = {
            OPERARIO: [],
            SUPER_ADMINISTRADOR: ["reactivate", "deactivate", "restore", "documents", "edit", "delete"],
            ADMINISTRADOR: ["reactivate", "deactivate", "restore", "documents", "edit", "delete"],
            JEFE_MANTENIMIENTO: ["reactivate", "deactivate", "restore", "documents", "edit", "delete"],
            RESPONSABLE_TALLER: [],
            RESPONSABLE: [],
            RESPONSABLE_MATERIALES: [],
        };

        const activo = items.filter((u) => u.id == props.value)[0];
        const state = activo?.estadoActivo.name;
        const availableActions = intersect(actionByState[state], actionByRole[AuthService.getUserRole()]);

        const buttonByAction = {
            restore: (
                <BtnTabla
                    key="restore"
                    icono="fa-solid fa-trash-can-arrow-up"
                    title={t("Restaurar Activo")}
                    onClick={() => handleAction("restore", props.value)}
                    disabled={isRefetching}
                    variant="success"
                />
            ),
            reactivate: (
                <BtnTabla
                    key="reactivate"
                    icono="fa-solid fa-arrow-rotate-right"
                    title={t("Reactivar activo")}
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
            documents: (
                <BtnTabla
                    key="documents"
                    icono="fa-solid fa-folder-open"
                    title={t("Gestor Documental")}
                    onClick={() => handleAction("documents", props.value)}
                    disabled={isRefetching}
                />
            ),
            edit: (
                <BtnTabla
                    key="edit"
                    icono="fa-solid fa-pen"
                    title={t("Editar Activo")}
                    onClick={() => handleAction("form", props.value)}
                    disabled={isRefetching}
                />
            ),
            delete: (
                <BtnTabla
                    key="delete"
                    icono="fa-solid fa-trash"
                    title={t("Eliminar Activo")}
                    variant="danger"
                    onClick={() => handleAction("delete", props.value)}
                    disabled={isRefetching}
                />
            ),
        };

        return (
            <div className="celdaBotonesTabla text-center">
                {availableActions
                    ?.map((action) => buttonByAction[action])
                    .reduce((accu, elem, index) => {
                        return accu === null ? [elem] : [...accu, " ", elem];
                    }, null)}
            </div>
        );
    };

    return (
        <>
            <div>
                <Row>
                    <Col>
                        <BtnCrearElemento elemento={t("Activo")} onClick={() => handleAction("form")} />
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
                                    <ActivoFormModal show={true} onClose={closeModal} initialData={modalState.target} />
                                );
                            case "delete":
                                return (
                                    <BasicActionModal
                                        show={true}
                                        onClose={closeModal}
                                        action={() => activosService.delete(modalState.target.id)}
                                        stringAction={t("eliminar")}
                                        description={modalState.target.descripcionES}
                                        modelName={t("Activo")}
                                        variant="danger"
                                    />
                                );
                            case "restore":
                                return (
                                    <BasicActionModal
                                        show={true}
                                        onClose={closeModal}
                                        action={() => activosService.changeState(modalState.target.id, "Activo")}
                                        stringAction={t("restaurar")}
                                        description={modalState.target.descripcionES}
                                        modelName={t("Activo")}
                                    />
                                );
                            case "reactivate":
                                return (
                                    <BasicActionModal
                                        show={true}
                                        onClose={closeModal}
                                        action={() => activosService.changeState(modalState.target.id, "Activo")}
                                        stringAction={t("reactivar")}
                                        description={modalState.target.descripcionES}
                                        modelName={t("Activo")}
                                    />
                                );
                            case "deactivate":
                                return (
                                    <BasicActionModal
                                        show={true}
                                        onClose={closeModal}
                                        action={() => activosService.changeState(modalState.target.id, "Inactivo")}
                                        stringAction={t("dar de baja temporalmente")}
                                        description={modalState.target.descripcionES}
                                        modelName={t("Activo")}
                                        variant="dark"
                                    />
                                );
                            case "documents":
                                return (
                                    <DocumentsModal
                                        idActivo={modalState.target.id}
                                        show={true}
                                        onClose={closeModal}
                                        style={{minHeight: "47vh"}}
                                    />
                                );
                            default:
                                return null;
                        }
                    })()}
            </div>
            <div>
                <ActivosTable items={items} isLoading={isLoading} ActionsComponent={ActionsComponent} />
            </div>
        </>
    );
};

export const ActivosTable = ({items, isLoading, ActionsComponent, ...rest}) => {
    const {t} = useTranslation();

    const getUniqueCriticidades = () => {
        // Obtener un array con todas las criticidades de cada objeto
        const criticidadesArray = items.flatMap((obj) => obj.criticidad || []);

        const uniqueCriticidades = new Set();

        criticidadesArray.forEach((row) => {
            if (row) {
                uniqueCriticidades.add(`${row.siglas}`);
            }
        });

        return Array.from(uniqueCriticidades);
    };

    const columns = [
        {
            field: "estadoActivo",
            headerName: t("Estado"),
            valueGetter: (e) => e.data?.estadoActivo?.name,
            flex: 1,
            minWidth: 70,
            filter: DatatableSelectFilter,
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
        },
        {
            field: "id",
            headerName: t("ID"),
            flex: 1,
            minWidth: 60,
        },
        {
            field: "descripcion_es",
            headerName: t("Descripción"),
            valueGetter: (e) => e.data?.descripcionES,
            valueFormatter: (e) => t(e.value),
            flex: 3,
            minWidth: 200,
        },
        {
            field: "criticidad",
            headerName: t("Criticidad"),
            valueGetter: (e) => e.data?.criticidad?.siglas,
            valueFormatter: (e) => e.data?.criticidad?.siglas + ` (${e.data?.valorCriticidad})`,
            flex: 1,
            minWidth: 90,
            filter: DatatableSelectFilter,
            filterParams: {customUniqueValues: getUniqueCriticidades()},
        },
        {
            field: "localizacion",
            headerName: t("Localización"),
            valueGetter: (e) => e.data?.localizacion?.descripcionES,
            valueFormatter: (e) => t(e.value),
            flex: 1.5,
            minWidth: 100,
        },
        {
            field: "id",
            headerName: "",
            cellRenderer: ActionsComponent,
            flex: 1,
            cellClass: "celdaBotonesTabla",
            minWidth: 140,
            suppressHeaderMenuButton: true,
            sortable: false,
            hide: !ActionsComponent,
            tooltipValueGetter: null,
            floatingFilter: false, // Deshabilitar el filtro flotante
            filter: false, // Deshabilitar el filtro completo para esta columna
        },
    ];

    return (
        <div id="tablaActivos">
            <TableComponent isLoading={isLoading} data={items} columnDefs={columns} tableId="tablaActivos" {...rest} />
        </div>
    );
};
