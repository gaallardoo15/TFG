using Microsoft.EntityFrameworkCore;

namespace GSMAO.Server.Database.Tables
{
    /// <summary>
    /// Representa la entidad tipo de la orden de la base de datos.
    /// </summary>
    [PrimaryKey(nameof(Id))]
    [Index(nameof(Name), IsUnique = true)]
    public class TipoOrden
    {
        /// <summary>
        /// Identificador del tipo de la orden.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nombre o descripción del tipo de la orden.
        /// </summary>
        public required string Name { get; set; }
    }
}
