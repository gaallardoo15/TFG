using System.ComponentModel.DataAnnotations;
using GSMAO.Server.Database.Tables;

namespace GSMAO.Server.DTOs
{
    /// <summary>
    /// Objeto de transferencia de datos utilizado para representar los datos básicos de un <see cref="MecanismoDeFallo"/>.
    /// </summary>
    public class BaseMecanismoDeFalloDTO
    {
        /// <summary>
        /// Nombre o descripción en español del mecanismo de fallo
        /// </summary>
        [Required(ErrorMessage = " El campo 'DescripcionES' es obligatorio.")]
        public required string DescripcionES { get; set; }

        /// <summary>
        /// Nombre o descripción en inglés del mecanismo de fallo
        /// </summary>
        public string? DescripcionEN { get; set; }
    }

    /// <summary>
    /// Objeto de transferencia de datos utilizado para representar los datos necesarios para crear un <see cref="MecanismoDeFallo"/>.
    /// </summary>
    public class CreateMecanismoDeFalloDTO : BaseMecanismoDeFalloDTO { }

    /// <summary>
    /// Objeto de transferencia de datos utilizado para representar los datos necesarios de un <see cref="MecanismoDeFallo"/>, además de para actualizarlo.
    /// </summary>
    public class MecanismoDeFalloDTO : BaseMecanismoDeFalloDTO
    {
        /// <summary>
        /// Identificador del mecanismo de fallo
        /// </summary>
        public required int Id {  get; set; }
    }
}
