using GSMAO.Server.Database.Tables;
using System.ComponentModel.DataAnnotations;

namespace GSMAO.Server.DTOs
{
    /// <summary>
    /// Objeto de transferencia de datos utilizado para representar los datos básicos necesarios de un <see cref="Activo"/>.
    /// </summary>
    public abstract class BaseActivoDTO
    {
        /// <summary>
        /// Identificador del activo
        /// </summary>
        [Required(ErrorMessage = "El campo 'ID Activo' es obligatorio.")]
        public required int Id { get; set; }

        /// <summary>
        /// Identificador SAP del activo
        /// </summary>
        [Required(ErrorMessage = "El campo 'Activo SAP' es obligatorio.")]
        public required string ActivoSAP { get; set; }

        /// <summary>
        /// Nombre o descripción en español del activo
        /// </summary>
        [Required(ErrorMessage = "El campo 'Descripcion ES' es obligatorio.")]
        public required string DescripcionES { get; set; }

        /// <summary>
        /// Nombre o descripción en inglés del activo
        /// </summary>
        public string? DescripcionEN { get; set; }

        /// <summary>
        /// Redundancia del activo
        /// </summary>
        public int? Redundancia { get; set; }

        /// <summary>
        /// HSE del activo
        /// </summary>
        public int? Hse { get; set; }

        /// <summary>
        /// Usabilidad del activo
        /// </summary>
        public int? Usabilidad { get; set; }

        /// <summary>
        /// Coste del activo
        /// </summary>
        public int? Coste { get; set; }

        /// <summary>
        /// Valor de criticidad del activo
        /// </summary>
        [Required(ErrorMessage = "El campo 'Valor Criticidad' es obligatorio.")]
        public required int ValorCriticidad { get; set; }
    }

    /// <summary>
    /// Objeto de transferencia de datos utilizado para representar los datos necesario para crear un <see cref="Activo"/>.
    /// </summary>
    public class CreateActivoDTO : BaseActivoDTO
    {
        /// <summary>
        /// Identificador de la criticidad del activo
        /// </summary>
        public required int IdCriticidad { get; set; }

        /// <summary>
        /// Identificador de la localización del activo
        /// </summary>
        public required int IdLocalizacion { get; set; }

        /// <summary>
        /// Identificador del centro de coste del activo
        /// </summary>
        public required int IdCentroCoste { get; set; }
    }

    /// <summary>
    /// Objeto de transferencia de datos utilizado para representar los datos necesario para actualizar un <see cref="Activo"/>.
    /// </summary>
    public class UpdateActivoDTO : BaseActivoDTO
    {
        /// <summary>
        /// Identificador de la criticidad del activo
        /// </summary>
        public required int IdCriticidad { get; set; }

        /// <summary>
        /// Identificador de la localización del activo
        /// </summary>
        public required int IdLocalizacion { get; set; }

        /// <summary>
        /// Identificador del centro de coste del activo
        /// </summary>
        public required int IdCentroCoste { get; set; }
    }

    /// <summary>
    /// Objeto de transferencia de datos utilizado para representar los datos necesarios de un <see cref="Activo"/> para una tabla.
    /// </summary>
    public class ActivoTableDTO : BaseActivoDTO
    {

        /// <summary>
        /// Información de la criticidad del activo
        /// </summary>
        public required CriticidadDTO Criticidad { get; set; }

        /// <summary>
        /// Información de la localización del activo
        /// </summary>
        public required LocalizacionTableDTO Localizacion { get; set; }

        /// <summary>
        /// Información del centro de coste del activo
        /// </summary>
        public required CentroCosteTableDTO CentroCoste { get; set; }

        /// <summary>
        /// Información del estado del activo
        /// </summary>
        public required EstadoActivoDTO EstadoActivo { get; set; }
    }

    /// <summary>
    /// Objeto de transferencia de datos utilizado para representar los datos necesarios de un <see cref="Activo"/>.
    /// </summary>
    public class ActivoDTO : BaseActivoDTO
    {
        /// <summary>
        /// Información de la criticidad del activo
        /// </summary>
        public required CriticidadDTO Criticidad { get; set; }

        /// <summary>
        /// Información de la localización del activo
        /// </summary>
        public required LocalizacionDTO Localizacion { get; set; }

        /// <summary>
        /// Información del centro de coste del activo
        /// </summary>
        public required CentroCosteDTO CentroCoste { get; set; }

        /// <summary>
        /// Información del estado del activo
        /// </summary>
        public required EstadoActivoDTO EstadoActivo { get; set; }
    }

    /// <summary>
    /// Objeto de transferencia de datos utilizado para representar los datos mínimos necesarios de un <see cref="Activo"/>.
    /// </summary>
    public class ActivoMinDTO
    {
        /// <summary>
        /// Identificador del activo
        /// </summary>
        public required int Id { get; set; }

        /// <summary>
        /// Nombre o descripción en español del activo
        /// </summary>
        public required string DescripcionES { get; set; }
    }

    /// <summary>
    /// Objeto de transferencia de datos utilizado para representar los datos necesarios de un <see cref="Activo"/> para una orden.
    /// </summary>
    public class ActivoOrdenDTO
    {
        /// <summary>
        /// Identificador del activo
        /// </summary>
        public required int Id { get; set; }

        /// <summary>
        /// Nombre o descripción en español del activo
        /// </summary>
        public required string DescripcionES { get; set; }

        /// <summary>
        /// Información del centro de coste del activo
        /// </summary>
        public required CentroCosteDTO CentroCoste { get; set; }
    }
}
