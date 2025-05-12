using GSMAO.Server.Database.Tables;
using Microsoft.EntityFrameworkCore;

namespace GSMAO.Server.Database.DAOs
{
    /// <summary>
    /// Proporciona acceso a los datos de plantas en la base de datos.
    /// </summary>
    public class PlantaDAO : BaseDAO<Planta, int>
    {
        private readonly UsersDAO _userDAO;

        public PlantaDAO(ApplicationDbContext context, ILogger<PlantaDAO> logger, UsersDAO userDAO) : base(context, logger)
        {
            _userDAO = userDAO;
        }
        
        /// <summary>
        /// Obtiene un listado de plantas.
        /// </summary>
        /// <param name="cancellationToken">Token de cancelación opcional.</param>
        /// <returns>Un listado de entidades <see cref="Planta"/> encontradas.</returns>
        protected override async Task<IEnumerable<Planta>> GetAllInternalAsync(CancellationToken cancellationToken = default)
        {
            var plantas = await _context.Plantas
                .Include(p => p.Empresa)
                .ToListAsync();

            return plantas;
        }
         
        /// <summary>
        /// Obtiene una planta por su identificador.
        /// </summary>
        /// <param name="idPlanta">Identificador de la planta.</param>
        /// <param name="cancellation">Token de cancelación opcional.</param>
        /// <returns>La entidad <see cref="Planta"/> encontrada.</returns>
        /// <exception cref="KeyNotFoundException">Cuando no se encuentra la entidad <see cref="Planta"/>.</exception>
        protected override async Task<Planta> GetByIdInternalAsync(int idPlanta, CancellationToken cancellationToken = default)
        {
            var planta = await _context.Plantas
                .Where(p => p.Id == idPlanta)
                .Include(p => p.Empresa)
                .FirstOrDefaultAsync(cancellationToken);

            if (planta == null)
                throw new KeyNotFoundException($"No hay plantas con el id seleccionado");

            return planta;
        }

        /// <summary>
        /// Obtiene un listado de plantas según el identificador de la empresa.
        /// </summary>
        /// <param name="idEmpresa">Identificador de la empresa.</param>
        /// <returns>Un listado de entidades <see cref="Planta"/> encontradas.</returns>
        public async Task<IEnumerable<Planta>> GetPlantasEmpresaAsync(int idEmpresa)
        {
            var plantas = await _context.Plantas.Where(p => p.Empresa.Id == idEmpresa).Include(p=>p.Empresa).ToListAsync();
            return plantas;
        }
    }
}
