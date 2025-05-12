using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace GSMAO.Server.Database.Tables
{
    /// <summary>
    /// Representa la entidad incidencia de la orden de la base de datos.
    /// </summary>
    [PrimaryKey(nameof(Id))]
    [Index(nameof(IdOrden))]
    public class IncidenciaOrden
    {
        /// <summary>
        /// Identificador del registro de la incidencia de la orden
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Fecha de inserción de la incidencia de la orden. Automática
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime FechaInsercion { get; set; }

        /// <summary>
        /// Fecha de detección de la incidencia de la orden
        /// </summary>
        public required DateTime FechaDeteccion { get; set; }

        /// <summary>
        /// Fecha de resolución de la incidencia de la orden
        /// </summary>
        public DateTime? FechaResolucion { get; set; }

        /// <summary>
        /// Identificador de la orden
        /// </summary>
        public required int IdOrden { get; set; }

        /// <summary>
        /// Identificador del componente de la incidencia
        /// </summary>
        public required int IdComponente { get; set; }

        /// <summary>
        /// Identificador de la incidencia
        /// </summary>
        public required int IdIncidencia { get; set; }

        /// <summary>
        /// Identificador de la resolución de la incidencia
        /// </summary>
        public int? IdResolucion { get; set; }

        /// <summary>
        /// Tiempo de parada de la incidencia
        /// </summary>
        public double? TiempoParada { get; set; }

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
        /// Orden de la incidencia. Relación de clave externa con la tabla <see cref="Orden"/>
        /// </summary>
        [ForeignKey("IdOrden")]
        public required virtual Orden Orden { get; set; }

        /// <summary>
        /// Componente de la incidencia. Relación de clave externa con la tabla <see cref="Componente"/>
        /// </summary>
        [ForeignKey("IdComponente")]
        public required virtual Componente Componente { get; set; }

        /// <summary>
        /// Incidencia de la orden. Relación de clave externa con la tabla <see cref="Incidencia"/>
        /// </summary>
        [ForeignKey("IdIncidencia")]
        public required virtual Incidencia Incidencia { get; set; }

        /// <summary>
        /// Resolución de la incidencia de la orden. Relación de clave externa con la tabla <see cref="Resolucion"/>
        /// </summary>
        [ForeignKey("IdResolucion")]
        public virtual Resolucion? Resolucion { get; set; }
    }
}
