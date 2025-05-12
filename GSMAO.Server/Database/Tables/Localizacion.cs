using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace GSMAO.Server.Database.Tables
{
    /// <summary>
    /// Representa la entidad localización de la base de datos.
    /// </summary>
    [PrimaryKey(nameof(Id))]
    [Index(nameof(DescripcionES), IsUnique = true)]
    [Index(nameof(LocalizacionSAP), IsUnique = true)]
    public class Localizacion
    {
        /// <summary>
        /// Identificador de la localización
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Fecha de creación de la orden. Automática
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime FechaCreacion { get; set; }

        /// <summary>
        /// Fecha de última modificación de la orden. Automática
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime UltimaModificacion { get; set; }

        /// <summary>
        /// Nombre o descripción en español de la localización
        /// </summary>
        public required string DescripcionES { get; set; }

        /// <summary>
        /// Nombre o descripción en inglés de la localización
        /// </summary>
        public string? DescripcionEN { get; set; }

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

        /// <summary>
        /// Identificador de la planta a la que pertenece la localización
        /// </summary>
        public int IdPlanta { get; set; }

        /// <summary>
        /// Planta asociada a la localización. Relación de clave externa con la tabla <see cref="Planta"/>
        /// </summary>
        [ForeignKey("IdPlanta")]
        public required virtual Planta Planta { get; set; }
    }
}
