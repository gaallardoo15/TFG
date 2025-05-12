import {ProgressBar} from "react-bootstrap";

export const PlantillaIndicador = ({
    tituloIndicador,
    valorIndicador,
    barraPorcentaje = false,
    unidad = "",
    variant,
    ...rest
}) => {
    return (
        <div className="plantillaIndicador" {...rest}>
            <span className="tituloIndicador">{tituloIndicador}</span>
            <span className="valorIndicador">{valorIndicador + " " + unidad}</span>
            {barraPorcentaje && <ProgressBar variant={variant} now={valorIndicador} />}
        </div>
    );
};
