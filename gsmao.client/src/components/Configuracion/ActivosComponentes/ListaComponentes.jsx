import {useEffect, useMemo, useState} from "react";
import {Col, Row} from "react-bootstrap";
import {useTranslation} from "react-i18next";

import {ComponenteFormModal} from "./ComponenteFormModal";

import {BtnCrearElemento} from "@/components/common/Buttons/BtnCrearElemento";
import {BtnTabla} from "@/components/common/Buttons/BtnTabla";
import {DynamicSmartSelectInputs} from "@/components/common/formInputs/DynamicSmartSelectInputs";
import {SelectInput} from "@/components/common/formInputs/SelectInput";
import {SimpleLoadingMessage} from "@/components/common/loading/SimpleLoadingMessage";
import {BasicActionModal} from "@/components/common/modales/BasicActionModal";
import {TableComponent} from "@/components/common/TableComponent";
import {useModalState} from "@/hooks/useModalState";
import {activosService} from "@/services/ActivoService";
import {AuthService} from "@/services/AuthService";
import {componentesService} from "@/services/ComponenteService";
import {intersect} from "@/utils/intersect";

export const ListaComponentes = ({activeTab}) => {
    const {t} = useTranslation();

    const [componentes, setComponentes] = useState([]);
    const [componentesBaseLevel, setComponentesBaseLevel] = useState([]);
    const [activo, setActivo] = useState(null);
    const [componente, setComponente] = useState(0);
    const [textoPinnedRow, setTextoPinnedRow] = useState("");
    const [isLoading, setIsLoading] = useState(false);
    const [errors, setErrors] = useState({});
    const [activos, setActivos] = useState([]);

    const fetchComponentes = (idActivo, idComponente) => {
        componentesService.getComponentes(idActivo, idComponente).then((response) => {
            if (!response.is_error) {
                const components = (response.content || []).sort((a, b) => a.id - b.id);
                setComponentes(components);
                // setComponentesBaseLevel(components);
            } else {
                console.log(response.error_content);
                setErrors(response.error_content);
            }
        });
    };
    const fetchActivos = () => {
        activosService.fetchAll().then((response) => {
            if (!response.is_error) {
                const filteredActivos = response.content.filter((activo) => activo.estadoActivo.name == "Activo");
                console.log("Activo: ", activo);
                setActivos(filteredActivos);
            } else {
                console.error(response.error_content);
                return [];
            }
        });
    };

    const refetch = () => fetchComponentes(activo.id, componente.id || 0);

    const {modalState: modalState, openModal, closeModal} = useModalState(refetch);

    useEffect(() => {
        if (activeTab) {
            fetchActivos(); // Cargar los activos al iniciar el componente
        }
    }, [activeTab]);

    useEffect(() => {
        if (activo && activo.id > 0) {
            fetchComponentes(activo.id, 0); // Cargar los componentes para el activo seleccionado
        } else {
            setComponentes([]);
            setComponentesBaseLevel([]);
        }
    }, [activo]); // Solo dependemos de 'activo'

    // Cuando 'activo' o 'componente' cambian, actualizamos el texto de la fila superior
    useEffect(() => {
        obtenerTextoPinnedRow();
    }, [activo, componente]);

    const handleAction = (actionType, id = null) => {
        const target = id ? componentes.find((u) => u.id === id) : {};
        openModal(actionType, target);
    };

    const handleActivoChange = (event) => {
        const itemData = activos.find((item) => item.id === event.target.value);
        setActivo(itemData);
    };

    const handleComponenteChange = (selectedComponent, children) => {
        setComponente(selectedComponent);
        setComponentes(children);
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
                    title={t("Editar Componente")}
                    onClick={() => handleAction("form", props.value)}
                    disabled={isLoading}
                />
            ),
            delete: (
                <BtnTabla
                    key="delete"
                    icono="fa-solid fa-trash"
                    title={t("Eliminar Componente")}
                    variant="danger"
                    onClick={() => handleAction("delete", props.value)}
                    disabled={isLoading}
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

    function obtenerTextoPinnedRow() {
        let texto = "";
        if (componente) {
            texto = componente.denominacion + " - " + componente.descripcionES;
        } else if (activo) {
            texto = activo?.id + " - " + activo?.descripcionES;
        } else {
            texto = t("No hay activo seleccionado");
        }
        setTextoPinnedRow(texto);
    }

    return (
        <>
            <div>
                <Row className="align-items-end">
                    <Col md="auto">
                        <BtnCrearElemento elemento={t("Componente")} onClick={() => handleAction("form")} />
                    </Col>
                    <Col md={3}>
                        <SelectInput
                            label={t("Activo")}
                            name="idActivo"
                            value={activo}
                            onChange={handleActivoChange}
                            options={activos.map((a) => ({
                                value: a.id,
                                label: a.id + " - " + t(a.descripcionES),
                            }))}
                            selectedv
                            disabled={isLoading}
                            required
                            className="mb-2"
                        />
                    </Col>
                    <Col md={4} className="d-flex flex-column justify-content-center align-items-center">
                        {isLoading && <SimpleLoadingMessage message={t("Recargando")} />}
                    </Col>
                </Row>

                {modalState.show &&
                    (() => {
                        switch (modalState.type) {
                            case "form":
                                return (
                                    <ComponenteFormModal
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
                                        action={() => componentesService.delete(modalState.target.id)}
                                        stringAction={t("eliminar")}
                                        description={modalState.target.descripcionES}
                                        modelName={t("Componente")}
                                        variant="danger"
                                    />
                                );
                            default:
                                return null;
                        }
                    })()}
            </div>
            <div className="contenedorNivelesBtnComponentesActivo">
                <div className="contenedorFiltrosNivelesComponentesActivo">
                    {activo?.id > 0 && (
                        <DynamicSmartSelectInputs
                            onChange={handleComponenteChange}
                            md="col-auto"
                            idActivo={activo?.id}
                            // componentesBase={componentesBaseLevel}
                            // componentesBase={componentes}
                        />
                    )}
                </div>
            </div>

            <div>
                <ComponentesTable
                    items={componentes}
                    isLoading={isLoading}
                    ActionsComponent={ActionsComponent}
                    textPinnedTopRowData={textoPinnedRow}
                />
            </div>
        </>
    );
};

export const ComponentesTable = ({items, isLoading, ActionsComponent, textPinnedTopRowData, ...rest}) => {
    const {t} = useTranslation();

    const pinnedTopRowData = useMemo(() => {
        return [
            {
                kks: textPinnedTopRowData,
            },
        ];
    }, [textPinnedTopRowData]);

    const columns = [
        {
            field: "kks",
            headerName: t(""),
            flex: 0.1,
            filter: false, // Deshabilitar el filtro completo para esta columna
            colSpan: (params) => (params.data.kks ? 4 : 1),
            // style color rojo, fondo rojo claro y fuente bold y letra más grande si el valor es "AAAX0001"
            cellStyle: (params) =>
                params.data.kks
                    ? {
                          backgroundColor: "var(--primarioClarito)",
                          fontWeight: "bold",
                          fontSize: "18",
                      }
                    : {},
        },
        {
            field: "denominacion",
            headerName: t("Denominación"),
            flex: 5,
            floatingFilter: false, // Deshabilitar el filtro flotante
            filter: true, // Deshabilitar el filtro completo para esta columna
        },
        {
            field: "descripcionES",
            headerName: t("Componente"),
            flex: 5,
            floatingFilter: false, // Deshabilitar el filtro flotante
            filter: true, // Deshabilitar el filtro completo para esta columna
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
            hide: !ActionsComponent,
            tooltipValueGetter: null,
            floatingFilter: false, // Deshabilitar el filtro flotante
            filter: false, // Deshabilitar el filtro completo para esta columna
        },
    ];

    return (
        <div id="tablaActivos">
            <TableComponent
                isLoading={isLoading}
                tableId="tablaComponentes"
                data={items}
                columnDefs={columns}
                pinnedTopRowData={pinnedTopRowData}
                overlayNoRowsTemplate={`<div style="margin-top: 50px; text-align: center;">
                       <p>${t("No hay datos para mostrar")}</p>
                     </div>`}
                {...rest}
            />
        </div>
    );
};
