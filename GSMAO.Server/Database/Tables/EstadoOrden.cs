using Microsoft.EntityFrameworkCore;

namespace GSMAO.Server.Database.Tables
{
    /// <summary>
    /// Representa la entidad estado de la orden de la base de datos.
    /// </summary>
    [PrimaryKey(nameof(Id))]
    [Index(nameof(Name), IsUnique = true)]
    public class EstadoOrden
    {
        /// <summary>
        /// Identificador del estado de la orden.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nombre o descripción del estado de la orden.
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// Indica el orden para las ordenaciones necesarias.
        /// </summary>
        public required int Orden { get; set; }
    }
}
