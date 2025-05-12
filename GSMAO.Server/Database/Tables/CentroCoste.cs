using GSMAO.Server.Database.DAOs;
using GSMAO.Server.DTOs;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace GSMAO.Server.Database.Tables
{
    /// <summary>
    /// Representa la entidad centro de coste de la base de datos
    /// </summary>
    [PrimaryKey(nameof(Id))]
    [Index(nameof(DescripcionES), IsUnique = true)]
    [Index(nameof(CentroCosteSAP), IsUnique = true)]
    public class CentroCoste
    {
        /// <summary>
        /// Identificador del centro de coste
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
        /// Nombre o descripción en español del centro de coste
        /// </summary>
        public required string DescripcionES { get; set; }

        /// <summary>
        /// Nombre o descripción en inglés del centro de coste
        /// </summary>
        public string? DescripcionEN { get; set; }

        /// <summary>
        /// Nombre o descripción del centro de coste en el sistema SAP
        /// </summary>
        public required string CentroCosteSAP { get; set; }

        /// <summary>
        /// Identificador de la planta al que pertenece el centro de coste
        /// </summary>
        public int IdPlanta { get; set; }

        /// <summary>
        /// Planta a la que pertenece el centro de coste. Relación de clave externa con la entidad <see cref="Planta"/>
        /// </summary>
        [ForeignKey("IdPlanta")]
        public required virtual Planta Planta { get; set; }
    }
}
