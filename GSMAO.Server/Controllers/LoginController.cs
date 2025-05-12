using GSMAO.Server.Database.DAOs;
using GSMAO.Server.Database;
using GSMAO.Server.Database.Tables;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using GSMAO.Server.Services;
using GSMAO.Server.DTOs;

namespace GSMAO.Server.Controllers
{
    /// <summary>
    /// Controlador que maneja la autenticación del sistema.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class LoginController : ControllerBase
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly ILogger<LoginController> _logger;
        private readonly JwtService _jwtService;
        private readonly EstadosUsuariosDAO _stateDAO;
        private readonly UsersDAO _userDAO;

        public LoginController(
            UserManager<Usuario> userManager,
            ILogger<LoginController> logger,
            JwtService jwtService,
            ApplicationDbContext context,
            EstadosUsuariosDAO stateDAO,
            UsersDAO userDAO)
        {
            _userManager = userManager;
            _logger = logger;
            _jwtService = jwtService;
            _stateDAO = stateDAO;
            _userDAO = userDAO;
        }

        /// <summary>
        /// Loguea un usuario en el sistema.
        /// </summary>
        /// <param name="loginDTO">Las credenciales del usuario que se desea loguear.</param>
        /// <returns>
        /// Un <see cref="ActionResult"/> que representa el resultado de la operación:
        /// <list type="bullet">
        ///     <item><description>
        ///         <b>200 OK</b>:
        ///             Se devuelve si las credenciales son correctas.
        ///             El cuerpo de la respuesta contiene el token generado para la sesión que acaba de iniciar.
        ///     </description></item>
        ///     <item><description>
        ///         <b>400 Bad Request</b>:
        ///             Se devuelve si ocurre alguno de los siguientes supuestos:
        ///             <list type="bullet">
        ///                 <item><description>Si el usuario no está registrado en el sistema o está con estado Borrado.</description></item>
        ///                 <item><description>Si las credenciales son incorrectas.</description></item>
        ///             </list>
        ///             El cuerpo de la respuesta contiene un mensaje indicando qué supuesto a ocurrido.
        ///     </description></item>
        /// </list>
        /// </returns>
        [HttpPost("login")]
        [Produces("application/json")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            try
            {
                // Obtain user state active to ensure lastest that the user is active.
                var state = await _stateDAO.GetByNameAsync("Activo", "Name");

                // Ensure the user with the UserName or email indicates exist.
                var user = await _userDAO.GetByUserOrEmailOrIdAsync(loginDTO.Email);

                if (user == null || user.IdEstadoUsuario != state!.Id)
                {
                    _logger.LogInformation($"Usuario no registrado (Email: {loginDTO.Email})");

                    return BadRequest(new
                    {
                        error = "Usuario no registrado.",
                        error_description = "Pida al Administrador del sistema que cree su usuario."
                    });
                }

                if (!await _userManager.CheckPasswordAsync(user, loginDTO.Password))
                {
                    _logger.LogInformation($"Contraseña incorrecta. (Email: {loginDTO.Email} - Password: {loginDTO.Password})");

                    return BadRequest(new
                    {
                        error = "",
                        error_description = "Contraseña incorrecta."
                    });
                }

                _logger.LogInformation($"Usuario logueado (Id: {user.Id} - Email: {user.Email})");

                return Ok(new { token = _jwtService.GenerateToken(user) });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogInformation($"Usuario no registrado. (Email: {loginDTO.Email}). {ex.Message}");

                return BadRequest(new
                {
                    error = "",
                    error_description = $"Usuario no registrado."
                });
            }
        }
    }
}
