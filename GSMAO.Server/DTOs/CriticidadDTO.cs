using GSMAO.Server.Database.Tables;

namespace GSMAO.Server.DTOs
{
    /// <summary>
    /// Objeto de transferencia de datos utilizado para representar los datos necesarios de una <see cref="Criticidad"/>.
    /// </summary>
    public class CriticidadDTO
    {
        /// <summary>
        /// El identificador del estado del activo
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Siglas de la descripción de la criticidad
        /// </summary>
        public required string Siglas { get; set; }

        /// <summary>
        /// El nombre o descripción del estado del activo
        /// </summary>
        public required string Descripcion { get; set; }
    }
}
