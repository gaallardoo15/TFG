namespace GSMAO.Server.DTOs
{
    /// <summary>
    /// Objeto de transferencia de datos utilizado para representar los filtros generales aplicables a un panel.
    /// Estos filtros permiten seleccionar distintos rangos de fechas, tipos de ordenes, criticidades y años para personalizar la visualización de los datos.
    /// </summary>
    public class FiltrosPanelGeneralDTO
    {
        /// <summary>
        /// Lista de años a filtrar.
        /// Permite seleccionar uno o varios años para filtrar los datos según el año de la orden o actividad.
        /// </summary>
        public List<int>? Years { get; set; }

        /// <summary>
        /// Lista de tipos de órdenes a filtrar.
        /// Permite seleccionar uno o varios tipos de orden para visualizar solo aquellos que corresponden con los tipos especificados.
        /// </summary>
        public List<int>? TiposOrdenes { get; set; }

        /// <summary>
        /// Lista de criticidades a filtrar.
        /// Permite seleccionar uno o varios niveles de criticidad para filtrar los datos según el grado de criticidad de las órdenes o activos.
        /// </summary>
        public List<int>? Criticidades { get; set; }

        /// <summary>
        /// Fecha de inicio del rango de fechas a filtrar.
        /// Permite seleccionar una fecha desde la cual mostrar los datos. Si no se proporciona, se omite el filtro por fecha desde.
        /// </summary>
        public DateTime? FechaDesde { get; set; }

        /// <summary>
        /// Fecha final del rango de fechas a filtrar.
        /// Permite seleccionar una fecha hasta la cual mostrar los datos. Si no se proporciona, se omite el filtro por fecha hasta.
        /// </summary>
        public DateTime? FechaHasta { get; set; }
    }

}
