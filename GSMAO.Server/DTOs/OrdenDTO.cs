using GSMAO.Server.Database.Tables;

namespace GSMAO.Server.DTOs
{
    /// <summary>
    /// Objeto de transferencia de datos utilizado para representar los detalles completos de una orden de trabajo,
    /// incluyendo información sobre el activo, la fecha de apertura y cierre, los mecanismos de fallo, las incidencias,
    /// el estado de la orden, el tipo de orden y los usuarios asignados.
    /// </summary>
    public class OrdenTableDTO
    {
        /// <summary>
        /// Identificador único de la orden.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Identificador SAP de la orden (si está disponible).
        /// </summary>
        public string? IdSAP { get; set; }

        /// <summary>
        /// Información del activo asociado a la orden.
        /// </summary>
        public required ActivoMinDTO Activo { get; set; }

        /// <summary>
        /// Fecha de apertura de la orden.
        /// </summary>
        public required DateTime FechaApertura { get; set; }

        /// <summary>
        /// Fecha de cierre de la orden, si está disponible.
        /// </summary>
        public DateTime? FechaCierre { get; set; }

        /// <summary>
        /// Lista de mecanismos de fallo asociados a la orden.
        /// </summary>
        public required List<MecanismoDeFalloDTO> MecanismosDeFallos { get; set; }

        /// <summary>
        /// Lista de incidencias asociadas a la orden.
        /// </summary>
        public required List<IncidenciaDTO> Incidencias { get; set; }

        /// <summary>
        /// Estado actual de la orden.
        /// </summary>
        public required EstadoOrdenDTO Estado { get; set; }

        /// <summary>
        /// Tipo de la orden (correctiva, preventiva, etc.).
        /// </summary>
        public required TipoOrdenDTO Tipo { get; set; }

        /// <summary>
        /// Lista de usuarios asignados a la orden.
        /// </summary>
        public required List<UsuarioOrdenDTO> Usuarios { get; set; }

        /// <summary>
        /// Comentario de la orden.
        /// </summary>
        public required string ComentarioOrden { get; set; }
    }

    /// <summary>
    /// Objeto de transferencia de datos utilizado para representar una orden de trabajo individual con detalles completos,
    /// incluyendo el creador, el activo asociado, el estado de la orden, el tipo de orden, los comentarios y las incidencias.
    /// </summary>
    public class OrdenDTO
    {
        /// <summary>
        /// Identificador único de la orden.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Identificador SAP de la orden (si está disponible).
        /// </summary>
        public string? IdSAP { get; set; }

        /// <summary>
        /// Usuario que creó la orden.
        /// </summary>
        public required UsuarioOrdenDTO Creador { get; set; }

        /// <summary>
        /// Información del activo asociado a la orden.
        /// </summary>
        public required ActivoOrdenDTO Activo { get; set; }

        /// <summary>
        /// Estado actual de la orden.
        /// </summary>
        public required EstadoOrdenDTO EstadoOrden { get; set; }

        /// <summary>
        /// Tipo de orden (correctiva, preventiva, etc.).
        /// </summary>
        public required TipoOrdenDTO TipoOrden { get; set; }

        /// <summary>
        /// Comentario adicional relacionado con la orden.
        /// </summary>
        public string? ComentarioOrden { get; set; }

        /// <summary>
        /// Materiales asociados a la orden (si está disponible).
        /// </summary>
        public string? Materiales { get; set; }

        /// <summary>
        /// Comentario adicional sobre la resolución de la orden.
        /// </summary>
        public string? ComentarioResolucion { get; set; }

        /// <summary>
        /// Lista de usuarios asignados a la orden.
        /// </summary>
        public required List<UsuarioOrdenDTO> Usuarios { get; set; }

        /// <summary>
        /// Fecha de apertura de la orden.
        /// </summary>
        public required DateTime FechaApertura { get; set; }

        /// <summary>
        /// Fecha de cierre de la orden, si está disponible.
        /// </summary>
        public DateTime? FechaCierre { get; set; }

        /// <summary>
        /// Lista de incidencias asociadas a la orden.
        /// </summary>
        public required List<IncidenciaOrdenDTO> IncidenciasOrden { get; set; }
    }

    /// <summary>
    /// Objeto de transferencia de datos utilizado para representar la actualización de una orden de trabajo.
    /// Incluye detalles sobre el estado, tipo, materiales y fechas de la orden.
    /// </summary>
    public class UpdateOrdenDTO
    {
        /// <summary>
        /// Identificador único de la orden.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Identificador SAP de la orden (si está disponible).
        /// </summary>
        public string? IdSAP { get; set; }

        /// <summary>
        /// Nuevo estado de la orden (identificador del estado).
        /// </summary>
        public required int IdEstadoOrden { get; set; }

        /// <summary>
        /// Identificador del activo.
        /// </summary>
        public int IdActivo { get; set; }

        /// <summary>
        /// Nuevo tipo de la orden (identificador del tipo de orden).
        /// </summary>
        public required int IdTipoOrden { get; set; }

        /// <summary>
        /// Comentario adicional relacionado con la orden.
        /// </summary>
        public string? ComentarioOrden { get; set; }

        /// <summary>
        /// Materiales asociados a la orden (si está disponible).
        /// </summary>
        public string? Materiales { get; set; }

        /// <summary>
        /// Comentario adicional sobre la resolución de la orden.
        /// </summary>
        public string? ComentarioResolucion { get; set; }

        /// <summary>
        /// Fecha de apertura de la orden (en formato de cadena).
        /// </summary>
        public required string FechaApertura { get; set; }

        /// <summary>
        /// Fecha de cierre de la orden, si está disponible.
        /// </summary>
        public string? FechaCierre { get; set; }
    }

}
