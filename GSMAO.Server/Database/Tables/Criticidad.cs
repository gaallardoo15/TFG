using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;

namespace GSMAO.Server.Database.Tables
{
    /// <summary>
    /// Representa la entidad criticidad de la base de datos.
    /// </summary>
    [PrimaryKey(nameof(Id))]
    [Index(nameof(Descripcion), IsUnique = true)]
    [Index(nameof(Siglas), IsUnique = true)]
    public class Criticidad
    {
        /// <summary>
        /// Identificador de la criticidad
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Siglas de la descripción de la criticidad
        /// </summary>
        public required string Siglas { get; set; }

        /// <summary>
        /// Nombre o descripción de la criticidad
        /// </summary>
        public required string Descripcion { get; set; }
    }
}
