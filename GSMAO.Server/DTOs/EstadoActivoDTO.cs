using GSMAO.Server.Database.Tables;

namespace GSMAO.Server.DTOs
{
    /// <summary>
    /// Objeto de transferencia de datos utilizado para representar los datos necesarios de una <see cref="EstadoActivo"/>.
    /// </summary>
    public class EstadoActivoDTO
    {
        /// <summary>
         /// El identificador del estado del activo
         /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// El nombre o descripción del estado del activo
        /// </summary>
        public required string Name { get; set; }
    }
}
