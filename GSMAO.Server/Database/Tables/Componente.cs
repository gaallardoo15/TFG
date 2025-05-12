using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace GSMAO.Server.Database.Tables
{
    /// <summary>
    /// Representa la entidad componente de la base de datos.
    /// </summary>
    [PrimaryKey(nameof(Id))]
    [Index(nameof(Denominacion), IsUnique = true)]
    public class Componente
    {
        /// <summary>
        /// Identificador del componente
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
        /// Denominación o KKS del componente
        /// </summary>
        public required string Denominacion { get; set; }

        /// <summary>
        /// Nombre o descripción en español del componente
        /// </summary>
        public required string DescripcionES { get; set; }

        /// <summary>
        /// Nombre o descripción en inglés del componente
        /// </summary>
        public string? DescripcionEN { get; set; }

        /// <summary>
        /// Identificador del componente padre del componente
        /// </summary>
        public int? IdComponentePadre { get; set; }

        /// <summary>
        /// Componente padre del componente. Relación de clave externa con la tabla <see cref="Componente"/>
        /// </summary>
        [ForeignKey("IdComponentePadre")]
        public virtual Componente? ComponentePadre { get; set; }
    }
}
