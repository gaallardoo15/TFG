import {useEffect, useState} from "react";
import {Button, Form, Modal, Row} from "react-bootstrap";
import {useTranslation} from "react-i18next";

import {DynamicSmartSelectInputs} from "../common/formInputs/DynamicSmartSelectInputs";
import {LoadingSpinner} from "../common/loading/LoadingSpinner";
import {BaseModal} from "../common/modales/BaseModal";
import {CustomModalHeader} from "../common/modales/CustomModalHeader";
import {MostrarErrores} from "../common/MostrarErrores";

import {componentesService} from "@/services/ComponenteService";

export const EditarComponenteIncidencia = ({show: showModal, onClose: handleClose, idActivo}) => {
    const defaultData = {
        idComponente: null,
        idIncidenciaOrden: null,
    };

    const {t} = useTranslation();
    const [formData, setFormData] = useState(defaultData);
    const [errors, setErrors] = useState({});
    const [componentesBaseLevel, setComponentesBaseLevel] = useState([]);
    const [isLoading, setIsLoading] = useState(false);

    const fetchComponentes = (idActivo, idComponente) => {
        componentesService.getComponentes(idActivo, idComponente).then((response) => {
            if (!response.is_error) {
                const components = (response.content || []).sort((a, b) => a.id - b.id);
                setComponentesBaseLevel(components);
            } else {
                console.log(response.error_content);
                setErrors(response.error_content);
            }
        });
    };

    useEffect(() => {
        if (idActivo > 0) {
            fetchComponentes(idActivo, 0); // Cargar los componentes para el activo seleccionado
        }
        setErrors({});
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [showModal]);

    const handleComponenteChange = (selectedComponent, children) => {
        let newValue = {
            ...formData,
        };
        newValue.idComponente = selectedComponent.id;
        console.log(newValue);
        setFormData(newValue);
    };

    const handleSubmit = (event) => {
        event.preventDefault();
        event.stopPropagation();
        setErrors({});
        // setIsLoading(true);

        // ordenService.addNuevaIncidencia(formData).then((response) => {
        //     if (!response.is_error) {
        //         handleClose({shouldRefetch: true});
        //     } else {
        //         setErrors(response.error_content);
        //     }
        //     setIsLoading(false);
        // });
    };

    return (
        <BaseModal show={showModal} onHide={() => handleClose({shouldRefetch: false})} isLoading={false}>
            <CustomModalHeader title={t("Nueva Incidencia")} variant="primary" />
            <Form onSubmit={handleSubmit}>
                <Modal.Body>
                    <Row>
                        <DynamicSmartSelectInputs
                            onChange={handleComponenteChange}
                            md="col-auto"
                            idActivo={idActivo}
                            componentesBase={componentesBaseLevel}
                        />

                        <MostrarErrores errors={errors} />
                    </Row>
                </Modal.Body>
                <Modal.Footer>
                    <Button variant="secondary" onClick={() => handleClose({shouldRefetch: false})} disabled={false}>
                        {t("Cerrar")}
                    </Button>
                    <Button variant="primary" type="submit" disabled={isLoading}>
                        {t("Guardar")}
                        <LoadingSpinner isLoading={isLoading} />
                    </Button>
                </Modal.Footer>
            </Form>
        </BaseModal>
    );
};
