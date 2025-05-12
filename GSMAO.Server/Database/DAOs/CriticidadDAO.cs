using GSMAO.Server.Database.Tables;
using Microsoft.EntityFrameworkCore;

namespace GSMAO.Server.Database.DAOs
{
    /// <summary>
    /// Proporciona acceso a los datos de criticidades en la base de datos.
    /// </summary>
    public class CriticidadDAO : BaseDAO<Criticidad, int>
    {
        public CriticidadDAO(ApplicationDbContext context, ILogger<CriticidadDAO> logger) : base(context, logger) { }

        /// <summary>
        /// Obtiene una lista de criticidades distintas asociadas a las órdenes confirmadas, filtradas por varios parámetros.
        /// </summary>
        /// <param name="year">El año de las órdenes a filtrar (opcional).</param>
        /// <param name="mes">El mes de las órdenes a filtrar (opcional).</param>
        /// <param name="idCentroCoste">El ID del centro de coste relacionado con las órdenes a filtrar (opcional).</param>
        /// <param name="idActivo">El ID del activo relacionado con las órdenes a filtrar (opcional).</param>
        /// <param name="cancellationToken">El token de cancelación para la operación asincrónica (opcional).</param>
        /// <returns>Una lista de objetos Criticidad distintos asociados a las órdenes filtradas.</returns>
        public async Task<IEnumerable<Criticidad>> GetCriticidadesFiltrosAsync(int? year, int? mes, int? idCentroCoste, int? idActivo, CancellationToken cancellationToken = default)
        {
            var query = _context.Ordenes.Where(o => o.Confirmada);

            if (year != null)
                query = query.Where(o => o.FechaApertura!.Value.Year == year);

            if (mes != null)
                query = query.Where(o => o.FechaApertura!.Value.Month == mes);

            if (idCentroCoste != null)
                query = query.Where(o => o.Activo!.IdCentroCoste == idCentroCoste);

            if (idActivo != null)
                query = query.Where(o => o.IdActivo == idActivo);

            return await query.Select(o => o.Activo!.Criticidad).Distinct().ToListAsync(cancellationToken);
        }
    }
}
