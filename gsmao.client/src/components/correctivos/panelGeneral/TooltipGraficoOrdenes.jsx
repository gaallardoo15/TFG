import Badge from "react-bootstrap/Badge";

export const TooltipGraficoOrdenes = ({fecha, data}) => {
    return (
        <div className="tooltipGraficaOrdenesPanelGeneral">
            <div>
                <Badge bg="primary" className="me-1">
                    {fecha}: {data.NumeroOrdenesTotales} OTs
                </Badge>
            </div>
            <div>
                <Badge bg="warning" className="me-1">
                    En Curso: {data.NOrdenesEnCurso}
                </Badge>
                <Badge bg="info" className="me-1">
                    Abiertas: {data.NOrdenesAbiertas}
                </Badge>
                <Badge bg="success" className="me-1">
                    Material: {data.NOrdenesMaterial}
                </Badge>
                <Badge bg="secondary" className="me-1">
                    Cerradas: {data.NOrdenesCerradas}
                </Badge>
                <Badge bg="danger" className="me-1">
                    Anuladas: {data.NOrdenesAnuladas}
                </Badge>
            </div>
        </div>
    );
};
