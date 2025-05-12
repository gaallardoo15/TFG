using GSMAO.Server.Database.Tables;

namespace GSMAO.Server.DTOs
{
    /// <summary>
    /// Objeto de transferencia de datos utilizado para representar los datos necesarios de un <see cref="EstadoOrden"/>.
    /// </summary>
    public class EstadoOrdenDTO
    {
        /// <summary>
        /// El identificador del estado de la orden
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// El nombre o descripción del estado de la orden
        /// </summary>
        public required string Name { get; set; }

        public required int Orden { get; set; }
    }
}
