namespace GSMAO.Server.DTOs
{
    /// <summary>
    /// Objeto de transferencia de datos utilizado para representar los datos necesarios para el login del usuario.
    /// </summary>
    public class LoginDTO
    {
        /// <summary>
        /// La dirección de correo electrónico del usuario
        /// </summary>
        public required string Email { get; set; }

        /// <summary>
        /// La contraseña del usuario
        /// </summary>
        public required string Password { get; set; }
    }
}
