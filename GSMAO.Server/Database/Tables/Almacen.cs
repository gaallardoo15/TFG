using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace GSMAO.Server.Database.Tables
{
    /// <summary>
    /// Representa la entidad almacén de la base de datos.
    /// </summary>
    [PrimaryKey(nameof(Id))]
    public class Almacen
    {
        /// <summary>
        /// Identificador del almacén
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
        /// Nombre o descripción del almacén
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// Contacto del almacén
        /// </summary>
        public string? Contacto { get; set; }

        /// <summary>
        /// Indicador si el almacén pertenece a Hitachi o no
        /// </summary>
        public bool Externo { get; set; }
    }
}
