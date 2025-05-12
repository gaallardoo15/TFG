using System.ComponentModel.DataAnnotations;
using GSMAO.Server.Database.Tables;

namespace GSMAO.Server.DTOs
{
    /// <summary>
    /// Objeto de transferencia de datos utilizado para representar los datos necesarios de una <see cref="Resolucion"/>.
    /// </summary>
    public class BaseResolucionDTO
    {
        /// <summary>
        /// Nombre o descripción en español de la resolución
        /// </summary>
        [Required(ErrorMessage = " El campo 'DescripcionES' es obligatorio.")]
        public required string DescripcionES { get; set; }

        /// <summary>
        /// Nombre o descripción en inglés de la resolución
        /// </summary>
        public string? DescripcionEN { get; set; }
    }

    /// <summary>
    /// Objeto de transferencia de datos utilizado para representar los datos necesarios para crear una <see cref="Resolucion"/>.
    /// </summary>
    public class CreateResolucionDTO : BaseResolucionDTO { }

    /// <summary>
    /// Objeto de transferencia de datos utilizado para representar los datos necesarios de una <see cref="Resolucion"/>, además de para actualizarla.
    /// </summary>
    public class ResolucionDTO : BaseResolucionDTO
    {
        /// <summary>
        /// Identificador de la resolución
        /// </summary>
        public required int Id { get; set; }
    }
}
