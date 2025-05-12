using AutoMapper;
using GSMAO.Server.Database.DAOs;
using GSMAO.Server.Database.Tables;
using GSMAO.Server.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GSMAO.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [Produces("application/json")]
    public class CentrosCostesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly CentroCosteDAO _centroCosteDAO;
        private readonly ILogger<UsuariosController> _logger;
        private readonly UsersDAO _usersDAO;
        private readonly PlantaDAO _plantaDAO;
        private readonly EmpresaDAO _empresaDAO;

        public CentrosCostesController(IMapper mapper, ILogger<UsuariosController> logger, CentroCosteDAO centroCosteDAO, UsersDAO userDAO, PlantaDAO plantaDAO, EmpresaDAO empresaDAO)
        {
            _mapper = mapper;
            _centroCosteDAO = centroCosteDAO;
            _usersDAO = userDAO;
            _plantaDAO = plantaDAO;
            _logger = logger;
            _empresaDAO = empresaDAO;
        }

        /// <summary>
        /// Crea un centro de coste en el sistema.
        /// </summary>
        /// <param name="createCentroCosteDTO">Objeto <see cref="CreateCentroCosteDTO"/> que contiene la información del centro de coste.</param>
        /// <returns>
        /// Un <see cref="IActionResult"/> que representa el resultado de la operación:
        /// <list type="bullet">
        ///     <item><description>
        ///         <b>200 OK</b>: El centro de coste se ha creado correctamente y se devuelve el objeto creado <see cref="CentroCosteDTO"/>.
        ///     </description></item>
        ///     <item><description>
        ///         <b>400 Bad Request</b>: La solicitud no es válida, se proporcionaron datos incorrectos o faltantes.
        ///     </description></item>
        ///     <item><description>
        ///         <b>500 Internal Server Error</b>: Se produjo un error interno al intentar crear el centro de coste.
        ///     </description></item>
        /// </list>
        /// </returns>
        [Authorize(Roles = "SUPER_ADMINISTRADOR, ADMINISTRADOR, JEFE_MANTENIMIENTO")]
        [HttpPost]
        public async Task<ActionResult<CentroCosteDTO>> CreateCentroCoste([FromBody] CreateCentroCosteDTO createCentroCosteDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Comprobamos que la planta indicada exista
            var planta = await _plantaDAO.GetByIdAsync(createCentroCosteDTO.IdPlanta);

            var rolUser = User.Claims.Where(u => u.Type == ClaimTypes.Role).Select(u => u.Value).FirstOrDefault();
            bool esSuperadmin = rolUser == "SUPER_ADMINISTRADOR";

            if (!esSuperadmin)
            {
                var userId = User.Claims.Where(u => u.Type == "user.id").Select(u => u.Value).FirstOrDefault();
                var user = await _usersDAO.GetByIdAsync(userId!);

                switch (rolUser)
                {
                    case "ADMINISTRADOR":
                        if (planta.IdEmpresa != user.IdEmpresa)
                            return BadRequest("La planta debe pertenecer a su empresa.");

                        break;

                    case "JEFE_MANTENIMIENTO":
                        if (planta.IdEmpresa != user.IdEmpresa && planta.Id != user.IdPlanta)
                            return BadRequest("La planta no corresponde con la planta que tiene asignada su usuario.");

                        break;
                }
            }

            try
            {
                var newCentroCoste = _mapper.Map<CentroCoste>(createCentroCosteDTO);

                var createdCentroCoste = await _centroCosteDAO.CreateAsync(newCentroCoste);
                _logger.LogInformation($"Nuevo centro de coste creado ({createdCentroCoste.DescripcionES} con ID {createdCentroCoste.Id})");

                return Ok(_mapper.Map<CentroCosteDTO>(createdCentroCoste));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear el centro de coste {createCentroCosteDTO.DescripcionES}: {ex.Message}");
                return StatusCode(500, $"Error al crear el centro de coste: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtiene la información de los Centros de Costes creados.
        /// </summary>
        /// <returns>
        /// Un <see cref="IActionResult"/> que representa el resultado de la operación:
        /// <list type="bullet">
        ///     <item><description>
        ///         <b>200 OK</b>: El listado de centros de costes se ha obtenido correctamente y se devuelve el objeto creado <see cref="IEnumerable{CentroCosteDTO}"/>
        ///     </description></item>
        /// </list>
        /// </returns>
        /// <exception cref="KeyNotFoundException"></exception>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CentroCosteDTO>>> RetrieveCentroCostes()
        {
            var rolUser = User.Claims.Where(u => u.Type == ClaimTypes.Role).Select(u => u.Value).FirstOrDefault();

            IEnumerable<CentroCoste> centrosCostes = new List<CentroCoste>();
            var idUser = "";
            Usuario? user = null;
            switch (rolUser)
            {
                case "SUPER_ADMINISTRADOR":
                    centrosCostes = await _centroCosteDAO.GetAllAsync();
                    break;

                case "ADMINISTRADOR":
                    idUser = User.Claims.Where(u => u.Type == "user.id").Select(u => u.Value).FirstOrDefault();
                    user = await _usersDAO.GetByIdAsync(idUser!);

                    centrosCostes = await _centroCosteDAO.GetEmpresaCentrosCostesAsync((int)user.IdEmpresa!);
                    break;

                default:
                    idUser = User.Claims.Where(u => u.Type == "user.id").Select(u => u.Value).FirstOrDefault();
                    user = await _usersDAO.GetByIdAsync(idUser!);

                    centrosCostes = await _centroCosteDAO.GetPlantaCentrosCostesAsync((int)user.IdPlanta!);
                    break;
            }

            return Ok(_mapper.Map<List<CentroCosteDTO>>(centrosCostes));
        }

        /// <summary>
        /// Actualiza la información de un Centro de Coste existente.
        /// </summary>
        /// <param name="id">Identificador del centro de coste.</param>
        /// <param name="updateCentroCosteDTO">Objeto <see cref="UpdateCentroCosteDTO"/> que contiene la nueva ínformación del centro de coste.</param>
        /// <returns>
        /// Un <see cref="IActionResult"/> que representa el resultado de la operación:
        /// <list type="bullet">
        ///     <item><description>
        ///         <b>200 OK</b>: El centro de coste se ha actualizado correctamente y se devuelve el objeto creado <see cref="CentroCosteDTO"/>.
        ///     </description></item>
        ///     <item><description>
        ///         <b>400 Bad Request</b>: La solicitud no es válida, se proporcionaron datos incorrectos o faltantes.
        ///     </description></item>
        ///     <item><description>
        ///         <b>500 Internal Server Error</b>: Se produjo un error interno al intentar actualizar el centro de coste.
        ///     </description></item>
        /// </list>
        /// </returns>
        [Authorize(Roles = "SUPER_ADMINISTRADOR, ADMINISTRADOR, JEFE_MANTENIMIENTO")]
        [HttpPut("{id}")]
        public async Task<ActionResult<CentroCoste>> UpdateCentroCoste(int id, [FromBody] UpdateCentroCosteDTO updateCentroCosteDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != updateCentroCosteDTO.Id)
            {
                return BadRequest("El identificador de planta no coincide con los datos del JSON enviado.");
            }

            // Comprobamos que la planta indicada exista
            var planta = await _plantaDAO.GetByIdAsync(updateCentroCosteDTO.IdPlanta);

            var rolUser = User.Claims.Where(u => u.Type == ClaimTypes.Role).Select(u => u.Value).FirstOrDefault();
            bool esSuperadmin = rolUser == "SUPER_ADMINISTRADOR";

            if (!esSuperadmin)
            {
                var userId = User.Claims.Where(u => u.Type == "user.id").Select(u => u.Value).FirstOrDefault();
                var user = await _usersDAO.GetByIdAsync(userId!);

                switch (rolUser)
                {
                    case "ADMINISTRADOR":
                        if (planta.IdEmpresa != user.IdEmpresa)
                            return BadRequest("La planta debe pertenecer a su empresa.");

                        break;

                    case "JEFE_MANTENIMIENTO":
                        if (planta.IdEmpresa != user.IdEmpresa && planta.Id != user.IdPlanta)
                            return BadRequest("La planta no corresponde con la planta que tiene asignada su usuario.");

                        break;
                }
            }

            try
            {
                var newCentroCoste = _mapper.Map<CentroCoste>(updateCentroCosteDTO);

                var updatedCentroCoste = await _centroCosteDAO.CreateAsync(newCentroCoste);

                _logger.LogInformation($"Actualizado el centro de coste ({updatedCentroCoste.DescripcionES} con ID {updatedCentroCoste.Id})");

                return Ok(_mapper.Map<CentroCosteDTO>(updatedCentroCoste));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el centro de coste {updateCentroCosteDTO.DescripcionES} con ID {updateCentroCosteDTO.Id}: {ex.Message}");

                return StatusCode(500, $"Error al actualizar el centro de coste: {ex.Message}");
            }
        }

        /// <summary>
        /// Elimina la información de un Centro de Coste existente.
        /// </summary>
        /// <param name="id">Identificador del centro de coste.</param>
        /// <returns>
        /// Un <see cref="IActionResult"/> que representa el resultado de la operación:
        /// <list type="bullet">
        ///     <item><description>
        ///         <b>200 OK</b>: El centro de coste se ha eliminado correctamente.
        ///     </description></item>
        ///     <item><description>
        ///         <b>400 Bad Request</b>: La solicitud no es válida, no se tienen permisos para borrar el centro de coste.
        ///     </description></item>
        ///     <item><description>
        ///         <b>500 Internal Server Error</b>: Se produjo un error interno al intentar eliminar el centro de coste.
        ///     </description></item>
        /// </list>
        /// </returns>
        [Authorize(Roles = "SUPER_ADMINISTRADOR, ADMINISTRADOR, JEFE_MANTENIMIENTO")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCentroCoste(int id)
        {
            var centroCoste = await _centroCosteDAO.GetByIdAsync(id);

            var rolUser = User.Claims.Where(u => u.Type == ClaimTypes.Role).Select(u => u.Value).FirstOrDefault();
            bool esSuperadmin = rolUser == "SUPER_ADMINISTRADOR";

            if (!esSuperadmin)
            {
                var userId = User.Claims.Where(u => u.Type == "user.id").Select(u => u.Value).FirstOrDefault();
                var user = await _usersDAO.GetByIdAsync(userId!);

                var planta = await _plantaDAO.GetByIdAsync(centroCoste.IdPlanta);

                switch (rolUser)
                {
                    case "ADMINISTRADOR":
                        if (planta.IdEmpresa != user.IdEmpresa)
                            return BadRequest("El centro de coste que quieres borrar debe pertenecer a su empresa.");

                        break;

                    case "JEFE_MANTENIMIENTO":
                        if (planta.IdEmpresa != user.IdEmpresa && planta.Id != user.IdPlanta)
                            return BadRequest("El centro de coste que quieres borrar debe pertenecer a su planta.");

                        break;
                }
            }

            try
            {
                await _centroCosteDAO.DeleteAsync(id);

                _logger.LogInformation($"Centro de coste {centroCoste.DescripcionES} eliminado (ID: {centroCoste.Id})");

                return Ok($"Centro de coste eliminado correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar el centro de coste {centroCoste.DescripcionES} con ID {centroCoste.Id}: {ex.Message}");
                return StatusCode(500, $"Error al eliminar el centro de coste: {ex.Message}");
            }
        }
    }
}
