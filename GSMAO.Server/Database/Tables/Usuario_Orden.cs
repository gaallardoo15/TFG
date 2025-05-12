using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace GSMAO.Server.Database.Tables
{
    /// <summary>
    /// Representa la entidad usuario_orden de la base de datos.
    /// 
    /// Esta entidad es la relación entre usuarios y ordenes,
    /// indica que usuarios están asignados a una orden para realizarla
    /// o quién la realizó.
    /// </summary>
    [PrimaryKey(nameof(IdUsuario), nameof(IdOrden))]
    public class Usuario_Orden
    {
        /// <summary>
        /// Identificador del usuario
        /// </summary>
        public required string IdUsuario { get; set; }

        /// <summary>
        /// Identificador de la orden
        /// </summary>
        public required int IdOrden { get; set; }

        /// <summary>
        /// Usuario asignado a la orden. Relación de clave externa con la tabla <see cref="Usuario"/>
        /// </summary>
        [ForeignKey("IdUsuario")]
        public required virtual Usuario Usuario { get; set; }
        
        /// <summary>
        /// Orden a la que está asignado el usuario. Relación de clave externa con la tabla <see cref="Orden"/>
        /// </summary>
        [ForeignKey("IdOrden")]
        public required virtual Orden Orden { get; set; }
    }
}
