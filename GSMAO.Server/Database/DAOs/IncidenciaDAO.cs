using AutoMapper;
using GSMAO.Server.Database.Tables;
using GSMAO.Server.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace GSMAO.Server.Database.DAOs
{
    /// <summary>
    /// Proporciona acceso a los datos de incidencias en la base de datos.
    /// </summary>
    public class IncidenciaDAO : BaseDAO<Incidencia, int>
    {
        private readonly UsersDAO _userDAO;
        private readonly ComponenteDAO _componenteDAO;
        private readonly OrdenesDAO _ordenesDAO;
        private readonly IMapper _mapper;

        public IncidenciaDAO(ApplicationDbContext context, ILogger<IncidenciaDAO> logger, UsersDAO userDAO, OrdenesDAO ordenesDAO, ComponenteDAO componenteDAO, IMapper mapper) : base(context, logger)
        {
            _userDAO = userDAO;
            _componenteDAO = componenteDAO;
            _ordenesDAO = ordenesDAO;
            _mapper = mapper;
        }

        /// <summary>
        /// Obtiene un listado de incidencias.
        /// </summary>
        /// <param name="cancellationToken">Token de cancelación opcional.</param>
        /// <returns>Un listado de entidades <see cref="Incidencia"/> encontradas.</returns>
        protected override async Task<IEnumerable<Incidencia>> GetAllInternalAsync(CancellationToken cancellationToken = default)
        {
            var incidencias = await _context.Incidencias
                .Include(p => p.MecanismoDeFallo)
                .ToListAsync();

            return incidencias;
        }

        /// <summary>
        /// Obtiene una incidencia.
        /// </summary>
        /// <param name="idIncidencia">Identificador de la incidencia.</param>
        /// <param name="cancellation">Token de cancelación opcional.</param>
        /// <returns>Una entidad <see cref="Incidencia"/>.</returns>
        /// <exception cref="KeyNotFoundException">Cuando no se encuentra la entidad <see cref="Incidencia"/>.</exception>
        protected override async Task<Incidencia> GetByIdInternalAsync(int idIncidencia, CancellationToken cancellation = default)
        {
            var incidencia = await _context.Incidencias
                .Where(p => p.Id == idIncidencia)
                .Include(p => p.MecanismoDeFallo)
                .FirstOrDefaultAsync(cancellation);

            if (incidencia == null)
                throw new KeyNotFoundException($"No hay incidencias con el id seleccionado");

            return incidencia;
        }

        /// <summary>
        /// Obtiene las incidencias asociadas a una orden especificada por su ID.
        /// </summary>
        /// <param name="idOrden">ID de la orden para la cual se desean obtener las incidencias.</param>
        /// <param name="cancellationToken">Token para cancelar la operación de forma asíncrona.</param>
        /// <returns>Una lista de incidencias asociadas a la orden.</returns>
        public async Task<IEnumerable<IncidenciaOrden>> GetOrdenIncidencias(int idOrden, CancellationToken cancellationToken = default)
        {
            var incidencias = await _context.IncidenciasOrdenes
                .Where(io => io.IdOrden == idOrden)
                .Include(o => o.Incidencia)
                    .ThenInclude(o => o.MecanismoDeFallo)
                .Include(o => o.Resolucion)
                .Include(o => o.Componente)
                .ToListAsync(cancellationToken);

            return incidencias;
        }

        /// <summary>
        /// Crea nuevas incidencias asociadas a una orden, basándose en los datos proporcionados.
        /// </summary>
        /// <param name="createIncidenciaOrdenDTO">DTO con la información necesaria para crear las incidencias.</param>
        /// <param name="cancellationToken">Token para cancelar la operación de forma asíncrona.</param>
        /// <returns>Una lista de nuevas entidades de incidencias de orden creadas.</returns>
        /// <exception cref="KeyNotFoundException">Lanzada si no se encuentra la orden o el componente.</exception>
        public async Task<IEnumerable<IncidenciaOrden>> CreateIncidenciasOrdenAsync(CreateIncidenciaOrdenDTO createIncidenciaOrdenDTO, CancellationToken cancellationToken = default)
        {
            // Comprobar que exista la orden indicada
            var orden = await _ordenesDAO.GetByIdAsync(createIncidenciaOrdenDTO.IdOrden);

            // Comprobar que exista el componente indicado
            var componente = await _componenteDAO.GetByIdAsync(createIncidenciaOrdenDTO.IdComponente);

            // Crear incidencia vacía
            var incidenciaVacia = new Incidencia()
            {
                DescripcionES = "",
                IdMecanismoFallo = 0,
                MecanismoDeFallo = new MecanismoDeFallo()
                {
                    DescripcionES = ""
                }
            };

            var newEntityList = Enumerable.Range(0, createIncidenciaOrdenDTO.Incidencias.Count)
                                        .Select(_ =>
                                        {
                                            var newEntity = _mapper.Map<IncidenciaOrden>(createIncidenciaOrdenDTO);

                                            // Personalización de propiedades después del mapeo
                                            newEntity.IdIncidencia = 0;
                                            newEntity.Incidencia = incidenciaVacia;

                                            return newEntity;
                                        })
                                        .ToList();

            for (int i = 0; i < newEntityList.Count; i++)
            {
                // Comprobar que existan todas las incidencias indicadas
                var incidencia = await GetByIdAsync(createIncidenciaOrdenDTO.Incidencias[i]);

                newEntityList[i].IdIncidencia = incidencia.Id;
                newEntityList[i].Incidencia = incidencia;

                await HandleWithLoggingAndExceptionAsync(
                    async ct => {
                        await _context.Set<IncidenciaOrden>().AddAsync(newEntityList[i], cancellationToken).ConfigureAwait(false);
                        await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                    },
                    $"Agregando nueva entidad {typeof(IncidenciaOrden).Name}",
                    cancellationToken
                ).ConfigureAwait(false);
            }

            return newEntityList;
        }
    }
}
