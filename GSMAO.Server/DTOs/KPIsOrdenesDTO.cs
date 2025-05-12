namespace GSMAO.Server.DTOs
{
    public class IndicadoresDTO
    {
        /// <summary>
        /// Total de órdenes en el sistema. Representa el número global de órdenes, sin clasificar.
        /// </summary>
        public int TotalOrdenes { get; set; }

        /// <summary>
        /// Porcentaje de órdenes completadas. Calculado como la proporción de órdenes completadas respecto al total.
        /// </summary>
        public double PorcentajeCompletadas { get; set; }

        /// <summary>
        /// Porcentaje de órdenes pendientes. Calculado como la proporción de órdenes pendientes respecto al total.
        /// </summary>
        public double PorcentajePendientes { get; set; }

        /// <summary>
        /// Porcentaje de órdenes que requieren material. Calculado como la proporción de órdenes que están a la espera de material respecto al total.
        /// </summary>
        public double PorcentajeMaterial { get; set; }
    }

    public class IndicadoresOTPorActivoDTO {
        public int IdActivo { get; set; }
        public required IndicadoresDTO Indicadores { get; set; }
    }

    /// <summary>
    /// Objeto de transferencia de datos utilizado para representar los KPIs (Indicadores Clave de Rendimiento)
    /// relacionados con las órdenes de mantenimiento, incluyendo el total de órdenes, su porcentaje de estado y
    /// la categorización por tipo de orden.
    /// </summary>
    public class KPIsOrdenesDTO
    {
        public int Year { get; set; }
        
        /// <summary>
        /// Total de órdenes en el sistema. Representa el número global de órdenes, sin clasificar.
        /// </summary>
        public int TotalOrdenes { get; set; }

        /// <summary>
        /// Porcentaje de órdenes completadas. Calculado como la proporción de órdenes completadas respecto al total.
        /// </summary>
        public double PorcentajeCompletadas { get; set; }

        /// <summary>
        /// Porcentaje de órdenes pendientes. Calculado como la proporción de órdenes pendientes respecto al total.
        /// </summary>
        public double PorcentajePendientes { get; set; }

        /// <summary>
        /// Porcentaje de órdenes que requieren material. Calculado como la proporción de órdenes que están a la espera de material respecto al total.
        /// </summary>
        public double PorcentajeMaterial { get; set; }

        /// <summary>
        /// Lista de objetos <see cref="OrdenesPeriodoDTO"/> que representan las órdenes preventivas durante el periodo seleccionado.
        /// </summary>
        public required List<OrdenesPeriodoDTO> Preventivas { get; set; }

        /// <summary>
        /// Lista de objetos <see cref="OrdenesPeriodoDTO"/> que representan las órdenes correctivas durante el periodo seleccionado.
        /// </summary>
        public required List<OrdenesPeriodoDTO> Correctivas { get; set; }

        /// <summary>
        /// Lista de objetos <see cref="OrdenesPeriodoDTO"/> que representan las órdenes de mejoras durante el periodo seleccionado.
        /// </summary>
        public required List<OrdenesPeriodoDTO> Mejoras { get; set; }

        /// <summary>
        /// Lista de objetos <see cref="OrdenesPeriodoDTO"/> que representan las órdenes relacionadas con fallas humanas durante el periodo seleccionado.
        /// </summary>
        public required List<OrdenesPeriodoDTO> FallaHumana { get; set; }

        /// <summary>
        /// Lista de objetos <see cref="MantenimientoGeneralDTO"/> que contiene los datos generales del mantenimiento realizado.
        /// </summary>
        public required List<MantenimientoGeneralDTO> MantenimientoGeneral { get; set; }

        public required List<IndicadoresOTPorActivoDTO> DesglosePorActivos { get; set; }
    }


    /// <summary>
    /// Objeto de transferencia de datos utilizado para representar los datos auxiliares de una orden
    /// dentro de los KPIs relacionados con las órdenes de mantenimiento.
    /// </summary>
    public class KPIsOrdenesAuxDTO
    {
        /// <summary>
        /// Identificador único de la orden de mantenimiento.
        /// </summary>
        public int IdOrden { get; set; }

        /// <summary>
        /// Identificador del activo asociado con la orden de mantenimiento.
        /// </summary>
        public int IdActivo { get; set; }

        /// <summary>
        /// Identificador del tipo de orden de mantenimiento (por ejemplo, preventiva, correctiva, etc.).
        /// </summary>
        public int IdTipoOrden { get; set; }

        /// <summary>
        /// Identificador del estado de la orden de mantenimiento (por ejemplo, pendiente, en curso, cerrada, etc.).
        /// </summary>
        public int IdEstado { get; set; }

        /// <summary>
        /// Fecha de la orden de mantenimiento. Indica el momento en el que la orden fue creada o registrada.
        /// </summary>
        public DateTime FechaApertura { get; set; }

        /// <summary>
        /// Periodo relacionado con la orden de mantenimiento. Este valor podría referirse al periodo de tiempo
        /// en el que se clasifica la orden, como un trimestre, un mes o un año.
        /// </summary>
        public int Periodo { get; set; }
    }

}
