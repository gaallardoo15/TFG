import {useState} from "react";
import {Button, Form, Row} from "react-bootstrap";
import {useTranslation} from "react-i18next";
import {toast} from "react-toastify";

import {TextAreaInput} from "./common/formInputs/TextAreaInput";
import {TextInput} from "./common/formInputs/TextInput";
import {LoadingSpinner} from "./common/loading/LoadingSpinner";
import {MostrarErrores} from "./common/MostrarErrores";

import "../styles/soporte.css";
import {soporteService} from "@/services/SoporteService";

export const SoportePage = () => {
    const defaultData = {
        nombre: "",
        telefono: "",
        asunto: "",
        mensaje: "",
    };

    const {t} = useTranslation();
    const [formData, setFormData] = useState(defaultData);
    const [errors, setErrors] = useState({});
    const [isLoading, setIsLoading] = useState(false);

    const handleInputChange = (event) => {
        const {name, value, type, checked} = event.target;

        let newValue = {
            ...formData,
            [name]: type === "checkbox" ? checked : value,
        };

        setFormData(newValue);
        setErrors({});
    };

    const handleSubmit = (event) => {
        event.preventDefault();
        setErrors({});
        setIsLoading(true);

        soporteService.save(formData).then((response) => {
            if (!response.is_error) {
                toast.success("Solicitud enviada correctamente. Le atenderemos lo antes posible.", {
                    className: "custom-toast",
                });
                setFormData(defaultData);
            } else {
                toast.error(`Fallo de comunicación con soporte. ${response.error_content}`, {
                    autoClose: 15000,
                    closeButton: true,
                });
                setErrors(response.error_content);
            }

            setIsLoading(false);
        });
    };
    return (
        <div className="contenedorGeneralSoporte">
            <section className="seccionImagenSoporte">
                <img src="imagenSoporte.svg" alt="" className="imagenSoporte" />
            </section>
            <section className="seccionSoporte">
                <h1 className="tituloFormularioSoporte">{t("Contáctanos")}</h1>
                <Form className="formularioSoporte" onSubmit={handleSubmit}>
                    <Row className="">
                        <TextInput
                            label={t("Nombre y Apellidos")}
                            placeholder={t(" ")}
                            required
                            name="nombre"
                            value={formData.nombre}
                            onChange={handleInputChange}
                            disabled={isLoading}
                            md="12"
                        />
                        <TextInput
                            label={t("Teléfono")}
                            placeholder={t(" ")}
                            type="tel"
                            name="telefono"
                            value={formData.telefono}
                            onChange={handleInputChange}
                            disabled={isLoading}
                            md="12"
                        />

                        <TextInput
                            label={t("Asunto")}
                            placeholder={t(" ")}
                            required
                            name="asunto"
                            value={formData.asunto}
                            onChange={handleInputChange}
                            disabled={isLoading}
                            md="12"
                        />

                        <TextAreaInput
                            label={t("Mensaje")}
                            name="mensaje"
                            value={formData.mensaje}
                            onChange={handleInputChange}
                            md="12"
                            rows={3}
                            required
                            disabled={isLoading}
                            placeholder={t(" ")}
                        />
                        <div style={{textAlign: "start"}} className="containerBtnSoporte mt-3">
                            <Button
                                type="submit"
                                id="botonEnviarSoporte"
                                style={{backgroundColor: "var(--identificativoCliente) !important"}}>
                                {t("Enviar Mensaje")}
                                <LoadingSpinner isLoading={isLoading} />
                            </Button>
                        </div>

                        <MostrarErrores errors={errors} />
                    </Row>
                </Form>
            </section>
        </div>
    );
};
