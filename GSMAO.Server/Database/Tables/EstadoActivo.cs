using Microsoft.EntityFrameworkCore;

namespace GSMAO.Server.Database.Tables
{
    /// <summary>
    /// Representa la entidad estado del activo de la base de datos.
    /// </summary>
    [PrimaryKey(nameof(Id))]
    [Index(nameof(Name), IsUnique = true)]
    public class EstadoActivo
    {
        /// <summary>
        /// Identificador del estado del activo.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nombre o descripción del estado del activo.
        /// </summary>
        public required string Name { get; set; }
    }
}
