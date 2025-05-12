export const PlantillaIndicadorMultiple = ({data, titulo, tooltip, colores, unidad, ...rest}) => {
    return (
        <div className="plantillaIndicador" {...rest}>
            <span title={tooltip} className="tituloIndicador">
                {titulo}
            </span>
            <div className="d-flex justify-content-around mt-2">
                {data.map((item, index) => (
                    <div key={item.anio} className="d-flex flex-column">
                        <span
                            className="px-3 py-1 d-flex align-items-center fw-bold rounded-2"
                            style={{backgroundColor: `${colores[index]}`}}>
                            {item.year}
                        </span>
                        <span className="valorIndicador mt-2">{Math.round(item.valor * 100) / 100 + " " + unidad}</span>
                    </div>
                ))}
            </div>
        </div>
    );
};
