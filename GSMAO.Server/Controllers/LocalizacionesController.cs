using AutoMapper;
using GSMAO.Server.Database;
using GSMAO.Server.Database.DAOs;
using GSMAO.Server.Database.Tables;
using GSMAO.Server.DTOs;
using GSMAO.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

namespace GSMAO.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "SUPER_ADMINISTRADOR, ADMINISTRADOR, JEFE_MANTENIMIENTO")]
    [Produces("application/json")]
    public class LocalizacionesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly LocalizacionDAO _localizacionesDAO;
        private readonly ILogger<LocalizacionesController> _logger;
        private readonly UsersDAO _usersDAO;
        private readonly PlantaDAO _plantaDAO;
        private readonly EmpresaDAO _empresaDAO;

        public LocalizacionesController(IMapper mapper, ILogger<LocalizacionesController> logger, LocalizacionDAO localizacionesDAO, UsersDAO userDAO, PlantaDAO plantaDAO, EmpresaDAO empresaDAO)
        {
            _mapper = mapper;
            _localizacionesDAO = localizacionesDAO;
            _usersDAO = userDAO;
            _plantaDAO = plantaDAO;
            _empresaDAO = empresaDAO;
            _logger = logger;
        }

        /// <summary>
        /// Crea una localización en el sistema.
        /// </summary>
        /// <param name="createLocalizacionDTO">Objeto <see cref="CreateLocalizacionDTO"/> que contiene la información de la localización.</param>
        /// <returns>
        /// Un <see cref="IActionResult"/> que representa el resultado de la operación:
        /// <list type="bullet">
        ///     <item><description>
        ///         <b>200 OK</b>: La localización se ha creado correctamente y se devuelve el objeto creado <see cref="LocalizacionDTO"/>.
        ///     </description></item>
        ///     <item><description>
        ///         <b>400 Bad Request</b>: La solicitud no es válida, se proporcionaron datos incorrectos o faltantes.
        ///     </description></item>
        ///     <item><description>
        ///         <b>500 Internal Server Error</b>: Se produjo un error interno al intentar crear la localización.
        ///     </description></item>
        /// </list>
        /// </returns>
        [HttpPost]
        public async Task<ActionResult<LocalizacionDTO>> CreateLocalizacion([FromBody] CreateLocalizacionDTO createLocalizacionDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Comprobamos que la planta indicada exista
            var planta = await _plantaDAO.GetByIdAsync(createLocalizacionDTO.IdPlanta);

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
                var newLocalizacion = _mapper.Map<Localizacion>(createLocalizacionDTO);

                var createdLocalizacion = await _localizacionesDAO.CreateAsync(newLocalizacion);
                
                _logger.LogInformation($"Nueva localización creada ({createdLocalizacion.DescripcionES} con ID {createdLocalizacion.Id})");
                
                return Ok(_mapper.Map<LocalizacionDTO>(createdLocalizacion));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear la localización {createLocalizacionDTO.DescripcionES}: {ex.Message}");
                
                return StatusCode(500, $"Error al crear la localización: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtiene la información de las Localizaciones creadas.
        /// </summary>
        /// <returns>
        /// Un <see cref="IActionResult"/> que representa el resultado de la operación:
        /// <list type="bullet">
        ///     <item><description>
        ///         <b>200 OK</b>: El listado de localizaciones se ha obtenido correctamente y se devuelve el objeto creado <see cref="IEnumerable{LocalizacionDTO}"/>.
        ///     </description></item>
        /// </list>
        /// </returns>
        /// <exception cref="KeyNotFoundException"></exception>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LocalizacionDTO>>> RetrieveLocalizaciones()
        {
            var rolUser = User.Claims.Where(u => u.Type == ClaimTypes.Role).Select(u => u.Value).FirstOrDefault();

            IEnumerable<Localizacion> localizaciones = new List<Localizacion>();
            var idUser = "";
            Usuario? user = null;
            switch (rolUser)
            {
                case "SUPER_ADMINISTRADOR":
                    localizaciones = await _localizacionesDAO.GetAllAsync();
                    break;

                case "ADMINISTRADOR":
                    idUser = User.Claims.Where(u => u.Type == "user.id").Select(u => u.Value).FirstOrDefault();
                    user = await _usersDAO.GetByIdAsync(idUser!);

                    localizaciones = await _localizacionesDAO.GetEmpresaLocalizacionesAsync((int)user.IdEmpresa!);
                    break;

                case "JEFE_MANTENIMIENTO":
                    idUser = User.Claims.Where(u => u.Type == "user.id").Select(u => u.Value).FirstOrDefault();
                    user = await _usersDAO.GetByIdAsync(idUser!);

                    localizaciones = await _localizacionesDAO.GetPlantaLocalizacionesAsync((int)user.IdPlanta!);
                    break;
            }

            return Ok(_mapper.Map<List<LocalizacionDTO>>(localizaciones));
        }

        /// <summary>
        /// Actualiza la información de una Localización existente.
        /// </summary>
        /// <param name="id">Identificador de la localización.</param>
        /// <param name="updateLocalizacionDTO">Objeto <see cref="UpdateLocalizacionDTO"/> que contiene la nueva información de la localización.</param>
        /// <returns>
        /// Un <see cref="IActionResult"/> que representa el resultado de la operación:
        /// <list type="bullet">
        ///     <item><description>
        ///         <b>200 OK</b>: La localización se ha actualizado correctamente y se devuelve el objeto creado <see cref="LocalizacionDTO"/>.
        ///     </description></item>
        ///     <item><description>
        ///         <b>400 Bad Request</b>: La solicitud no es válida, se proporcionaron datos incorrectos o faltantes.
        ///     </description></item>
        ///     <item><description>
        ///         <b>500 Internal Server Error</b>: Se produjo un error interno al intentar actualizar la localización.
        ///     </description></item>
        /// </list>
        /// </returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<LocalizacionDTO>> UpdateLocalizacion(int id, [FromBody] UpdateLocalizacionDTO updateLocalizacionDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != updateLocalizacionDTO.Id)
            {
                return BadRequest("El identificador de planta no coincide con los datos del JSON enviado.");
            }

            // Comprobamos que la planta indicada exista
            var planta = await _plantaDAO.GetByIdAsync(updateLocalizacionDTO.IdPlanta);

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
                var localizacion = await _localizacionesDAO.GetByIdAsync(id);

                _mapper.Map(updateLocalizacionDTO, localizacion);

                await _localizacionesDAO.UpdateAsync(localizacion);
                localizacion = await _localizacionesDAO.GetByIdAsync(id);

                _logger.LogInformation($"Actualizada la localización ({localizacion.DescripcionES} con ID {localizacion.Id})");

                return Ok(_mapper.Map<LocalizacionDTO>(localizacion));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar la localización {updateLocalizacionDTO.DescripcionES} con ID {updateLocalizacionDTO.Id}: {ex.Message}");
                
                return StatusCode(500, $"Error al actualizar la localización: {ex.Message}");
            }
        }

        /// <summary>
        /// Elimina la información de una Localización existente.
        /// </summary>
        /// <param name="id">Identificador de la localización.</param>
        /// <returns>
        /// Un <see cref="IActionResult"/> que representa el resultado de la operación:
        /// <list type="bullet">
        ///     <item><description>
        ///         <b>200 OK</b>: La localización se ha eliminado correctamente.
        ///     </description></item>
        ///     <item><description>
        ///         <b>400 Bad Request</b>: La solicitud no es válida, no se tienen permisos para borrar la localización.
        ///     </description></item>
        ///     <item><description>
        ///         <b>500 Internal Server Error</b>: Se produjo un error interno al intentar eliminar la localización.
        ///     </description></item>
        /// </list>
        /// </returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteLocalizacion(int id)
        {
            var localizacion = await _localizacionesDAO.GetByIdAsync(id);

            var rolUser = User.Claims.Where(u => u.Type == ClaimTypes.Role).Select(u => u.Value).FirstOrDefault();
            bool esSuperadmin = rolUser == "SUPER_ADMINISTRADOR";

            if (!esSuperadmin)
            {
                var userId = User.Claims.Where(u => u.Type == "user.id").Select(u => u.Value).FirstOrDefault();
                var user = await _usersDAO.GetByIdAsync(userId!);

                var planta = await _plantaDAO.GetByIdAsync(localizacion.IdPlanta);

                switch (rolUser)
                {
                    case "ADMINISTRADOR":
                        if (planta.IdEmpresa != user.IdEmpresa)
                            return BadRequest("La localización que quieres borrar debe pertenecer a su empresa.");

                        break;

                    case "JEFE_MANTENIMIENTO":
                        if (planta.IdEmpresa != user.IdEmpresa && planta.Id != user.IdPlanta)
                            return BadRequest("La localización que quieres borrar debe pertenecer a su planta.");

                        break;
                }
            }

            try
            {
                await _localizacionesDAO.DeleteAsync(id);

                _logger.LogInformation($"Localización {localizacion.DescripcionES} eliminada (ID: {localizacion.Id})");

                return Ok($"Localización eliminada correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar la localización {localizacion.DescripcionES} con ID {localizacion.Id}: {ex.Message}");
                
                return StatusCode(500, $"Error al eliminar la localización: {ex.Message}");
            }
        }
    }
}
