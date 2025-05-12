namespace GSMAO.Server.DTOs
{
    /// <summary>
    /// Clase base para los objetos de transferencia de datos relacionados con los componentes.
    /// Contiene la descripción en español del componente.
    /// </summary>
    public abstract class BaseComponenteDTO
    {
        /// <summary>
        /// Descripción del componente en español.
        /// </summary>
        public required string DescripcionES { get; set; }
    }

    /// <summary>
    /// Objeto de transferencia de datos utilizado para representar un componente en una tabla.
    /// </summary>
    public class ComponenteTableDTO : BaseComponenteDTO
    {
        /// <summary>
        /// Identificador único del componente.
        /// </summary>
        public required int Id { get; set; }

        /// <summary>
        /// Denominación del componente.
        /// </summary>
        public required string Denominacion { get; set; }

        /// <summary>
        /// Identificador del componente padre, si existe.
        /// </summary>
        public int? IdComponentePadre { get; set; }
    }

    /// <summary>
    /// Objeto de transferencia de datos utilizado para representar un componente completo con detalles.
    /// </summary>
    public class ComponenteDTO : BaseComponenteDTO
    {
        /// <summary>
        /// Identificador único del componente.
        /// </summary>
        public required int Id { get; set; }

        /// <summary>
        /// Denominación del componente.
        /// </summary>
        public required string Denominacion { get; set; }

        /// <summary>
        /// Descripción del componente en inglés.
        /// </summary>
        public string? DescripcionEN { get; set; }

        /// <summary>
        /// Información del componente padre, si existe.
        /// </summary>
        public ComponenteTableDTO? ComponentePadre { get; set; }

        /// <summary>
        /// Información del activo asociado al componente.
        /// </summary>
        public required ActivoMinDTO Activo { get; set; }
    }

    /// <summary>
    /// Objeto de transferencia de datos utilizado para representar los datos necesarios para crear un nuevo componente.
    /// </summary>
    public class CreateComponenteDTO : BaseComponenteDTO
    {
        /// <summary>
        /// Denominación del componente.
        /// </summary>
        public string? Denominacion { get; set; }

        /// <summary>
        /// Descripción del componente en inglés.
        /// </summary>
        public string? DescripcionEN { get; set; }

        /// <summary>
        /// Identificador del componente padre, si existe.
        /// </summary>
        public int? IdComponentePadre { get; set; }

        /// <summary>
        /// Identificador del activo asociado al componente.
        /// </summary>
        public int IdActivo { get; set; }
    }

    /// <summary>
    /// Objeto de transferencia de datos utilizado para representar los datos necesarios para actualizar un componente existente.
    /// </summary>
    public class UpdateComponenteDTO : BaseComponenteDTO
    {
        /// <summary>
        /// Identificador único del componente a actualizar.
        /// </summary>
        public required int Id { get; set; }

        /// <summary>
        /// Denominación del componente.
        /// </summary>
        public required string Denominacion { get; set; }

        /// <summary>
        /// Descripción del componente en inglés.
        /// </summary>
        public string? DescripcionEN { get; set; }

        /// <summary>
        /// Identificador del activo asociado al componente.
        /// </summary>
        public int IdActivo { get; set; }
    }
}
