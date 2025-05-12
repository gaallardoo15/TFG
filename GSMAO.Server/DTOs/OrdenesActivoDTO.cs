namespace GSMAO.Server.DTOs
{
    /// <summary>
    /// Objeto de transferencia de datos utilizado para representar la información relacionada con las órdenes
    /// de mantenimiento asociadas a un activo específico.
    /// </summary>
    public class OrdenesActivoDTO
    {
        /// <summary>
        /// Identificador único del activo asociado a las órdenes de mantenimiento.
        /// </summary>
        public int IdActivo { get; set; }

        /// <summary>
        /// Número total de órdenes de mantenimiento asociadas al activo específico.
        /// </summary>
        public int NumOrdenesActivo { get; set; }
    }

}
