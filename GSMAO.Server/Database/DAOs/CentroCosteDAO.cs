using AutoMapper;
using GSMAO.Server.Database.Tables;
using GSMAO.Server.DTOs;
using Microsoft.EntityFrameworkCore;

namespace GSMAO.Server.Database.DAOs
{
    /// <summary>
    /// Proporciona acceso a los datos de centros de costes en la base de datos.
    /// </summary>
    public class CentroCosteDAO : BaseDAO<CentroCoste, int>
    {
        private readonly PlantaDAO _plantaDAO;
        public CentroCosteDAO(ApplicationDbContext context, ILogger<CentroCosteDAO> logger, PlantaDAO plantaDAO) : base(context, logger) 
        {
            _plantaDAO = plantaDAO;
        }

        /// <summary>
        /// Obtiene un listado de centros de costes.
        /// </summary>
        /// <param name="cancellationToken">Token de cancelación opcinal.</param>
        /// <returns>Un listado de entidaddes <see cref="CentroCoste"/> encontradas.</returns>
        protected override async Task<IEnumerable<CentroCoste>> GetAllInternalAsync(CancellationToken cancellationToken = default)
        {
            return await _context.CentrosDeCostes.Include(l => l.Planta).ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Obtiene un listado de centros de costes según la empresa indicada.
        /// </summary>
        /// <param name="idEmpresa">Identificador de la empresa.</param>
        /// <param name="cancellationToken">Token de cancelación opcinal.</param>
        /// <returns>Un listado de entidaddes <see cref="CentroCoste"/> encontradas.</returns>
        public async Task<IEnumerable<CentroCoste>> GetEmpresaCentrosCostesAsync(int idEmpresa, CancellationToken cancellationToken = default)
        {
            return await _context.CentrosDeCostes.Where(l => l.Planta.IdEmpresa == idEmpresa)
                                                    .Include(l => l.Planta)
                                                    .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Obtiene un listado de centros de costes según la planta indicada.
        /// </summary>
        /// <param name="idPlanta">Identificador de la planta.</param>
        /// <param name="cancellationToken">Token de cancelación opcinal.</param>
        /// <returns>Un listado de entidaddes <see cref="CentroCoste"/> encontradas.</returns>
        public async Task<IEnumerable<CentroCoste>> GetPlantaCentrosCostesAsync(int idPlanta, CancellationToken cancellationToken = default)
        {
            return await _context.CentrosDeCostes.Where(l => l.IdPlanta == idPlanta)
                                                    .Include(l => l.Planta)
                                                    .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Obtiene los centros de coste filtrados según los parámetros proporcionados.
        /// </summary>
        /// <param name="year">El año para filtrar las órdenes por fecha de apertura (opcional).</param>
        /// <param name="mes">El mes para filtrar las órdenes por fecha de apertura (opcional).</param>
        /// <param name="idCriticidad">El ID de la criticidad para filtrar las órdenes por criticidad del activo (opcional).</param>
        /// <param name="idActivo">El ID del activo para filtrar las órdenes por activo (opcional).</param>
        /// <param name="cancellationToken">El token de cancelación para la operación asincrónica (opcional).</param>
        /// <returns>Una tarea asincrónica que devuelve una lista de centros de coste distintos filtrados según los parámetros proporcionados.</returns>
        public async Task<IEnumerable<CentroCoste>> GetCentrosCostesFiltrosAsync(int? year, int? mes, int? idCriticidad, int? idActivo, CancellationToken cancellationToken = default)
        {
            var query = _context.Ordenes.Where(o => o.Confirmada);

            if (year != null)
                query = query.Where(o => o.FechaApertura!.Value.Year == year);

            if (mes != null)
                query = query.Where(o => o.FechaApertura!.Value.Month == mes);

            if (idCriticidad != null)
                query = query.Where(o => o.Activo!.IdCriticidad == idCriticidad);

            if (idActivo != null)
                query = query.Where(o => o.IdActivo == idActivo);

            return await query.Select(o => o.Activo!.CentroCoste).Distinct().ToListAsync(cancellationToken);
        }
    }
}
