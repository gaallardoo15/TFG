using GSMAO.Server.Database.Tables;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GSMAO.Server.DTOs
{
    /// <summary>
    /// Objeto de transferencia de datos utilizado para representar los datos básicos de una <see cref="Incidencia"/>.
    /// </summary>
    public class BaseIncidenciaDTO
    {
        /// <summary>
        /// Nombre o descripción en español de la incidencia
        /// </summary>
        [Required(ErrorMessage = " El campo 'DescripcionES' es obligatorio.")]
        public required string DescripcionES { get; set; }

        /// <summary>
        /// Nombre o descripción en inglés de la incidencia
        /// </summary>
        public string? DescripcionEN { get; set; }

    }

    /// <summary>
    /// Objeto de transferencia de datos utilizado para representar los datos básicos para crear una <see cref="Incidencia"/>.
    /// </summary>
    public class CreateIncidenciaDTO : BaseIncidenciaDTO
    {
        /// <summary>
        /// Identificador del mecanismo de fallo asociado
        /// </summary>
        public required int IdMecanismoFallo { get; set; }

    }

    /// <summary>
    /// Objeto de transferencia de datos utilizado para representar los datos básicos para actualizar una <see cref="Incidencia"/>.
    /// </summary>
    public class UpdateIncidenciaDTO : BaseIncidenciaDTO
    {
        /// <summary>
        /// Identificador de la incidencia
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Identificador del mecanismo de fallo asociado
        /// </summary>
        public required int IdMecanismoFallo { get; set; }
    }

    /// <summary>
    /// Objeto de transferencia de datos utilizado para representar los datos básicos de una <see cref="Incidencia"/>.
    /// </summary>
    public class IncidenciaDTO : BaseIncidenciaDTO
    {
        /// <summary>
        /// Identificador de la incidencia
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Mecanismo de fallo asociado a la incidencia. Relación de clave externa con la tabla <see cref="MecanismoDeFalloDTO"/>
        /// </summary>
        [Required(ErrorMessage = " El campo 'MecanismoDeFallo' es obligatorio.")]
        public required MecanismoDeFalloDTO MecanismoDeFallo { get; set; }
    }

    /// <summary>
    /// Objeto de transferencia de datos utilizado para representar una incidencia asociada a una orden.
    /// </summary>
    public class IncidenciaOrdenDTO
    {
        /// <summary>
        /// Identificador del registro de la incidencia de la orden
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Indicador si el activo se ha parado o no
        /// </summary>
        public bool ParoMaquina { get; set; }

        /// <summary>
        /// Indicador si se realiza o debe realizar un cambio de pieza o no
        /// </summary>
        public bool CambioPieza { get; set; }

        /// <summary>
        /// Indicador si la incidencia afecta a la producción o no
        /// </summary>
        public bool AfectaProduccion { get; set; }

        /// <summary>
        /// Fecha de detección de la incidencia de la orden
        /// </summary>
        public required DateTime FechaDeteccion { get; set; }

        /// <summary>
        /// Incidencia de la orden
        /// </summary>
        public required IncidenciaDTO Incidencia { get; set; }

        /// <summary>
        /// Fecha de resolución de la incidencia de la orden
        /// </summary>
        public DateTime? FechaResolucion { get; set; }

        /// <summary>
        /// Resolución de la incidencia de la orden
        /// </summary>
        public ResolucionDTO? Resolucion { get; set; }

        /// <summary>
        /// Componente de la incidencia de la orden
        /// </summary>
        public required ComponenteTableDTO Componente { get; set; }
    }

    /// <summary>
    /// Objeto de transferencia de datos utilizado para crear una nueva incidencia dentro de una orden.
    /// </summary>
    public class CreateIncidenciaOrdenDTO
    {
        /// <summary>
        /// Identificador de la orden
        /// </summary>
        public int IdOrden { get; set; }

        /// <summary>
        /// Indicador si el activo se ha parado o no
        /// </summary>
        public bool ParoMaquina { get; set; }

        /// <summary>
        /// Indicador si se realiza o debe realizar un cambio de pieza o no
        /// </summary>
        public bool CambioPieza { get; set; }

        /// <summary>
        /// Indicador si la incidencia afecta a la producción o no
        /// </summary>
        public bool AfectaProduccion { get; set; }

        /// <summary>
        /// Fecha de detección de la incidencia de la orden
        /// </summary>
        public required string FechaDeteccion { get; set; }

        /// <summary>
        /// Incidencia de la orden
        /// </summary>
        public required List<int> Incidencias { get; set; }

        /// <summary>
        /// Componente de la incidencia de la orden
        /// </summary>
        public required int IdComponente { get; set; }
    }

    /// <summary>
    /// Objeto de transferencia de datos utilizado para actualizar una incidencia existente dentro de una orden.
    /// </summary>
    public class UpdateIncidenciaOrdenDTO
    {
        /// <summary>
        /// Identificador de la incidencia de la orden
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Identificador de la orden
        /// </summary>
        public int IdOrden { get; set; }

        /// <summary>
        /// Indicador si el activo se ha parado o no
        /// </summary>
        public bool ParoMaquina { get; set; }

        /// <summary>
        /// Indicador si se realiza o debe realizar un cambio de pieza o no
        /// </summary>
        public bool CambioPieza { get; set; }

        /// <summary>
        /// Indicador si la incidencia afecta a la producción o no
        /// </summary>
        public bool AfectaProduccion { get; set; }

        /// <summary>
        /// Fecha de detección de la incidencia de la orden
        /// </summary>
        public required string FechaDeteccion { get; set; }

        /// <summary>
        /// Incidencia de la orden
        /// </summary>
        public required int IdIncidencia { get; set; }

        /// <summary>
        /// Componente de la incidencia de la orden
        /// </summary>
        public required int IdComponente { get; set; }
    }

    /// <summary>
    /// Objeto de transferencia de datos utilizado para actualizar la resolución de una incidencia dentro de una orden.
    /// </summary>
    public class UpdateResolucionIncidenciaOrdenDTO
    {
        /// <summary>
        /// Identificador de la incidencia de la orden
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Identificador de la orden
        /// </summary>
        public int IdOrden { get; set; }

        /// <summary>
        /// Resolución de la incidencia de la orden
        /// </summary>
        public int? IdResolucion { get; set; }

        /// <summary>
        /// Fecha de detección de la incidencia de la orden
        /// </summary>
        public required string FechaDeteccion { get; set; }

        /// <summary>
        /// Fecha de resolución de la incidencia de la orden
        /// </summary>
        public string? FechaResolucion{ get; set; }
    }
}
