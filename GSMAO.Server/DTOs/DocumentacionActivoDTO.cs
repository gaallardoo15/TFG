using GSMAO.Server.Database.Tables;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace GSMAO.Server.DTOs
{
    /// <summary>
    /// Objeto de transferencia de datos utilizado para representar la estructura de la carpeta de documentación de un <see cref="Activo"/>.
    /// </summary>
    public class DocumentacionDTO
    {
        /// <summary>
        /// Ruta del fichero o la carpeta
        /// </summary>
        public required string Key { get; set; }

        /// <summary>
        /// Tipo de la documentación ("folder" o "file")
        /// </summary>
        public required string Type { get; set; }

        /// <summary>
        /// Extensión del fichero ("" cuando es una carpeta)
        /// </summary>
        public required string Extension { get; set; }

        /// <summary>
        /// Fecha de la última modificación del fichero o la carpeta
        /// </summary>
        public required string Modified { get; set; }

        /// <summary>
        /// Tamaño del fichero (null cuando es una carpeta)
        /// </summary>
        public required string? Size { get; set; }
    }

    /// <summary>
    /// Objeto de transferencia de datos utilizado para indicar la ruta principal para las funciones descritas en la variable Ruta de la clase base.
    /// </summary>
    public class BaseDocumentacionDTO
    {
        /// <summary>
        /// Ruta donde se ubica una carpeta o un fichero para las siguientes funciones:
        /// 
        ///     1. Indicar donde se va a crear la nueva carpeta (incluyendo el nombre de la nueva carpeta).
        ///     2. Renombrar una carpeta, indicando la ruta actual y el nombre actual de la carpeta que se quiere renombrar.
        ///     3. Indicar donde se va a eliminar una carpeta (incluyendo el nombre de la carpeta).
        ///     4. Indicar desde donde se va a descargar un fichero.
        /// </summary>
        [Required(ErrorMessage = "Es necesario indicar una Ruta para crear la carpeta.")]
        public required string Ruta { get; set; }

        public bool Materiales { get; set; }
    }

    /// <summary>
    /// Objeto de transferencia de datos utilizado para crear una nueva carpeta dentro de la carpeta de documentación de un <see cref="Activo"/>.
    /// </summary>
    public class CreateCarpetaDTO : BaseDocumentacionDTO { }

    /// <summary>
    /// Objeto de transferencia de datos utilizado para renombrar una carpeta dentro de la carpeta de documentación de un <see cref="Activo"/>.
    /// </summary>
    public class RenameCarpetaDTO : BaseDocumentacionDTO
    {
        /// <summary>
        /// Ruta donde se indica el nuevo nombre de la carpeta.
        /// </summary>
        [Required(ErrorMessage = "Es necesario indicar la nueva Ruta para renombrar la carpeta.")]
        public required string RutaNueva { get; set; }
    }

    /// <summary>
    /// Objeto de transferencia de datos utilizado para eliminar una carpeta dentro de la carpeta de documentación de un <see cref="Activo"/>.
    /// </summary>
    public class DeleteCarpetaDTO : BaseDocumentacionDTO { }

    /// <summary>
    /// Objeto de transferencia de datos utilizado para subir un nuevo fichero dentro de la carpeta de documentación de un <see cref="Activo"/>.
    /// </summary>
    public class CreateFicheroDTO
    {
        /// <summary>
        /// Ruta donde se va a guardar el fichero dentro de la documentación del <see cref="Activo"/>.
        /// </summary>
        public string? Ruta { get; set; }

        public bool Materiales { get; set; }
    }

    /// <summary>
    /// Objeto de transferencia de datos utilizado para descargar un fichero dentro de la carpeta de documentación de un <see cref="Activo"/>.
    /// </summary>
    public class DownloadFicheroDTO : BaseDocumentacionDTO { }

    /// <summary>
    /// Objeto de transferencia de datos utilizado para crear una nueva carpeta dentro de la carpeta de documentación de un <see cref="Activo"/>.
    /// </summary>
    public class DeleteFicheroDTO : BaseDocumentacionDTO { }
}
