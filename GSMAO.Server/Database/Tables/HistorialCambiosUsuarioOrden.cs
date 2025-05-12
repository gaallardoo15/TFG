using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace GSMAO.Server.Database.Tables
{
    /// <summary>
    /// Representa la entidad historial de cambios de usuarios de una orden de la base de datos.
    /// </summary>
    [PrimaryKey(nameof(Id))]
    public class HistorialCambiosUsuarioOrden
    {
        /// <summary>
        /// Identificador del registro de historial
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Fecha de inserción del registro. Automática
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime FechaCambio { get; set; }

        /// <summary>
        /// Identificador de la orden.
        /// </summary>
        public required int IdOrden { get; set; }

        /// <summary>
        /// Identificador del usuario de origen
        /// 
        /// Es el usuario que estaba asignado en la orden antes de hacer el cambio.
        /// Puede ser NULL cuando se asigna por primera vez un usuario a la orden.
        /// </summary>
        public string? IdUsuarioOrigen { get; set; }

        /// <summary>
        /// Identificador del usuario de destino
        /// 
        /// Es el usuario que se ha asignado a la orden tras hacer el cambio.
        /// Puede ser NULL cuando deja de estar asignado el usuario de origen de la orden.
        /// </summary>
        public string? IdUsuarioDestino { get; set; }

        /// <summary>
        /// Estado de la orden. Relación de clave externa con la tabla <see cref="Orden"/>
        /// </summary>
        [ForeignKey("IdOrden")]
        public virtual Orden? Orden { get; set; }

        /// <summary>
        /// Estado de la orden. Relación de clave externa con la tabla <see cref="Usuario"/>
        /// </summary>
        [ForeignKey("IdUsuarioOrigen")]
        public virtual Usuario? UsuarioOrigen { get; set; }

        /// <summary>
        /// Estado de la orden. Relación de clave externa con la tabla <see cref="Usuario"/>
        /// </summary>
        [ForeignKey("IdUsuarioDestino")]
        public virtual Usuario? UsuarioDestino { get; set; }
    }
}
