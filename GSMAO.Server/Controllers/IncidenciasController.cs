using AutoMapper;
using GSMAO.Server.Database.DAOs;
using GSMAO.Server.Database.Tables;
using GSMAO.Server.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GSMAO.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [Produces("application/json")]
    public class IncidenciasController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILogger<IncidenciasController> _logger;
        private readonly IncidenciaDAO _incidenciaDAO;
        private readonly MecanismoDeFalloDAO _mecanismoDeFalloDAO;
        private readonly UsersDAO _usuarioDAO;

        public IncidenciasController(IMapper mapper, ILogger<IncidenciasController> logger, IncidenciaDAO incidenciaDAO, MecanismoDeFalloDAO mecanismoDeFalloDAO, UsersDAO usersDAO)
        {
            _mapper = mapper;
            _logger = logger;
            _incidenciaDAO = incidenciaDAO;
            _mecanismoDeFalloDAO = mecanismoDeFalloDAO;
            _usuarioDAO = usersDAO;
        }

        /// <summary>
        /// Crea una incidencia en el sistema.
        /// </summary>
        /// <param name="createIncidenciaDTO">Objeto <see cref="CreateIncidenciaDTO"/> que contiene la información de la incidencia.</param>
        /// <returns>
        /// Un <see cref="IActionResult"/> que representa el resultado de la operación:
        /// <list type="bullet">
        ///     <item><description>
        ///         <b>200 OK</b>: La incidencia se ha creado correctamente y se devuelve el objeto creado <see cref="IncidenciaDTO"/>.
        ///     </description></item>
        ///     <item><description>
        ///         <b>500 Internal Server Error</b>: Se produjo un error interno al intentar crear la incidencia.
        ///     </description></item>
        /// </list>
        /// </returns>
        [Authorize(Roles = "SUPER_ADMINISTRADOR, ADMINISTRADOR, JEFE_MANTENIMIENTO, RESPONSABLE_MATERIALES, RESPONSABLE_TALLER, RESPONSABLE")]
        [HttpPost]
        public async Task<ActionResult<IncidenciaDTO>> CreateIncidencia([FromBody] CreateIncidenciaDTO createIncidenciaDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Comprobar que el mecanismo de fallo indicado exista
            await _mecanismoDeFalloDAO.GetByIdAsync(createIncidenciaDTO.IdMecanismoFallo);

            try
            {
                var newIncidencia = _mapper.Map<Incidencia>(createIncidenciaDTO);

                var createdIncidencia = await _incidenciaDAO.CreateAsync(newIncidencia);
                
                _logger.LogInformation($"Nueva incidencia creada ({createdIncidencia.DescripcionES} con ID {createdIncidencia.Id})");
                
                return Ok(_mapper.Map<IncidenciaDTO>(createdIncidencia));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear la incidencia {createIncidenciaDTO.DescripcionES}: {ex.Message}");
                
                return StatusCode(500, $"Error al crear la incidencia: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtiene la información de las Incidencias creadas.
        /// </summary>
        /// <returns>
        /// Un <see cref="IActionResult"/> que representa el resultado de la operación:
        /// <list type="bullet">
        ///     <item><description>
        ///         <b>200 OK</b>: El listado de incidencias se ha obtenido correctamente y se devuelve el objeto creado <see cref="IEnumerable{IncidenciaDTO}"/>.
        ///     </description></item>
        /// </list>
        /// </returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<IncidenciaDTO>>> RetrieveIncidencias()
        {
            var incidencias = await _incidenciaDAO.GetAllAsync();
            return Ok(_mapper.Map<List<IncidenciaDTO>>(incidencias));
        }

        /// <summary>
        /// Actualiza la información de una Incidencia existente.
        /// </summary>
        /// <param name="id">Identificador de la incidencia.</param>
        /// <param name="updateIncidenciaDTO">Objeto <see cref="IncidenciaDTO"/> que contiene la nueva información de la incidencia.</param>
        /// <returns>
        /// Un <see cref="IActionResult"/> que representa el resultado de la operación:
        /// <list type="bullet">
        ///     <item><description>
        ///         <b>200 OK</b>: La incidencia se ha actualizado correctamente y se devuelve el objeto creado <see cref="IncidenciaDTO"/>.
        ///     </description></item>
        ///     <item><description>
        ///         <b>500 Internal Server Error</b>: Se produjo un error interno al intentar actualizar la incidencia.
        ///     </description></item>
        /// </list>
        /// </returns>
        [Authorize(Roles = "SUPER_ADMINISTRADOR, ADMINISTRADOR, JEFE_MANTENIMIENTO, RESPONSABLE_MATERIALES, RESPONSABLE_TALLER, RESPONSABLE")]
        [HttpPut("{id}")]
        public async Task<ActionResult<IncidenciaDTO>> UpdateIncidencia(int id, [FromBody] UpdateIncidenciaDTO updateIncidenciaDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != updateIncidenciaDTO.Id)
            {
                return BadRequest("El identificador de incidencia no coincide con los datos del JSON enviado.");
            }

            try
            {
                // Comprobar que la incidencia que se quiere actualizar exista en el sistema.
                var incidencia = await _incidenciaDAO.GetByIdAsync(id);

                // Comprobar que el mecanismo de fallo indicado exista en el sistema.
                await _mecanismoDeFalloDAO.GetByIdAsync(updateIncidenciaDTO.IdMecanismoFallo);

                _mapper.Map(updateIncidenciaDTO, incidencia);

                await _incidenciaDAO.UpdateAsync(incidencia);
                incidencia = await _incidenciaDAO.GetByIdAsync(id);
                
                _logger.LogInformation($"Actualizada la incidencia ({incidencia.DescripcionES} con ID {incidencia.Id})");
                
                return Ok(_mapper.Map<IncidenciaDTO>(incidencia));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar la incidencia {updateIncidenciaDTO.DescripcionES}: {ex.Message}");
                
                return StatusCode(500, $"Error al actualizar la incidencia: {ex.Message}");
            }
        }

        /// <summary>
        /// Elimina la información de una Incidencia existente.
        /// </summary>
        /// <param name="id">Identificador de la Incidencia.</param>
        /// <returns>
        /// Un <see cref="IActionResult"/> que representa el resultado de la operación:
        /// <list type="bullet">
        ///     <item><description>
        ///         <b>200 OK</b>: La incidencia se ha eliminado correctamente.
        ///     </description></item>
        ///     <item><description>
        ///         <b>500 Internal Server Error</b>: Se produjo un error interno al intentar eliminar la incidencia.
        ///     </description></item>
        /// </list>
        /// </returns>
        [Authorize(Roles = "SUPER_ADMINISTRADOR, ADMINISTRADOR, JEFE_MANTENIMIENTO, RESPONSABLE_MATERIALES, RESPONSABLE_TALLER, RESPONSABLE")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteIncidencia(int id)
        {
            var incidencia = await _incidenciaDAO.GetByIdAsync(id);

            try
            {
                await _incidenciaDAO.DeleteAsync(id);
                
                _logger.LogInformation($"Incidencia {incidencia.DescripcionES} eliminada (ID: {incidencia.Id})");
                
                return Ok($"Incidencia eliminada correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar la incidencia {incidencia.DescripcionES} con ID {incidencia.Id}: {ex.Message}");
                
                return StatusCode(500, $"Error al eliminar la incidencia: {ex.Message}");
            }
        }

    }
}
