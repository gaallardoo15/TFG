export const InformadorFiltrosSeleccionados = ({filtros, isLoading = false}) => {
    const formatFecha = (fecha) => {
        if (!fecha) {
            return "";
        } // Manejar casos donde la fecha es nula o indefinida
        const [year, month, day] = fecha.split("-");
        return `${day}-${month}-${year}`; // Formato: DD-MM-YYYY
    };

    const nonZeroValues = Object.entries(filtros)
        .filter(([key, value]) => {
            if (Array.isArray(value)) {
                return value.length > 0; // Incluir arrays con elementos
            }
            if (typeof value === "string") {
                return value.trim() !== ""; // Incluir strings no vacíos (para fechas)
            }
            return value !== 0 && value !== null && value !== undefined; // Incluir valores no nulos
        })
        .flatMap(([key, value]) => {
            if (Array.isArray(value)) {
                return value; // Mantener arrays desestructurados
            }
            // Manejar fechas específicas con "Desde:" y "Hasta:"
            if (key === "fechaDesde" && typeof value === "string" && value.split("-").length == 3) {
                return `Desde: ${formatFecha(value)}`;
            }
            if (key === "fechaHasta" && typeof value === "string" && value.split("-").length == 3) {
                return `Hasta: ${formatFecha(value)}`;
            }
            return value; // Mantener valores simples
        });

    return (
        <div className="d-flex gap-2">
            {isLoading ? (
                <span className="px-2 d-flex align-items-center fw-semibold text-primary-emphasis bg-primary-subtle border border-primary-subtle rounded-2">
                    <div className="spinner-border spinner-border-sm text-primary-emphasis" role="status"></div>
                </span>
            ) : (
                nonZeroValues.map((value, index) => (
                    <span
                        key={index}
                        className="px-2 d-flex align-items-center fw-semibold text-primary-emphasis bg-primary-subtle border border-primary-subtle rounded-2">
                        {value}
                    </span>
                ))
            )}
        </div>
    );
};
