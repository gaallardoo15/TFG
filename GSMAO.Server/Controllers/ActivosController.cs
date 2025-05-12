using AutoMapper;
using GSMAO.Server.Database.DAOs;
using GSMAO.Server.Database.Tables;
using GSMAO.Server.DTOs;
using GSMAO.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Web;

namespace GSMAO.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [Produces("application/json")]
    public class ActivosController : BaseFileController
    {
        private readonly IMapper _mapper;
        private new readonly ILogger<ActivosController> _logger;
        private readonly ActivoDAO _activoDAO;

        public ActivosController(
            IFileService fileService,
            IMapper mapper,
            ILogger<ActivosController> logger,
            ActivoDAO activoDAO
        ) : base (fileService, logger)
        {
            _mapper = mapper;
            _logger = logger;
            _activoDAO = activoDAO;
        }

        /// <summary>
        /// Valor concreto de la carpeta raíz para Órdenes.
        /// </summary>
        protected override string RootFolderName => "Activos";

        /// <summary>
        /// Valida la existencia de la Orden en BD (ejemplo).
        /// </summary>
        protected override async Task ValidateEntityExistsAsync(int id)
        {
            // Aquí se valida en la BD que la Orden exista:
            var activo = await _activoDAO.GetByIdAsync(id);

            if (activo.EstadoActivo.Name == "Borrado")
                throw new KeyNotFoundException($"El activo {id} no se encuentra registrado en el sistema.");
        }

        /// <summary>
        /// Crea un activo en el sistema.
        /// </summary>
        /// <param name="createActivoDTO">Objeto <see cref="CreateActivoDTO"/> que contiene la información del activo.</param>
        /// <returns>
        /// Un <see cref="IActionResult"/> que representa el resultado de la operación:
        /// <list type="bullet">
        ///     <item><description>
        ///         <b>200 OK</b>: El activo se ha creado correctamente y se devuelve el objeto creado <see cref="ActivoDTO"/>.
        ///     </description></item>
        ///     <item><description>
        ///         <b>500 Internal Server Error</b>: Se produjo un error interno al intentar crear el activo.
        ///     </description></item>
        /// </list>
        /// </returns>
        [Authorize(Roles = "SUPER_ADMINISTRADOR, ADMINISTRADOR, JEFE_MANTENIMIENTO")]
        [HttpPost]
        public async Task<ActionResult<ActivoDTO>> CreateActivo([FromBody] CreateActivoDTO createActivoDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var newActivo = _mapper.Map<Activo>(createActivoDTO);

                var createdActivo = await _activoDAO.CreateAsync(newActivo);

                _logger.LogInformation($"Nuevo activo creado ({createdActivo.DescripcionES} con ID {createdActivo.Id})");

                return Ok(_mapper.Map<ActivoDTO>(createdActivo));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear el activo {createActivoDTO.DescripcionES}: {ex.Message}");
                
                return StatusCode(500, $"Error al crear el activo: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtiene la información de los Activos creados.
        /// </summary>
        /// <returns>
        /// Un <see cref="IActionResult"/> que representa el resultado de la operación:
        /// <list type="bullet">
        ///     <item><description>
        ///         <b>200 OK</b>: El listado de activos se ha obtenido correctamente y se devuelve el objeto creado <see cref="IEnumerable{ActivoTableDTO}"/>.
        ///     </description></item>
        /// </list>
        /// </returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ActivoTableDTO>>> RetrieveActivos()
        {
            var activos = await _activoDAO.GetAllAsync();
            return Ok(_mapper.Map<List<ActivoTableDTO>>(activos));
        }

        /// <summary>
        /// Actualiza la información de un Activo existente.
        /// </summary>
        /// <param name="id">Identificador del activo.</param>
        /// <param name="updateActivoDTO">Objeto <see cref="UpdateActivoDTO"/> que contiene la nueva información del activo.</param>
        /// <returns>
        /// Un <see cref="IActionResult"/> que representa el resultado de la operación:
        /// <list type="bullet">
        ///     <item><description>
        ///         <b>200 OK</b>: El activo se ha actualizado correctamente y se devuelve el objeto creado <see cref="ActivoDTO"/>.
        ///     </description></item>
        ///     <item><description>
        ///         <b>400 Bad Request</b>:  La solicitud no es válida, se proporcionaron datos incorrectos o faltantes.
        ///     </description></item>
        ///     <item><description>
        ///         <b>404 Not Found</b>: No se encuentra la información actual del activo que se intenta actualizar.
        ///     </description></item>
        ///     <item><description>
        ///         <b>500 Internal Server Error</b>: Se produjo un error interno al intentar actualizar el activo.
        ///     </description></item>
        /// </list>
        /// </returns>
        [Authorize(Roles = "SUPER_ADMINISTRADOR, ADMINISTRADOR, JEFE_MANTENIMIENTO")]
        [HttpPut("{id}")]
        public async Task<ActionResult<ActivoTableDTO>> UpdateActivo(int id, [FromBody] UpdateActivoDTO updateActivoDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != updateActivoDTO.Id)
            {
                return BadRequest("El identificador del activo no coincide con los datos del JSON enviado.");
            }

            try
            {
                // Comprobar que exista el activo indicado
                var activo = await _activoDAO.GetByIdAsync(id);

                if (activo.EstadoActivo.Name == "Borrado")
                    throw new KeyNotFoundException($"El activo {id} no se encuentra registrado en el sistema.");

                _mapper.Map(updateActivoDTO, activo);

                await _activoDAO.UpdateAsync(activo);
                activo = await _activoDAO.GetByIdAsync(id);

                _logger.LogInformation($"Actualizado el activo ({activo.DescripcionES} con ID {activo.Id})");

                return Ok(_mapper.Map<ActivoDTO>(activo));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el activo {updateActivoDTO.DescripcionES} con ID {updateActivoDTO.Id}: {ex.Message}");

                return StatusCode(500, $"Error al actualiza el activo: {ex.Message}");
            }
        }

        /// <summary>
        /// Elimina la información de un Activo existente.
        /// </summary>
        /// <param name="id">Identificador del Activo.</param>
        /// <returns>
        /// Un <see cref="IActionResult"/> que representa el resultado de la operación:
        /// <list type="bullet">
        ///     <item><description>
        ///         <b>200 OK</b>: El activo se ha eliminado correctamente.
        ///     </description></item>
        ///     <item><description>
        ///         <b>401 Unauthorized</b>: No se puede eliminar el activo por tener permisos insuficientes.
        ///     </description></item>
        ///     <item><description>
        ///         <b>404 Not Found</b>: No se encuentra la información actual del activo que se intenta eliminar.
        ///     </description></item>
        ///     <item><description>
        ///         <b>500 Internal Server Error</b>: Se produjo un error interno al intentar eliminar el activo.
        ///     </description></item>
        /// </list>
        /// </returns>
        [Authorize(Roles = "SUPER_ADMINISTRADOR, ADMINISTRADOR, JEFE_MANTENIMIENTO")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteActivo(int id)
        {
            // Comprobar que exista el activo indicado
            var activo = await _activoDAO.GetByIdAsync(id);

            try
            {
                if (activo.EstadoActivo.Name == "Borrado")
                    throw new KeyNotFoundException($"El activo {id} no se encuentra registrado en el sistema.");

                await _activoDAO.ChangeStateAsync(id, "Borrado");

                _logger.LogInformation($"Activo {activo.DescripcionES} eliminado (ID: {activo.Id})");

                return Ok($"Activo eliminado correctamente.");
            }
            catch (Exception ex) when (ex is KeyNotFoundException || ex is InvalidOperationException)
            {
                _logger.LogInformation($"{ex.Message}");

                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar el activo {activo.DescripcionES} con ID: {activo.Id} o su documentación: {ex.Message}");

                return StatusCode(500, $"Error al eliminar el activo {activo.DescripcionES} con ID: {activo.Id} o su documentación: {ex.Message}");
            }
        }

        /// <summary>
        /// Cambia el estado de un Activo existente.
        /// </summary>
        /// <param name="id">Identificador del activo.</param>
        /// <param name="state">Cadena de identificación del estado que se le desea asignar al activo.</param>
        /// <returns>
        /// Un <see cref="IActionResult"/> que representa el resultado de la operación:
        /// <list type="bullet">
        ///     <item><description>
        ///         <b>200 OK</b>: El estado del activo se ha actualizado correctamente.
        ///     </description></item>
        ///     <item><description>
        ///         <b>400 Bad Request</b>: La solicitud no es válida, se proporcionaron datos incorrectos o faltantes.
        ///     </description></item>
        ///     <item><description>
        ///         <b>404 Not Found</b>: No se encuentra la información actual del activo que se intenta actualizar el estado.
        ///     </description></item>
        ///     <item><description>
        ///         <b>500 Internal Server Error</b>: Se produjo un error interno al intentar actualizar el estado.
        ///     </description></item>
        /// </list>
        /// </returns>
        [Authorize(Roles = "SUPER_ADMINISTRADOR, ADMINISTRADOR, JEFE_MANTENIMIENTO")]
        [HttpPut("{id}/change-state/{state}")]
        public async Task<ActionResult> ChangeState(int id, string state)
        {
            try
            {
                if (state.ToLower() == "borrado")
                    throw new ArgumentException("No se puede borrar un activo, utiliza el endpoint establecido para este fin.");

                await _activoDAO.ChangeStateAsync(id, state);
                return Ok();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error inesperado al cambiar el estado del activo: {ex.Message}");
                
                return StatusCode(500, "Error inesperado al cambiar el estado del activo.");
            }
        }
    }
}
