using GSMAO.Server.Database.Tables;

namespace GSMAO.Server.DTOs
{
    /// <summary>
    /// Objeto de transferencia de datos utilizado para representar los datos necesarios de un <see cref="Rol"/>.
    /// </summary>
    public class RolDTO
    {
        /// <summary>
        /// El identificador del rol
        /// </summary>
        public required string Id { get; set; }

        /// <summary>
        /// El nombre o descripción del rol
        /// </summary>
        public required string Name { get; set; }
    }
}
