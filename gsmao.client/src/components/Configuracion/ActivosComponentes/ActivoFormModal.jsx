import {useEffect, useState} from "react";
import {Button, Form, Modal, Row} from "react-bootstrap";
import {useTranslation} from "react-i18next";

import {NumberInput} from "../../common/formInputs/NumberInput";
import {SmartSelectInput} from "../../common/formInputs/SmartSelectInput";
import {TextInput} from "../../common/formInputs/TextInput";
import {LoadingSpinner} from "../../common/loading/LoadingSpinner";
import {BaseModal} from "../../common/modales/BaseModal";
import {CustomModalHeader} from "../../common/modales/CustomModalHeader";
import {MostrarErrores} from "../../common/MostrarErrores";

import {activosService} from "@/services/ActivoService";
import {centrosCostesService} from "@/services/CentroCosteService";
import {criticidadesService} from "@/services/CriticidadesService";
import {localizacionesService} from "@/services/LocalizacionService";

export const ActivoFormModal = ({show: showModal, onClose: handleClose, initialData: init}) => {
    const defaultData = {
        id: null,
        descripcionES: "",
        descripcionEN: "",
        activoSAP: "",
        idLocalizacion: null,
        idCentroCoste: null,
        actividad: "",
        idCriticidad: 5,
        redundancia: 0,
        coste: 0,
        usabilidad: 0,
        hse: 0,
        valorCriticidad: 0,
    };

    const {t} = useTranslation();
    const [formData, setFormData] = useState(defaultData);
    const [errors, setErrors] = useState({});
    const [isLoading, setIsLoading] = useState(false);
    const isEdit = !!init?.id;

    useEffect(() => {
        let init2 =
            Object.keys(init).length !== 0
                ? {
                      ...init,
                      idLocalizacion: init.localizacion?.id,
                      idCentroCoste: init.centroCoste?.id,
                      idCriticidad: init.criticidad?.id,
                  }
                : {};

        setFormData({...defaultData, ...init2});
        setErrors({});
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [showModal, init]);

    const handleInputChange = (event) => {
        const {name, value, type, checked} = event.target;

        let newValue = {
            ...formData,
            [name]: type === "checkbox" ? checked : value,
        };
        if (name == "valorCriticidad") {
            let valorCriticidadActivo = parseInt(value);
            console.log(valorCriticidadActivo);
            if (valorCriticidadActivo >= 0 && valorCriticidadActivo <= 100) {
                if (valorCriticidadActivo >= 80) {
                    newValue.idCriticidad = 1;
                } else if (valorCriticidadActivo >= 60 && valorCriticidadActivo <= 79) {
                    newValue.idCriticidad = 2;
                } else if (valorCriticidadActivo >= 40 && valorCriticidadActivo <= 59) {
                    newValue.idCriticidad = 3;
                } else if (valorCriticidadActivo >= 15 && valorCriticidadActivo <= 39) {
                    newValue.idCriticidad = 4;
                } else if (valorCriticidadActivo <= 14) {
                    newValue.idCriticidad = 5;
                }
                setErrors({});
            } else {
                newValue.idCriticidad = 5;
                setErrors("El valor de la criticidad debe estar entre 0 y 100");
            }
        }
        setFormData(newValue);
    };

    const handleSubmit = (event) => {
        event.preventDefault();
        setErrors({});
        setIsLoading(true);

        if (formData.valorCriticidad < 0 || formData.valorCriticidad > 100) {
            setErrors("El valor de la criticidad debe estar entre 0 y 100");
            setIsLoading(false);
            return;
        }

        const actionFunction = isEdit ? activosService.update : activosService.create;

        actionFunction(formData).then((response) => {
            if (!response.is_error) {
                handleClose({shouldRefetch: true});
            } else {
                setErrors(response.error_content);
            }
            setIsLoading(false);
        });
    };
    console.log(formData);

    return (
        <BaseModal show={showModal} onHide={() => handleClose({shouldRefetch: false})} isLoading={isLoading}>
            <CustomModalHeader
                title={isEdit ? "Editar Activo" : "Crear Activo"}
                variant={isEdit ? "primary" : "success"}
            />

            <Form onSubmit={handleSubmit}>
                <Modal.Body>
                    <Row className="">
                        <NumberInput
                            label={t("ID Activo")}
                            placeholder={t("Id")}
                            required
                            name="id"
                            value={formData.id || 0}
                            onChange={handleInputChange}
                            disabled={isLoading}
                            md="2"
                            autoFocus={true}
                        />
                        <TextInput
                            label={t("Descipción ES")}
                            placeholder={t("Descripción en español")}
                            required
                            name="descripcionES"
                            value={formData.descripcionES}
                            onChange={handleInputChange}
                            disabled={isLoading}
                            md="10"
                        />
                        <TextInput
                            label={t("Descipción EN")}
                            placeholder={t("Descripción en inglés")}
                            name="descripcionEN"
                            value={formData.descripcionEN}
                            onChange={handleInputChange}
                            disabled={isLoading}
                            md="12"
                        />
                        <TextInput
                            label={t("Activo SAP")}
                            placeholder={t("ID SAP del Activo")}
                            required
                            name="activoSAP"
                            value={formData.activoSAP}
                            onChange={handleInputChange}
                            disabled={isLoading}
                            md="4"
                        />
                        <NumberInput
                            label={t("Redundancia")}
                            placeholder={t("redundancia")}
                            name="redundancia"
                            value={formData.redundancia || 0}
                            onChange={handleInputChange}
                            disabled={isLoading}
                            md="4"
                        />
                        <NumberInput
                            label={t("HSE")}
                            placeholder={t("HSE")}
                            name="hse"
                            value={formData.hse || 0}
                            onChange={handleInputChange}
                            disabled={isLoading}
                            md="4"
                        />
                        <NumberInput
                            label={t("Usabilidad")}
                            placeholder={t("Usabilidad")}
                            name="usabilidad"
                            value={formData.usabilidad || 0}
                            onChange={handleInputChange}
                            disabled={isLoading}
                            md="4"
                        />
                        <NumberInput
                            label={t("Coste")}
                            placeholder={t("coste")}
                            name="coste"
                            value={formData.coste || 0}
                            onChange={handleInputChange}
                            disabled={isLoading}
                            md="4"
                        />
                        <TextInput
                            label={t("Valor Criticidad")}
                            placeholder={t("valor criticidad activo")}
                            required
                            name="valorCriticidad"
                            value={formData.valorCriticidad}
                            onChange={handleInputChange}
                            disabled={isLoading}
                            md="4"
                        />
                        <SmartSelectInput
                            label={t("Localizacion")}
                            required
                            name="idLocalizacion"
                            value={formData.idLocalizacion}
                            fetcher={localizacionesService.fetchAll}
                            valueKey="id"
                            labelComponent={(l) => `${l.localizacionSAP} -- ${l.descripcionES}`}
                            onChange={handleInputChange}
                            disabled={isLoading}
                            md="4"
                        />
                        <SmartSelectInput
                            label={t("Centro Coste")}
                            required
                            name="idCentroCoste"
                            value={formData.idCentroCoste}
                            fetcher={centrosCostesService.fetchAll}
                            valueKey="id"
                            labelComponent={(c) => `${c.centroCosteSAP} -- ${c.descripcionES}`}
                            onChange={handleInputChange}
                            disabled={isLoading}
                            md="4"
                        />
                        <SmartSelectInput
                            label={t("Criticidad")}
                            required
                            name="idCriticidad"
                            value={formData.idCriticidad}
                            fetcher={criticidadesService.fetchAll}
                            valueKey="id"
                            labelComponent={(c) => `${c.siglas} - ${c.descripcion}`}
                            onChange={handleInputChange}
                            disabled={isLoading}
                            md="4"
                        />
                        <MostrarErrores errors={errors} />
                    </Row>
                </Modal.Body>
                <Modal.Footer>
                    <Button
                        variant="secondary"
                        onClick={() => handleClose({shouldRefetch: false})}
                        disabled={isLoading}>
                        {t("Cancelar")}
                    </Button>
                    <Button variant={isEdit ? "primary" : "success"} type="submit" disabled={isLoading}>
                        {t("Guardar")}
                        <LoadingSpinner isLoading={isLoading} />
                    </Button>
                </Modal.Footer>
            </Form>
        </BaseModal>
    );
};
