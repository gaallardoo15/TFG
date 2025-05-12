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
    public class MecanismosDeFalloController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILogger<MecanismosDeFalloController> _logger;
        private readonly MecanismoDeFalloDAO _mecanismoDeFalloDAO;

        public MecanismosDeFalloController(IMapper mapper, ILogger<MecanismosDeFalloController> logger, MecanismoDeFalloDAO mecanismoDeFalloDAO)
        {
            _mapper = mapper;
            _logger = logger;
            _mecanismoDeFalloDAO = mecanismoDeFalloDAO;
        }

        /// <summary>
        /// Crea un mecanismo de fallo en el sistema.
        /// </summary>
        /// <param name="createMecanismoDeFalloDTO">Objeto <see cref="CreateMecanismoDeFalloDTO"/> que contiene la información del mecanismo de fallo.</param>
        /// <returns>
        /// Un <see cref="IActionResult"/> que representa el resultado de la operación:
        /// <list type="bullet">
        ///     <item><description>
        ///         <b>200 OK</b>: El mecanismo de fallo se ha creado correctamente y se devuelve el objeto creado <see cref="MecanismoDeFalloDTO"/>.
        ///     </description></item>
        ///     <item><description>
        ///         <b>500 Internal Server Error</b>: Se produjo un error interno al intentar crear el mecanismo de fallo.
        ///     </description></item>
        /// </list>
        /// </returns>
        [Authorize(Roles = "SUPER_ADMINISTRADOR, ADMINISTRADOR, JEFE_MANTENIMIENTO, RESPONSABLE_MATERIALES, RESPONSABLE_TALLER, RESPONSABLE")]
        [HttpPost]
        public async Task<ActionResult<MecanismoDeFalloDTO>> CreateMecanismoDeFallo([FromBody] CreateMecanismoDeFalloDTO createMecanismoDeFalloDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var newMecanismoDeFallo = _mapper.Map<MecanismoDeFallo>(createMecanismoDeFalloDTO);

                var createdMecanismoDeFallo = await _mecanismoDeFalloDAO.CreateAsync(newMecanismoDeFallo);
                
                _logger.LogInformation($"Nuevo mecanismo de fallo creado ({createdMecanismoDeFallo.DescripcionES} ID {createdMecanismoDeFallo.Id})");
                
                return Ok(_mapper.Map<MecanismoDeFalloDTO>(createdMecanismoDeFallo));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear el mecanismo de fallo {createMecanismoDeFalloDTO.DescripcionES}: {ex.Message}");
                return StatusCode(500, $"Error al crear el mecanismo de fallo: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtiene la información de los Mecanismos de fallo creados.
        /// </summary>
        /// <returns>
        /// Un <see cref="IActionResult"/> que representa el resultado de la operación:
        /// <list type="bullet">
        ///     <item><description>
        ///         <b>200 OK</b>: El listado de mecanismos de fallo se ha obtenido correctamente y se devuelve el objeto creado <see cref="IEnumerable{MecanismoDeFalloDTO}"/>.
        ///     </description></item>
        /// </list>
        /// </returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MecanismoDeFalloDTO>>> RetrieveMecanismosDeFallos()
        {
            var mecanismosDeFallos = await _mecanismoDeFalloDAO.GetAllAsync();
            return Ok(_mapper.Map<List<MecanismoDeFalloDTO>>(mecanismosDeFallos));
        }

        /// <summary>
        /// Actualiza la información de un Mecanismo de fallo existente.
        /// </summary>
        /// <param name="id">Identificador del mecanismo de fallo.</param>
        /// <param name="updateMecanismoDeFalloDTO">Objeto <see cref="MecanismoDeFalloDTO"/> que contiene la nueva información del mecanismo de fallo.</param>
        /// <returns>
        /// Un <see cref="IActionResult"/> que representa el resultado de la operación:
        /// <list type="bullet">
        ///     <item><description>
        ///         <b>200 OK</b>: El mecanismo de fallo se ha actualizado correctamente y se devuelve el objeto creado <see cref="MecanismoDeFalloDTO"/>.
        ///     </description></item>
        ///     <item><description>
        ///         <b>500 Internal Server Error</b>: Se produjo un error interno al intentar actualizar el mecanismo de fallo.
        ///     </description></item>
        /// </list>
        /// </returns>
        [Authorize(Roles = "SUPER_ADMINISTRADOR, ADMINISTRADOR, JEFE_MANTENIMIENTO, RESPONSABLE_MATERIALES, RESPONSABLE_TALLER, RESPONSABLE")]
        [HttpPut("{id}")]
        public async Task<ActionResult<MecanismoDeFalloDTO>> UpdateMecanismoDeFallo(int id, [FromBody] MecanismoDeFalloDTO updateMecanismoDeFalloDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != updateMecanismoDeFalloDTO.Id)
            {
                return BadRequest("El identificador del mecanismo de fallo no coincide con los datos del JSON enviado.");
            }

            try
            {
                var mecanismoDeFallo = await _mecanismoDeFalloDAO.GetByIdAsync(id);                

                _mapper.Map(updateMecanismoDeFalloDTO, mecanismoDeFallo);

                await _mecanismoDeFalloDAO.UpdateAsync(mecanismoDeFallo);
                mecanismoDeFallo = await _mecanismoDeFalloDAO.GetByIdAsync(id);
                
                _logger.LogInformation($"Actualizado el mecanismo de fallo ({mecanismoDeFallo.DescripcionES} con ID {mecanismoDeFallo.Id})");
                
                return Ok(_mapper.Map<MecanismoDeFalloDTO>(mecanismoDeFallo));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el mecanismo de fallo {updateMecanismoDeFalloDTO.DescripcionES} con ID {updateMecanismoDeFalloDTO.Id}: {ex.Message}");
                
                return StatusCode(500, $"Error al actualiza el mecanismo de fallo: {ex.Message}");
            }
        }

        /// <summary>
        /// Elimina la información de un Mecanismo de fallo existente.
        /// </summary>
        /// <param name="id">Identificador del Mecanismo de fallo.</param>
        /// <returns>
        /// Un <see cref="IActionResult"/> que representa el resultado de la operación:
        /// <list type="bullet">
        ///     <item><description>
        ///         <b>200 OK</b>: El mecanismo de fallo se ha eliminado correctamente.
        ///     </description></item>
        ///     <item><description>
        ///         <b>500 Internal Server Error</b>: Se produjo un error interno al intentar eliminar el mecanismo de fallo.
        ///     </description></item>
        /// </list>
        /// </returns>
        [Authorize(Roles = "SUPER_ADMINISTRADOR, ADMINISTRADOR, JEFE_MANTENIMIENTO, RESPONSABLE_MATERIALES, RESPONSABLE_TALLER, RESPONSABLE")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMecanismoDeFallo(int id)
        {
            var mecanismosDeFallo = await _mecanismoDeFalloDAO.GetByIdAsync(id);

            try
            {
                await _mecanismoDeFalloDAO.DeleteAsync(id);
                
                _logger.LogInformation($"Mecanismo de fallo {mecanismosDeFallo.DescripcionES} eliminado (ID: {mecanismosDeFallo.Id})");
                
                return Ok($"Mecanismo de fallo eliminado correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar el mecanismo de fallo {mecanismosDeFallo.DescripcionES} con ID: {mecanismosDeFallo.Id}: {ex.Message}");
                
                return StatusCode(500, $"Error al eliminar el mecanismo de fallo: {ex.Message}");
            }
        }
    }
}
