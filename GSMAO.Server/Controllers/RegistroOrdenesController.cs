using GSMAO.Server.Database.DAOs;
using GSMAO.Server.Database.Tables;
using GSMAO.Server.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GSMAO.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [Produces("application/json")]
    public class RegistroOrdenesController : ControllerBase
    {
        private readonly ILogger<RegistroOrdenesController> _logger;
        private readonly OrdenesDAO _ordenesDAO;
        private readonly UsersDAO _userDAO;

        public RegistroOrdenesController(ILogger<RegistroOrdenesController> logger, OrdenesDAO ordenesDAO, UsersDAO userDAO)
        {
            _logger = logger;
            _ordenesDAO = ordenesDAO;
            _userDAO = userDAO;
        }

        /// <summary>
        /// Crea una nueva orden vacía asociada al usuario autenticado.
        /// </summary>
        /// <returns>
        /// Un <see cref="Task{ActionResult}"/> que representa el resultado de la operación:
        /// <list type="bullet">
        ///     <item><description>
        ///         <b>200 OK</b>: Retorna un objeto con el identificador (<c>IdOrden</c>) de la orden creada.
        ///     </description></item>
        ///     <item><description>
        ///         <b>500 Internal Server Error</b>: Se produjo un error inesperado al intentar crear la orden.
        ///     </description></item>
        /// </list>
        /// </returns>
        [HttpPost]
        public async Task<ActionResult<int>> CreateOrden()
        {
            try
            {
                var idUser = User.Claims.Where(u => u.Type == "user.id").Select(u => u.Value).FirstOrDefault();
                var user = await _userDAO.GetByIdAsync(idUser!);

                var newOrden = new Orden()
                {
                    IdUsuarioCreador = idUser!,
                    Usuario = user,
                    IncidenciasOrden = new List<IncidenciaOrden>(),
                    UsuariosOrden = new List<Usuario_Orden>()
                };

                var createdOrden = await _ordenesDAO.CreateAsync(newOrden);

                return Ok(new { IdOrden = createdOrden.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear la orden vacía: {ex}");

                return StatusCode(500, $"Error al crear la orden vacía.");
            }
        }
    }
}
