using Microsoft.EntityFrameworkCore;

namespace GSMAO.Server.Database.Tables
{
    /// <summary>
    /// Representa la entidad estado del repuesto de la base de datos.
    /// </summary>
    [PrimaryKey(nameof(Id))]
    [Index(nameof(Name), IsUnique = true)]
    public class EstadoRepuesto
    {
        /// <summary>
        /// Identificador del estado del repuesto.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nombre o descripción del estado del repuesto.
        /// </summary>
        public required string Name { get; set; }
    }
}
