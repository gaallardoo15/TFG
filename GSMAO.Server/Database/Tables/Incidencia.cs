using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace GSMAO.Server.Database.Tables
{
    /// <summary>
    /// Representa la entidad incidencia de la base de datos.
    /// </summary>
    [PrimaryKey(nameof(Id))]
    [Index(nameof(DescripcionES), nameof(IdMecanismoFallo), IsUnique = true)]
    public class Incidencia
    {
        /// <summary>
        /// Identificador de la incidencia
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
        /// Nombre o descripción en español de la incidencia
        /// </summary>
        public required string DescripcionES { get; set; }

        /// <summary>
        /// Nombre o descripción en inglés de la incidencia
        /// </summary>
        public string? DescripcionEN { get; set; }

        /// <summary>
        /// Identificador del mecanismo de fallo
        /// </summary>
        public required int IdMecanismoFallo { get; set; }

        /// <summary>
        /// Mecanismo de fallo asociado a la incidencia. Relación de clave externa con la tabla <see cref="MecanismoDeFallo"/>
        /// </summary>
        [ForeignKey("IdMecanismoFallo")]
        public required virtual MecanismoDeFallo MecanismoDeFallo { get; set; }
    }
}
