import {useEffect, useState} from "react";
import {Row} from "react-bootstrap";
import {useTranslation} from "react-i18next";

import {CLIENT, CONFIG} from "../../../../config";

import {DateInput} from "@/components/common/formInputs/DateInput";
import {TimeInput} from "@/components/common/formInputs/TimeInput";
import {AuthService} from "@/services/AuthService";

export const FechasOrdenFormModal = ({
    onChange,
    formData,
    setFormData,
    isLoading,
    activeTab,
    isVisible,
    estadosCerrados,
    ...rest
}) => {
    const {t} = useTranslation();
    const [permitirModificacion, setPermitirModificacion] = useState(isVisible);
    const [fechaCierre, setFechaCierre] = useState(formData.fechaCierre);
    const [horaCierre, setHoraCierre] = useState(formData.horaCierre);
    const isUserAuthorizedToEditFechas = CONFIG[
        CLIENT
    ].logicaDelNegocio.UsuariosPermitidosModificarFechasOrdenCerrada.includes(AuthService.getUserData().UserName)
        ? true
        : false;

    useEffect(() => {
        setPermitirModificacion(isVisible);

        //Solo aplico cuando el usuario es acastro
        if (
            formData?.estadoOrden?.id == 2 &&
            [1, 4, 6, 7].includes(formData?.idEstadoOrden) &&
            isUserAuthorizedToEditFechas
        ) {
            setPermitirModificacion(true);
        }

        //RECUPERO FECHAS
        if (formData.fechaCierre != null) {
            setFechaCierre(formData.fechaCierre);
            setHoraCierre(formData.horaCierre);
        }

        if (
            (formData.fechaCierre == null || formData.horaCierre == null) &&
            [2, 3, 5, 8].includes(formData?.idEstadoOrden) &&
            [2, 3, 5, 8].includes(formData?.estadoOrden?.id)
        ) {
            setFormData({
                ...formData,
                fechaCierre: fechaCierre,
                horaCierre: horaCierre,
            });
        }
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [formData]);

    const disabledFechaCierre = () => {
        let disabled = !isVisible;
        if (isUserAuthorizedToEditFechas) {
            //Habilitar cuando la orden esta cerrada y selecciono un estado abierto
            if (formData?.estadoOrden?.id == 2 && [1, 4, 6, 7].includes(formData?.idEstadoOrden)) {
                disabled = false;
                //Habilitar cuando la orden esta Abierta o cualquiera de sus variantes y selecciono un estado cerrado
            } else if (
                [1, 4, 6, 7].includes(formData?.estadoOrden?.id) &&
                [2, 3, 5, 8].includes(formData?.idEstadoOrden)
            ) {
                disabled = false;
            } else {
                disabled = true;
            }
        } else {
            disabled = (!estadosCerrados.includes(formData.idEstadoOrden) ? true : false) || !isVisible;
        }

        return disabled;
    };

    function FechaCortaToEspañol(fecha) {
        if (fecha) {
            // Convertir la fecha al formato dd/mm/yyyy
            const [year, month, day] = fecha.split("-");
            const fechaFormateada = `${day}-${month}-${year}`;

            return fechaFormateada;
        }
        return;
    }
    return (
        activeTab && (
            <Row>
                <DateInput
                    label={t("Fecha Creación")}
                    required
                    name="fechaApertura"
                    value={formData.fechaApertura}
                    onChange={onChange}
                    md="12"
                    disabled={!permitirModificacion || isLoading}
                />
                <TimeInput
                    label={t("Hora Creación")}
                    required
                    name="horaApertura"
                    value={formData.horaApertura}
                    onChange={onChange}
                    md="12"
                    disabled={!permitirModificacion || isLoading}
                />
                <DateInput
                    label={t(
                        `Fecha Cierre ${!formData.fechaCierre && estadosCerrados.includes(formData?.estadoOrden?.id) && isUserAuthorizedToEditFechas ? "(Antigua: " + FechaCortaToEspañol(fechaCierre) + ")" : ""}`,
                    )}
                    name="fechaCierre"
                    value={formData.fechaCierre}
                    onChange={onChange}
                    required={estadosCerrados.includes(formData.idEstadoOrden)}
                    md="12"
                    disabled={disabledFechaCierre() || isLoading}
                />
                <TimeInput
                    label={t(
                        `Hora Cierre ${formData.horaCierre == null && estadosCerrados.includes(formData?.estadoOrden?.id) && isUserAuthorizedToEditFechas ? "(Antigua: " + horaCierre + ")" : ""}`,
                    )}
                    name="horaCierre"
                    value={formData.horaCierre}
                    required={estadosCerrados.includes(formData.idEstadoOrden)}
                    onChange={onChange}
                    md="12"
                    disabled={disabledFechaCierre() || isLoading}
                />
            </Row>
        )
    );
};
