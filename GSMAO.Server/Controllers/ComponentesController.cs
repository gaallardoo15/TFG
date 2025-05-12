using AutoMapper;
using GSMAO.Server.Database.DAOs;
using GSMAO.Server.Database.Tables;
using GSMAO.Server.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Globalization;
using System.Text.RegularExpressions;

namespace GSMAO.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [Produces("application/json")]
    public class ComponentesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILogger<ComponentesController> _logger;
        private readonly ActivoDAO _activoDAO;
        private readonly ComponenteDAO _componenteDAO;
        private readonly ActivoComponenteDAO _activoComponenteDAO;

        public ComponentesController(IMapper mapper, ILogger<ComponentesController> logger, ActivoDAO activoDAO, ComponenteDAO componenteDAO, ActivoComponenteDAO activoComponenteDAO)
        {
            _mapper = mapper;
            _logger = logger;
            _activoDAO = activoDAO;
            _componenteDAO = componenteDAO;
            _activoComponenteDAO = activoComponenteDAO;
        }

        /// <summary>
        /// Crea un Componente en el sistema.
        /// </summary>
        /// <param name="createComponenteDTO">Objeto <see cref="CreateComponenteDTO"/> que contiene la información del componente.</param>
        /// <returns>
        /// Un <see cref="IActionResult"/> que representa el resultado de la operación:
        /// <list type="bullet">
        ///     <item><description>
        ///         <b>200 OK</b>: El componente se ha creado correctamente y se devuelve el objeto creado <see cref="ComponenteDTO"/>.
        ///     </description></item>
        ///     <item><description>
        ///         <b>400 Bad Request</b>: La solicitud no es válida, se proporcionaron datos incorrectos o faltantes.
        ///     </description></item>
        ///     <item><description>
        ///         <b>404 Not Found</b>: El activo se encuentra en estado borrado o no existe.
        ///     </description></item>
        ///     <item><description>
        ///         <b>500 Internal Server Error</b>: Se produjo un error interno al intentar crear el componente.
        ///     </description></item>
        /// </list>
        /// </returns>
        [Authorize(Roles = "SUPER_ADMINISTRADOR, ADMINISTRADOR, JEFE_MANTENIMIENTO")]
        [HttpPost]
        public async Task<ActionResult<ComponenteDTO>> CreateComponente([FromBody] CreateComponenteDTO createComponenteDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Comprobamos que el activo
                var activo = await _activoDAO.GetByIdAsync(createComponenteDTO.IdActivo);

                createComponenteDTO.IdComponentePadre = createComponenteDTO.IdComponentePadre == 0 ?
                                                        createComponenteDTO.IdComponentePadre = null :
                                                        createComponenteDTO.IdComponentePadre;

                Componente? componentePadre = null;
                // Comprobamos que el componente padre exista, si se indica
                if (createComponenteDTO.IdComponentePadre != null)
                    componentePadre = await _componenteDAO.GetByIdAsync((int)createComponenteDTO.IdComponentePadre);

                if (createComponenteDTO.Denominacion == null || createComponenteDTO.Denominacion == "")
                {
                    TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
                    var descripcion = textInfo.ToTitleCase(createComponenteDTO.DescripcionES.ToLower()).Replace(" ", "");


                    createComponenteDTO.Denominacion = createComponenteDTO.IdComponentePadre == null ?
                                                        "/" + createComponenteDTO.IdActivo + "/" + descripcion :
                                                        componentePadre!.Denominacion + "/" + descripcion;
                }
                else
                {
                    // Expresión regular para buscar el ID del activo entre "/"
                    string pattern = $@"/{createComponenteDTO.IdActivo}/";
                    bool contieneIdActivo = Regex.IsMatch(createComponenteDTO.Denominacion, pattern);

                    if (!contieneIdActivo)
                    {
                        _logger.LogError($"El KKS debe contener entre \"/\" el identificador del activo.");

                        throw new ArgumentException($"El KKS debe contener entre \"/\" el identificador del activo.");
                    }
                }

                // Crear el nuevo componente
                var newComponente = _mapper.Map<Componente>(createComponenteDTO);

                var createdComponente = await _componenteDAO.CreateAsync(newComponente);
                _logger.LogInformation($"Nuevo componente creado ({createdComponente.Denominacion} con ID {createdComponente.Id})");

                // Relacionar el componente con el activo, si es de primer nivel (no tiene componente padre)
                if (createComponenteDTO.IdComponentePadre == null)
                {
                    var newActivoComponente = new Activo_Componente()
                    {
                        IdActivo = createComponenteDTO.IdActivo,
                        IdComponente = createdComponente.Id,
                        Activo = activo,
                        Componente = createdComponente
                    };

                    var createdActivoComponente = await _activoComponenteDAO.CreateAsync(newActivoComponente);
                    _logger.LogInformation($"Nuevo activo_componente creado (Activo {createdActivoComponente.Activo.Id} relacionado con nuevo componente {createdActivoComponente.Componente.Denominacion} con ID {createdActivoComponente.Componente.Id})");
                }

                return Ok(_mapper.Map<ComponenteDTO>(createdComponente));
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogInformation($"{ex.Message}");

                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                _logger.LogInformation($"{ex.Message}");

                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear el componente {createComponenteDTO.DescripcionES}: {ex.Message}");

                return StatusCode(500, $"Error al crear el componente: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtiene la información de los componentes según su padre, ya sea el activo directamente o cualquier otro componente.
        /// </summary>
        /// <param name="idActivo">Identificador del activo.</param>
        /// <param name="idComponente">Identificador del componente padre.</param>
        /// <returns>
        /// Un <see cref="IActionResult"/> que representa el resultado de la operación:
        /// <list type="bullet">
        ///     <item><description>
        ///         <b>200 OK</b>: El listado de componentes se ha obtenido correctamente y se devuelve el objeto creado <see cref="IEnumerable{ComponenteTableDTO}"/>.
        ///     </description></item>
        ///     <item><description>
        ///         <b>404 Not Found</b>: El activo se encuentra en estado borrado o el componente indicado no existe.
        ///     </description></item>
        ///     <item><description>
        ///         <b>500 Internal Server Error</b>: Se produjo un error interno al obtener los componentes.
        ///     </description></item>
        /// </list>
        /// </returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ComponenteTableDTO>>> RetrieveComponentes([FromQuery] int idActivo, [FromQuery] int idComponente)
        {
            try
            {
                // Comprobar que exista el activo indicado
                var activo = await _activoDAO.GetByIdAsync(idActivo);

                if (activo.EstadoActivo.Name == "Borrado")
                    throw new KeyNotFoundException($"El activo {idActivo} no se encuentra registrado en el sistema.");

                var componentesDTO = new List<ComponenteTableDTO>();
                if (idComponente == 0)
                {
                    // Obtener el primer nivel de componentes del activo
                    var componentes = await _componenteDAO.GetByIdActivoAsync(idActivo);
                    componentesDTO = _mapper.Map<List<ComponenteTableDTO>>(componentes);
                }
                else
                {
                    // Comprobar que exista el componente del que queremos obtener sus hijos
                    await _componenteDAO.GetByIdAsync(idComponente);

                    // Obtener los hijos del componente indicado
                    var componentes = await _componenteDAO.GetByIdComponenteAsync(idComponente);
                    componentesDTO = _mapper.Map<List<ComponenteTableDTO>>(componentes);
                }

                return Ok(componentesDTO);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex.Message);

                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error inesperado al obtener los componentes: {ex.Message}");

                return StatusCode(500, "Error inesperado al obtener los componentes.");
            }
        }

        /// <summary>
        /// Obtiene la información de un Componente.
        /// </summary>
        /// <param name="id">Identificador del componente.</param>
        /// <returns>
        /// Un <see cref="IActionResult"/> que representa el resultado de la operación:
        /// <list type="bullet">
        ///     <item><description>
        ///         <b>200 OK</b>: La información del componente se ha obtenido correctamente y se devuelve el objeto creado <see cref="ComponenteDTO"/>.
        ///     </description></item>
        ///     <item><description>
        ///         <b>404 Not Found</b>: El componente indicado no existe.
        ///     </description></item>
        ///     <item><description>
        ///         <b>500 Internal Server Error</b>: Se produjo un error interno al obtener el componente.
        ///     </description></item>
        /// </list>
        /// </returns>
        [Authorize(Roles = "SUPER_ADMINISTRADOR, ADMINISTRADOR, JEFE_MANTENIMIENTO")]
        [HttpGet("{id}")]
        public async Task<ActionResult<ComponenteDTO>> RetrieveComponente(int id)
        {
            try
            {
                var componente = await _componenteDAO.GetByIdAsync(id);

                var componenteDTO = _mapper.Map<ComponenteDTO>(componente);

                var idActivo = int.Parse(componente.Denominacion.Split("/")[1]);
                var activo = await _activoDAO.GetByIdAsync(idActivo);

                componenteDTO.Activo = _mapper.Map<ActivoMinDTO>(activo);

                return Ok(componenteDTO);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex.Message);

                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error inesperado al obtener la información del componente: {ex.Message}");

                return StatusCode(500, $"Error inesperado al obtener la información del componente: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtiene la información de los componentes de todos los niveles de un activo.
        /// </summary>
        /// <param name="id">Identificador del activo.</param>
        /// <returns>
        /// Un <see cref="IActionResult"/> que representa el resultado de la operación:
        /// <list type="bullet">
        ///     <item><description>
        ///         <b>200 OK</b>: El listado de componentes se ha obtenido correctamente y se devuelve el objeto creado <see cref="IEnumerable{ComponenteTableDTO}"/>.
        ///     </description></item>
        ///     <item><description>
        ///         <b>404 Not Found</b>: El activo se encuentra en estado borrado o no existe.
        ///     </description></item>
        ///     <item><description>
        ///         <b>500 Internal Server Error</b>: Se produjo un error interno al obtener los componentes.
        ///     </description></item>
        /// </list>
        /// </returns>
        [Authorize(Roles = "SUPER_ADMINISTRADOR, ADMINISTRADOR, JEFE_MANTENIMIENTO")]
        [HttpGet("activo/{id}")]
        public async Task<ActionResult<IEnumerable<ComponenteTableDTO>>> RetrieveComponentesActivo(int id)
        {
            try
            {
                // Comprobar que exista el activo indicado
                var activo = await _activoDAO.GetByIdAsync(id);

                if (activo.EstadoActivo.Name == "Borrado")
                    throw new KeyNotFoundException($"El activo {id} no se encuentra registrado en el sistema.");

                // Obtener los componentes del activo
                var componentes = await _componenteDAO.GetAllComponentesActivoAsync(id);

                return Ok(_mapper.Map<List<ComponenteTableDTO>>(componentes));
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex.Message);

                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error inesperado al obtener la información de los componentes del activo: {ex.Message}");

                return StatusCode(500, $"Error inesperado al obtener la información de los componentes del activo: {ex.Message}");
            }
        }

        /// <summary>
        /// Actualiza un Componente existente.
        /// </summary>
        /// <param name="id">Identificador del componente.</param>
        /// <param name="updateComponenteDTO">Objeto <see cref="UpdateComponenteDTO"/> que contiene la información del componente.</param>
        /// <returns>
        /// Un <see cref="IActionResult"/> que representa el resultado de la operación:
        /// <list type="bullet">
        ///     <item><description>
        ///         <b>200 OK</b>: El componente se ha actualizado correctamente y se devuelve el objeto actualizado <see cref="ComponenteDTO"/>.
        ///     </description></item>
        ///     <item><description>
        ///         <b>400 Bad Request</b>: La solicitud no es válida, se proporcionaron datos incorrectos o faltantes.
        ///     </description></item>
        ///     <item><description>
        ///         <b>404 Not Found</b>: El activo se encuentra en estado borrado o el componente no existe.
        ///     </description></item>
        ///     <item><description>
        ///         <b>500 Internal Server Error</b>: Se produjo un error interno al intentar actualizar el componente.
        ///     </description></item>
        /// </list>
        /// </returns>
        [Authorize(Roles = "SUPER_ADMINISTRADOR, ADMINISTRADOR, JEFE_MANTENIMIENTO")]
        [HttpPut("{id}")]
        public async Task<ActionResult<ComponenteDTO>> UpdateComponente(int id, [FromBody] UpdateComponenteDTO updateComponenteDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != updateComponenteDTO.Id)
            {
                return BadRequest("El identificador del componente no coincide con los datos del JSON enviado.");
            }

            try
            {
                // Comprobar que exista el componente indicado
                var componente = await _componenteDAO.GetByIdAsync(id);

                if (updateComponenteDTO.Denominacion == null || updateComponenteDTO.Denominacion == "")
                {
                    TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
                    var descripcion = textInfo.ToTitleCase(updateComponenteDTO.DescripcionES.ToLower()).Replace(" ", "");


                    updateComponenteDTO.Denominacion = componente.IdComponentePadre == null ?
                                                        "/" + updateComponenteDTO.IdActivo + "/" + descripcion :
                                                        updateComponenteDTO!.Denominacion + "/" + descripcion;
                }
                else
                {
                    // Expresión regular para buscar el ID del activo entre "/"
                    string pattern = $@"/{updateComponenteDTO.IdActivo}/";
                    bool contieneIdActivo = Regex.IsMatch(updateComponenteDTO.Denominacion, pattern);

                    if (!contieneIdActivo)
                    {
                        _logger.LogError($"El KKS debe contener entre \"/\" el identificador del activo.");

                        throw new ArgumentException($"El KKS debe contener entre \"/\" el identificador del activo.");
                    }
                }

                _mapper.Map(updateComponenteDTO, componente);

                await _componenteDAO.UpdateAsync(componente);
                componente = await _componenteDAO.GetByIdAsync(id);

                var idActivo = int.Parse(componente.Denominacion.Split("/")[1]);
                var activo = await _activoDAO.GetByIdAsync(idActivo);
                
                var componenteDTO = _mapper.Map<ComponenteDTO>(componente);
                componenteDTO.Activo = _mapper.Map<ActivoMinDTO>(activo);

                _logger.LogInformation($"Actualizado el componente ({componente.DescripcionES} con ID {componente.Id})");

                return Ok(componenteDTO);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex.Message);

                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el componente {updateComponenteDTO.DescripcionES} con ID {updateComponenteDTO.Id}: {ex.Message}");

                return StatusCode(500, $"Error al actualiza el componente: {ex.Message}");
            }
        }

        /// <summary>
        /// Elimina la información de un Componente existente.
        /// </summary>
        /// <param name="id">Identificador del Componente.</param>
        /// <returns>
        /// Un <see cref="IActionResult"/> que representa el resultado de la operación:
        /// <list type="bullet">
        ///     <item><description>
        ///         <b>200 OK</b>: El componente se ha actualizado correctamente y se devuelve el objeto actualizado <see cref="ComponenteDTO"/>.
        ///     </description></item>
        ///     <item><description>
        ///         <b>400 Bad Request</b>: La solicitud no es válida, se proporcionaron datos incorrectos o faltantes.
        ///     </description></item>
        ///     <item><description>
        ///         <b>404 Not Found</b>: El activo se encuentra en estado borrado o el componente no existe.
        ///     </description></item>
        ///     <item><description>
        ///         <b>500 Internal Server Error</b>: Se produjo un error interno al intentar actualizar el componente.
        ///     </description></item>
        /// </list>
        /// </returns>
        [Authorize(Roles = "SUPER_ADMINISTRADOR, ADMINISTRADOR, JEFE_MANTENIMIENTO")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteComponente(int id)
        {
            // Comprobar que exista el activo indicado
            var componente = await _componenteDAO.GetByIdAsync(id);

            try
            {
                await _componenteDAO.DeleteAsync(id);

                _logger.LogInformation($"Componente {componente.DescripcionES} eliminado (ID: {componente.Id})");

                return Ok($"Componente eliminado correctamente.");
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogInformation($"{ex.Message}");

                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogInformation($"{ex.Message}");

                return StatusCode(403, $"Error al eliminar el componente {componente.DescripcionES} con ID: {componente.Id}: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar el componente {componente.DescripcionES} con ID: {componente.Id}: {ex.Message}");

                return StatusCode(500, $"Error al eliminar el componente {componente.DescripcionES} con ID: {componente.Id}: {ex.Message}");
            }
        }
    }
}
