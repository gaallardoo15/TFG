import {useEffect, useState} from "react";
import {Row} from "react-bootstrap";
import {useTranslation} from "react-i18next";

import {SelectInputWithConfirmation} from "@/components/common/formInputs/SelectInputWithConfirmation";
import {SmartSelectInput} from "@/components/common/formInputs/SmartSelectInput";
import {TextAreaInput} from "@/components/common/formInputs/TextAreaInput";
import {TextInput} from "@/components/common/formInputs/TextInput";
import {activosService} from "@/services/ActivoService";
import {centrosCostesService} from "@/services/CentroCosteService";
import {ordenService} from "@/services/OrdenService";

export const OrdenFormModal = ({
    onChange,
    formData,
    isLoading,
    activeTab,
    setErrors,
    isVisible,
    estadosCerrados,
    operarioIsInOrden,
    ...rest
}) => {
    const {t} = useTranslation();
    const [estados, setEstados] = useState([]);
    const [filteredEstados, setFilteredEstados] = useState([]);
    const [isLoadingOrdenTab, setIsLoadingOrdenTab] = useState([]);
    const [hayIncidenciasResueltas, setHayIncidenciasResueltas] = useState(false);
    const estadosMaterial = [4, 5, 7, 8];

    const fetchEstados = () => {
        setIsLoadingOrdenTab(true);
        ordenService.getEstadosOrdenes().then((response) => {
            if (!response.is_error) {
                setEstados(response.content);
                setFilteredEstados(response.content);
            } else {
                setErrors(response.error_content);
            }
            setIsLoadingOrdenTab(false);
        });
    };

    useEffect(() => {
        if (estados.length == 0) {
            fetchEstados();
        } else {
            if (!estadosMaterial.includes(formData.estadoOrden?.id)) {
                setFilteredEstados(
                    estados.length > 0
                        ? estados.filter(
                              (estado) =>
                                  estado.name != "Abierta: Material Gestionado" &&
                                  estado.name != "Cerrada: Material Gestionado",
                          )
                        : [],
                );
            } else {
                setFilteredEstados(estados);
            }
        }
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [estados, formData]);

    useEffect(() => {
        if (formData.incidenciasOrden) {
            //funcion que comprueba que dentro del array de formdata.incidenciasOrden haya incidencias cuya resolucion sea distinta de null
            const hayIncidenciasResueltas = formData.incidenciasOrden.some(
                (incidencia) => incidencia.resolucion != null,
            );
            setHayIncidenciasResueltas(hayIncidenciasResueltas);
        }

        if (formData.estadoOrden) {
            // Todos los estados menos los estados con nombre "Abierta: Pendiente Material" o "Cerrada: Pendiente Material"
            if (
                formData.estadoOrden.name == "Abierta: Pendiente Material" ||
                formData.estadoOrden.name == "Cerrada: Pendiente Material"
            ) {
                setFilteredEstados(estados);
            }
            //Si la orden está anulada,
            if (formData.estadoOrden.name == "Anulada") {
                setFilteredEstados(
                    estados.length > 0
                        ? estados.filter((estado) => estado.name == "Abierta" || estado.name == "Anulada")
                        : [],
                );
            }
        }
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [formData]);

    return (
        <Row>
            <TextInput
                label={t("ID Orden")}
                placeholder={t("IdOrden")}
                name="id"
                value={formData.id || ""}
                onChange={onChange}
                disabled
                md="2"
            />
            <SmartSelectInput
                label={t("Activo")}
                required
                name="idActivo"
                value={formData.idActivo}
                fetcher={activosService.fetchAll}
                valueKey="id"
                labelComponent={(a) => `${a.id} - ${a.descripcionES}`}
                onChange={onChange}
                disabled
                md="5"
            />
            <SelectInputWithConfirmation
                key={formData.idComponentePadre}
                label={t("Estado")}
                name="idEstadoOrden"
                value={formData.idEstadoOrden}
                onChange={onChange}
                options={filteredEstados.map((e) => ({
                    value: e.id,
                    label: e.name,
                }))}
                disabled={isLoadingOrdenTab || isLoading || !operarioIsInOrden}
                md="2"
            />
            <SmartSelectInput
                label={t("Tipo Orden")}
                required
                name="idTipoOrden"
                value={formData.idTipoOrden}
                fetcher={ordenService.getTiposOrdenes}
                valueKey="id"
                labelKey="name"
                onChange={onChange}
                disabled={!isVisible || isLoading}
                md="3"
            />
            <TextAreaInput
                label={t("Comentario")}
                name="comentarioOrden"
                value={formData.comentarioOrden}
                onChange={onChange}
                md="12"
                required
                disabled={formData.idEstadoOrden == 2 || isLoading}
                rows={4}
            />
            <TextAreaInput
                label={t("Comentario Resolución")}
                name="comentarioResolucion"
                value={formData.comentarioResolucion}
                onChange={onChange}
                md="12"
                rows={4}
                required={estadosCerrados.includes(formData.idEstadoOrden)}
                disabled={!hayIncidenciasResueltas || formData.idEstadoOrden == 2 || isLoading}
            />
        </Row>
    );
};
