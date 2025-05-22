import {useEffect, useState} from "react";
import {Col, Form, Row} from "react-bootstrap";
import {useTranslation} from "react-i18next";
import {useNavigate} from "react-router";
import {toast} from "react-toastify";

import {CLIENT, CONFIG} from "../../../../config";
import {AdjuntosModal} from "./AdjuntosModal";
import {MaterialesModal} from "./MaterialesModal";
import {UsuariosModal} from "./UsuariosModal";

import {RoutePaths} from "@/components/App";
import {ButtonLabel} from "@/components/common/Buttons/ButtonLabel";
import {CustomButtonIconText} from "@/components/common/Buttons/CustomButtonIconText";
import {CustomPopover} from "@/components/common/CustomPopover";
import {DateInput} from "@/components/common/formInputs/DateInput";
import {SelectInput} from "@/components/common/formInputs/SelectInput";
import {SelectInputWithConfirmation} from "@/components/common/formInputs/SelectInputWithConfirmation";
import {SmartSelectInput} from "@/components/common/formInputs/SmartSelectInput";
import {TextAreaInput} from "@/components/common/formInputs/TextAreaInput";
import {TextInput} from "@/components/common/formInputs/TextInput";
import {TimeInput} from "@/components/common/formInputs/TimeInput";
import {Icono} from "@/components/common/Icono";
import {CrearIncidenciaModal} from "@/components/correctivos/CrearIncidenciaModal";
import {TablaIncidenciasOrden} from "@/components/correctivos/TablaIncidenciasOrden";
import {useModalState} from "@/hooks/useModalState";
import {activosService} from "@/services/ActivoService";
import {AuthService} from "@/services/AuthService";
import {ordenService} from "@/services/OrdenService";
import {registroOrdenService} from "@/services/RegistroOrdenesService";

export const RegistroOrdenesPage = () => {
    const defaultData = {
        id: 0,
        idSAP: "",
        fechaApertura: new Date().toISOString().split("T")[0],
        horaApertura: new Date().toTimeString().split(" ")[0].substring(0, 5),
        localizacion: "",
        centroCoste: "",
        activo: {},
        idActivo: "",
        idEstadoOrden: 1,
        idTipoOrden: 1,
        comentarioOrden: "",
        materiales: "",
        fechaCierre: "",
        horaCierre: "",
        comentarioResolucion: "- DEFECTO: \n- CAUSA:\n - ACCIÓN:\n - ESTADO:\n - OBSERVACIONES:",
        creador: "",
        confirmada: false,
    };

    const {t} = useTranslation();
    const navigate = useNavigate();
    const [formData, setFormData] = useState(defaultData);
    const [incidencias, setIncidencias] = useState([]);
    const [usuariosOrden, setUsuariosOrden] = useState([]);
    const [documentacionOrden, setDocumentacionOrden] = useState([]);
    const [materialesOrden, setMaterialesOrden] = useState([]);
    const [activos, setActivos] = useState([]);
    const [isLoading, setIsLoading] = useState(false);
    const [errors, setErrors] = useState({});
    const {modalState, openModal, closeModal} = useModalState();
    const userData = AuthService.getUserData();
    const [lastUpdate, setLastUpdate] = useState(new Date().toISOString());
    const [hayIncidenciasResuelta, setHayIncidenciasResuelta] = useState(false);
    const [todasIncidenciasResueltas, setTodasIncidenciasResueltas] = useState(false);
    const [materialIsRequired, setMaterialIsRequired] = useState(false);
    const [estados, setEstados] = useState([]);
    const [filteredEstados, setFilteredEstados] = useState([]);
    const estadosCerrados = [2, 5, 8];
    const estadosMaterial = [4, 5, 7, 8];
    const extensionesImagenes = [".jpg", ".jpeg", ".png", ".ico"];

    const fetchActivos = () => {
        activosService.fetchAll().then((response) => {
            if (!response.is_error) {
                const filteredActivos = response.content.filter((activo) => activo.estadoActivo.name == "Activo");
                setActivos(filteredActivos);
            } else {
                console.error(response.error_content);
                return [];
            }
        });
    };

    const fetchEstados = () => {
        setIsLoading(true);
        ordenService.getEstadosOrdenes().then((response) => {
            if (!response.is_error) {
                setEstados(response.content);
                setFilteredEstados(response.content);
            } else {
                setErrors(response.error_content);
            }
            setIsLoading(false);
        });
    };

    const crearOrdenVacía = () => {
        registroOrdenService.create().then((response) => {
            if (!response.is_error) {
                const respuesta = response.content || {};
                setFormData({
                    ...defaultData,
                    id: respuesta.idOrden,
                    creador: {nombre: userData.name, apellidos: userData.lastname},
                });
            } else {
                console.error(response.error_content);
                return [];
            }
        });
    };

    useEffect(() => {
        if (formData?.id == 0) {
            crearOrdenVacía();
            fetchActivos();
            setFormData({...defaultData});
        }
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, []);

    useEffect(() => {
        setHayIncidenciasResuelta(incidencias?.some((item) => item.resolucion !== null));
        setTodasIncidenciasResueltas(incidencias?.every((item) => item.resolucion !== null));

        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [incidencias]);

    useEffect(() => {
        if (estados.length == 0) {
            fetchEstados();
        } else {
            setFilteredEstados(
                estados.length > 0
                    ? estados.filter(
                          (estado) =>
                              estado.name != "Abierta: Material Gestionado" &&
                              estado.name != "Cerrada: Material Gestionado",
                      )
                    : [],
            );
        }
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [estados]);

    const handleAction = (actionType) => {
        openModal(actionType);
    };

    const handleInputChange = (event) => {
        const {name, value, type, checked} = event.target;
        const description = event.target.description;

        let newValue = {
            ...formData,
            [name]: type === "checkbox" ? checked : value,
        };

        if (name == "idActivo") {
            console.log(name);
            const activo = activos.find((a) => a.id === value);
            newValue.activo = activo;
            newValue.centroCoste = activo?.centroCoste?.descripcionES;
            newValue.localizacion = activo?.localizacion?.descripcionES;
        }
        if (name == "idEstadoOrden") {
            newValue.estadoOrden = {id: value, name: description};
            if (estadosMaterial.includes(newValue?.idEstadoOrden)) {
                setMaterialIsRequired(true);
            } else {
                setMaterialIsRequired(false);
            }
        }

        //Establecemos esta visibilidad en función del estado. Si el estado de la orden es uno de los estados que no permiten editar, se deshabilitan los campos
        //setIsVisible(!estadosCamposDeshabilitados.includes(orden.estadoOrden?.name));
        setFormData(newValue);
        setErrors({});
    };

    const validarOrden = () => {
        let ordenValida = true;

        if (
            CONFIG[CLIENT].logicaDelNegocio.RolesObligadosAAdjuntarFotoAlCerrarOrden.includes(userData.role) &&
            estadosCerrados.includes(formData.idEstadoOrden) &&
            (documentacionOrden.length == 0 ||
                documentacionOrden?.every((item) => item.type != "file") ||
                documentacionOrden?.every((item) => !extensionesImagenes.includes(item.extension.toLowerCase())))
        ) {
            toast.error("Debe adjuntar alguna imagen a la orden para guardar una orden en estado cerrada. ", {
                autoClose: 15000,
                closeButton: true,
            });
            ordenValida = false;
        }

        if (incidencias.length == 0) {
            toast.error("Error al guardar la orden. Debe añadir al menos una incidencia para continuar. ", {
                autoClose: 15000,
                closeButton: true,
            });
            ordenValida = false;
        }

        if (usuariosOrden.length == 0 && !materialIsRequired) {
            toast.error("Error al guardar la orden. Debe añadir al menos un usuario para continuar. ", {
                autoClose: 15000,
                closeButton: true,
            });
            ordenValida = false;
        }

        if (
            CONFIG[CLIENT].logicaDelNegocio.RolesObligadosAAdjuntarFotoMaterialAlCerrarOrden.includes(userData.role) &&
            estadosMaterial.includes(formData.idEstadoOrden) &&
            (materialesOrden.length == 0 ||
                materialesOrden?.every((item) => item.type != "file") ||
                materialesOrden?.every((item) => !extensionesImagenes.includes(item.extension.toLowerCase())))
        ) {
            toast.error(
                "Debe adjuntar alguna imagen de materiales a la orden para guardar una orden en cualquier variante de estado de materiales. ",
                {
                    autoClose: 15000,
                    closeButton: true,
                },
            );
            ordenValida = false;
        }

        if (materialIsRequired && formData.materiales == "") {
            toast.error("Error al guardar la orden. Debe completar el campo material para el estado seleccionado.", {
                autoClose: 20000,
                closeButton: true,
            });
            ordenValida = false;
        }

        if (hayIncidenciasResuelta && formData.comentarioResolucion.length <= longitudMinimaPlantillaResolucion) {
            const mensajeError =
                longitudMinimaPlantillaResolucion == 60
                    ? "Error al guardar la orden. Debe completar la plantilla de resolución de la orden para continuar. Vuelva a abrir la orden para completar dicha información. "
                    : "Error al guardar la orden. Debe completar el campo de resolución de la orden para continuar. Vuelva a abrir la orden para completar dicha información. ";

            toast.error(mensajeError, {
                autoClose: 15000,
                closeButton: true,
            });
            ordenValida = false;
        }

        return ordenValida;
    };

    const handleClose = ({shouldRefetch}) => {
        closeModal();
        if (shouldRefetch) {
            setLastUpdate(new Date().toISOString());
        }
    };

    const handleSubmit = (event) => {
        event.preventDefault();
        setIsLoading(true);

        if (validarOrden()) {
            const dataToSend = {
                ...formData,
                fechaApertura: formData.fechaApertura + " " + formData.horaApertura,
                fechaCierre: (formData.fechaCierre && formData.fechaCierre + " " + formData.horaCierre) || "",
            };

            ordenService.save(dataToSend).then((response) => {
                if (!response.is_error) {
                    toast.success("Orden guardada correctamente", {className: "custom-toast"});
                    navigate(RoutePaths.PanelGeneral.path);
                } else {
                    toast.error(`Hubo un error al guardar la orden. ${response.error_content}`);
                    setErrors(response.error_content);
                }
                setIsLoading(false);
            });
        } else {
            setIsLoading(false);
        }
    };

    const longitudMinimaPlantillaResolucion =
        formData?.comentarioResolucion.length < 60 ||
        (formData.comentarioResolucion.length == 60 && !formData.comentarioResolucion.includes("DEFECTO:")) //Tiene el tamaño de la plantilla vacía pero no es la plantilla
            ? 0
            : 60; // Longitud mínima de la plantilla de resolución sin rellenar

    return (
        <Form onSubmit={handleSubmit}>
            <div className="d-flex flex-column gap-4">
                <div className="customCard">
                    <Row>
                        <TextInput
                            label={t("ID")}
                            name="id"
                            value={formData.id}
                            onChange={handleInputChange}
                            disabled
                            className="col-12 col-lg-2"
                        />
                        <SelectInputWithConfirmation
                            key={formData.idEstadoOrden}
                            label={t("Estado")}
                            name="idEstadoOrden"
                            value={formData.idEstadoOrden}
                            onChange={handleInputChange}
                            options={filteredEstados.map((e) => ({
                                value: e.id,
                                label: e.name,
                            }))}
                            required
                            disabled={isLoading}
                            className="col-12 col-lg-2"
                        />
                        <SmartSelectInput
                            label={t("Tipo")}
                            required
                            name="idTipoOrden"
                            value={formData.idTipoOrden}
                            fetcher={ordenService.getTiposOrdenes}
                            valueKey="id"
                            labelKey="name"
                            onChange={handleInputChange}
                            className="col-12 col-lg-2"
                        />
                        <DateInput
                            label={t("Fecha Apertura")}
                            required
                            name="fechaApertura"
                            value={formData.fechaApertura}
                            onChange={handleInputChange}
                            className="col-12 col-lg-3"
                        />
                        <TimeInput
                            label={t("Hora Apertura")}
                            required
                            name="horaApertura"
                            value={formData.horaApertura}
                            onChange={handleInputChange}
                            className="col-12 col-lg-3"
                        />
                        <TextInput
                            label={t("Localización")}
                            name="localizacion"
                            value={formData.localizacion}
                            onChange={handleInputChange}
                            disabled
                            className="col-12 col-lg-3"
                        />
                        <div className="col-12 col-lg-3 mb-3">
                            <ButtonLabel
                                titulo={t("Usuarios orden")}
                                texto={t("Usuarios - Orden")}
                                icono="fa-solid fa-users"
                                textLabel={t("Usuarios")}
                                variant="warning"
                                onClick={() => handleAction("usuarios")}
                                disabled={materialIsRequired}
                                badge={usuariosOrden?.length > 0}
                                badgeValue={usuariosOrden?.length}
                            />
                        </div>
                        <SelectInput
                            label={t("Activo")}
                            name="idActivo"
                            value={formData.idActivo}
                            onChange={handleInputChange}
                            options={activos.map((a) => ({
                                value: a.id,
                                label: a.id + " - " + t(a.descripcionES),
                            }))}
                            disabled={isLoading || incidencias.length !== 0}
                            required
                            className="col-12 col-lg-4"
                        />
                        <Col className="col-12 col-lg-2 d-flex justify-content-center align-items-end mb-3">
                            <CustomButtonIconText
                                titulo={t("Añadir Incidencia")}
                                texto={t("Incidencia")}
                                icono="fa-solid fa-plus"
                                variant="warning"
                                onClick={() => handleAction("incidencias")}
                                disabled={!formData.idActivo}
                            />
                        </Col>
                        <TextAreaInput
                            label={t("Comentario")}
                            name="comentarioOrden"
                            value={formData.comentarioOrden}
                            onChange={handleInputChange}
                            className="col-12"
                            rows={1}
                        />
                    </Row>
                </div>
                <Col className="col-12" hidden={!incidencias.length > 0}>
                    <div className="customCard">
                        <TablaIncidenciasOrden
                            formdata={formData}
                            key={lastUpdate}
                            id="tablaIncidenciasRegistro"
                            tableHeight={0.3}
                            registroOrdenes={true}
                            actualizarIncidencias={setIncidencias}
                        />
                    </div>
                </Col>

                <div id="seccionBajaRegistroOrdenes" className="row g-4">
                    <div className="col-12 col-lg-6">
                        <div className="customCard">
                            <Row>
                                <DateInput
                                    label={t("Fecha Cierre")}
                                    name="fechaCierre"
                                    value={formData.fechaCierre}
                                    onChange={handleInputChange}
                                    className="col-12 col-lg-6"
                                    disabled={!estadosCerrados.includes(formData.idEstadoOrden)}
                                    required={estadosCerrados.includes(formData.idEstadoOrden)}
                                />
                                <TimeInput
                                    label={t("Hora Cierre")}
                                    name="horaCierre"
                                    value={formData.horaCierre}
                                    onChange={handleInputChange}
                                    className="col-12 col-lg-6"
                                    disabled={!estadosCerrados.includes(formData.idEstadoOrden)}
                                    required={estadosCerrados.includes(formData.idEstadoOrden)}
                                />
                                <TextAreaInput
                                    label={t("Comentario Resolución")}
                                    name="comentarioResolucion"
                                    value={formData.comentarioResolucion}
                                    onChange={handleInputChange}
                                    className="col-12"
                                    rows={5}
                                    disabled={!hayIncidenciasResuelta}
                                />
                            </Row>
                        </div>
                    </div>

                    <div className="col-12 col-lg-6">
                        <div className="customCard d-flex flex-column gap-3 h-100">
                            <div className="text-end">
                                <CustomPopover
                                    body={t(
                                        "Para insertar imágenes o archivos referentes a la orden acceda a adjuntos.\nPara insertar imágenes referentes a algún material, acceda a materiales y complete el campo de texto.",
                                    )}
                                />
                            </div>

                            <div className="divAnadirAdjuntosNuevaOrden">
                                <div className="divIconoBoton">
                                    <div className="divLogosAdjuntosNuevaOrden">
                                        <Icono name={"fa-solid fa-folder-tree"} />
                                    </div>
                                    <CustomButtonIconText
                                        titulo={t("Abrir adjuntos")}
                                        texto={t("Adjuntos")}
                                        variant="warning"
                                        onClick={() => handleAction("adjuntos")}
                                        badge={documentacionOrden.filter((item) => item.type === "file")?.length > 0}
                                        badgeValue={documentacionOrden.filter((item) => item.type === "file")?.length}
                                    />
                                </div>
                                <div className="divIconoBoton">
                                    <div className="divLogosAdjuntosNuevaOrden">
                                        <Icono name={"fa-solid fa-shapes"} />
                                    </div>
                                    <CustomButtonIconText
                                        titulo={t("Abrir Materiales")}
                                        texto={t("Materiales")}
                                        variant="warning"
                                        onClick={() => handleAction("materiales")}
                                        badge={materialIsRequired}
                                        badgeValue={
                                            formData.materiales.length > 0 ? (
                                                <Icono name={"fa-solid fa-check"} style={{ fontSize: "12px" }} />
                                            ) : (
                                                <Icono name={"fa-solid fa-xmark"} style={{ fontSize: "12px" }} />
                                            )
                                        }
                                        badgeColor={formData.materiales.length > 0 ? "success" : "danger"}
                                    />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <div className="divContainerConfirmarOrden mt-4">
                    <CustomButtonIconText
                        type="submit"
                        titulo={t("Confirmar Orden")}
                        texto={t("Confirmar Orden")}
                        icono={"fa-solid fa-check"}
                        disabled={isLoading}
                        isLoading={isLoading}
                        style={{ backgroundColor: "var(--identificativoCliente) !important" }}
                    />
                </div>

                {modalState.show &&
                    (() => {
                        switch (modalState.type) {
                            case "usuarios":
                                return (
                                    <UsuariosModal
                                        show={true}
                                        onClose={closeModal}
                                        formData={formData}
                                        registroOrdenes={true}
                                        actualizarUsuariosOrden={setUsuariosOrden}
                                    />
                                );
                            case "incidencias":
                                return (
                                    <CrearIncidenciaModal
                                        show={true}
                                        onClose={handleClose}
                                        idActivo={formData.idActivo}
                                        idOrden={formData.id}
                                    />
                                );
                            case "adjuntos":
                                return (
                                    <AdjuntosModal
                                        show={true}
                                        onClose={closeModal}
                                        initialData={{id: formData.id}}
                                        actualizarAdjuntosOrden={setDocumentacionOrden}
                                    />
                                );
                            case "materiales":
                                return (
                                    <MaterialesModal
                                        show={true}
                                        onClose={closeModal}
                                        formData={formData}
                                        required={materialIsRequired}
                                        onChange={handleInputChange}
                                        actualizarAdjuntosOrden={setMaterialesOrden}
                                        style={{height: "31vh"}}
                                    />
                                );
                            default:
                                return null;
                        }
                    })()}
            </div>
        </Form>
    );
};
