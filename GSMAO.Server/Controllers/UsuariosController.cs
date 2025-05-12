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
using System.Security.Claims;

namespace GSMAO.Server.Controllers
{
    /// <summary>
    /// Controlador que maneja la gestión de usuarios.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "SUPER_ADMINISTRADOR, ADMINISTRADOR")]
    [Produces("application/json")]
    public class UsuariosController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly UserManager<Usuario> _userManager;
        private readonly ILogger<UsuariosController> _logger;
        private readonly ValidateData _validateUserData;
        //private readonly IEmailSender _emailSender;
        private readonly UsersDAO _userDAO;
        private readonly EstadosUsuariosDAO _stateDAO;
        private readonly RolesDAO _roleDAO;

        public UsuariosController(IMapper mapper,
            UserManager<Usuario> userManager, ILogger<UsuariosController> logger,
            UsersDAO userDAO, EstadosUsuariosDAO stateDAO, RolesDAO rolesDAO, ValidateData validateUserData/*, IEmailSender emailSender*/)
        {
            _mapper = mapper;
            _userManager = userManager;
            _logger = logger;
            _validateUserData = validateUserData;
            //_emailSender = emailSender;
            _userDAO = userDAO;
            _stateDAO = stateDAO;
            _roleDAO = rolesDAO;
        }

        /// <summary>
        /// Crea un usuario en el sistema.
        /// </summary>
        /// <param name="createUserDTO">Objeto <see cref="CreateUserDTO"/> que contiene la información del usuario.</param>
        /// <returns>
        /// Un <see cref="IActionResult"/> que representa el resultado de la operación:
        /// <list type="bullet">
        ///     <item><description>
        ///         <b>200 OK</b>: El usuario se ha creado correctamente y se devuelve el objeto creado <see cref="UsuarioDTO"/>.
        ///     </description></item>
        ///     <item><description>
        ///         <b>400 Bad Request</b>: La solicitud no es válida, se proporcionaron datos incorrectos o faltantes.
        ///     </description></item>
        ///     <item><description>
        ///         <b>500 Internal Server Error</b>: Se produjo un error interno al intentar crear el usuario.
        ///     </description></item>
        /// </list>
        /// </returns>
        [HttpPost]
        public async Task<ActionResult<UsuarioDTO>> CreateUsuario([FromBody] CreateUserDTO createUserDTO)
        {
            // Obtener el rol del token
            var rolUsuarioLogueado = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)!.Value;

            // Verificar los datos del modelo individualmente.
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Validar si el usuario puede crear el usuario con el rol especificado
            try
            {
                await _validateUserData.IsValidUser(rolUsuarioLogueado, createUserDTO);

            }
            catch (Exception ex) when (ex is InvalidDataException || ex is ValidationException)
            {
                return BadRequest(ex.Message);
            }

            // Comprobar si el Email es único
            var userExists = await _userManager.FindByEmailAsync(createUserDTO.Email);
            if (userExists != null)
            {
                return BadRequest("El email indicado se encuentra registrado.");
            }

            // Convertir al modelo de la entidad
            var newUser = _mapper.Map<Usuario>(createUserDTO);
            newUser.UserName = newUser.Email!.Split("@")[0];

            try
            {
                // Guardar en MySQL el usuario
                var result = await _userManager.CreateAsync(newUser, createUserDTO.Password);

                if (!result.Succeeded)
                {
                    var errors = string.Join(";", result.Errors.Select(e => e.Description));
                    
                    _logger.LogError($"Error al crear el usuario: {errors}");
                    
                    return BadRequest($"Error al crear el usuario: {errors}");
                }

                // Obtener los datos guardados
                var user = await _userDAO.GetByUserOrEmailOrIdAsync(newUser.Email!);

                // Registrar la creación del usuario
                _logger.LogInformation($"Nuevo usuario registrado (id: {user!.Id})");
                    
                // Devolver los datos del nuevo usuario
                return Ok(_mapper.Map<UsuarioDTO>(user));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear un usuario: {ex.Message}");
                
                return StatusCode(500, $"Error al crear un usuario.");
            }
        }

        /// <summary>
        /// Obtiene la información de los Usuarios creados.
        /// </summary>
        /// <returns>
        /// Un <see cref="IActionResult"/> que representa el resultado de la operación:
        /// <list type="bullet">
        ///     <item><description>
        ///         <b>200 OK</b>: El listado de usuarios se ha obtenido correctamente y se devuelve el objeto creado <see cref="IEnumerable{UsuarioDTO}"/>.
        ///     </description></item>
        /// </list>
        /// </returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UsuarioDTO>>> RetrieveUsuarios()
        {
            // Obtener el rol del token
            var rolUser = User.Claims.Where(u => u.Type == ClaimTypes.Role).Select(u => u.Value).FirstOrDefault();

            // Comprobamos si el usuario tiene rol Superadministrador
            bool esSuperadmin = rolUser == "SUPER_ADMINISTRADOR";

            IEnumerable<Usuario> usuarios;
            if (esSuperadmin)
            {
                usuarios = await _userDAO.GetAllAsync();
            }
            else
            {
                var userId = User.Claims.Where(u => u.Type == "user.id").Select(u => u.Value).FirstOrDefault();
                usuarios = await _userDAO.GetEmpresaUsersAsync(userId!);
            }

            return Ok(_mapper.Map<List<UsuarioDTO>>(usuarios));
        }

        /// <summary>
        /// Actualiza la información de un Usuario existente.
        /// </summary>
        /// <param name="id">Cadena de identificación del usuario.</param>
        /// <param name="updateUsuario">Objeto <see cref="UpdateUsuarioDTO"/> que contiene la nueva información del usuario.</param>
        /// <returns>
        /// Un <see cref="IActionResult"/> que representa el resultado de la operación:
        /// <list type="bullet">
        ///     <item><description>
        ///         <b>200 OK</b>: El usuario se ha actualizado correctamente y se devuelve el objeto creado <see cref="UsuarioDTO"/>.
        ///     </description></item>
        ///     <item><description>
        ///         <b>400 Bad Request</b>: La solicitud no es válida, se proporcionaron datos incorrectos o faltantes.
        ///     </description></item>
        ///     <item><description>
        ///         <b>404 Not Found</b>: No se encuentra la información actual del usuario que se intenta actualizar.
        ///     </description></item>
        ///     <item><description>
        ///         <b>500 Internal Server Error</b>: Se produjo un error interno al intentar actualizar el usuario.
        ///     </description></item>
        /// </list>
        /// </returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<UsuarioDTO>> UpdateUsuario(string id, [FromBody] UpdateUsuarioDTO updateUsuario)
        {
            // Obtener el rol del token
            var rolUsuarioLogueado = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)!.Value;

            // Verificar los datos del modelo individualmente.
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Verificar que el identificador del usuario coincide con el JSON
            if (id != updateUsuario.Id)
            {
                return BadRequest("El identificador de usuario no coincide con los datos del JSON enviado.");
            }

            // Validar información al actualizar el usuario
            try
            {
                await _validateUserData.IsValidUser(rolUsuarioLogueado, updateUsuario);

            }
            catch (Exception ex) when (ex is InvalidDataException || ex is ValidationException)
            {
                return BadRequest(ex.Message);
            }

            try
            {
                var usuario = await _userDAO.GetByIdAsync(id);

                _mapper.Map(updateUsuario, usuario);

                await _userDAO.UpdateAsync(usuario);
                usuario = await _userDAO.GetByUserOrEmailOrIdAsync(id);

                _logger.LogInformation($"Usuario actualizado (id: {usuario!.Id})");
                
                return Ok(_mapper.Map<UsuarioDTO>(usuario));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el usuario: {ex.Message}");
                
                return StatusCode(500, $"Error al actualizar el usuario.");
            }
        }

        /// <summary>
        /// Cambia la contraseña de un Usuario existente.
        /// </summary>
        /// <param name="id">Cadena de identificación del usuario.</param>
        /// <param name="resetPasswordDTO">Objeto <see cref="ResetPasswordDTO"/> que contiene la nueva contraseña del usuario.</param>
        /// <returns>
        /// Un <see cref="IActionResult"/> que representa el resultado de la operación:
        /// <list type="bullet">
        ///     <item><description>
        ///         <b>200 OK</b>: El usuario se ha actualizado correctamente.
        ///     </description></item>
        ///     <item><description>
        ///         <b>400 Bad Request</b>: La solicitud no es válida, se proporcionaron datos incorrectos o faltantes.
        ///     </description></item>
        ///     <item><description>
        ///         <b>404 Not Found</b>: No se encuentra la información actual del usuario que se intenta actualizar.
        ///     </description></item>
        ///     <item><description>
        ///         <b>500 Internal Server Error</b>: Se produjo un error interno al intentar actualizar la contraseña del usuario.
        ///     </description></item>
        /// </list>
        /// </returns>
        [HttpPut("{id}/reset-password")]
        public async Task<ActionResult> ResetPassword(string id, ResetPasswordDTO resetPasswordDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != resetPasswordDTO.Id)
            {
                return BadRequest("El ID no coincide con los datos del JSON enviado.");
            }

            try
            {
                await _userDAO.ResetPasswordAsync(resetPasswordDTO);
                return Ok();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error inesperado al restablecer la contraseña: {ex.Message}");
                
                return StatusCode(500, "Error inesperado al restablecer la contraseña.");
            }
        }

        /// <summary>
        /// Cambia el estado de un Usuario existente.
        /// </summary>
        /// <param name="id">Cadena de identificación del usuario.</param>
        /// <param name="state">Cadena de identificación del estado que se le desea asignar al usuario.</param>
        /// <returns>
        /// Un <see cref="IActionResult"/> que representa el resultado de la operación:
        /// <list type="bullet">
        ///     <item><description>
        ///         <b>200 OK</b>: El estado del usuario se ha actualizado correctamente.
        ///     </description></item>
        ///     <item><description>
        ///         <b>400 Bad Request</b>: La solicitud no es válida, se proporcionaron datos incorrectos o faltantes.
        ///     </description></item>
        ///     <item><description>
        ///         <b>404 Not Found</b>: No se encuentra la información actual del usuario que se intenta actualizar.
        ///     </description></item>
        ///     <item><description>
        ///         <b>500 Internal Server Error</b>: Se produjo un error interno al intentar actualizar el estado del usuario.
        ///     </description></item>
        /// </list>
        /// </returns>
        [HttpPut("{id}/change-state/{state}")]
        public async Task<ActionResult> ChangeState(string id, string state)
        {
            try
            {
                if (state.ToLower() == "borrado")
                    throw new ArgumentException("No se puede borrar un usuario, utiliza la establecida para este fin.");

                await _userDAO.ChangeStateAsync(id, state);
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
                _logger.LogError($"Error inesperado al cambiar al estado '{state}' el usuario: {ex.Message}");

                return StatusCode(500, $"Error inesperado al {state}r el usuario.");
            }
        }

        /// <summary>
        /// Cambia el estado de un Usuario existente a "Borrado".
        /// </summary>
        /// <param name="id">Cadena de identificación del usuario.</param>
        /// <returns>
        /// Un <see cref="IActionResult"/> que representa el resultado de la operación:
        /// <list type="bullet">
        ///     <item><description>
        ///         <b>200 OK</b>: El estado del usuario se ha actualizado correctamente.
        ///     </description></item>
        ///     <item><description>
        ///         <b>400 Bad Request</b>: La solicitud no es válida, se proporcionaron datos incorrectos o faltantes.
        ///     </description></item>
        ///     <item><description>
        ///         <b>404 Not Found</b>: No se encuentra la información actual del usuario que se intenta actualizar.
        ///     </description></item>
        ///     <item><description>
        ///         <b>500 Internal Server Error</b>: Se produjo un error interno al intentar actualizar el estado del usuario.
        ///     </description></item>
        /// </list>
        /// </returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUsuario(string id)
        {
            try
            {
                await _userDAO.ChangeStateAsync(id, "Borrado");
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
                _logger.LogError($"Error inesperado al cambiar al estado 'Borrado' el usuario: {ex.Message}");

                return StatusCode(500, "Error inesperado al borrar el usuario.");
            }
        }
    }
}