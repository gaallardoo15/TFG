using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace GSMAO.Server.Database.Tables
{
    /// <summary>
    /// Representa la entidad estado de usuario de la base de datos.
    /// </summary>
    [PrimaryKey(nameof(Id))]
    [Index(nameof(Name), IsUnique = true)]
    public class EstadoUsuario
    {
        /// <summary>
        /// Identificador del estado de usuario.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nombre o descripción del estado de usuario.
        /// </summary>
        public required string Name { get; set; }
    }
}
