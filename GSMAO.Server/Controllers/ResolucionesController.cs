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
    public class ResolucionesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILogger<ResolucionesController> _logger;
        private readonly ResolucionDAO _resolucionDAO;

        public ResolucionesController(IMapper mapper,
            ILogger<ResolucionesController> logger, ResolucionDAO resolucionDAO)
        {
            _mapper = mapper;
            _logger = logger;
            _resolucionDAO = resolucionDAO;
        }

        /// <summary>
        /// Crea una resolución en el sistema.
        /// </summary>
        /// <param name="createResolucionDTO">Objeto <see cref="CreateResolucionDTO"/> que contiene la información de la resolución.</param>
        /// <returns>
        /// Un <see cref="IActionResult"/> que representa el resultado de la operación:
        /// <list type="bullet">
        ///     <item><description>
        ///         <b>200 OK</b>: La resolución se ha creado correctamente y se devuelve el objeto creado <see cref="ResolucionDTO"/>.
        ///     </description></item>
        ///     <item><description>
        ///         <b>500 Internal Server Error</b>: Se produjo un error interno al intentar crear la resolución.
        ///     </description></item>
        /// </list>
        /// </returns>
        [Authorize(Roles = "SUPER_ADMINISTRADOR, ADMINISTRADOR, JEFE_MANTENIMIENTO, RESPONSABLE_MATERIALES, RESPONSABLE_TALLER, RESPONSABLE")]
        [HttpPost]
        public async Task<ActionResult<ResolucionDTO>> CreateResolucion([FromBody] CreateResolucionDTO createResolucionDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var newResolucion = _mapper.Map<Resolucion>(createResolucionDTO);

                var createdCreateResolucion = await _resolucionDAO.CreateAsync(newResolucion);
                
                _logger.LogInformation($"Nueva resolución creada ({createdCreateResolucion.DescripcionES} con ID {createdCreateResolucion.Id})");
                
                return Ok(_mapper.Map<ResolucionDTO>(createdCreateResolucion));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear la resolución {createResolucionDTO.DescripcionES}: {ex.Message}");
                
                return StatusCode(500, $"Error al crear la resolución: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtiene la información de las Resoluciones creadas.
        /// </summary>
        /// <returns>
        /// Un <see cref="IActionResult"/> que representa el resultado de la operación:
        /// <list type="bullet">
        ///     <item><description>
        ///         <b>200 OK</b>: El listado de resoluciones se ha obtenido correctamente y se devuelve el objeto creado <see cref="IEnumerable{ResolucionDTO}"/>.
        ///     </description></item>
        /// </list>
        /// </returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResolucionDTO>>> RetrieveResoluciones()
        {
            var resoluciones = await _resolucionDAO.GetAllAsync();
            return Ok(_mapper.Map<List<ResolucionDTO>>(resoluciones));
        }

        /// <summary>
        /// Actualiza la información de una Resolución existente.
        /// </summary>
        /// <param name="id">Identificador de la resolución.</param>
        /// <param name="updateResolucionDTO">Objeto <see cref="ResolucionDTO"/> que contiene la nueva información de la resolución.</param>
        /// <returns>
        /// Un <see cref="IActionResult"/> que representa el resultado de la operación:
        /// <list type="bullet">
        ///     <item><description>
        ///         <b>200 OK</b>: La resolución se ha actualizado correctamente y se devuelve el objeto creado <see cref="ResolucionDTO"/>.
        ///     </description></item>
        ///     <item><description>
        ///         <b>500 Internal Server Error</b>: Se produjo un error interno al intentar actualizar la resolución.
        ///     </description></item>
        /// </list>
        /// </returns>
        [Authorize(Roles = "SUPER_ADMINISTRADOR, ADMINISTRADOR, JEFE_MANTENIMIENTO, RESPONSABLE_MATERIALES, RESPONSABLE_TALLER, RESPONSABLE")]
        [HttpPut("{id}")]
        public async Task<ActionResult<ResolucionDTO>> UpdateResolucion(int id, [FromBody] ResolucionDTO updateResolucionDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != updateResolucionDTO.Id)
            {
                return BadRequest("El identificador de resolucion no coincide con los datos del JSON enviado.");
            }

            try
            {
                var resolucion = await _resolucionDAO.GetByIdAsync(id);

                _mapper.Map(updateResolucionDTO, resolucion);

                await _resolucionDAO.UpdateAsync(resolucion);
                resolucion = await _resolucionDAO.GetByIdAsync(id);

                _logger.LogInformation($"Actualizada la resolución ({resolucion.DescripcionES} con ID {resolucion.Id})");

                return Ok(_mapper.Map<ResolucionDTO>(resolucion));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar la resolucion {updateResolucionDTO.DescripcionES} con ID {updateResolucionDTO.Id}: {ex.Message}");
                
                return StatusCode(500, $"Error al actualizar la resolucion: {ex.Message}");
            }
        }

        /// <summary>
        /// Elimina la información de una Resolución existente.
        /// </summary>
        /// <param name="id">Identificador de la Resolución.</param>
        /// <returns>
        /// Un <see cref="IActionResult"/> que representa el resultado de la operación:
        /// <list type="bullet">
        ///     <item><description>
        ///         <b>200 OK</b>: La resolución se ha eliminado correctamente.
        ///     </description></item>
        ///     <item><description>
        ///         <b>500 Internal Server Error</b>: Se produjo un error interno al intentar eliminar la resolución.
        ///     </description></item>
        /// </list>
        /// </returns>
        [Authorize(Roles = "SUPER_ADMINISTRADOR, ADMINISTRADOR, JEFE_MANTENIMIENTO, RESPONSABLE_MATERIALES, RESPONSABLE_TALLER, RESPONSABLE")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteResolucion(int id)
        {
            var resolucion = await _resolucionDAO.GetByIdAsync(id);

            try
            {
                await _resolucionDAO.DeleteAsync(id);
                
                _logger.LogInformation($"Resolución {resolucion.DescripcionES} eliminada (ID: {resolucion.Id})");
                
                return Ok($"Resolución eliminada correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar la resolución {resolucion.DescripcionES} con ID {resolucion.Id}: {ex.Message}");
                
                return StatusCode(500, $"Error al eliminar la resolución: {ex.Message}");
            }
        }
    }
}
