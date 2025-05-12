using AutoMapper;
using GSMAO.Server.Database;
using GSMAO.Server.Database.DAOs;
using GSMAO.Server.Database.Tables;
using GSMAO.Server.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace GSMAO.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [Produces("application/json")]
    public class PlantasController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILogger<PlantasController> _logger;
        private readonly PlantaDAO _plantaDAO;
        private readonly EmpresaDAO _empresaDAO;
        private readonly UsersDAO _usuarioDAO;
        
        public PlantasController(IMapper mapper, ILogger<PlantasController> logger, PlantaDAO plantaDAO, EmpresaDAO empresaDAO, UsersDAO usersDAO)
        {
            _mapper = mapper;
            _logger = logger;
            _plantaDAO = plantaDAO;
            _empresaDAO= empresaDAO;
            _usuarioDAO = usersDAO;
        }

        /// <summary>
        /// Crea una planta en el sistema.
        /// </summary>
        /// <param name="createPlantaDTO">Objeto <see cref="CreatePlantaDTO"/> que contiene la información de la planta.</param>
        /// <returns>
        /// Un <see cref="IActionResult"/> que representa el resultado de la operación:
        /// <list type="bullet">
        ///     <item><description>
        ///         <b>200 OK</b>: La planta se ha creado correctamente y se devuelve el objeto creado <see cref="PlantaDTO"/>.
        ///     </description></item>
        ///     <item><description>
        ///         <b>400 Bad Request</b>: La solicitud no es válida, se proporcionaron datos incorrectos o faltantes.
        ///     </description></item>
        ///     <item><description>
        ///         <b>500 Internal Server Error</b>: Se produjo un error interno al intentar crear la planta.
        ///     </description></item>
        /// </list>
        /// </returns>
        [HttpPost]
        [Authorize(Roles = "SUPER_ADMINISTRADOR")]
        public async Task<ActionResult<PlantaDTO>> CreatePlanta([FromBody] CreatePlantaDTO createPlantaDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Comprobar que la empresa indicada exista
            await _empresaDAO.GetByIdAsync(createPlantaDTO.idEmpresa);

            var newPlanta = _mapper.Map<Planta>(createPlantaDTO);

            try
            {
                var createdPlanta = await _plantaDAO.CreateAsync(newPlanta);
                _logger.LogInformation($"Nueva planta creada (id: {createdPlanta!.Id})");
                return Ok(_mapper.Map<PlantaDTO>(createdPlanta));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear una planta: {ex.Message}");
                return StatusCode(500, $"Error al crear una planta: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtiene la información de las Plantas creadas.
        /// </summary>
        /// <returns>
        /// Un <see cref="IActionResult"/> que representa el resultado de la operación:
        /// <list type="bullet">
        ///     <item><description>
        ///         <b>200 OK</b>: El listado de plantas se ha obtenido correctamente y se devuelve el objeto creado <see cref="IEnumerable{PlantaDTO}"/>.
        ///     </description></item>
        /// </list>
        /// </returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlantaDTO>>> RetrievePlantas()
        {
            // Solamente se consulta porque tenemos al principio de este método el Authorize para rol SUPER_ADMINISTRADOR únicamente.
            var plantas = await _plantaDAO.GetAllAsync();
            return Ok(_mapper.Map<List<PlantaDTO>>(plantas));
        }

        /// <summary>
        /// Obtiene la información de las Plantas creadas de una empresa.
        /// </summary>
        /// <param name="idEmpresa">Identificador de la empresa.</param>
        /// <returns>
        /// Un <see cref="IActionResult"/> que representa el resultado de la operación:
        /// <list type="bullet">
        ///     <item><description>
        ///         <b>200 OK</b>: El listado de plantas se ha obtenido correctamente y se devuelve el objeto creado <see cref="IEnumerable{PlantaDTO}"/>
        ///     </description></item>
        /// </list>
        /// </returns>
        /// <exception cref="KeyNotFoundException"></exception>
        [HttpGet("por-Empresa/{idEmpresa}")]
        public async Task<ActionResult<List<PlantaDTO>>> RetrievePlantasEmpresa(int idEmpresa)
        {
            // Solamente se consulta porque tenemos al principio del controlador el Authorize para los roles permitidos.
            var plantas = await _plantaDAO.GetPlantasEmpresaAsync(idEmpresa);
            return Ok(_mapper.Map<List<PlantaDTO>>(plantas));
        }

        /// <summary>
        /// Actualiza la información de una Planta existente.
        /// </summary>
        /// <param name="id">Identificador de la planta.</param>
        /// <param name="updatePlantaDTO">Objeto <see cref="UpdatePlantaDTO"/> que contiene la nueva información de la planta.</param>
        /// <returns>
        /// Un <see cref="IActionResult"/> que representa el resultado de la operación:
        /// <list type="bullet">
        ///     <item><description>
        ///         <b>200 OK</b>: La planta se ha actualizado correctamente y se devuelve el objeto creado <see cref="PlantaDTO"/>.
        ///     </description></item>
        ///     <item><description>
        ///         <b>400 Bad Request</b>: La solicitud no es válida, se proporcionaron datos incorrectos o faltantes.
        ///     </description></item>
        ///     <item><description>
        ///         <b>500 Internal Server Error</b>: Se produjo un error interno al intentar actualizar la planta.
        ///     </description></item>
        /// </list>
        /// </returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "SUPER_ADMINISTRADOR")]
        public async Task<ActionResult<PlantaDTO>> UpdatePlanta(int id, [FromBody] UpdatePlantaDTO updatePlantaDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != updatePlantaDTO.Id)
            {
                return BadRequest("El identificador de planta no coincide con los datos del JSON enviado.");
            }

            try
            {
                var planta = await _plantaDAO.GetByIdAsync(id);

                // Comprobar que la empresa indicada exista
                await _empresaDAO.GetByIdAsync(updatePlantaDTO.idEmpresa);

                _mapper.Map(updatePlantaDTO, planta);

                await _plantaDAO.UpdateAsync(planta);
                planta = await _plantaDAO.GetByIdAsync(id);
                
                _logger.LogInformation($"Planta actualizada (id: {planta.Id})");
                
                return Ok(_mapper.Map<PlantaDTO>(planta));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar la planta (ID: {id}): {ex.Message}");
                
                return StatusCode(500, $"Error al actualizar el planta.");
            }
        }

        /// <summary>
        /// Elimina la información de una Planta existente.
        /// </summary>
        /// <param name="id">Identificador de la planta.</param>
        /// <returns>
        /// Un <see cref="IActionResult"/> que representa el resultado de la operación:
        /// <list type="bullet">
        ///     <item><description>
        ///         <b>200 OK</b>: La planta se ha eliminado correctamente.
        ///     </description></item>
        ///     <item><description>
        ///         <b>500 Internal Server Error</b>: Se produjo un error interno al intentar actualizar la planta.
        ///     </description></item>
        /// </list>
        /// </returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "SUPER_ADMINISTRADOR")]
        public async Task<ActionResult> DeletePlanta(int id)
        {
            try
            {
                // Comprobar que la planta que se intenta eliminar exista.
                await _plantaDAO.GetByIdAsync(id);
            
                // Eliminar la planta
                await _plantaDAO.DeleteAsync(id);
                
                _logger.LogInformation($"Planta eliminada (id: {id})");
                
                return Ok($"Planta con id {id} eliminada correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar la planta (ID: {id}): {ex.Message}");
                
                return StatusCode(500, $"Error al eliminar la planta.");
            }
        }
       
    }
}
