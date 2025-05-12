using AutoMapper;
using GSMAO.Server.Database.DAOs;
using GSMAO.Server.Database.Tables;
using GSMAO.Server.DTOs;
using GSMAO.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography.X509Certificates;

namespace GSMAO.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [Produces("application/json")]
    public class OrdenesController : BaseFileController
    {
        private readonly IMapper _mapper;
        private new readonly ILogger<OrdenesController> _logger;
        private readonly OrdenesDAO _ordenesDAO;
        private readonly EstadosOrdenesDAO _estadosDAO;
        private readonly TiposOrdenesDAO _tiposDAO;
        private readonly UsersDAO _userDAO;
        private readonly IncidenciaDAO _incidenciaDAO;
        private readonly ComponenteDAO _componenteDAO;
        private readonly IncidenciasOrdenesDAO _incidenciaOrdenDAO;
        private readonly ResolucionDAO _resolucionDAO;
        private readonly ValidateData _validateOrdenData;
        private readonly CriticidadDAO _criticidadDAO;

        public OrdenesController(IFileService fileService,
                                    IMapper mapper,
                                    ILogger<OrdenesController> logger,
                                    OrdenesDAO ordenesDAO,
                                    EstadosOrdenesDAO estadosDAO,
                                    TiposOrdenesDAO tiposDAO,
                                    UsersDAO userDAO,
                                    IncidenciaDAO incidenciaDAO,
                                    ComponenteDAO componenteDAO,
                                    IncidenciasOrdenesDAO incidenciaOrdenDAO,
                                    ResolucionDAO resolucionDAO,
                                    ValidateData validateOrdenData,
                                    CriticidadDAO criticidadDAO
            ) : base (fileService, logger)
        {
            _mapper = mapper;
            _logger = logger;
            _ordenesDAO = ordenesDAO;
            _estadosDAO = estadosDAO;
            _tiposDAO = tiposDAO;
            _userDAO = userDAO;
            _incidenciaDAO = incidenciaDAO;
            _componenteDAO = componenteDAO;
            _incidenciaOrdenDAO = incidenciaOrdenDAO;
            _resolucionDAO = resolucionDAO;
            _validateOrdenData = validateOrdenData;
            _criticidadDAO = criticidadDAO;
        }

        /// <summary>
        /// Valor concreto de la carpeta raíz para Órdenes.
        /// </summary>
        protected override string RootFolderName => "Ordenes";

        /// <summary>
        /// Valida la existencia de la Orden en BD (ejemplo).
        /// </summary>
        protected override async Task ValidateEntityExistsAsync(int id)
        {
            // Aquí se valida en la BD que la Orden exista:
            await _ordenesDAO.GetByIdAsync(id);
        }

        /// <summary>
        /// Obtiene la lista de estados disponibles para las órdenes.
        /// </summary>
        /// <returns>
        /// Un <see cref="Task{ActionResult}"/> con la lista de estados:
        /// <list type="bullet">
        ///     <item><description><b>200 OK</b>: Lista de estados de las órdenes en formato <see cref="EstadoOrdenDTO"/>.</description></item>
        /// </list>
        /// </returns
        [HttpGet("estados")]
        public async Task<ActionResult<IEnumerable<EstadoOrdenDTO>>> RetrieveEstadosOrdenes()
        {
            var estados = await _estadosDAO.GetAllAsync();
            return Ok(_mapper.Map<List<EstadoOrdenDTO>>(estados));
        }

        /// <summary>
        /// Obtiene la lista de tipos de órdenes disponibles.
        /// </summary>
        /// <returns>
        /// Un <see cref="Task{ActionResult}"/> con la lista de tipos:
        /// <list type="bullet">
        ///     <item><description><b>200 OK</b>: Lista de tipos de órdenes en formato <see cref="TipoOrdenDTO"/>.</description></item>
        /// </list>
        /// </returns>
        [HttpGet("tipos")]
        public async Task<ActionResult<IEnumerable<TipoOrdenDTO>>> RetrieveTiposOrdenes()
        {
            var tipos = await _tiposDAO.GetAllAsync();
            return Ok(_mapper.Map<List<TipoOrdenDTO>>(tipos));
        }

        /// <summary>
        /// Obtiene los años para los que existen órdenes registradas.
        /// </summary>
        /// <returns>
        /// Un <see cref="Task{ActionResult}"/> con la lista de años:
        /// <list type="bullet">
        ///     <item><description><b>200 OK</b>: Lista de años como valores enteros.</description></item>
        /// </list>
        /// </returns>
        [HttpGet("years")]
        public async Task<ActionResult<IEnumerable<int>>> RetrieveYears()
        {
            var years = await _ordenesDAO.GetYears();
            return Ok(years);
        }

        [HttpGet("criticidades")]
        public async Task<ActionResult<IEnumerable<CriticidadDTO>>> RetrieveCriticidades()
        {
            var criticidades = await _criticidadDAO.GetAllAsync();
            return Ok(_mapper.Map<List<CriticidadDTO>>(criticidades));
        }

        /// <summary>
        /// Obtiene la lista de usuarios asignados a una orden específica.
        /// </summary>
        /// <param name="idOrden">Identificador único de la orden.</param>
        /// <returns>
        /// Un <see cref="Task{ActionResult}"/> con la lista de usuarios:
        /// <list type="bullet">
        ///     <item><description><b>200 OK</b>: Lista de usuarios asignados en formato <see cref="UsuarioOrdenDTO"/>.</description></item>
        /// </list>
        /// </returns>
        [HttpGet("{idOrden}/usuarios")]
        public async Task<ActionResult<IEnumerable<UsuarioOrdenDTO>>> RetrieveUsuariosOrden(int idOrden)
        {
            var usuarios = await _userDAO.GetOrdenUsers(idOrden);
            return Ok(_mapper.Map<List<UsuarioOrdenDTO>>(usuarios));
        }

        /// <summary>
        /// Obtiene la lista de incidencias asociadas a una orden específica.
        /// </summary>
        /// <param name="idOrden">Identificador único de la orden.</param>
        /// <returns>
        /// Un <see cref="Task{ActionResult}"/> con la lista de incidencias:
        /// <list type="bullet">
        ///     <item><description><b>200 OK</b>: Lista de incidencias asociadas en formato <see cref="IncidenciaOrdenDTO"/>.</description></item>
        /// </list>
        /// </returns>
        [HttpGet("{idOrden}/incidencias")]
        public async Task<ActionResult<IEnumerable<IncidenciaOrdenDTO>>> RetrieveIncidenciasOrden(int idOrden)
        {
            var incidencias = await _incidenciaDAO.GetOrdenIncidencias(idOrden);
            return Ok(_mapper.Map<List<IncidenciaOrdenDTO>>(incidencias));
        }

        /// <summary>
        /// Obtiene la lista de usuarios disponibles para ser asignados a una orden específica.
        /// </summary>
        /// <param name="idOrden">Identificador único de la orden.</param>
        /// <returns>
        /// Un <see cref="Task{ActionResult}"/> con la lista de usuarios:
        /// <list type="bullet">
        ///     <item><description><b>200 OK</b>: Lista de usuarios disponibles en formato <see cref="UsuarioOrdenDTO"/>.</description></item>
        /// </list>
        /// </returns
        [HttpGet("{idOrden}/usuarios-disponibles")]
        public async Task<ActionResult<IEnumerable<UsuarioOrdenDTO>>> RetrieveUsuariosDisponiblesOrden(int idOrden)
        {
            var userId = User.Claims.Where(u => u.Type == "user.id").Select(u => u.Value).FirstOrDefault();

            var usuarios = await _userDAO.GetOrdenUsersAvailable(userId!, idOrden);
            return Ok(_mapper.Map<List<UsuarioOrdenDTO>>(usuarios));
        }

        /// <summary>
        /// Elimina un usuario específico de una orden.
        /// </summary>
        /// <param name="idOrden">Identificador único de la orden.</param>
        /// <param name="idUser">Identificador único del usuario a eliminar.</param>
        /// <returns>
        /// Un <see cref="Task{ActionResult}"/> que representa el resultado de la operación:
        /// <list type="bullet">
        ///     <item><description><b>200 OK</b>: Usuario eliminado correctamente de la orden.</description></item>
        ///     <item><description><b>404 Not Found</b>: No se encontró la orden o el usuario indicado.</description></item>
        ///     <item><description><b>500 Internal Server Error</b>: Error inesperado al eliminar el usuario de la orden.</description></item>
        /// </list>
        /// </returns>
        [HttpDelete("{idOrden}/eliminar-usuario/{idUser}")]
        public async Task<ActionResult> DeleteUsuarioOrden(int idOrden, string idUser)
        {
            // Comprobar que exista la orden indicada
            var orden = await _ordenesDAO.GetByIdAsync(idOrden);

            // Comprobar que exista el usuario indicado
            var user = await _userDAO.GetByIdAsync(idUser);

            try
            {
                await _ordenesDAO.DeleteUsuarioOrdenAsync(idUser, idOrden);

                _logger.LogInformation($"Usuario {user.Nombre} {user.Apellidos} eliminado de la orden {orden.Id}");

                return Ok($"Usuario eliminado correctamente de la orden.");
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogInformation($"{ex.Message}");

                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar el usuario {user.Nombre} {user.Apellidos} con ID: {user.Id} de la orden {orden.Id}: {ex.Message}");

                return StatusCode(500, $"Error al eliminar el usuario {user.Nombre} {user.Apellidos} con ID: {user.Id} de la orden {orden.Id}: {ex.Message}");
            }
        }

        /// <summary>
        /// Asigna un usuario a una orden específica.
        /// </summary>
        /// <param name="idOrden">Identificador único de la orden.</param>
        /// <param name="idUser">Identificador único del usuario a asignar.</param>
        /// <returns>
        /// Un <see cref="Task{ActionResult}"/> que representa el resultado de la operación:
        /// <list type="bullet">
        ///     <item><description><b>200 OK</b>: Usuario asignado correctamente a la orden.</description></item>
        ///     <item><description><b>404 Not Found</b>: No se encontró la orden o el usuario indicado.</description></item>
        ///     <item><description><b>500 Internal Server Error</b>: Error inesperado al asignar el usuario a la orden.</description></item>
        /// </list>
        /// </returns>
        [HttpPost("{idOrden}/asignar-usuario/{idUser}")]
        public async Task<ActionResult> CreateUsuarioOrden(int idOrden, string idUser)
        {
            // Comprobar que exista la orden indicada
            var orden = await _ordenesDAO.GetByIdAsync(idOrden);

            // Comprobar que exista el usuario indicado
            var user = await _userDAO.GetByIdAsync(idUser);

            try
            {
                var entity = new Usuario_Orden
                {
                    IdUsuario = idUser,
                    IdOrden = idOrden,
                    Usuario = user,
                    Orden = orden
                };

                var createdUsuarioOrden = await _ordenesDAO.CreateUsuarioOrdenAsync(entity);

                _logger.LogInformation($"Usuario {user.Nombre} {user.Apellidos} asignado a la orden {orden.Id}");

                return Ok($"Usuario asignado correctamente a la orden.");
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogInformation($"{ex.Message}");

                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al asignar el usuario {user.Nombre} {user.Apellidos} con ID: {user.Id} a la orden {orden.Id}: {ex.Message}");

                return StatusCode(500, $"Error al asignar el usuario {user.Nombre} {user.Apellidos} con ID: {user.Id} a la orden {orden.Id}: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtiene la jerarquía de un componente específico, es decir, sus componentes padres.
        /// </summary>
        /// <param name="idComponente">Identificador único del componente.</param>
        /// <returns>
        /// Un <see cref="Task{ActionResult}"/> con la jerarquía de componentes:
        /// <list type="bullet">
        ///     <item><description><b>200 OK</b>: Lista de componentes en formato <see cref="ComponenteTableDTO"/>.</description></item>
        ///     <item><description><b>500 Internal Server Error</b>: Error al obtener la jerarquía del componente.</description></item>
        /// </list>
        /// </returns>
        [HttpGet("obtener-jerarquia/{idComponente}")]
        public async Task<ActionResult<IEnumerable<ComponenteTableDTO>>> RetrieveJerarquiaComponente(int idComponente)
        {
            // Comprobar que exista el componente
            var componente = await _componenteDAO.GetByIdAsync(idComponente);

            try
            {
                List<ComponenteDTO> jerarquia = new List<ComponenteDTO>();

                var componenteDTO = _mapper.Map<ComponenteDTO>(componente);

                await _componenteDAO.ObtenerJerarquia(jerarquia, componenteDTO);

                jerarquia.Reverse();

                return Ok(_mapper.Map<List<ComponenteTableDTO>>(jerarquia));
            }
            catch(Exception ex)
            {
                _logger.LogError($"Error al obtener la jerarquía del componente indicado: {ex.Message}");

                return StatusCode(500, $"Error al obtener la jerarquía del componente indicado.");
            }
        }

        /// <summary>
        /// Crea nuevas incidencias para una orden específica.
        /// </summary>
        /// <param name="idOrden">Identificador único de la orden.</param>
        /// <param name="createIncidenciaOrdenDTO">Datos para crear las incidencias de la orden.</param>
        /// <returns>
        /// Un <see cref="Task{ActionResult}"/> que representa el resultado de la operación:
        /// <list type="bullet">
        ///     <item><description><b>200 OK</b>: Incidencias asignadas correctamente a la orden.</description></item>
        ///     <item><description><b>400 Bad Request</b>: El identificador de la orden no coincide con los datos del JSON enviado.</description></item>
        ///     <item><description><b>500 Internal Server Error</b>: Error al crear la incidencia para la orden.</description></item>
        /// </list>
        /// </returns>
        [HttpPost("{idOrden}/nueva-incidencia")]
        public async Task<ActionResult> CreateIncidenciaOrden(int idOrden, CreateIncidenciaOrdenDTO createIncidenciaOrdenDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (idOrden != createIncidenciaOrdenDTO.IdOrden)
            {
                return BadRequest("El identificador de la orden no coincide con los datos del JSON enviado.");
            }

            try
            {
                // Comprobar que al menos tenga una incidencia
                if (createIncidenciaOrdenDTO.Incidencias.Count == 0)
                    throw new ValidationException("Debes seleccionar al menos una incidencia.");

                // Comprobar que tenga seleccionado un componente
                if (createIncidenciaOrdenDTO.IdComponente == 0)
                    throw new ValidationException("Debes seleccionar un componente.");

                // Insertar las incidencias para el componente indicado de la orden
                var createdIncidenciasOrden = await _incidenciaDAO.CreateIncidenciasOrdenAsync(createIncidenciaOrdenDTO);

                _logger.LogInformation($"Incidencias asignadas a la orden {idOrden}. INCIDENCIAS: {createdIncidenciasOrden}");

                return Ok($"Incidencias asignadas a la orden {idOrden}.");
            }
            catch (Exception ex) when (ex is KeyNotFoundException || ex is ValidationException)
            {
                _logger.LogError(ex.Message);

                return StatusCode(500, ex.Message);
            }
            catch(Exception ex)
            {
                _logger.LogError($"Error al crear la incidencia para la orden con ID {idOrden}: {ex.Message}");

                return StatusCode(500, $"Error al crear la incidencia para la orden con ID {idOrden}.");
            }
        }

        /// <summary>
        /// Actualiza los datos de una incidencia específica en una orden.
        /// </summary>
        /// <param name="idOrden">Identificador único de la orden.</param>
        /// <param name="idIncidenciaConjunto">Identificador único del conjunto de incidencia a actualizar.</param>
        /// <param name="updateIncidenciaOrdenDTO">Datos de la incidencia a actualizar.</param>
        /// <returns>
        /// Un <see cref="Task{ActionResult}"/> que representa el resultado de la operación:
        /// <list type="bullet">
        ///     <item><description><b>200 OK</b>: Incidencia actualizada correctamente.</description></item>
        ///     <item><description><b>400 Bad Request</b>: El identificador de la orden o incidencia no coincide con los datos enviados.</description></item>
        ///     <item><description><b>500 Internal Server Error</b>: Error al actualizar la incidencia.</description></item>
        /// </list>
        /// </returns>
        [HttpPut("{idOrden}/editar-incidencia/{idIncidenciaConjunto}")]
        public async Task<ActionResult> UpdateIncidenciaOrden(int idOrden, int idIncidenciaConjunto, UpdateIncidenciaOrdenDTO updateIncidenciaOrdenDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (idIncidenciaConjunto != updateIncidenciaOrdenDTO.Id)
            {
                return BadRequest("El identificador de conjunto de elementos de la incidencia no coincide con los datos del JSON enviado.");
            }

            if (idOrden != updateIncidenciaOrdenDTO.IdOrden)
            {
                return BadRequest("El identificador de la orden no coincide con los datos del JSON enviado.");
            }

            try
            {
                // Comprobar que la incidencia exista
                await _incidenciaDAO.GetByIdAsync(updateIncidenciaOrdenDTO.IdIncidencia);

                // Comprobar que el registro de la incidencia para la orden exista
                var incidenciaOrden = await _incidenciaOrdenDAO.GetByIdAsync(idIncidenciaConjunto);

                var updateIncidenciaOrden = _mapper.Map(updateIncidenciaOrdenDTO, incidenciaOrden);

                // Actualizar la incidencia indicada de la orden
                await _incidenciaOrdenDAO.UpdateAsync(updateIncidenciaOrden);

                var updatedIncidenciaOrden = await _incidenciaOrdenDAO.GetByIdAsync(idIncidenciaConjunto);

                _logger.LogInformation($"Registro de incidencia con ID {idIncidenciaConjunto} actualizado. Nuevo registro: {updatedIncidenciaOrden}");

                return Ok($"Incidencia actualizada correctamente.");
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex.Message);

                return StatusCode(500, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar la incidencia para la orden con ID {idOrden}: {ex.Message}");

                return StatusCode(500, $"Error al actualizar la incidencia para la orden con ID {idOrden}.");
            }
        }

        /// <summary>
        /// Actualiza la resolución de una incidencia en una orden.
        /// </summary>
        /// <param name="idOrden">Identificador único de la orden.</param>
        /// <param name="idIncidenciaConjunto">Identificador único del conjunto de incidencia cuya resolución se actualizará.</param>
        /// <param name="updateResolucionDTO">Datos de la resolución a actualizar.</param>
        /// <returns>
        /// Un <see cref="Task{ActionResult}"/> que representa el resultado de la operación:
        /// <list type="bullet">
        ///     <item><description><b>200 OK</b>: Resolución de la incidencia actualizada correctamente.</description></item>
        ///     <item><description><b>400 Bad Request</b>: El identificador de la incidencia o la orden no coincide con los datos enviados.</description></item>
        ///     <item><description><b>500 Internal Server Error</b>: Error al actualizar la resolución de la incidencia.</description></item>
        /// </list>
        /// </returns>
        [HttpPut("{idOrden}/asignar-resolucion/{idIncidenciaConjunto}")]
        public async Task<ActionResult> UpdateResolucionIncidenciaOrden(int idOrden, int idIncidenciaConjunto, UpdateResolucionIncidenciaOrdenDTO updateResolucionDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (idIncidenciaConjunto != updateResolucionDTO.Id)
            {
                return BadRequest("El identificador de conjunto de elementos de la incidencia no coincide con los datos del JSON enviado.");
            }

            if (idOrden != updateResolucionDTO.IdOrden)
            {
                return BadRequest("El identificador de la orden no coincide con los datos del JSON enviado.");
            }

            try
            {
                // Comprobar que la incidencia exista
                if (updateResolucionDTO.IdResolucion != null)
                    await _resolucionDAO.GetByIdAsync((int)updateResolucionDTO.IdResolucion);

                var incidenciaOrden = await _incidenciaOrdenDAO.GetByIdAsync(idIncidenciaConjunto);

                if (updateResolucionDTO.FechaResolucion == "")
                    updateResolucionDTO.FechaResolucion = null;

                // Validar que la fecha de detección no sea posterior a la fecha de resolución, y viceversa.
                if (updateResolucionDTO.FechaResolucion != null && DateTime.Parse(updateResolucionDTO.FechaDeteccion) > DateTime.Parse(updateResolucionDTO.FechaResolucion))
                    throw new ValidationException("La fecha de detección debe ser anterior a la fecha de resolución.");

                var updateIncidenciaOrden = _mapper.Map(updateResolucionDTO, incidenciaOrden);

                // Actualizar la incidencia indicada de la orden
                await _incidenciaOrdenDAO.UpdateAsync(updateIncidenciaOrden);

                var updatedIncidenciaOrden = await _incidenciaOrdenDAO.GetByIdAsync(idIncidenciaConjunto);

                _logger.LogInformation($"Resolución de la incidencia con ID {idIncidenciaConjunto} actualizada. Nuevo registro: {updatedIncidenciaOrden}");

                return Ok($"Resolución actualizada correctamente para la incidencia de la orden.");
            }
            catch (Exception ex) when (ex is KeyNotFoundException || ex is ValidationException)
            {
                _logger.LogError(ex.Message);

                return StatusCode(500, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar la resolución de la incidencia para la orden con ID {idOrden}: {ex.Message}");

                return StatusCode(500, $"Error al actualizar la resolución de la incidencia para la orden con ID {idOrden}.");
            }
        }

        /// <summary>
        /// Actualiza los datos de una orden específica.
        /// </summary>
        /// <param name="idOrden">Identificador único de la orden.</param>
        /// <param name="updateOrdenDTO">Datos de la orden a actualizar.</param>
        /// <returns>
        /// Un <see cref="Task{ActionResult}"/> que representa el resultado de la operación:
        /// <list type="bullet">
        ///     <item><description><b>200 OK</b>: Orden actualizada correctamente.</description></item>
        ///     <item><description><b>400 Bad Request</b>: El identificador de la orden no coincide con los datos enviados.</description></item>
        ///     <item><description><b>500 Internal Server Error</b>: Error al actualizar la orden.</description></item>
        /// </list>
        /// </returns>
        [HttpPut("{idOrden}")]
        public async Task<ActionResult<OrdenDTO>> UpdateOrden(int idOrden, UpdateOrdenDTO updateOrdenDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (idOrden != updateOrdenDTO.Id)
            {
                return BadRequest("El identificador de la orden no coincide con los datos del JSON enviado.");
            }

            try
            {
                // Validar información a actualizar a la orden
                var orden = await _validateOrdenData.IsValidOrden(updateOrdenDTO);

                bool ordenConfirmadaAntesGuardar = orden.Confirmada;
                string estadoAnteriorOrden = ordenConfirmadaAntesGuardar ? orden.EstadoOrden!.Name : "";

                var updateOrden = _mapper.Map(updateOrdenDTO, orden);

                if (!updateOrden.Confirmada && updateOrdenDTO.IdActivo == 0)
                    throw new ValidationException("Debes de indicar el identificador del Activo para el que se crea la orden.");
                else if (!updateOrden.Confirmada && updateOrdenDTO.IdActivo != 0)
                    updateOrden.Confirmada = true;

                    await _ordenesDAO.UpdateAsync(updateOrden);

                if ((!ordenConfirmadaAntesGuardar && updateOrden.EstadoOrden!.Name.Contains("Pendiente Material")) || (ordenConfirmadaAntesGuardar && !estadoAnteriorOrden.Contains("Pendiente Material") && updateOrden.EstadoOrden!.Name.Contains("Pendiente Material")))
                {
                    // Si la orden va a cambiar a estado "Pendiente Material" se deben eliminar sus usuarios actuales y cambiarlos por:
                    //      * Responsable Taller
                    //      * Responsable Materiales
                    //      * Personal Royse
                    var usuariosMateriales = await _userDAO.GetMaterialesUsersAsync();
                    List<Usuario_Orden> usuariosOrden = usuariosMateriales
                        .Select(u => new Usuario_Orden
                        {
                            IdUsuario = u.Id,
                            IdOrden = idOrden,
                            Usuario = u,
                            Orden = orden
                        })
                        .ToList();
                    if (orden.UsuariosOrden.Count() != 0) await _ordenesDAO.DeleteUsuariosOrdenAsync(orden.UsuariosOrden.Select(uo => uo.IdUsuario).ToList(), idOrden);
                    await _ordenesDAO.CreateUsuarioOrdenAsync(usuariosOrden, idOrden);
                }

                orden = await _ordenesDAO.GetByIdAsync(idOrden);

                _logger.LogInformation($"Orden con ID {idOrden} actualizada");

                return Ok(_mapper.Map<OrdenDTO>(orden));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ValidationException ex)
            {
                _logger.LogError(ex.Message);

                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar la orden con ID {idOrden}: {ex.Message}");

                return StatusCode(500, $"Error al actualizar la orden con ID {idOrden}: {ex.Message}");
            }
        }

        /// <summary>
        /// Elimina una incidencia de una orden.
        /// </summary>
        /// <param name="idOrden">Identificador único de la orden.</param>
        /// <param name="idIncidenciaConjunto">Identificador único del conjunto de incidencia a eliminar.</param>
        /// <returns>
        /// Un <see cref="Task{ActionResult}"/> que representa el resultado de la operación:
        /// <list type="bullet">
        ///     <item><description><b>200 OK</b>: Incidencia eliminada correctamente.</description></item>
        ///     <item><description><b>500 Internal Server Error</b>: Error al eliminar la incidencia.</description></item>
        /// </list>
        /// </returns>
        [HttpDelete("{idOrden}/eliminar-incidencia/{idIncidenciaConjunto}")]
        public async Task<ActionResult> DeleteIncidenciaOrden(int idOrden, int idIncidenciaConjunto)
        {
            // Comprobar que exista la orden indicada
            var orden = await _ordenesDAO.GetByIdAsync(idOrden);

            // Comprobar que exista la incidencia indicada
            var incidenciaOrden = await _incidenciaOrdenDAO.GetByIdAsync(idIncidenciaConjunto);

            try
            {
                await _incidenciaOrdenDAO.DeleteAsync(idIncidenciaConjunto);

                _logger.LogInformation($"Incidencia {incidenciaOrden.Incidencia.DescripcionES} con fecha de detección {incidenciaOrden.FechaDeteccion} eliminada (ID: {incidenciaOrden.Id})");

                return Ok($"Incidencia eliminada correctamente.");
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex.Message);

                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar la incidencia de la orden con ID {idIncidenciaConjunto}: {ex.Message}");

                return StatusCode(500, $"Error al eliminar la incidencia de la orden.");
            }
        }
    }
}
