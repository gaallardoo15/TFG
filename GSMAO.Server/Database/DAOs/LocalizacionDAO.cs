using AutoMapper;
using GSMAO.Server.Database.Tables;
using GSMAO.Server.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.Numerics;
using System.Threading;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GSMAO.Server.Database.DAOs
{
    /// <summary>
    /// Proporciona acceso a los datos de localizaciones en la base de datos
    /// </summary>
    public class LocalizacionDAO : BaseDAO<Localizacion, int>
    {
        private readonly IMapper _mapper;
        private readonly EmpresaDAO _empresaDAO;
        private readonly PlantaDAO _plantaDAO;

        public LocalizacionDAO(ApplicationDbContext context, ILogger<LocalizacionDAO> logger, IMapper mapper, EmpresaDAO empresaDAO, PlantaDAO plantaDAO) :base (context, logger)
        {
            _mapper = mapper;
            _empresaDAO = empresaDAO;
            _plantaDAO = plantaDAO;
        }

        /// <summary>
        /// Obtiene un listado de localizaciones.
        /// </summary>
        /// <param name="cancellationToken">Token de cancelación opcinal.</param>
        /// <returns>Un listado de entidaddes <see cref="Localizacion"/> encontradas.</returns>
        protected override async Task<IEnumerable<Localizacion>> GetAllInternalAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Localizaciones.Include(l => l.Planta).ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Obtiene un listado de localizaciones según la empresa indicada.
        /// </summary>
        /// <param name="idEmpresa">Identificador de la empresa.</param>
        /// <param name="cancellationToken">Token de cancelación opcinal.</param>
        /// <returns>Un listado de entidaddes <see cref="Localizacion"/> encontradas.</returns>
        public async Task<IEnumerable<Localizacion>> GetEmpresaLocalizacionesAsync(int idEmpresa, CancellationToken cancellationToken = default)
        {
            return await _context.Localizaciones.Where(l => l.Planta.IdEmpresa == idEmpresa)
                                                    .Include(l => l.Planta)
                                                    .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Obtiene un listado de localizaciones según la planta indicada.
        /// </summary>
        /// <param name="idPlanta">Identificador de la planta.</param>
        /// <param name="cancellationToken">Token de cancelación opcinal.</param>
        /// <returns>Un listado de entidaddes <see cref="Localizacion"/> encontradas.</returns>
        public async Task<IEnumerable<Localizacion>> GetPlantaLocalizacionesAsync(int idPlanta, CancellationToken cancellationToken = default)
        {
            return await _context.Localizaciones.Where(l => l.IdPlanta == idPlanta)
                                                    .Include(l => l.Planta)
                                                    .ToListAsync(cancellationToken);
        }
    }

}
