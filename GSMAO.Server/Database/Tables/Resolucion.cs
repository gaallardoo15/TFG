using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace GSMAO.Server.Database.Tables
{
    /// <summary>
    /// Representa la entidad resolución de la base de datos.
    /// </summary>
    [PrimaryKey(nameof(Id))]
    [Index(nameof(DescripcionES), IsUnique = true)]
    public class Resolucion
    {
        /// <summary>
        /// Identificador de la resolución
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
        /// Nombre o descripción en español de la resolución.
        /// </summary>
        public required string DescripcionES { get; set; }

        /// <summary>
        /// Nombre o descripción en inglés de la resolución.
        /// </summary>
        public string? DescripcionEN { get; set; }
    }
}
