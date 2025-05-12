using GSMAO.Server.Database.Tables;
using Microsoft.EntityFrameworkCore;

namespace GSMAO.Server.Database.DAOs
{
    public class HistorialCambiosOrdenDAO : BaseDAO<HistorialCambiosUsuarioOrden, int>
    {
        public HistorialCambiosOrdenDAO(ApplicationDbContext context, ILogger<HistorialCambiosOrdenDAO> logger) : base(context, logger) { }

        /// <summary>
        /// Obtiene el historial de cambios de usuarios asociados a una orden específica, dado su ID.
        /// </summary>
        /// <param name="idOrden">ID de la orden para la cual se desean obtener los cambios de usuario.</param>
        /// <param name="cancellationToken">Token para cancelar la operación de forma asíncrona.</param>
        /// <returns>Una lista de registros de cambios de usuario asociados a la orden.</returns>
        /// <exception cref="KeyNotFoundException">Lanzada si no se encuentran registros de cambios de usuario para la orden especificada.</exception>
        public async Task<IEnumerable<HistorialCambiosUsuarioOrden>> GetAllOrdenAsync(int idOrden, CancellationToken cancellationToken = default)
        {
            var historial = await _context.HistorialCambiosUsuariosOrdenes
                .Where(h => h.IdOrden == idOrden)
                .Include(h => h.UsuarioOrigen)
                .Include(h => h.UsuarioDestino)
                .OrderByDescending(h => h.FechaCambio)
                .ToListAsync();

            if (historial == null || historial.Count == 0)
                throw new KeyNotFoundException($"No hay registros de cambios de usuario para la orden {idOrden}.");

            return historial;
        }
    }
}
