using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace GSMAO.Server.Database.Tables
{
    /// <summary>
    /// Representa la entidad orden de la base de datos.
    /// </summary>
    [PrimaryKey(nameof(Id))]
    public class Orden
    {
        /// <summary>
        /// Identificador de la orden
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Identificador SAP de la orden
        /// </summary>
        public string? IdSAP { get; set; }

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

        private DateTime? _fechaApertura;
        /// <summary>
        /// Fecha de apertura de la orden
        /// </summary>
        public DateTime? FechaApertura
        {
            get => _fechaApertura;
            set
            {
                if (_fechaApertura.HasValue && value == null)
                {
                    throw new InvalidOperationException("No se puede establecer FechaApertura a NULL después de asignar un valor.");
                }
                _fechaApertura = value;
            }
        }

        /// <summary>
        /// Fecha de cierre de la orden
        /// </summary>
        public DateTime? FechaCierre { get; set; }

        /// <summary>
        /// Comentario de la orden
        /// </summary>
        public string? ComentarioOrden { get; set; }

        /// <summary>
        /// Comentario de resolución de la orden
        /// </summary>
        public string? ComentarioResolucion { get; set; }

        /// <summary>
        /// Materiales de la orden
        /// </summary>
        public string? Materiales { get; set; }

        /// <summary>
        /// Tiempo de parada de la orden
        /// </summary>
        public double? TiempoParada { get; set; }

        /// <summary>
        /// Indica si una orden está registrada por completo o a medias
        /// </summary>
        public bool Confirmada { get; set; }

        private int? _idActivo;
        /// <summary>
        /// Identificador del activo de la orden
        /// </summary>
        public int? IdActivo
        {
            get => _idActivo;
            set
            {
                if (_idActivo.HasValue && value == null)
                {
                    throw new InvalidOperationException("No se puede establecer IdActivo a NULL después de asignar un valor.");
                }
                _idActivo = value;
            }
        }

        private int? _idEstadoOrden;
        /// <summary>
        /// Identificador del estado de la orden
        /// </summary>
        public int? IdEstadoOrden
        {
            get => _idEstadoOrden;
            set
            {
                if (_idEstadoOrden.HasValue && value == null)
                {
                    throw new InvalidOperationException("No se puede establecer IdEstadoOrden a NULL después de asignar un valor.");
                }
                _idEstadoOrden = value;
            }
        }

        /// <summary>
        /// Identificador del usuario creador de la orden
        /// </summary>
        public required string IdUsuarioCreador { get; set; }

        private int? _idTipoOrden;
        /// <summary>
        /// Identificador del tipo de la orden
        /// </summary>
        public int? IdTipoOrden
        {
            get => _idTipoOrden;
            set
            {
                if (_idTipoOrden.HasValue && value == null)
                {
                    throw new InvalidOperationException("No se puede establecer IdTipoOrden a NULL después de asignar un valor.");
                }
                _idTipoOrden = value;
            }
        }

        private Activo? _activo;
        /// <summary>
        /// Activo asociado a la orden. Relación de clave externa con la tabla <see cref="Activo"/>
        /// </summary>
        [ForeignKey("IdActivo")]
        public virtual Activo? Activo
        {
            get => _activo;
            set
            {
                if (_activo != null && value == null)
                {
                    throw new InvalidOperationException("No se puede establecer Activo a NULL después de asignar un valor.");
                }
                _activo = value;
            }
        }

        private EstadoOrden? _estadoOrden;
        /// <summary>
        /// Estado de la orden. Relación de clave externa con la tabla <see cref="EstadoOrden"/>
        /// </summary>
        [ForeignKey("IdEstadoOrden")]
        public virtual EstadoOrden? EstadoOrden
        {
            get => _estadoOrden;
            set
            {
                if (_estadoOrden != null && value == null)
                {
                    throw new InvalidOperationException("No se puede establecer EstadoOrden a NULL después de asignar un valor.");
                }
                _estadoOrden = value;
            }
        }


        /// <summary>
        /// Usuario creador de la orden. Relación de clave externa con la tabla <see cref="Usuario"/>
        /// </summary>
        [ForeignKey("IdUsuarioCreador")]
        public required virtual Usuario Usuario { get; set; }

        private TipoOrden? _tipoOrden;
        /// <summary>
        /// Tipo de la orden. Relación de clave externa con la tabla <see cref="TipoOrden"/>
        /// </summary>
        [ForeignKey("IdTipoOrden")]
        public virtual TipoOrden? TipoOrden
        {
            get => _tipoOrden;
            set
            {
                if (_tipoOrden != null && value == null)
                {
                    throw new InvalidOperationException("No se puede establecer TipoOrden a NULL después de asignar un valor.");
                }
                _tipoOrden = value;
            }
        }

        /// <summary>
        /// Listado de incidencias de la orden. Relación de clave externa con la tabla <see cref="IncidenciasOrden"/>
        /// </summary>
        public required virtual List<IncidenciaOrden> IncidenciasOrden { get; set; }

        /// <summary>
        /// Listado de usuarios asignados a la orden. Relación de clave externa con la tabla <see cref="Usuario_Orden"/>
        /// </summary>
        public required virtual List<Usuario_Orden> UsuariosOrden { get; set; }
    }
}
