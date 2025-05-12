using System.ComponentModel.DataAnnotations;
using GSMAO.Server.Database.Tables;

namespace GSMAO.Server.DTOs
{
    /// <summary>
    /// Objeto de transferencia de datos utilizado para representar los datos básicos necesarios de un <see cref="Usuario"/>.
    /// </summary>
    public abstract class BaseUsuarioDTO
    {
        /// <summary>
        /// Nombre del usuario
        /// </summary>
        [Required(ErrorMessage = "El campo 'Nombre' es obligatorio.")]
        public required string Nombre { get; set; }

        /// <summary>
        /// Apellidos del usuario
        /// </summary>
        [Required(ErrorMessage = "El campo 'Apellidos' es obligatorio.")]
        public required string Apellidos { get; set; }

        /// <summary>
        /// Email del usuario
        /// </summary>
        [Required(ErrorMessage = "El campo 'Email' es obligatorio.")]
        [EmailAddress(ErrorMessage = "El email indicado no tiene un formato válido.")]
        public required string Email { get; set; }
    }

    /// <summary>
    /// Objeto de transferencia de datos utilizado para representar los datos necesarios de un <see cref="Usuario"/>.
    /// </summary>
    public class UsuarioDTO : BaseUsuarioDTO
    {
        /// <summary>
        /// Identificador del usuario
        /// </summary>
        public required string Id { get; set; }

        /// <summary>
        /// Rol del usuario. Relación de clave externa con la entidad <see cref="RolDTO"/>
        /// </summary>
        public required RolDTO Rol { get; set; }

        /// <summary>
        /// Planta a la que pertenece el usuario. Relación de clave externa con la entidad <see cref="PlantaDTO"/>
        /// </summary>
        public required UserPlantaDTO Planta { get; set; }

        /// <summary>
        /// Empresa a la que pertenece el usuario. Relación de clave externa con la entidad <see cref="EmpresaDTO"/>
        /// </summary>
        public required EmpresaDTO Empresa { get; set; }

        /// <summary>
        /// Estado del usuario. Relación de clave externa con la entidad <see cref="EstadoUsuarioDTO"/>
        /// </summary>
        public required EstadoUsuarioDTO EstadoUsuario { get; set; }
    }

    /// <summary>
    /// Objeto de transferencia de datos utilizado para representar los datos comunes de crear y actualizar de un <see cref="Usuario"/>.
    /// </summary>
    public class TempDTO : BaseUsuarioDTO
    {
        /// <summary>
        /// Identificador del rol del usuario
        /// </summary>
        [Required(ErrorMessage = "El campo 'Rol' es obligatorio.")]
        public required string IdRol { get; set; }

        /// <summary>
        /// Identificador de la planta a la que pertenece el usuario
        /// </summary>
        public int? IdPlanta { get; set; }

        /// <summary>
        /// Identificador de la empresa a la que pertenece el usuario
        /// </summary>
        public int? IdEmpresa { get; set; }
    }

    /// <summary>
    /// Objeto de transferencia de datos utilizado para representar los datos necesarios para crear un <see cref="Usuario"/>.
    /// </summary>
    public class CreateUserDTO : TempDTO
    {
        /// <summary>
        /// Contraseña del nuevo usuario.
        /// </summary>
        public required string Password { get; set; }
    }

    /// <summary>
    /// Objeto de transferencia de datos utilizado para representar los datos necesarios para actualizar un <see cref="Usuario"/>.
    /// </summary>
    public class UpdateUsuarioDTO : TempDTO
    {
        /// <summary>
        /// Identificador del usuario
        /// </summary>
        public required string Id { get; set; }
    }

    /// <summary>
    /// Objeto de transferencia de datos utilizado para representar los datos necesarios para restablecer la contraseña de un <see cref="Usuario"/>.
    /// </summary>
    public class ResetPasswordDTO
    {
        /// <summary>
        /// Identificador del usuario
        /// </summary>
        public required string Id { get; set; }

        /// <summary>
        /// Nueva contraseña del usuario.
        /// </summary>
        public required string NewPassword { get; set; }
    }

    /// <summary>
    /// Objeto de transferencia de datos utilizado para representar los datos necesarios para mostrar el nombre completo de un <see cref="Usuario"/>.
    /// </summary>
    public class UsuarioOrdenDTO
    {
        /// <summary>
        /// Identificador del usuario
        /// </summary>
        public required string Id { get; set; }

        /// <summary>
        /// Nombre del usuario
        /// </summary>
        public required string Nombre { get; set; }

        /// <summary>
        /// Apellidos del usuario
        /// </summary>
        public required string Apellidos { get; set; }
    }

    /// <summary>
    /// Objeto de transferencia de datos utilizado para representar los datos necesarios para reasignar un usuario de una orden.
    /// </summary>
    public class ReasignarUsuarioOrdenDTO
    {
        /// <summary>
        /// Identificador de la orden que será reasignada.
        /// </summary>
        public required int idOrden { get; set; }
        
        /// <summary>
        /// Identificador del usuario actual asignado a la orden (usuario origen).
        /// </summary>
        public required string idUsuarioOrigen { get; set; }

        /// <summary>
        /// Identificador del nuevo usuario al que se reasignará la orden (usuario destino).
        /// </summary>
        public required string idUsuarioDestino { get; set; }
    }
}