using AutoMapper;
using GSMAO.Server.Database.Tables;
using GSMAO.Server.DTOs;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace GSMAO.Server.Database.DAOs
{
    public class ComponenteDAO : BaseDAO<Componente, int>
    {
        private readonly IMapper _mapper;

        public ComponenteDAO(ApplicationDbContext context, ILogger<ComponenteDAO> logger, EstadosActivosDAO stateDAO, IMapper mapper) : base(context, logger)
        {
            _mapper = mapper;
        }
        
        /// <summary>
        /// Obtiene un listado de componentes de primer nivel de un activo.
        /// </summary>
        /// <param name="idActivo">Identificador del activo.</param>
        /// <param name="cancellationToken">Token de cancelación opcional.</param>
        /// <returns>Un listado de entidades <see cref="Componente"/> encontradas.</returns>
        public async Task<IEnumerable<Componente>> GetByIdActivoAsync(int idActivo, CancellationToken cancellationToken = default)
        {
            return await _context.Componentes
                                    .Where(
                                        c => _context.Activo_Componentes
                                                        .Where(ac => ac.IdActivo == idActivo)
                                                        .Select(ac => ac.IdComponente)
                                                        .Contains(c.Id)
                                    )
                                    .OrderByDescending(c => c.Id)
                                    .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Obtiene un listado de componentes según su componente padre.
        /// </summary>
        /// <param name="idComponente">Identificador del componente padre.</param>
        /// <param name="cancellationToken">Token de cancelación opcional.</param>
        /// <returns>Un listado de entidades <see cref="Componente"/> encontradas.</returns>
        public async Task<IEnumerable<Componente>> GetByIdComponenteAsync(int idComponente, CancellationToken cancellationToken = default)
        {
            return await _context.Componentes
                                    .Where(c => c.IdComponentePadre == idComponente)
                                    .OrderByDescending(c => c.Id)
                                    .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Obtiene un listado de componentes de un activo.
        /// </summary>
        /// <param name="idActivo">Identificador del activo.</param>
        /// <param name="cancellationToken">Token de cancelación opcional.</param>
        /// <returns>Un listado de entidades <see cref="Componente"/> encontradas.</returns>
        public async Task<IEnumerable<Componente>> GetAllComponentesActivoAsync(int idActivo, CancellationToken cancellationToken = default)
        {
            return await _context.Componentes
                                    .Where(c => c.Denominacion.Contains($"/{idActivo}/"))
                                    .OrderByDescending(c => c.Id)
                                    .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Obtiene un componente por su identificador.
        /// </summary>
        /// <param name="idComponente">Identificador del componente.</param>
        /// <param name="cancellationToken">Token de cancelación opcional.</param>
        /// <returns>La entidad <see cref="Componente"/> encontrada.</returns>
        /// <exception cref="KeyNotFoundException">Cuando no se encuentra la entidad <see cref="Componente"/>.</exception>
        protected override async Task<Componente> GetByIdInternalAsync(int idComponente, CancellationToken cancellationToken = default)
        {
            var componente = await _context.Componentes
                .Where(p => p.Id == idComponente)
                .Include(p => p.ComponentePadre)
                .FirstOrDefaultAsync(cancellationToken);

            if (componente == null)
                throw new KeyNotFoundException($"No hay componentes con el id seleccionado");

            return componente;
        }

        public string ObtenerJerarquiaInformeAntiguo(ComponenteDTO componenteDTO)
        {
            List<ComponenteDTO> jerarquia = new List<ComponenteDTO>();
            ObtenerJerarquia(jerarquia, componenteDTO).GetAwaiter().GetResult();
            return jerarquia[^1].Denominacion + " - " + jerarquia[^1].DescripcionES;
        }

        /// <summary>
        /// Obtiene la jerarquía de componentes de manera recursiva, agregando el componente actual y sus componentes padres hasta llegar al componente raíz.
        /// </summary>
        /// <param name="jerarquia">Lista que contiene los componentes en la jerarquía (se va llenando durante la recursión).</param>
        /// <param name="componente">El componente actual cuya jerarquía se va a obtener.</param>
        /// <param name="cancellationToken">El token de cancelación para la operación asincrónica (opcional).</param>
        /// <returns>Una tarea asincrónica que completa la operación de obtener la jerarquía de componentes.</returns>
        public async Task ObtenerJerarquia(List<ComponenteDTO> jerarquia, ComponenteDTO componente, CancellationToken cancellationToken = default)
        {
            jerarquia.Add(componente);

            if (componente.ComponentePadre == null)
            {
                return;
            }
            else
            {
                var newComponente = await GetByIdAsync(componente.ComponentePadre.Id);
                await ObtenerJerarquia(jerarquia, _mapper.Map<ComponenteDTO>(newComponente));
            }
        }
    }
}
