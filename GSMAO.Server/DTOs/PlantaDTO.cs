using System.ComponentModel.DataAnnotations;

using GSMAO.Server.Database.Tables;

namespace GSMAO.Server.DTOs
{
    /// <summary>
    /// Objeto de transferencia de datos utilizado para representar los datos básicos de una <see cref="Planta"/>.
    /// </summary>
    public class BasePlantaDTO
    {
        /// <summary>
        /// Descripción de la empresa
        /// </summary>
        [Required(ErrorMessage = " El campo 'Descripcion' es obligatorio.")]
        public required string Descripcion { get; set; }
        
        /// <summary>
        /// Configuración SMTP de la planta
        /// </summary>
        public string? StmpConfig { get; set; }


        /// <summary>
        /// Latitud de la planta
        /// </summary>
        public string? Latitud { get; set; }

        /// <summary>
        /// Longitud de la planta
        /// </summary>
        public string? Longitud { get; set; }

    }

    /// <summary>
    /// Objeto de transferencia de datos utilizado para representar los datos básicos de la <see cref="Planta"/> de un <see cref="Usuario"/>.
    /// </summary>
    public class UserPlantaDTO : BasePlantaDTO
    {
        /// <summary>
        /// Identificador de la planta
        /// </summary>
        public int Id { get; set; }
    }

    /// <summary>
    /// Objeto de transferencia de datos utilizado para representar los datos básicos para crear una <see cref="Planta"/>.
    /// </summary>
    public class CreatePlantaDTO : BasePlantaDTO
    {

        /// <summary>
        /// Identificador de la empresa asociada a la planta
        /// </summary>
        public required int idEmpresa { get; set; }
    }

    /// <summary>
    /// Objeto de transferencia de datos utilizado para representar los datos básicos de una <see cref="Planta"/>.
    /// </summary>
    public class PlantaDTO : BasePlantaDTO
    {
        /// <summary>
        /// Identificador de la planta
        /// </summary>
        public int Id { get; set; }


        /// <summary>
        /// Información de la empresa asociada a la planta
        /// </summary>
        [Required(ErrorMessage = " El campo 'Empresa' es obligatorio.")]
        public required EmpresaDTO Empresa { get; set; }
    }

    /// <summary>
    /// Objeto de transferencia de datos utilizado para representar los datos básicos para actualizar una <see cref="Planta"/>.
    /// </summary>
    public class UpdatePlantaDTO : BasePlantaDTO
    {
        /// <summary>
        /// Identificador de la planta
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Identificador de la empresa asociada a la planta
        /// </summary>
        public required int idEmpresa { get; set; }
    }

}

