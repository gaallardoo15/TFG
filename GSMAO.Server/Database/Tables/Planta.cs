using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace GSMAO.Server.Database.Tables
{
    /// <summary>
    /// Representa la entidad planta de la base de datos
    /// </summary>
    [PrimaryKey(nameof(Id))]
    [Index(nameof(Descripcion), nameof(IdEmpresa), IsUnique = true)]
    public class Planta
    {
        /// <summary>
        /// Identificador de la planta
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
        /// Nombre o descripción de la planta
        /// </summary>
        public required string Descripcion { get; set; }

        /// <summary>
        /// Cadena de configuración de STMP para envío de emails
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

        /// <summary>
        /// Identificador de la empresa de la planta
        /// </summary>
        public required int IdEmpresa { get; set; }

        /// <summary>
        /// Empresa a la que pertenece la planta. Relación de clave externa con la entidad <see cref="Empresa"/>
        /// </summary>
        [ForeignKey("IdEmpresa")]
        public required virtual Empresa Empresa { get; set; }
    }
}
