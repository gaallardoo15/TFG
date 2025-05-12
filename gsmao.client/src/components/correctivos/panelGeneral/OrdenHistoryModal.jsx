import {useEffect, useState} from "react";
import {Badge, Button, Modal} from "react-bootstrap";
import {useTranslation} from "react-i18next";

import {EmptyDatatable} from "@/components/common/EmptyDatatable";
import {Icono} from "@/components/common/Icono";
import {BaseModal} from "@/components/common/modales/BaseModal";
import {CustomModalHeader} from "@/components/common/modales/CustomModalHeader";
import {TableComponent} from "@/components/common/TableComponent";
import {panelGeneralService} from "@/services/PanelGeneralService";

export const OrdenHistoryModal = ({show: showModal, onClose: handleClose, initialData: init}) => {
    const {t} = useTranslation();
    const [historial, sethistorial] = useState([]);
    const [errors, setErrors] = useState({});
    const [isLoading, setIsLoading] = useState(false);

    useEffect(() => {
        setErrors({});
        fetchData(init.id);
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [showModal, init]);

    const fetchData = (id) => {
        panelGeneralService.getHistorialModificacionesUsuariosOrden(id).then((response) => {
            if (!response.is_error) {
                sethistorial(response.content.reverse());
            } else {
                console.log(response.error_content);
                setErrors(response.error_content);
            }
        });
    };
    const FormatearFecha = (fecha) => {
        if (fecha) {
            // Separar la fecha y la hora
            const partesFecha = fecha.split("T");
            console.log(partesFecha);
            const date = partesFecha[0];
            const partesHora = partesFecha[1].split(":");
            const hora = partesHora[0] + ":" + partesHora[1];
            // Convertir la fecha al formato yyyy/mm/dd
            const [year, month, day] = date.split("-");
            const fechaFormateada = `${day}-${month}-${year} ${hora}`;
            return fechaFormateada;
        }
        return;
    };

    return (
        <BaseModal show={showModal} onHide={() => handleClose({shouldRefetch: false})} isLoading={isLoading}>
            <CustomModalHeader title={t("Historial de usuarios Orden")} />
            {!isLoading && (
                <Modal.Body style={{minHeight: "35vh"}}>
                    {historial.length > 0 ? (
                        <div>
                            <TableComponent
                                pagination={false}
                                isLoading={isLoading}
                                data={historial}
                                globalQuickFilter={false}
                                columnDefs={[
                                    {
                                        field: "IdOrden",
                                        headerName: t("Acción"),
                                        valueGetter: (e) => e.data,
                                        cellRenderer: (e) => {
                                            let UsuarioOrigen = e.data?.usuarioOrigen;
                                            let UsuarioDestino = e.data?.usuarioDestino;
                                            let badgeColor = "success";
                                            let badgeText = t("Usuario añadido");
                                            let iconName = "fa-solid fa-person-circle-plus";
                                            //Usuario Reasignado
                                            if (UsuarioOrigen?.nombre != null && UsuarioDestino?.nombre != null) {
                                                badgeColor = "badgeWarning";
                                                iconName = "fa-solid fa-people-arrows";
                                                // badgeText = t("Usuario modificado");
                                            }
                                            //Usuario Añadido
                                            else if (UsuarioOrigen?.nombre == null && UsuarioDestino?.nombre != null) {
                                                badgeColor = "badgeSuccess";
                                                iconName = "fa-solid fa-person-circle-plus";
                                                // badgeText = t("Usuario añadido");
                                            }
                                            //UsuarioEliminado
                                            else {
                                                badgeColor = "badgeDanger";
                                                iconName = "fa-solid fa-person-circle-xmark";
                                                // badgeText = t("Usuario eliminado");
                                            }
                                            return (
                                                <Badge
                                                    pill
                                                    style={{
                                                        padding: "6px 12px 6px 12px",
                                                        backgroundColor: `var(--${badgeColor}) !important`,
                                                    }}>
                                                    <Icono name={iconName} size="xl" />
                                                    {/* {badgeText} */}
                                                </Badge>
                                            );
                                        },
                                        flex: 1,
                                        minWidth: 80,
                                        filter: false, // Deshabilitar el filtro completo para esta columna
                                        tooltipValueGetter: () => null,
                                    },
                                    {
                                        field: "usuarioOrigen",
                                        headerName: t("Usuario Origen"),
                                        valueGetter: (e) =>
                                            e.data?.usuarioOrigen
                                                ? e.data?.usuarioOrigen?.nombre + " " + e.data?.usuarioOrigen?.apellidos
                                                : "-",
                                        flex: 6,
                                        minWidth: 110,
                                        editable: false,
                                    },
                                    {
                                        field: "usuarioDestino",
                                        headerName: t("Usuario Destino"),
                                        valueGetter: (e) =>
                                            e.data?.usuarioDestino
                                                ? e.data?.usuarioDestino?.nombre +
                                                  " " +
                                                  e.data?.usuarioDestino?.apellidos
                                                : "-",
                                        flex: 6,
                                        minWidth: 110,
                                        editable: false,
                                    },
                                    {
                                        field: "fechaCambio",
                                        headerName: t("Fecha modificación"),
                                        valueGetter: (e) => e.data?.fechaCambio,
                                        valueFormatter: (e) => FormatearFecha(e.data?.fechaCambio),
                                        flex: 6,
                                        minWidth: 110,
                                        editable: false,
                                    },
                                ]}
                            />
                        </div>
                    ) : (
                        <div>
                            <EmptyDatatable informacion={errors} />
                        </div>
                    )}
                </Modal.Body>
            )}
            <Modal.Footer>
                <Button variant="secondary" onClick={() => handleClose({shouldRefetch: false})} disabled={isLoading}>
                    {t("Cerrar")}
                </Button>
            </Modal.Footer>
        </BaseModal>
    );
};
