using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace GSMAO.Server.Database.Tables
{
    /// <summary>
    /// Representa la entidad repuesto de la base de datos.
    /// </summary>
    [PrimaryKey(nameof(Id))]
    public class Repuesto
    {
        /// <summary>
        /// Identificador del repuesto
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Fecha de creación de la orden. Automática
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime FechaCreacion { get; set; }

        /// <summary>
        /// Fecha de última modificación de la orden. Automática
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime UltimaModificacion { get; set; }

        /// <summary>
        /// Nombre o descripción en español del repuesto
        /// </summary>
        public required string DescripcionES { get; set; }

        /// <summary>
        /// Nombre o descripción en inglés del repuesto
        /// </summary>
        public string? DescripcionEN { get; set; }

        /// <summary>
        /// Referencia del albarán del repuesto
        /// </summary>
        public string? Ref_Albaran { get; set; }

        /// <summary>
        /// Precio del repuesto
        /// </summary>
        public float Precio { get; set; }

        /// <summary>
        /// Identificador del estado del repuesto
        /// </summary>
        public int IdEstadoRepuesto { get; set; }

        /// <summary>
        /// Identificador del almacén del repuesto
        /// </summary>
        public int IdAlmacen { get; set; }

        /// <summary>
        /// Estado del repuesto. Relación de clave externa con la tabla <see cref="EstadoRepuesto"/>
        /// </summary>
        [ForeignKey("IdEstadoRepuesto")]
        public required virtual EstadoRepuesto EstadoRepuesto { get; set; }

        /// <summary>
        /// Almacen al que pertenece el repuesto. Relación de clave externa con la tabla <see cref="Almacen"/>
        /// </summary>
        [ForeignKey("IdAlmacen")]
        public virtual Almacen? Almacen { get; set; }
    }
}
