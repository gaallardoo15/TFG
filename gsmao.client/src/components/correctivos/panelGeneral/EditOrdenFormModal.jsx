import {useEffect, useState} from "react";
import {Button, Form, Modal} from "react-bootstrap";
import {useTranslation} from "react-i18next";

import {CLIENT, CONFIG} from "../../../../config";
import {AdjuntosOrdenFormModal} from "./AdjuntosOrdenFormModal";
import {FechasOrdenFormModal} from "./FechasOrdenFormModal";
import {IncidenciasOrdenFormModal} from "./IncidenciasOrdenFormModal";
import {MaterialesFormModal} from "./MaterialesFormModal";
import {OrdenFormModal} from "./OrdenFormModal";
import {UsuariosOrdenFormModal} from "./UsuariosOrdenFormModal";

import {CustomTabs} from "@/components/common/CustomTabs";
import {LoadingSpinner} from "@/components/common/loading/LoadingSpinner";
import {BaseModal} from "@/components/common/modales/BaseModal";
import {MostrarErrores} from "@/components/common/MostrarErrores";
import {useFetchItemData} from "@/hooks/useFetchData";
import {AuthService} from "@/services/AuthService";
import {ordenService} from "@/services/OrdenService";
import {panelGeneralService} from "@/services/PanelGeneralService";

export function EditOrdenFormModal({show: showModal, onClose: handleClose, initialData: init}) {
    const defaultData = {
        id: null,
        idSAP: "",
        fechaApertura: "",
        horaApertura: "",
        fechaCierre: null,
        horaCierre: null,
        idEstadoOrden: null,
        idActivo: null,
        idTipoOrden: null,
        idCentroCoste: null,
        comentarioOrden: "",
        comentarioResolucion: "",
        materiales: "",
    };
    const {t} = useTranslation();
    const {item: orden, isLoadingFetch, isRefetching, fetchData} = useFetchItemData(panelGeneralService, init.id);
    const [formData, setFormData] = useState(defaultData);
    const [incidencias, setIncidencias] = useState([]);
    const [documentacionOrden, setDocumentacionOrden] = useState([]);
    const [materialesOrden, setMaterialesOrden] = useState([]);
    const [usuariosOrden, setUsuariosOrden] = useState([]);
    const [errors, setErrors] = useState({});
    const [isLoading, setIsLoading] = useState(false);
    const [activeTab, setActiveTab] = useState("tabOrden"); // Tab predeterminado
    const [isVisible, setIsVisible] = useState(true);
    const [operarioIsInOrden, setOperarioIsInOrden] = useState(true);
    const [algunaIncidenciaReuelta, setAlgunaIncidenciaResuelta] = useState(true);
    const [materialIsRequired, setMaterialIsRequired] = useState(false);

    const userData = AuthService.getUserData();

    const estadosCerrados = [2, 5, 8];
    const estadosMaterial = [4, 5, 7, 8];
    //Utilizado para deshabilitar campos en función del estado de la orden que obtienes de la base de datos
    const estadosCamposDeshabilitados = [
        "Cerrada",
        "Cerrada: Pendiente Material",
        "Cerrada: Material Gestionado",
        "Anulada",
    ];
    const extensionesImagenes = [".jpg", ".jpeg", ".png", ".ico"];
    const longitudMinimaPlantillaResolucion =
        formData?.comentarioResolucion.length < 60 ||
        (formData.comentarioResolucion.length == 60 && !formData.comentarioResolucion.includes("DEFECTO:")) //Tiene el tamaño de la plantilla vacía pero no es la plantilla
            ? 0
            : 60; // Longitud mínima de la plantilla de resolución sin rellenar

    useEffect(() => {
        let fApertura = FormatearFecha(orden?.fechaApertura)?.split(" ");
        let fCierre = FormatearFecha(orden?.fechaCierre)?.split(" ");
        let init2 =
            Object.keys(orden).length !== 0
                ? {
                      ...orden,
                      idEstadoOrden: orden.estadoOrden?.id,
                      idActivo: orden.activo?.id,
                      idTipoOrden: orden.tipoOrden?.id,
                      idCentroCoste: orden.activo.centroCoste.id,
                      fechaApertura: fApertura[0],
                      horaApertura: fApertura[1],
                      fechaCierre: fCierre ? fCierre[0] : null,
                      horaCierre: fCierre ? fCierre[1] : null,
                  }
                : {};
        setFormData({...defaultData, ...init2});

        //Establecemos esta visibilidad en función del estado. Si el estado de la orden es uno de los estados que no permiten editar, se deshabilitan los campos
        setIsVisible(!estadosCamposDeshabilitados.includes(orden.estadoOrden?.name));

        //Ahora comprobamos si el usuario que accede a editar la orden es operario y esta dentro de los usuarios de la orden. Si no lo está, deshabilitamos los campos
        if (userData && userData.role == "OPERARIO") {
            const usuariosOrden = orden.usuarios;
            if (usuariosOrden) {
                const userIsInOrden = usuariosOrden.some((usuario) => usuario.id === userData.id);
                if (!userIsInOrden) {
                    setIsVisible(false);
                    setOperarioIsInOrden(false);
                }
            }
        }
        if (estadosMaterial.includes(orden.estadoOrden?.id)) {
            setMaterialIsRequired(true);
        } else {
            setMaterialIsRequired(false);
        }

        setErrors({});
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [showModal, orden]);

    useEffect(() => {
        setErrors({});
    }, []);

    useEffect(() => {
        setFormData({
            ...formData,
            incidenciasOrden: incidencias,
        });
    }, [incidencias]);

    const handleInputChange = (event) => {
        const {name, value, type, checked} = event.target;

        let newValue = {
            ...formData,
            [name]: type === "checkbox" ? checked : value,
        };

        if (name === "idEstadoOrden") {
            if (!estadosCerrados.includes(value)) {
                newValue.fechaCierre = null;
                newValue.horaCierre = null;
            }
            if (estadosMaterial.includes(newValue?.idEstadoOrden)) {
                setMaterialIsRequired(true);
            } else {
                setMaterialIsRequired(false);
            }
        }
        setFormData(newValue);

        setErrors({});
    };

    const handleSubmit = (event) => {
        event.preventDefault();

        const formElement = event.target; // Formulario actual
        if (!formElement.checkValidity()) {
            console.log("Ha habido un error");
            return;
        }

        setIsLoading(true);

        if (validarOrden()) {
            const dataToSend = {
                ...formData,
                fechaApertura: formData.fechaApertura + " " + formData.horaApertura,
                fechaCierre: (formData.fechaCierre && formData.fechaCierre + " " + formData.horaCierre) || "",
            };

            ordenService.save(dataToSend).then((response) => {
                if (!response.is_error) {
                    handleClose({shouldRefetch: true});
                } else {
                    setErrors(response.error_content);
                }
                setIsLoading(false);
            });
        } else {
            setIsLoading(false);
        }
    };

    const getDialogClass = () => {
        if (activeTab === "tabIncidenciasOrden") {
            return "modal-width-98";
        } // Clase personalizada para anchura 90%
        return ""; // Clase predeterminada
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
            setErrors("Debe adjuntar alguna imagen a la orden para guardar una orden en estado cerrada. ");
            ordenValida = false;
        }

        if (
            CONFIG[CLIENT].logicaDelNegocio.RolesObligadosAAdjuntarFotoMaterialAlCerrarOrden.includes(userData.role) &&
            estadosMaterial.includes(formData.idEstadoOrden) &&
            (materialesOrden.length == 0 ||
                materialesOrden?.every((item) => item.type != "file") ||
                materialesOrden?.every((item) => !extensionesImagenes.includes(item.extension.toLowerCase())))
        ) {
            setErrors(
                "Debe adjuntar alguna imagen de materiales a la orden para guardar una orden en cualquier variante de estado de materiales.",
            );
            ordenValida = false;
        }

        if (incidencias.length == 0) {
            setErrors("Error al guardar la orden. Debe añadir al menos una incidencia para continuar. ");
            ordenValida = false;
        }

        if (usuariosOrden.length == 0 && !materialIsRequired) {
            setErrors("Error al guardar la orden. Debe añadir al menos un usuario para continuar. ");
            ordenValida = false;
        }

        if (materialIsRequired && formData.materiales == "") {
            setErrors(
                "Error al guardar la orden. Debe completar el campo material para el estado seleccionado. Acceda a la sección Materiales para completar la información. ",
            );
            ordenValida = false;
        }

        //Comprobamos que el usuario haya completado el formulario de comentarioResolucion si el estado de la orden es cerrado o cerrada: pendiente material

        if (
            estadosCerrados.includes(formData.idEstadoOrden) &&
            formData.comentarioResolucion.length <= longitudMinimaPlantillaResolucion
        ) {
            setErrors(
                longitudMinimaPlantillaResolucion == 60
                    ? "Error al guardar la orden. Debe completar la plantilla de resolución de la orden para continuar. Vuelva a abrir la orden para completar dicha información. "
                    : "Error al guardar la orden. Debe completar el campo de resolución de la orden para continuar. Vuelva a abrir la orden para completar dicha información. ",
            );
            ordenValida = false;
        }

        return ordenValida;
    };

    const FormatearFecha = (fecha) => {
        if (fecha) {
            // Separar la fecha y la hora
            const partesFecha = fecha.split("T");

            const date = partesFecha[0];
            const hora = partesFecha[1];
            // Convertir la fecha al formato yyyy/mm/dd
            const [year, month, day] = date.split("-");
            const fechaFormateada = `${year}-${month}-${day} ${hora}`;
            return fechaFormateada;
        }
        return;
    };

    return (
        <BaseModal
            size="xl"
            show={showModal}
            onHide={() => handleClose({shouldRefetch: false})}
            isLoading={isLoading}
            dialogClassName={getDialogClass()}>
            <Form onSubmit={handleSubmit}>
                <Modal.Body id="bodyEditarOrden" style={{minHeight: "55vh"}}>
                    <CustomTabs defaultActiveKey="tabOrden" onSelect={(key) => setActiveTab(key)}>
                        <OrdenFormModal
                            eventKey="tabOrden"
                            title={t("Orden")}
                            onChange={handleInputChange}
                            isLoading={isLoading}
                            formData={formData}
                            activeTab={activeTab === "tabOrden"}
                            isVisible={isVisible}
                            operarioIsInOrden={operarioIsInOrden}
                            estadosCerrados={estadosCerrados}
                            algunaIncidenciaResuelta={algunaIncidenciaReuelta}
                        />
                        <FechasOrdenFormModal
                            eventKey="tabFechasOrden"
                            title={t("Fechas")}
                            onChange={handleInputChange}
                            isLoading={isLoading}
                            formData={formData}
                            setFormData={setFormData}
                            activeTab={activeTab === "tabFechasOrden"}
                            isVisible={isVisible}
                            estadosCerrados={estadosCerrados}
                        />
                        <MaterialesFormModal
                            eventKey="tabMaterialesOrden"
                            title={t("Materiales")}
                            activeTab={activeTab === "tabMaterialesOrden"}
                            formData={formData}
                            required={materialIsRequired}
                            isVisible={isVisible}
                            isLoading={isLoading}
                            onChange={handleInputChange}
                            style={{minHeight: "31vh"}}
                            actualizarAdjuntosOrden={setMaterialesOrden}
                        />
                        <UsuariosOrdenFormModal
                            eventKey="tabUsuariosOrden"
                            title={t("Usuarios")}
                            activeTab={activeTab === "tabUsuariosOrden"}
                            formData={formData}
                            creador={formData.creador}
                            isVisible={isVisible}
                            actualizarUsuariosOrden={setUsuariosOrden}
                        />
                        <IncidenciasOrdenFormModal
                            eventKey="tabIncidenciasOrden"
                            title={t("Incidencias")}
                            formdata={formData}
                            activeTab={activeTab === "tabIncidenciasOrden"}
                            isVisible={isVisible}
                            estadosCerrados={estadosCerrados}
                            actualizarIncidencias={setIncidencias}
                        />
                        <AdjuntosOrdenFormModal
                            eventKey="tabAdjuntosOrden"
                            title={t("Adjuntos")}
                            isLoading={isLoading}
                            formData={formData}
                            style={{minHeight: "43vh"}}
                            activeTab={activeTab === "tabAdjuntosOrden"}
                            isVisible={isVisible}
                            actualizarAdjuntosOrden={setDocumentacionOrden}
                        />
                    </CustomTabs>
                    <MostrarErrores className="ms-3 text-danger" errors={errors} />
                </Modal.Body>
                <Modal.Footer>
                    <Button variant="secondary" onClick={() => handleClose({shouldRefetch: true})} disabled={isLoading}>
                        {t("Cancelar")}
                    </Button>
                    <Button
                        variant="primary"
                        id="btnGuardarOrden"
                        type="submit"
                        disabled={isLoading}>
                        {t("Guardar")}
                        <LoadingSpinner isLoading={isLoading} />
                    </Button>
                    
                </Modal.Footer>
            </Form>
        </BaseModal>
    );
}
