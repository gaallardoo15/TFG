using GSMAO.Server.Database.Tables;
using System.ComponentModel.DataAnnotations;

namespace GSMAO.Server.DTOs
{
    /// <summary>
    /// Objeto de transferencia de datos utilizado para representar los datos necesarios de un <see cref="CentroCoste"/>.
    /// </summary> 
    public class BaseCentroCosteDTO
    {
        /// <summary>
        /// Nombre o descripción en español del centro de coste
        /// </summary>
        [Required(ErrorMessage = "El campo 'DescripciónES' es obligatorio")]
        public required string DescripcionES { get; set; }

        /// <summary>
        /// Nombre o descripción en inglés del centro de coste
        /// </summary>
        public string? DescripcionEN { get; set; }

        /// <summary>
        /// Nombre o descripción del centro de coste en el sistema SAP
        /// </summary>
        [Required(ErrorMessage = " El campo 'CentroCosteSAP' es obligatorio")]
        public required string CentroCosteSAP { get; set; }

    }

    /// <summary>
    /// Objeto de transferencia de datos utilizado para representar los datos necesarios para crear un <see cref="CentroCoste"/>.
    /// </summary>
    public class CreateCentroCosteDTO: BaseCentroCosteDTO
    {
        /// <summary>
        /// Identificador de la planta asociada al centro de coste
        /// </summary>
        [Required(ErrorMessage = "El campo 'IdPlanta' es obligatorio")]
        public required int IdPlanta {  get; set; }

    }

    /// <summary>
    /// Objeto de transferencia de datos utilizado para representar los datos necesarios para actualizar un <see cref="CentroCoste"/>.
    /// </summary>
    public class UpdateCentroCosteDTO : BaseCentroCosteDTO
    {
        /// <summary>
        /// Identificador del centro de coste
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Identificador de la planta asociada al centro de coste
        /// </summary>
        public required int IdPlanta { get; set; }
    }

    /// <summary>
    /// Objeto de transferencia de datos utilizado para representar los datos necesarios de un <see cref="CentroCoste"/>
    /// </summary>
    public class CentroCosteDTO : BaseCentroCosteDTO
    {
        /// <summary>
        /// Identificador del centro de coste
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Planta a la que pertenece el centro de coste. Relación de clave externa con la entidad <see cref="PlantaDTO"/>
        /// </summary>
        public required PlantaDTO Planta { get; set; }
    }

    /// <summary>
    /// Objeto de transferencia de datos utilizado para representar los datos necesarios de un <see cref="CentroCoste"/> para una tabla
    /// </summary>
    public class CentroCosteTableDTO
    {
        /// <summary>
        /// Identificador del centro de coste
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nombre o descripción en español del centro de coste
        /// </summary>
        [Required(ErrorMessage = " El campo 'DescripcionES' es obligatorio.")]
        public required string DescripcionES { get; set; }
    }
}
