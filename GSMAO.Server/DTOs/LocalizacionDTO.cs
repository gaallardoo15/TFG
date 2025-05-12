using System.ComponentModel.DataAnnotations;
using GSMAO.Server.Database.Tables;

namespace GSMAO.Server.DTOs
{
    /// <summary>
    /// Objeto de transferencia de datos utilizado para representar los datos necesarios de una <see cref="Localizacion"/>.
    /// </summary>
    public abstract class BaseLocalizacionDTO
    {
        /// <summary>
        /// Nombre o descripción en español de la localización
        /// </summary>
        [Required(ErrorMessage = " El campo 'DescripcionES' es obligatorio.")]
        public required string DescripcionES { get; set; }

        /// <summary>
        /// Nombre o descripción en inglés de la localización
        /// </summary>
        public required string DescripcionEN { get; set; }

        /// <summary>
        /// Nombre o descripción de la localización en el sistema SAP
        /// </summary>
        public required string LocalizacionSAP { get; set; }        

        /// <summary>
        /// Latitud de la localización
        /// </summary>
        public string? Latitud { get; set; }

        /// <summary>
        /// Longitud de la localización
        /// </summary>
        public string? Longitud { get; set; }

        /// <summary>
        /// Contacto de la localización para la petición de los repuestos
        /// </summary>
        public string? ContactoRepuestos { get; set; }        
    }

    /// <summary>
    /// Objeto de transferencia de datos utilizado para representar los datos básicos para crear una <see cref="Localizacion"/>.
    /// </summary>
    public class CreateLocalizacionDTO : BaseLocalizacionDTO
    {
        /// <summary>
        /// Id de la planta a la que pertenece la localización
        /// </summary>
        public required int IdPlanta { get; set; }
    }

    /// <summary>
    /// Objeto de transferencia de datos utilizado para representar los datos básicos para actualizar una <see cref="Localizacion"/>.
    /// </summary>
    public class UpdateLocalizacionDTO : BaseLocalizacionDTO
    {
        /// <summary>
        /// Identificador de la localización
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Id de la planta a la que pertenece la localización
        /// </summary>
        public required int IdPlanta { get; set; }
    }

    /// <summary>
    /// Objeto de transferencia de datos utilizado para representar los datos básicos de una <see cref="Localizacion"/>
    /// </summary>
    public class LocalizacionDTO : BaseLocalizacionDTO 
    {
        /// <summary>
        /// Identificador de la localización
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Información de la planta a la que pertenece la localización. Relación de clave externa con la entidad <see cref="PlantaDTO"/>
        /// </summary>
        public required PlantaDTO Planta { get; set; }
    }

    /// <summary>
    /// Objeto de transferencia de datos utilizado para representar los datos básicos de una <see cref="Localizacion"/>
    /// </summary>
    public class LocalizacionTableDTO
    {
        /// <summary>
        /// Identificador de la localización
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nombre o descripción en español de la localización
        /// </summary>
        [Required(ErrorMessage = " El campo 'DescripcionES' es obligatorio.")]
        public required string DescripcionES { get; set; }
    }
}
