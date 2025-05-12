using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace GSMAO.Server.Database.Tables
{
    /// <summary>
    /// Representa la entidad mecanismo de fallo de la base de datos.
    /// </summary>
    [PrimaryKey(nameof(Id))]
    [Index(nameof(DescripcionES), IsUnique = true)]
    public class MecanismoDeFallo
    {
        /// <summary>
        /// Identificador del mecanismo de fallo
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
        /// Nombre o descripción en español del mecanismo de fallo
        /// </summary>
        public required string DescripcionES { get; set; }

        /// <summary>
        /// Nombre o descripción en inglés del mecanismo de fallo
        /// </summary>
        public string? DescripcionEN { get; set; }
    }
}
