using GSMAO.Server.Database.Tables;

namespace GSMAO.Server.DTOs
{
    /// <summary>
    /// Objeto de transferencia de datos utilizado para representar los datos necesarios de una <see cref="EstadoUsuario"/>.
    /// </summary>
    public class EstadoUsuarioDTO
    {
        /// <summary>
        /// El identificador del estado del usuario
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// El nombre o descripción del estado del usuario
        /// </summary>
        public required string Name { get; set; }
    }
}
