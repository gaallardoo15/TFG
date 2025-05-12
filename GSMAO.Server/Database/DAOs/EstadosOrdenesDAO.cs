using GSMAO.Server.Database.Tables;
using Microsoft.EntityFrameworkCore;

namespace GSMAO.Server.Database.DAOs
{
    public class EstadosOrdenesDAO : BaseDAO<EstadoOrden, int>
    {
        public EstadosOrdenesDAO(ApplicationDbContext context, ILogger<EstadosOrdenesDAO> logger) : base(context, logger) { }

        /// <summary>
        /// Obtiene todos los estados de orden, excluyendo aquellos cuyo nombre sea "Anulada".
        /// </summary>
        /// <returns>Una lista de objetos EstadoOrden que representan todos los estados de orden, excepto los anulados.</returns>
        public async Task<IEnumerable<EstadoOrden>> GetAllSemaforoAsync()
        {
            var estados = await _context.EstadosOrden.Where(t => t.Name != "Anulada").ToListAsync();
            return estados;
        }
    }
}
