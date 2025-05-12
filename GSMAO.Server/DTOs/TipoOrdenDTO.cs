using GSMAO.Server.Database.Tables;

namespace GSMAO.Server.DTOs
{
    /// <summary>
    /// Objeto de transferencia de datos utilizado para representar los datos necesarios de un <see cref="TipoOrden"/>.
    /// </summary>
    public class TipoOrdenDTO
    {
        /// <summary>
        /// El identificador del tipo de orden
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// El nombre o descripción del tipo de orden
        /// </summary>
        public required string Name { get; set; }
    }
}
