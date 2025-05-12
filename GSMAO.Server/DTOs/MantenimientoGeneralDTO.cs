namespace GSMAO.Server.DTOs
{
    /// <summary>
    /// Objeto de transferencia de datos utilizado para representar los indicadores de mantenimiento general
    /// durante un periodo específico, incluyendo el porcentaje de incidencias de diferentes tipos.
    /// </summary>
    public class MantenimientoGeneralDTO
    {
        /// <summary>
        /// Identificador del periodo (por ejemplo, año o mes).
        /// </summary>
        public int Periodo { get; set; }

        /// <summary>
        /// Nombre del periodo, como el nombre del mes o año, que describe el periodo de tiempo.
        /// </summary>
        public required string NombrePeriodo { get; set; }

        /// <summary>
        /// Porcentaje de incidencias correctivas en el periodo.
        /// </summary>
        public float PorcentajeCorrectivas { get; set; }

        /// <summary>
        /// Porcentaje de incidencias preventivas en el periodo.
        /// </summary>
        public float PorcentajePreventivas { get; set; }

        /// <summary>
        /// Porcentaje de incidencias relacionadas con mejoras en el periodo.
        /// </summary>
        public float PorcentajeMejoras { get; set; }

        /// <summary>
        /// Porcentaje de incidencias debido a fallas humanas en el periodo.
        /// </summary>
        public float PorcentajeFallaHumana { get; set; }
    }

}
