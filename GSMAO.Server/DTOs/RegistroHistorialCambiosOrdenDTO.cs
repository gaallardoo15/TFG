namespace GSMAO.Server.DTOs
{
    /// <summary>
    /// Objeto de transferencia de datos utilizado para representar el historial de cambios de una orden de trabajo.
    /// Incluye la fecha del cambio, así como los usuarios que realizaron el cambio y el usuario que recibe la asignación.
    /// </summary>
    public class RegistroHistorialCambiosOrdenDTO
    {
        /// <summary>
        /// Fecha en la que se realizó el cambio en la orden.
        /// </summary>
        public DateTime FechaCambio { get; set; }

        /// <summary>
        /// Usuario que originó el cambio (si está disponible).
        /// </summary>
        public UsuarioOrdenDTO? UsuarioOrigen { get; set; }

        /// <summary>
        /// Usuario que recibió el cambio (si está disponible).
        /// </summary>
        public UsuarioOrdenDTO? UsuarioDestino { get; set; }
    }

}
