import {useEffect} from "react";
import {useTranslation} from "react-i18next";

import {BtnTabla} from "../common/Buttons/BtnTabla";
import {CeldaBotonesTabla} from "../common/CeldaBotonesTabla";
import {BasicActionModal} from "../common/modales/BasicActionModal";
import {TableComponent} from "../common/TableComponent";
import {EditarIncidenciaOrdenModal} from "./editarIncidenciaOrdenModal";
import {JerarquiaComponenteModal} from "./JerarquiaComponenteModal";
import {ResolverIncidenciaModal} from "./ResolverIncidenciaModal";

import {useFetchListData} from "@/hooks/useFetchData";
import {useModalState} from "@/hooks/useModalState";
import {AuthService} from "@/services/AuthService";
import {ordenService} from "@/services/OrdenService";
import {intersect} from "@/utils/intersect";

export const TablaIncidenciasOrden = ({
    formdata,
    isVisible = true,
    id,
    tableHeight = 0.4,
    registroOrdenes = false,
    actualizarIncidencias,
}) => {
    const {t} = useTranslation();
    const {
        items: incidencias,
        isLoading,
        isRefetching,
        fetchData,
    } = useFetchListData(() =>
        formdata.id
            ? ordenService.getIncidenciasOrden(formdata.id)
            : new Promise((resolve) => {
                  resolve({response: {content: []}});
              }),
    );
    const {modalState: modalState, openModal, closeModal} = useModalState(fetchData);
    const handleAction = (actionType, id = null) => {
        const target = id ? incidencias.find((u) => u.id === id) : {};
        openModal(actionType, target);
    };

    useEffect(() => {
        fetchData();
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [formdata.id]);

    useEffect(() => {
        actualizarIncidencias(incidencias);
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [incidencias]);

    const ActionsComponent = (props) => {
        const actionByRole = {
            OPERARIO: ["jerarchy", "solve", "edit", "delete"],
            RESPONSABLE_MATERIALES: ["jerarchy", "solve", "edit", "delete"],
            RESPONSABLE_TALLER: ["jerarchy", "solve", "edit", "delete"],
            RESPONSABLE: ["jerarchy", "solve", "edit", "delete"],
            JEFE_MANTENIMIENTO: ["jerarchy", "solve", "edit", "delete"],
            ADMINISTRADOR: ["jerarchy", "solve", "edit", "delete"],
            SUPER_ADMINISTRADOR: ["jerarchy", "solve", "edit", "delete"],
        };

        const actionByState = {
            "Abierta": ["jerarchy", "solve", "edit", "delete"],
            "Cerrada": ["jerarchy", "solve", "edit"],
            "Anulada": ["jerarchy", "solve", "edit"],
            "Abierta: Pendiente Material": ["jerarchy", "solve", "edit", "delete"],
            "Cerrada: Pendiente Material": ["jerarchy", "solve", "edit"],
            "Abierta: Material Gestionado": ["jerarchy", "solve", "edit", "delete"],
            "Cerrada: Material Gestionado": ["jerarchy", "solve", "edit"],
            "En Curso": ["jerarchy", "solve", "edit", "delete"],
        };

        const actionByUnicaIncidencia = {
            true: ["jerarchy", "solve", "edit"],
            false: ["jerarchy", "solve", "edit", "delete"],
        };

        let availableActions = ["jerarchy", "solve", "edit", "delete"];

        if (!registroOrdenes) {
            const state = formdata?.estadoOrden?.name;
            availableActions = intersect(actionByState[state], actionByRole[AuthService.getUserRole()]);

            // Si solo hay una incidencia, se deshabilita la acción de eliminar
            availableActions = intersect(availableActions, actionByUnicaIncidencia[incidencias?.length === 1]);
        }

        const buttonByAction = {
            ...(props.componente.idComponentePadre > 0 && {
                jerarchy: (
                    <BtnTabla
                        key="jerarchy"
                        icono="fa-solid fa-sitemap"
                        title={t("Jerarquía de componentes")}
                        variant="warning"
                        onClick={() => handleAction("jerarchy", props.id)}
                        disabled={!props.componente.idComponentePadre > 0}
                    />
                ),
            }),
            delete: (
                <BtnTabla
                    key="delete"
                    icono="fa-solid fa-trash"
                    title={t("Eliminar incidencia")}
                    variant="danger"
                    onClick={() => handleAction("delete", props.id)}
                    disabled={isRefetching || !isVisible}
                />
            ),
            solve: (
                <BtnTabla
                    key="solve"
                    icono="fa-solid fa-check-to-slot"
                    title={t("Editar Resolución")}
                    onClick={() => handleAction("solve", props.id)}
                    disabled={isLoading}
                    variant="success"
                />
            ),
            edit: (
                <BtnTabla
                    key="edit"
                    icono="fa-solid fa-pen"
                    title={t("Editar incidencia")}
                    onClick={() => handleAction("edit", props.id)}
                    disabled={isLoading}
                />
            ),
        };

        return <CeldaBotonesTabla availableActions={availableActions} buttonByAction={buttonByAction} />;
    };

    const FormatearFecha = (fecha) => {
        if (fecha) {
            // Separar la fecha y la hora
            const partesFecha = fecha.split("T");

            const date = partesFecha[0];
            const hora = partesFecha[1];
            // Convertir la fecha al formato yyyy/mm/dd
            const [year, month, day] = date.split("-");
            const fechaFormateada = `${day}-${month}-${year} ${hora}`;
            return fechaFormateada;
        }
        return;
    };

    return (
        <div>
            {modalState.show &&
                (() => {
                    switch (modalState.type) {
                        case "edit":
                            return (
                                <EditarIncidenciaOrdenModal
                                    show={true}
                                    onClose={closeModal}
                                    initialData={modalState.target}
                                    datosOrden={formdata}
                                    isVisible={isVisible}
                                />
                            );
                        case "delete":
                            return (
                                <BasicActionModal
                                    show={true}
                                    onClose={closeModal}
                                    action={() => ordenService.eliminarIncidenciaOrden(formdata, modalState.target.id)}
                                    stringAction={t("eliminar")}
                                    description={"esta incidencia"}
                                    modelName={t("Incidencia")}
                                    variant="danger"
                                    cursive={false}
                                />
                            );
                        case "jerarchy":
                            return (
                                <JerarquiaComponenteModal
                                    show={true}
                                    onClose={closeModal}
                                    componente={modalState.target.componente}
                                    formdata={formdata}
                                />
                            );
                        case "solve":
                            return (
                                <ResolverIncidenciaModal
                                    show={true}
                                    onClose={closeModal}
                                    initialData={modalState.target}
                                    datosOrden={formdata}
                                    isVisible={isVisible}
                                />
                            );
                        default:
                            return null;
                    }
                })()}

            <div id={id}>
                <TableComponent
                    isLoading={isLoading}
                    data={incidencias}
                    pagination={true}
                    tableHeight={tableHeight}
                    globalQuickFilter={false}
                    columnDefs={[
                        {
                            field: "componente",
                            headerName: t("Componente"),
                            valueGetter: (e) =>
                                e.data?.componente?.denominacion + " - " + e.data?.componente?.descripcionES || "-",
                            flex: 3,
                            minWidth: 110,
                            editable: true,
                        },

                        {
                            field: "fechaDeteccion",
                            headerName: t("Fecha Deteccion"),
                            valueGetter: (e) => e.data?.fechaDeteccion,
                            valueFormatter: (e) => FormatearFecha(e.data?.fechaDeteccion),
                            flex: 1,
                            minWidth: 145,
                            editable: true,
                        },
                        {
                            field: "mecanismoDeFallo",
                            headerName: t("Mecanismo de Fallo"),
                            valueGetter: (e) => e.data?.incidencia?.mecanismoDeFallo?.descripcionES || "-",
                            flex: 1.5,
                            minWidth: 200,
                            editable: true,
                        },
                        {
                            field: "incidencia",
                            headerName: t("Incidencia"),
                            valueGetter: (e) => e.data?.incidencia?.descripcionES || "-",
                            valueFormatter: (e) => t(e.value),
                            flex: 1.5,
                            minWidth: 120,
                            editable: false,
                        },
                        {
                            field: "fechaResolucion",
                            headerName: t("Fecha Resolución"),
                            valueGetter: (e) => e.data?.fechaResolucion || "",
                            valueFormatter: (e) => FormatearFecha(e.data?.fechaResolucion) || "-",
                            flex: 1,
                            minWidth: 150,
                            editable: false,
                        },
                        {
                            field: "resolucion",
                            headerName: t("Resolución"),
                            valueGetter: (e) => e.data?.resolucion?.descripcionES || "",
                            valueFormatter: (e) => t(e.value) || "-",
                            flex: 1.5,
                            minWidth: 200,
                        },
                        {
                            field: "id",
                            headerName: "",
                            cellRenderer: (e) => ActionsComponent(e.data),
                            flex: 1,
                            cellClass: "celdaBotonesTablaRight",
                            minWidth: 100,
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
