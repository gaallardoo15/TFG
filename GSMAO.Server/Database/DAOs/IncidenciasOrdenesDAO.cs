using GSMAO.Server.Database.Tables;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System.Xml.Linq;

namespace GSMAO.Server.Database.DAOs
{
    public class IncidenciasOrdenesDAO : BaseDAO<IncidenciaOrden, int>
    {
        public IncidenciasOrdenesDAO(ApplicationDbContext context, ILogger<IncidenciasOrdenesDAO> logger) : base(context, logger) { }

        /// <summary>
        /// Obtiene una incidencia de orden por su ID. Incluye las entidades relacionadas de <see cref="Incidencia"/> y <see cref="Orden"/>.
        /// </summary>
        /// <param name="id">ID de la incidencia de orden que se desea obtener.</param>
        /// <param name="cancellation">Token de cancelación para permitir la cancelación de la operación asincrónica.</param>
        /// <returns>Una tarea que representa la operación asincrónica, con un objeto <see cref="IncidenciaOrden"/> como resultado.</returns>
        /// <exception cref="KeyNotFoundException">Se lanza si no se encuentra la incidencia de orden con el ID proporcionado.</exception>
        protected override async Task<IncidenciaOrden> GetByIdInternalAsync(int id, CancellationToken cancellation = default)
        {
            var incidenciaOrden = await _context.IncidenciasOrdenes
                .Where(p => p.Id == id)
                .Include(p => p.Incidencia)
                .Include(p => p.Orden)
                .FirstOrDefaultAsync(cancellation);

            if (incidenciaOrden == null)
                throw new KeyNotFoundException($"No existe la incidencia de la orden con el id seleccionado");

            return incidenciaOrden;
        }

        /// <summary>
        /// Elimina una incidencia de orden por su ID, con validaciones sobre el estado de la orden relacionada.
        /// </summary>
        /// <param name="id">ID de la incidencia de orden a eliminar.</param>
        /// <param name="cancellationToken">Token de cancelación para permitir la cancelación de la operación asincrónica.</param>
        /// <returns>Una tarea que representa la operación asincrónica.</returns>
        /// <exception cref="InvalidOperationException">Se lanza si la orden está cerrada, anulada o si es la única incidencia asociada a la orden.</exception>
        /// <exception cref="KeyNotFoundException">Se lanza si no se encuentra la incidencia de orden con el ID proporcionado.</exception>
        protected override async Task DeleteInternalAsync(int id, CancellationToken cancellationToken = default)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null && entity.Orden.Confirmada && entity.Orden.EstadoOrden!.Name.Contains("Cerrada"))
            {
                throw new InvalidOperationException(
                    $"No se puede eliminar la entidad {typeof(IncidenciaOrden).Name} con ID {id} porque la orden está cerrada.");
            }
            else if (entity != null && entity.Orden.Confirmada && entity.Orden.EstadoOrden!.Name.Contains("Anulada"))
            {
                throw new InvalidOperationException(
                    $"No se puede eliminar la entidad {typeof(IncidenciaOrden).Name} con ID {id} porque la orden está anulada.");
            }
            else if (entity != null && entity.Orden.Confirmada && entity.Orden.IncidenciasOrden.Count == 1)
            {
                throw new InvalidOperationException(
                    $"No se puede eliminar la entidad {typeof(IncidenciaOrden).Name} con ID {id} porque la orden tiene que tener al menos una incidencia registrada.");
            }
            else if (entity != null)
            {
                _context.Set<IncidenciaOrden>().Remove(entity);
                await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
            else
            {
                throw new KeyNotFoundException(
                    $"No se puede eliminar la entidad {typeof(IncidenciaOrden).Name} con ID {id} porque no existe.");
            }
        }
    }
}
