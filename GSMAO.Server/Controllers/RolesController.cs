using AutoMapper;
using GSMAO.Server.Database;
using GSMAO.Server.Database.DAOs;
using GSMAO.Server.Database.Tables;
using GSMAO.Server.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GSMAO.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "SUPER_ADMINISTRADOR, ADMINISTRADOR")]
    [Produces("application/json")]
    public class RolesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<RolesController> _logger;
        private readonly RolesDAO _roleDAO;

        public RolesController(
            ApplicationDbContext context, IMapper mapper,
            ILogger<RolesController> logger, RolesDAO rolesDAO)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
            _roleDAO = rolesDAO;
        }

        /// <summary>
        /// Obtiene la información de los Roles creados.
        /// </summary>
        /// <returns>
        /// Un <see cref="IActionResult"/> que representa el resultado de la operación:
        /// <list type="bullet">
        ///     <item><description>
        ///         <b>200 OK</b>: El listado de roles se ha obtenido correctamente y se devuelve el objeto creado <see cref="IEnumerable{RolDTO}"/>.
        ///     </description></item>
        /// </list>
        /// </returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RolDTO>>> RetrieveRoles()
        {
            // Obtener el rol del token
            var rolUser = User.Claims.Where(u => u.Type == ClaimTypes.Role).Select(u => u.Value).FirstOrDefault();

            // Comprobamos si el usuario tiene rol Superadministrador
            bool esSuperadmin = rolUser == "SUPER_ADMINISTRADOR";

            IEnumerable<Rol> roles;
            if (esSuperadmin)
            {
                roles = await _roleDAO.GetAllAsync();
            }
            else
            {
                roles = await _roleDAO.GetNotAdminRolesAsync();
            }

            return Ok(_mapper.Map<List<RolDTO>>(roles));
        }
    }
}
