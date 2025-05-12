using AutoMapper;
using GSMAO.Server.Database.DAOs;
using GSMAO.Server.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace GSMAO.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [Produces("application/json")]
    public class PanelGeneralController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILogger<PanelGeneralController> _logger;
        private readonly OrdenesDAO _ordenesDAO;
        private readonly TiposOrdenesDAO _tiposOrdenesDAO;
        private readonly EstadosOrdenesDAO _estadosDAO;
        private readonly UsersDAO _userDAO;
        private readonly HistorialCambiosOrdenDAO _historialDAO;

        public PanelGeneralController(
            IMapper mapper, 
            ILogger<PanelGeneralController> logger, 
            OrdenesDAO ordenesDAO, 
            TiposOrdenesDAO tiposOrdenesDAO, 
            EstadosOrdenesDAO estadosDAO, 
            UsersDAO userDAO, 
            HistorialCambiosOrdenDAO historialDAO
        )
        {
            _mapper = mapper;
            _logger = logger;
            _ordenesDAO = ordenesDAO;
            _tiposOrdenesDAO = tiposOrdenesDAO;
            _estadosDAO = estadosDAO;
            _userDAO = userDAO;
            _historialDAO = historialDAO;
        }

        /// <summary>
        /// Obtiene un conjunto de órdenes filtradas y agrupa la información en diferentes métricas.
        /// </summary>
        /// <param name="years">Lista de años en formato JSON para filtrar las órdenes. Puede ser null.</param>
        /// <param name="tiposOrdenes">Lista de tipos de órdenes en formato JSON. Puede ser null.</param>
        /// <param name="criticidades">Lista de criticidades en formato JSON. Puede ser null.</param>
        /// <param name="fechaDesde">Fecha inicial para el rango de búsqueda. Puede ser null.</param>
        /// <param name="fechaHasta">Fecha final para el rango de búsqueda. Puede ser null.</param>
        /// <returns>
        /// Un <see cref="Task{ActionResult}"/> que representa el resultado de la operación:
        /// <list type="bullet">
        ///     <item><description>
        ///         <b>200 OK</b>: Retorna un <see cref="PanelGeneralDTO"/> con las órdenes filtradas y sus métricas.
        ///     </description></item>
        ///     <item><description>
        ///         <b>400 Bad Request</b>: Alguno de los filtros proporcionados no tiene un formato JSON válido.
        ///     </description></item>
        ///     <item><description>
        ///         <b>500 Internal Server Error</b>: Se produjo un error inesperado al obtener las órdenes.
        ///     </description></item>
        /// </list>
        /// </returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PanelGeneralDTO>>> RetrieveOrdenes(
            [FromQuery] string? years,
            [FromQuery] string? tiposOrdenes,
            [FromQuery] string? criticidades,
            [FromQuery] DateTime? fechaDesde,
            [FromQuery] DateTime? fechaHasta
        )
        {
            try
            {
                var idUser = User.Claims.Where(u => u.Type == "user.id").Select(u => u.Value).FirstOrDefault();
                var rolUser = User.Claims.Where(u => u.Type == "user.rol.orden").Select(u => u.Value).FirstOrDefault();

                FiltrosPanelGeneralDTO filtrosDTO = new FiltrosPanelGeneralDTO();
                try
                {
                    filtrosDTO.Years = !string.IsNullOrWhiteSpace(years) ? JsonSerializer.Deserialize<List<int>>(years) : null;
                    filtrosDTO.TiposOrdenes = !string.IsNullOrWhiteSpace(tiposOrdenes) ? JsonSerializer.Deserialize<List<int>>(tiposOrdenes) : null;
                    filtrosDTO.Criticidades = !string.IsNullOrWhiteSpace(criticidades) ? JsonSerializer.Deserialize<List<int>>(criticidades) : null;
                }
                catch (JsonException ex)
                {
                    _logger.LogError($"Error al deserializar filtros: {ex.Message}");
                    return BadRequest("Error al procesar los filtros. Verifique que el formato JSON sea correcto.");
                }
                filtrosDTO.FechaDesde = fechaDesde;
                filtrosDTO.FechaHasta = fechaHasta;

                var ordenes = await _ordenesDAO.GetOrdenesTableAsync(filtrosDTO, int.Parse(rolUser!), idUser!);
                var tipos = await _tiposOrdenesDAO.GetAllSemaforoAsync();
                var estados = await _estadosDAO.GetAllSemaforoAsync();

                string NormalizarEstado(string estadoName) => estadoName switch
                {
                    var name when name.Contains("Material") => "Material",
                    _ => estadoName
                };

                PanelGeneralDTO panelGeneral = new PanelGeneralDTO()
                {
                    Table = ordenes,
                    Grafica = ordenes.GroupBy(o => o.FechaApertura.Date)
                                        .Select(s => new GraficaDTO
                                        {
                                            Fecha = s.Key,
                                            NumeroOrdenesTotales = s.Count(),
                                            NOrdenesAbiertas = s.Count(o => o.Estado.Name == "Abierta"),
                                            NOrdenesEnCurso = s.Count(o => o.Estado.Name == "En Curso"),
                                            NOrdenesMaterial = s.Count(o => o.Estado.Name.Contains("Material")),
                                            NOrdenesCerradas = s.Count(o => o.Estado.Name == "Cerrada"),
                                            NOrdenesAnuladas = s.Count(o => o.Estado.Name == "Anulada")
                                        })
                                        .OrderBy(s => s.Fecha)
                                        .ToList(),
                    SemaforoTipos = tipos.GroupJoin(
                                            ordenes.Where(o => (o.Estado.Name == "Abierta" || o.Estado.Name == "En Curso") && (o.Tipo.Name != "Predictivo" || o.Tipo.Name != "Todas"))
                                                   .GroupBy(o => new { o.Tipo.Id, o.Tipo.Name }),
                                            tipo => new { tipo.Id, tipo.Name },
                                            grupo => grupo.Key,
                                            (tipo, grupo) => new { Tipo = tipo, Grupo = grupo.FirstOrDefault() }
                                        )
                                        .OrderBy(s => s.Tipo.Id) // Ordenación por el nombre del tipo
                                        .Select(s => new SemaforoDTO
                                        {
                                            DescripcionOpcion = s.Tipo.Name,
                                            NOrdenesOpcion = s.Grupo?.Count() ?? 0,
                                            PorcentajeOpcion = (s.Grupo != null && s.Grupo.Count() > 0 && ordenes.Count(o => o.Estado.Name == "Abierta" || o.Estado.Name == "En Curso") > 0)
                                                                ? s.Grupo.Count() * 100 / ordenes.Count(o => o.Estado.Name == "Abierta" || o.Estado.Name == "En Curso")
                                                                : 0
                                        })
                                        .ToList(),
                    SemaforoEstados = estados.GroupJoin(
                                            ordenes,
                                            estado => NormalizarEstado(estado.Name),
                                            grupo => NormalizarEstado(grupo.Estado.Name),
                                            (estado, grupo) => new { EstadoName = NormalizarEstado(estado.Name), Grupo = grupo }
                                        )
                                        .Distinct()
                                        .GroupBy(s => s.EstadoName) // Agrupar por estado normalizado
                                        .Select(g => new SemaforoDTO
                                        {
                                            DescripcionOpcion = g.Key, // Nombre del estado (normalizado)
                                            NOrdenesOpcion = g.SelectMany(s => s.Grupo).Count(), // Total de órdenes para este estado
                                            PorcentajeOpcion = (g.SelectMany(s => s.Grupo).Count() > 0 && ordenes.Count() > 0)
                                                                ? g.SelectMany(s => s.Grupo).Count() * 100 / ordenes.Count()
                                                                : 0
                                        })
                                        .ToList()
                };

                return Ok(panelGeneral);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error inesperado al obtener las ordenes: {ex.Message}");

                return StatusCode(500, "Error inesperado al obtener las ordenes.");
            }
        }

        /// <summary>
        /// Obtiene los detalles de una orden específica por su identificador.
        /// </summary>
        /// <param name="idOrden">Identificador único de la orden.</param>
        /// <returns>
        /// Un <see cref="Task{ActionResult}"/> que representa el resultado de la operación:
        /// <list type="bullet">
        ///     <item><description>
        ///         <b>200 OK</b>: Retorna un <see cref="OrdenDTO"/> con los detalles de la orden.
        ///     </description></item>
        ///     <item><description>
        ///         <b>404 Not Found</b>: No se encuentra la orden con el identificador proporcionado.
        ///     </description></item>
        ///     <item><description>
        ///         <b>500 Internal Server Error</b>: Se produjo un error inesperado al obtener la orden.
        ///     </description></item>
        /// </list>
        /// </returns>
        [HttpGet("{idOrden}")]
        public async Task<ActionResult<OrdenDTO>> RetrieveOrden(
            int idOrden
        )
        {
            try
            {
                var orden = await _ordenesDAO.GetByIdAsync(idOrden);

                var ordenDTO = _mapper.Map<OrdenDTO>(orden);

                return Ok(ordenDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error inesperado al obtener la orden: {ex.Message}");

                return StatusCode(500, "Error inesperado al obtener la orden.");
            }
        }

        /// <summary>
        /// Reasigna un usuario a una orden específica.
        /// </summary>
        /// <param name="idOrden">Identificador único de la orden a reasignar.</param>
        /// <param name="reasignarUsuarioOrdenDTO">
        /// Objeto <see cref="ReasignarUsuarioOrdenDTO"/> que contiene la información del usuario de origen, destino y el identificador de la orden.
        /// </param>
        /// <returns>
        /// Un <see cref="Task{ActionResult}"/> que representa el resultado de la operación:
        /// <list type="bullet">
        ///     <item><description>
        ///         <b>200 OK</b>: Retorna un <see cref="UsuarioOrdenDTO"/> con los datos del nuevo usuario asignado a la orden.
        ///     </description></item>
        ///     <item><description>
        ///         <b>400 Bad Request</b>: El modelo proporcionado no es válido o el identificador de la orden no coincide con los datos enviados.
        ///     </description></item>
        ///     <item><description>
        ///         <b>404 Not Found</b>: No se encuentra la orden o los usuarios indicados.
        ///     </description></item>
        ///     <item><description>
        ///         <b>500 Internal Server Error</b>: Se produjo un error inesperado al reasignar el usuario de la orden.
        ///     </description></item>
        /// </list>
        /// </returns>
        [HttpPut("{idOrden}/reasignar-usuario")]
        public async Task<ActionResult> ChangeUser(
            int idOrden, 
            ReasignarUsuarioOrdenDTO reasignarUsuarioOrdenDTO
        )
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (idOrden != reasignarUsuarioOrdenDTO.idOrden)
            {
                return BadRequest("El identificador de la orden no coincide con los datos del JSON enviado.");
            }

            try
            {
                // Comprobar que exista la orden indicada
                await _ordenesDAO.GetByIdAsync(idOrden);

                // Comprobar que exista el usuario de origen indicado
                await _userDAO.GetByIdAsync(reasignarUsuarioOrdenDTO.idUsuarioOrigen);

                // Comprobar que exista el usuario de destino indicado
                await _userDAO.GetByIdAsync(reasignarUsuarioOrdenDTO.idUsuarioDestino);

                var usuario_orden = await _ordenesDAO.ChangeUsuarioOrdenAsync(reasignarUsuarioOrdenDTO);

                _logger.LogInformation($"Actualizado el usuario asignado a la orden con ID {reasignarUsuarioOrdenDTO.idOrden} (USUARIO ORIGEN: {reasignarUsuarioOrdenDTO.idUsuarioOrigen} - USUARIO DESTINO: {reasignarUsuarioOrdenDTO.idUsuarioDestino})");

                return Ok(_mapper.Map<UsuarioOrdenDTO>(usuario_orden.Usuario));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al cambiar el usuario asignado a la orden con ID {reasignarUsuarioOrdenDTO.idOrden}: {ex.Message}");

                return StatusCode(500, $"Error al cambiar el usuario asignado a la orden con ID {reasignarUsuarioOrdenDTO.idOrden}: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtiene el historial de cambios de usuarios asociados a una orden específica.
        /// </summary>
        /// <param name="idOrden">Identificador único de la orden.</param>
        /// <returns>
        /// Un <see cref="Task{ActionResult}"/> que representa el resultado de la operación:
        /// <list type="bullet">
        ///     <item><description>
        ///         <b>200 OK</b>: Retorna un listado de <see cref="RegistroHistorialCambiosOrdenDTO"/> con los cambios de usuarios de la orden.
        ///     </description></item>
        ///     <item><description>
        ///         <b>404 Not Found</b>: No se encuentra la orden con el identificador proporcionado.
        ///     </description></item>
        ///     <item><description>
        ///         <b>500 Internal Server Error</b>: Se produjo un error inesperado al obtener el historial de la orden.
        ///     </description></item>
        /// </list>
        /// </returns>
        [HttpGet("{idOrden}/historial-reasignaciones")]
        public async Task<ActionResult<IEnumerable<RegistroHistorialCambiosOrdenDTO>>> RetrieveHistorialCambiosOrden(
            int idOrden
        )
        {
            try
            {
                // Comprobar que exista la orden indicada
                var orden = await _ordenesDAO.GetByIdAsync(idOrden);

                var historial = await _historialDAO.GetAllOrdenAsync(idOrden);

                _logger.LogInformation($"Historial de cambios de usuarios de la orden {idOrden} obtenido.");

                return Ok(_mapper.Map<List<RegistroHistorialCambiosOrdenDTO>>(historial));
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex.Message);

                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener el historial de cambios de usuarios de la orden con ID {idOrden}: {ex.Message}");

                return StatusCode(500, $"Error al obtener el historial de cambios de usuarios de la orden con ID {idOrden}.");
            }
        }
    }
}
