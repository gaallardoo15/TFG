import {useState} from "react";
import {Badge, Button, Card, Col, Row} from "react-bootstrap";
import {useTranslation} from "react-i18next";

import {LoadingSpinner} from "@/components/common/loading/LoadingSpinner";
import {MostrarErrores} from "@/components/common/MostrarErrores";
import {userService} from "@/services/UserService";

export const ConflictUserCard = ({user, onResolve, onSetIsLoading}) => {
    const {t, i18n} = useTranslation();
    const [errors, setErrors] = useState(null);
    const [isLoading, setIsLoading] = useState(false);

    const handleResolve = () => {
        setErrors(null);
        onSetIsLoading(true);
        setIsLoading(true);
        userService.restore(user.id).then((response) => {
            if (!response.is_error) {
                onResolve();
            } else {
                setErrors(response.error_content);
            }
            onSetIsLoading(false);
            setIsLoading(false);
        });
    };

    const dateFormatter = (dateIsoString) =>
        dateIsoString.value
            ? new Date(dateIsoString.value).toLocaleString(i18n.language, {dateStyle: "short", timeStyle: "short"})
            : "-";

    const commonStylesBadges = {
        marginBottom: "0",
        marginRight: "3px",
    };
    const commonStylesContainers = {
        marginBottom: "0px",
        marginRight: "3px",
    };

    return (
        <>
            <Row className="container-fluid pe-0">
                <Col md={7}>
                    <Card>
                        <Card.Header as="h5" className="text-center">
                            {t("Información del usuario en conflicto")}
                        </Card.Header>
                        <Card.Body>
                            <Card.Title className="mb-1">{`${user.nombre} ${user.apellidos}`}</Card.Title>{" "}
                            <div style={commonStylesContainers}>
                                <Badge bg="secondary" style={commonStylesBadges}>
                                    {t("Fecha de creación")}
                                </Badge>
                                <span>{dateFormatter(user.fechaCreacion)}</span>
                            </div>
                            <div style={commonStylesContainers}>
                                <Badge bg="secondary" style={commonStylesBadges}>
                                    {t("Estado")}
                                </Badge>
                                <span>{t(user.estadoUsuario?.descripcion)}</span>
                            </div>
                            <div style={commonStylesContainers}>
                                <Badge bg="secondary" style={commonStylesBadges}>
                                    {t("Rol")}
                                </Badge>
                                <span>{t(user.rol?.name)}</span>
                            </div>
                            <div style={commonStylesContainers}>
                                <Badge bg="secondary" style={commonStylesBadges}>
                                    {t("ID Proveedor")}
                                </Badge>
                                <span>{user.idProveedor}</span>
                            </div>
                            <div style={commonStylesContainers}>
                                <Badge bg="secondary" style={commonStylesBadges}>
                                    {t("Nombre de usuario")}
                                </Badge>
                                <span>{!!user?.username && user.username.substring(user.id.length + 1)}</span>
                            </div>
                            <div style={commonStylesContainers}>
                                <Badge bg="secondary" style={commonStylesBadges}>
                                    {t("Email")}
                                </Badge>
                                <span>{!!user?.email && user.email.substring(user.id.length + 1)}</span>
                            </div>
                            <div style={commonStylesContainers}>
                                <Badge bg="secondary" style={commonStylesBadges}>
                                    {t("Empresa")}
                                </Badge>
                                <span>{user.empresa ? user.empresa.descripcion : "N/A"}</span>
                            </div>
                        </Card.Body>
                    </Card>
                </Col>
                <Col md={5} className="d-flex align-items-center justify-content-end pe-0">
                    <Button variant="outline-primary" className="mt-2" onClick={handleResolve} disabled={isLoading}>
                        {t("Restaurar este usuario")}
                        <LoadingSpinner isLoading={isLoading} />
                    </Button>
                </Col>
            </Row>
            <Row>{errors && <MostrarErrores errors={errors} />}</Row>
        </>
    );
};
