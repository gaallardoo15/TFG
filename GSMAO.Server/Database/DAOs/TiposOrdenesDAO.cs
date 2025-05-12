using GSMAO.Server.Database.Tables;
using Microsoft.EntityFrameworkCore;

namespace GSMAO.Server.Database.DAOs
{
    public class TiposOrdenesDAO : BaseDAO<TipoOrden, int>
    {
        public TiposOrdenesDAO(ApplicationDbContext context, ILogger<TiposOrdenesDAO> logger) : base(context, logger) { }

        /// <summary>
        /// Obtiene un listado de tipos de orden excluyendo "Todas" y "Predictivo".
        /// </summary>
        /// <returns>Un listado de entidades <see cref="TipoOrden"/> que cumplen con los criterios establecidos.</returns>
        public async Task<IEnumerable<TipoOrden>> GetAllSemaforoAsync()
        {
            var tipos = await _context.TiposOrden.Where(t => t.Name != "Todas" && t.Name != "Predictivo").ToListAsync();
            return tipos;
        }
    }
}
