import {useEffect, useState} from "react";
import {Button, Form, Modal, Row} from "react-bootstrap";
import {useTranslation} from "react-i18next";

import {SmartSelectInput} from "../../common/formInputs/SmartSelectInput";
import {CustomModalHeader} from "../../common/modales/CustomModalHeader";
import {ConflictUserCard} from "./ConflictUserCard";

import {CheckInput} from "@/components/common/formInputs/CheckInput";
import {EmailInput} from "@/components/common/formInputs/EmailInput";
import {SelectInput} from "@/components/common/formInputs/SelectInput";
import {TextInput} from "@/components/common/formInputs/TextInput";
import {LoadingSpinner} from "@/components/common/loading/LoadingSpinner";
import {BaseModal} from "@/components/common/modales/BaseModal";
import {MostrarErrores} from "@/components/common/MostrarErrores";
import {empresasService} from "@/services/EmpresaService";
import {plantasService} from "@/services/PlantaService";
import {rolesService} from "@/services/RolService";
import {userService} from "@/services/UserService";

export function UserFormModal({show: showModal, onClose: handleClose, initialData: init}) {
    const defaultData = {
        id: null,
        nombre: "",
        apellidos: "",
        password: "",
        repeatPassword: "",
        email: "",
        idRol: "",
        idEmpresa: null,
        idPlanta: null,
        idEstadoUsuario: null,
    };

    const {t} = useTranslation();
    const [formData, setFormData] = useState(defaultData);
    const [errors, setErrors] = useState({});
    const [conflictUser, setConflictUser] = useState(null);
    const [isLoading, setIsLoading] = useState(false);
    const [roles, setRoles] = useState([]);
    const [plantas, setPlantas] = useState([]);
    const [mostrarContraseña, setMostrarContraseña] = useState(false);
    const isEdit = !!init?.id;

    useEffect(() => {
        let init2 =
            Object.keys(init).length !== 0
                ? {...init, idRol: init.rol?.id, idEmpresa: init.empresa?.id, idPlanta: init.planta?.id}
                : {};
        setFormData({...defaultData, ...init2});
        fetchRoles();
        isEdit && fetchPlantasEmpresa(init2.idEmpresa);
        setErrors({});
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [showModal, init]);

    const fetchRoles = () => {
        rolesService.fetchAll().then((response) => {
            if (!response.is_error) {
                setRoles(response.content || []);
            } else {
                console.error(response.error_content);
            }
        });
    };

    const fetchPlantasEmpresa = (idEmpresa) => {
        if (idEmpresa) {
            plantasService.getPlantasEmpresa(idEmpresa).then((response) => {
                if (!response.is_error) {
                    setPlantas(response.content || []);
                } else {
                    console.error(response.error_content);
                    return [];
                }
            });
        }
    };

    let isSuperAdmin = false;
    let isAdmin = false;
    const handleInputChange = (event) => {
        const {name, value, type, checked} = event.target;

        let newValue = {
            ...formData,
            [name]: type === "checkbox" ? checked : value,
        };
        if (name == "idRol") {
            if (roles.filter((r) => r.id == value)[0]?.name === "SUPER_ADMINISTRADOR") {
                isSuperAdmin = true;
            } else if (roles.filter((r) => r.id == value)[0]?.name === "ADMINISTRADOR") {
                isAdmin = true;
            }
        } else if (name == "idEmpresa") {
            newValue.idPlanta = null;
            setPlantas([]);
            fetchPlantasEmpresa(value);
        }
        setFormData(newValue);
        setConflictUser(null);
        setErrors({});
    };

    const handleSubmit = (event) => {
        event.preventDefault();
        setErrors({});
        setIsLoading(true);

        userService.save(formData).then((response) => {
            if (!response.is_error) {
                handleClose({shouldRefetch: true});
            } else {
                if (response.statusCode == 409) {
                    setConflictUser(response.error_content);
                    setErrors(t("Ya existe un usuario con el email introducido."));
                } else {
                    setErrors(response.error_content);
                }
            }
            setIsLoading(false);
        });
    };
    const showPassword = !formData.id;
    isSuperAdmin = formData.idRol && roles.filter((r) => r.id == formData.idRol)[0]?.name == "SUPER_ADMINISTRADOR";
    isAdmin = formData.idRol && roles.filter((r) => r.id == formData.idRol)[0]?.name == "ADMINISTRADOR";

    return (
        <BaseModal show={showModal} onHide={() => handleClose({shouldRefetch: false})} isLoading={isLoading}>
            <CustomModalHeader
                title={isEdit ? "Editar Usuario" : "Crear Usuario"}
                variant={isEdit ? "primary" : "success"}
            />

            <Form onSubmit={handleSubmit}>
                <Modal.Body>
                    <Row className="">
                        <TextInput
                            label={t("Nombre")}
                            placeholder={t("Ingresa el nombre")}
                            required
                            name="nombre"
                            value={formData.nombre}
                            onChange={handleInputChange}
                            disabled={isLoading}
                            md="6"
                            autoFocus={true}
                        />

                        <TextInput
                            label={t("Apellidos")}
                            placeholder={t("Ingresa los apellidos")}
                            required
                            name="apellidos"
                            value={formData.apellidos}
                            onChange={handleInputChange}
                            disabled={isLoading}
                            md="6"
                        />

                        <EmailInput
                            label={t("Email")}
                            required
                            name="email"
                            value={formData.email}
                            onChange={handleInputChange}
                            disabled={isLoading}
                            md="6"
                        />
                        <SelectInput
                            label={t("Rol")}
                            required
                            name="idRol"
                            value={formData.idRol}
                            onChange={handleInputChange}
                            options={roles.map((r) => ({value: r.id, label: t(r.name)}))}
                            disabled={isLoading}
                            md="6"
                        />

                        {showPassword && (
                            <>
                                <TextInput
                                    label={t("Contraseña")}
                                    required
                                    name="password"
                                    value={formData.password}
                                    type={mostrarContraseña ? "text" : "password"}
                                    onChange={handleInputChange}
                                    disabled={isLoading}
                                    md="6"
                                    className="mb-1"
                                />
                                <TextInput
                                    label={t("Repetir Contraseña")}
                                    required
                                    name="repeatPassword"
                                    value={formData.repeatPassword}
                                    type={mostrarContraseña ? "text" : "password"}
                                    onChange={handleInputChange}
                                    disabled={isLoading}
                                    md="6"
                                    className="mb-1"
                                />
                                <CheckInput
                                    label={t("Mostrar contraseña")}
                                    name="showPassword"
                                    value={formData.cambioPieza}
                                    onChange={() => setMostrarContraseña(!mostrarContraseña)}
                                    md="4"
                                    disabled={isLoading}
                                    className="mb-3"
                                />
                            </>
                        )}
                        <SmartSelectInput
                            label={t("Empresa")}
                            required={!isSuperAdmin}
                            name="idEmpresa"
                            value={formData.idEmpresa}
                            fetcher={empresasService.fetchAll}
                            valueKey="id"
                            labelKey="descripcion"
                            onChange={handleInputChange}
                            disabled={isLoading || isSuperAdmin}
                            md="6"
                        />
                        <SelectInput
                            key={formData.idEmpresa}
                            label={t("Planta")}
                            required={!isSuperAdmin && !isAdmin}
                            name="idPlanta"
                            value={formData.idPlanta}
                            onChange={handleInputChange}
                            options={plantas.map((p) => ({value: p.id, label: t(p.descripcion)}))}
                            disabled={isLoading || isSuperAdmin || isAdmin || !formData.idEmpresa}
                            md="6"
                        />

                        <MostrarErrores errors={errors} />
                        {!!conflictUser && (
                            <ConflictUserCard
                                user={conflictUser}
                                onResolve={() => handleClose({shouldRefetch: true})}
                                onSetIsLoading={setIsLoading}
                            />
                        )}
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
}
