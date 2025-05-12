using Microsoft.AspNetCore.Routing.Constraints;

namespace GSMAO.Server.DTOs
{
    /// <summary>
    /// Objeto de transferencia de datos utilizado para representar el resumen de órdenes de mantenimiento
    /// agrupadas por periodo (por ejemplo, por trimestre o año) dentro de un KPI.
    /// </summary>
    public class OrdenesPeriodoDTO
    {
        /// <summary>
        /// Periodo de tiempo (puede ser un año, mes, trimestre, etc.) que se utiliza para clasificar las órdenes de mantenimiento.
        /// </summary>
        public int Periodo { get; set; }

        /// <summary>
        /// Nombre del periodo (por ejemplo, "2023 Q1", "Enero 2024", etc.) para representar el periodo de tiempo de manera legible.
        /// </summary>
        public required string NombrePeriodo { get; set; }

        /// <summary>
        /// Número total de órdenes de mantenimiento dentro del periodo.
        /// </summary>
        public int NumOrdenes { get; set; }

        /// <summary>
        /// Porcentaje de órdenes dentro del periodo con respecto al total de órdenes en el sistema o en el periodo de análisis.
        /// </summary>
        public float PorcentajeOrdenes { get; set; }

        /// <summary>
        /// Lista de órdenes clasificadas por activo dentro del periodo. Cada entrada en la lista representa una orden
        /// de mantenimiento asociada a un activo específico durante el periodo.
        /// </summary>
        public required List<OrdenesActivoDTO> OrdenesPorActivo { get; set; }
    }

    /// <summary>
    /// Objeto de transferencia de datos utilizado para representar el resumen de órdenes de mantenimiento
    /// agrupadas por periodo, con información adicional como el tipo de orden.
    /// </summary>
    public class OrdenesPeriodoAuxDTO
    {
        /// <summary>
        /// Identificador del tipo de orden (por ejemplo, preventiva, correctiva, etc.).
        /// </summary>
        public int IdTipoOrden { get; set; }

        /// <summary>
        /// Periodo de tiempo (puede ser un año, mes, trimestre, etc.) que se utiliza para clasificar las órdenes de mantenimiento.
        /// </summary>
        public int Periodo { get; set; }

        /// <summary>
        /// Nombre del periodo (por ejemplo, "2023 Q1", "Enero 2024", etc.) para representar el periodo de tiempo de manera legible.
        /// </summary>
        public required string NombrePeriodo { get; set; }

        /// <summary>
        /// Número total de órdenes de mantenimiento dentro del periodo para el tipo de orden especificado.
        /// </summary>
        public int NumOrdenes { get; set; }

        /// <summary>
        /// Porcentaje de órdenes dentro del periodo con respecto al total de órdenes del tipo de orden.
        /// </summary>
        public float PorcentajeOrdenes { get; set; }

        /// <summary>
        /// Lista de órdenes clasificadas por activo dentro del periodo. Cada entrada en la lista representa una orden
        /// de mantenimiento asociada a un activo específico durante el periodo para el tipo de orden indicado.
        /// </summary>
        public required List<OrdenesActivoDTO> OrdenesPorActivo { get; set; }
    }

}
