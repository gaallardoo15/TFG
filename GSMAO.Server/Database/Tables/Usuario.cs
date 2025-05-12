using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace GSMAO.Server.Database.Tables
{
    /// <summary>
    /// Representa la entidad usuario de la base de datos.
    /// </summary>
    [PrimaryKey(nameof(Id))]
    public class Usuario : IdentityUser
    {
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
        /// Nombre del usuario
        /// </summary>
        public required string Nombre { get; set; }

        /// <summary>
        /// Apellidos del usuario
        /// </summary>
        public required string Apellidos { get; set; }

        /// <summary>
        /// Identificador del rol del usuario
        /// </summary>
        public required string IdRol { get; set; }

        /// <summary>
        /// Indica si el email del usuario está confirmado
        /// </summary>
        public int Confirmado { get; set; }

        /// <summary>
        /// Identificador del estado del usuario
        /// </summary>
        public int IdEstadoUsuario { get; set; }

        /// <summary>
        /// Identificador de la planta a la que pertenece el usuario
        /// </summary>
        public int? IdPlanta { get; set; }

        /// <summary>
        /// Identificador de la empresa a la que pertenece el usuario
        /// </summary>
        public int? IdEmpresa { get; set; }

        /// <summary>
        /// Ultimo acceso del usuario al sistema
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime? UltimoAcceso { get; set; }

        /// <summary>
        /// Rol asociado al usuario. Relación de clave externa con la tabla <see cref="Rol"/>
        /// </summary>
        [ForeignKey("IdRol")]
        public required virtual Rol Rol { get; set; }

        /// <summary>
        /// Estado del usuario. Relación de clave externa con la tabla <see cref="EstadoUsuario"/>
        /// </summary>
        [ForeignKey("IdEstadoUsuario")]
        public required virtual EstadoUsuario EstadoUsuario { get; set; }

        /// <summary>
        /// Planta a la que pertenece el usuario. Relación de clave externa con la tabla <see cref="Planta"/>
        /// </summary>
        [ForeignKey("IdPlanta")]
        public virtual Planta? Planta { get; set; }

        /// <summary>
        /// Empresa a la que pertenece el usuario. Relación de clave externa con la tabla <see cref="Empresa"/>
        /// </summary>
        [ForeignKey("IdEmpresa")]
        public virtual Empresa? Empresa { get; set; }
    }
}
