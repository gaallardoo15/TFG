using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace GSMAO.Server.Database.Tables
{
    /// <summary>
    /// Representa la entidad activo de la base de datos.
    /// </summary>
    [PrimaryKey(nameof(Id))]
    public class Activo
    {
        /// <summary>
        /// Identificador del activo
        /// </summary>
        public required int Id { get; set; }

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
        /// Identificador SAP del activo
        /// </summary>
        public required string ActivoSAP { get; set; }

        /// <summary>
        /// Nombre o descripción en español del activo
        /// </summary>
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
        public required int ValorCriticidad { get; set; }

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

        /// <summary>
        /// Identificador del estado del activo
        /// </summary>
        public required int IdEstadoActivo { get; set; }

        // Relaciones
        /// <summary>
        /// Criticidad del activo. Relación de clave externa con la tabla <see cref="Criticidad"/>
        /// </summary>
        [ForeignKey("IdCriticidad")]
        public required virtual Criticidad Criticidad { get; set; }

        /// <summary>
        /// Localización del activo. Relación de clave externa con la tabla <see cref="Localizacion"/>
        /// </summary>
        [ForeignKey("IdLocalizacion")]
        public required virtual Localizacion Localizacion { get; set; }

        /// <summary>
        /// Centro de coste del activo. Relación de clave externa con la tabla <see cref="CentroCoste"/>
        /// </summary>
        [ForeignKey("IdCentroCoste")]
        public required virtual CentroCoste CentroCoste { get; set; }

        /// <summary>
        /// Estado del activo. Relación de clave externa con la tabla <see cref="EstadoActivo"/>
        /// </summary>
        [ForeignKey("IdEstadoActivo")]
        public required virtual EstadoActivo EstadoActivo { get; set; }

    }
}
