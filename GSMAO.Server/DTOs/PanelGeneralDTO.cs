namespace GSMAO.Server.DTOs
{
    /// <summary>
    /// Objeto de transferencia de datos utilizado para representar el panel general con varias métricas,
    /// como la tabla de órdenes, gráficas y semáforos que indican el estado de las órdenes y tipos de órdenes.
    /// </summary>
    public class PanelGeneralDTO
    {
        /// <summary>
        /// Lista de objetos <see cref="OrdenTableDTO"/> que representan la tabla de órdenes.
        /// Contiene las órdenes que se deben visualizar en la tabla del panel.
        /// </summary>
        public List<OrdenTableDTO>? Table { get; set; }

        /// <summary>
        /// Lista de objetos <see cref="GraficaDTO"/> que contienen los datos para las gráficas del panel general.
        /// Esta lista muestra estadísticas relacionadas con las órdenes, como el número de órdenes en diferentes estados.
        /// </summary>
        public List<GraficaDTO>? Grafica { get; set; }

        /// <summary>
        /// Lista de objetos <see cref="SemaforoDTO"/> que representan los semáforos para los tipos de órdenes.
        /// Cada semáforo indica el estado o cantidad de órdenes para un tipo específico de orden.
        /// </summary>
        public List<SemaforoDTO>? SemaforoTipos { get; set; }

        /// <summary>
        /// Lista de objetos <see cref="SemaforoDTO"/> que representan los semáforos para los estados de las órdenes.
        /// Cada semáforo indica el estado de las órdenes, como "abiertas", "cerradas", "en curso", etc.
        /// </summary>
        public List<SemaforoDTO>? SemaforoEstados { get; set; }
    }

    /// <summary>
    /// Objeto de transferencia de datos utilizado para representar la información necesaria para las gráficas
    /// del panel general, incluyendo el número de órdenes en diferentes estados.
    /// </summary>
    public class GraficaDTO
    {
        /// <summary>
        /// Fecha de la gráfica, utilizada para representar los datos de las órdenes en una fecha específica.
        /// </summary>
        public required DateTime Fecha { get; set; }

        /// <summary>
        /// Número total de órdenes en la fecha representada.
        /// </summary>
        public int NumeroOrdenesTotales { get; set; }

        /// <summary>
        /// Número de órdenes que están en curso en la fecha representada.
        /// </summary>
        public int NOrdenesEnCurso { get; set; }

        /// <summary>
        /// Número de órdenes que están abiertas en la fecha representada.
        /// </summary>
        public int NOrdenesAbiertas { get; set; }

        /// <summary>
        /// Número de órdenes que están a la espera de material en la fecha representada.
        /// </summary>
        public int NOrdenesMaterial { get; set; }

        /// <summary>
        /// Número de órdenes que han sido cerradas en la fecha representada.
        /// </summary>
        public int NOrdenesCerradas { get; set; }

        /// <summary>
        /// Número de órdenes que han sido anuladas en la fecha representada.
        /// </summary>
        public int NOrdenesAnuladas { get; set; }
    }

    /// <summary>
    /// Objeto de transferencia de datos utilizado para representar el estado de los semáforos,
    /// tanto para los tipos de órdenes como para los estados de las órdenes.
    /// </summary>
    public class SemaforoDTO
    {
        /// <summary>
        /// Descripción de la opción representada por el semáforo, como el tipo de orden o el estado de la orden.
        /// </summary>
        public required string DescripcionOpcion { get; set; }

        /// <summary>
        /// Número de órdenes que pertenecen a la opción representada por el semáforo.
        /// </summary>
        public int NOrdenesOpcion { get; set; }

        /// <summary>
        /// Porcentaje de órdenes que pertenecen a la opción representada por el semáforo,
        /// en relación con el total de órdenes.
        /// </summary>
        public float PorcentajeOpcion { get; set; }
    }

}
