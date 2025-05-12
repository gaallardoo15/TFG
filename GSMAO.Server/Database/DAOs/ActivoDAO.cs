using AutoMapper;
using GSMAO.Server.Database.Tables;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace GSMAO.Server.Database.DAOs
{
    /// <summary>
    /// Proporciona acceso a los datos de activos en la base de datos.
    /// </summary>
    public class ActivoDAO : BaseDAO<Activo, int>
    {
        private readonly EstadosActivosDAO _stateDAO;

        public ActivoDAO(ApplicationDbContext context, ILogger<ActivoDAO> logger, EstadosActivosDAO stateDAO) : base(context, logger)
        {
            _stateDAO = stateDAO;
        }

        /// <summary>
        /// Obtiene un listado de activos.
        /// </summary>
        /// <param name="cancellationToken">Token de cancelación opcinal.</param>
        /// <returns>Un listado de entidaddes <see cref="Activo"/> encontradas.</returns>
        protected override async Task<IEnumerable<Activo>> GetAllInternalAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Activos
                                    .Include(a => a.Criticidad)
                                    .Include(a => a.Localizacion)
                                    .Include(a => a.CentroCoste)
                                    .Include(a => a.EstadoActivo)
                                    .OrderBy(a => a.IdEstadoActivo)
                                    .ThenByDescending(a => a.ValorCriticidad)
                                    .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Obtiene un activo.
        /// </summary>
        /// <param name="id">Identificador del activo</param>
        /// <param name="cancellationToken">Token de cancelación opcional.</param>
        /// <returns>La entidad <see cref="Activo"/> encontrada.</returns>
        protected override async Task<Activo> GetByIdInternalAsync(int id, CancellationToken cancellationToken = default)
        {
            var activo = await _context.Activos
                                    .Where(a => a.Id == id)
                                    .Include(a => a.Criticidad)
                                    .Include(a => a.Localizacion)
                                    .Include(a => a.CentroCoste)
                                    .Include(a => a.EstadoActivo)
                                    .FirstOrDefaultAsync(cancellationToken);

            if (activo == null)
                throw new KeyNotFoundException($"No hay activos con el id seleccionado");

            return activo;
        }

        /// <summary>
        /// Actualiza el estado de un activo, entre "Activo", "Inactivo" y "Borrado".
        /// </summary>
        /// <param name="id">El identificador del activo.</param>
        /// <param name="stateName">El estado al que se va a actualizar el activo.</param>
        /// <param name="cancellationToken">Token de cancelación opcional.</param>
        public async Task ChangeStateAsync(int id, string stateName, CancellationToken cancellationToken = default)
        {
            await HandleWithLoggingAndExceptionAsync(
            async ct =>
            {
                var activo = await GetByIdAsync(id);
                var state = await _stateDAO.GetByNameAsync(stateName, "Name");

                activo.EstadoActivo = state;
                await UpdateAsync(activo);
            },
            $"Cambio de estado para el activo con ID {id} a {stateName}",
            cancellationToken);
        }

        /// <summary>
        /// Obtiene los activos filtrados según los parámetros proporcionados.
        /// </summary>
        /// <param name="year">El año para filtrar las órdenes por fecha de apertura (opcional).</param>
        /// <param name="mes">El mes para filtrar las órdenes por fecha de apertura (opcional).</param>
        /// <param name="idCentroCoste">El ID del centro de coste para filtrar los activos por centro de coste (opcional).</param>
        /// <param name="idCriticidad">El ID de la criticidad para filtrar los activos por criticidad (opcional).</param>
        /// <param name="cancellationToken">El token de cancelación para la operación asincrónica (opcional).</param>
        /// <returns>Una tarea asincrónica que devuelve una lista de activos distintos filtrados según los parámetros proporcionados.</returns>
        public async Task<IEnumerable<Activo>> GetActivosFiltrosAsync(int? year, int? mes, int? idCentroCoste, int? idCriticidad, CancellationToken cancellationToken = default)
        {
            var query = _context.Ordenes.Where(o => o.Confirmada);

            if (year != null)
                query = query.Where(o => o.FechaApertura!.Value.Year == year);

            if (mes != null)
                query = query.Where(o => o.FechaApertura!.Value.Month == mes);

            if (idCentroCoste != null)
                query = query.Where(o => o.Activo!.IdCentroCoste == idCentroCoste);

            if (idCriticidad != null)
                query = query.Where(o => o.Activo!.IdCriticidad == idCriticidad);

            return await query.Select(o => o.Activo!).Distinct().ToListAsync(cancellationToken);
        }
    }
}
