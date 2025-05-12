using AutoMapper;
using GSMAO.Server.Database.Tables;
using GSMAO.Server.DTOs;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GSMAO.Server.Database.DAOs
{
    /// <summary>
    /// Proporciona acceso a los datos de usuarios en la base de datos.
    /// </summary>
    public class UsersDAO : BaseDAO<Usuario, string>
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly EstadosUsuariosDAO _stateDAO;

        public UsersDAO(
            ApplicationDbContext context,
            ILogger<UsersDAO> logger,
            UserManager<Usuario> userManager,
            EstadosUsuariosDAO stateDAO
        ) : base (context, logger)
        {
            _userManager = userManager;
            _stateDAO = stateDAO;
        }

        /// <summary>
        /// Obtiene un usuario según la información indicada.
        /// </summary>
        /// <param name="data">Información del usuario, puede corresponder con el Username, el Email o el Id.</param>
        /// <param name="cancellationToken">Token de cancelación opcional.</param>
        /// <returns>La entidad <see cref="Usuario"/> encontrada.</returns>
        /// <exception cref="ArgumentException">Cuando la información por la que se desea buscar viene vacía.</exception>
        /// <exception cref="KeyNotFoundException">Cuando no se encuentra la entidad <see cref="Usuario"/>.</exception>
        public async Task<Usuario> GetByUserOrEmailOrIdAsync(string data, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(data))
                throw new ArgumentException("El parámetro 'data' no puede ser nulo o vacío.", nameof(data));

            var user = await _context.Usuarios
                .Where(u => u.UserName == data || u.Email == data || u.Id == data)
                .Include(u => u.Rol)
                .Include(u => u.EstadoUsuario)
                .Include(u => u.Empresa)
                .Include(u => u.Planta)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);

            if (user == null)
                throw new KeyNotFoundException($"No se encontró el usuario '{data}'.");

            return user;
        }

        /// <summary>
        /// Obtiene un listado de usuarios.
        /// </summary>
        /// <param name="cancellationToken">Token de cancelación opcional.</param>
        /// <returns>Un listado de entidades <see cref="Usuario"/> encontradas.</returns>
        protected override async Task<IEnumerable<Usuario>> GetAllInternalAsync(CancellationToken cancellationToken = default)
        {
            var users = await _context.Usuarios
                .Include(u => u.Rol)
                .Include(u => u.EstadoUsuario)
                .Include(u => u.Empresa)
                .Include(u => u.Planta)
                .OrderBy(u => u.IdEstadoUsuario)
                .ThenBy(u => u.Rol.Orden)
                .ToListAsync(cancellationToken);

            return users;
        }

        /// <summary>
        /// Obtiene un listado de usuarios de una empresa, según el identificador del usuario.
        /// </summary>
        /// <param name="userId">El identificador del usuario.</param>
        /// <returns>Un listado de entidades <see cref="Usuario"/> encontradas.</returns>
        /// <exception cref="KeyNotFoundException">Cuando no se encuentra la entidad <see cref="Usuario"/>.</exception>
        public async Task<IEnumerable<Usuario>> GetEmpresaUsersAsync(string userId, CancellationToken cancellationToken = default)
        {
            var actualUser = await _context.Users
                .Where(u => u.Id == userId)
                .Include(u => u.Empresa)
                .FirstOrDefaultAsync(cancellationToken);

            if (actualUser == null)
                throw new KeyNotFoundException($"No se encontró el usuario que tiene la sesión iniciada.");

            var users = await _context.Users
                .Where(u => u.Empresa != null && u.Empresa.Descripcion == actualUser!.Empresa!.Descripcion)
                .Include(u => u.Rol)
                .Include(u => u.EstadoUsuario)
                .Include(u => u.Empresa)
                .Include(u => u.Planta)
                .OrderBy(u => u.IdEstadoUsuario)
                .ThenBy(u => u.Rol.Orden)
                .ToListAsync(cancellationToken);

            return users;
        }

        /// <summary>
        /// Actualiza la contraseña de un usuario.
        /// </summary>
        /// <param name="resetPasswordDTO">La información del usuario, incluye el identificador del usuario y la nueva contraseña.</param>
        /// <param name="cancellationToken"></param>
        /// <exception cref="InvalidOperationException">Cuando la contraseña no cumple los requisitos de seguridad mínimos.</exception>
        public async Task ResetPasswordAsync(ResetPasswordDTO resetPasswordDTO, CancellationToken cancellationToken = default)
        {
            await HandleWithLoggingAndExceptionAsync(
            async ct =>
            {
                var user = await GetByIdAsync(resetPasswordDTO.Id, ct);

                // Genera el token de restablecimiento de forma interna
                var tokenToReset = await _userManager.GeneratePasswordResetTokenAsync(user);

                // Utiliza el token para restablecer la contraseña
                var result = await _userManager.ResetPasswordAsync(user, tokenToReset, resetPasswordDTO.NewPassword);

                if (!result.Succeeded)
                {
                    var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                    _logger.LogError($"Error al restablecer la contraseña del usuario: {errors}");
                    throw new InvalidOperationException($"Error al restablecer la contraseña del usuario: {errors}");
                }

                // Registrar el evento de restablecimiento de contraseña
                _logger.LogInformation($"Un administrador restableció la contraseña del usuario {resetPasswordDTO.Id}");
            },
            $"Restableciendo contraseña para el usuario ID {resetPasswordDTO.Id}",
            cancellationToken);
        }

        /// <summary>
        /// Actualiza el estado de un usuario, entre "Activo", "Inactivo" y "Borrado".
        /// </summary>
        /// <param name="id">El identificador del usuario.</param>
        /// <param name="stateName">El estado al que se va a actualizar el usuario.</param>
        /// <param name="cancellationToken">Token de cancelación opcional.</param>
        public async Task ChangeStateAsync(string id, string stateName, CancellationToken cancellationToken = default)
        {
            await HandleWithLoggingAndExceptionAsync(
            async ct =>
            {
                var user = await GetByUserOrEmailOrIdAsync(id);
                var state = await _stateDAO.GetByNameAsync(stateName, "Name");

                user.EstadoUsuario = state;
                await UpdateAsync(user);
            },
            $"Cambio de estado para el usuario con ID {id} a {stateName}",
            cancellationToken);
        }

        /// <summary>
        /// Obtiene un listado de usuarios asignados a una orden específica.
        /// </summary>
        /// <param name="idOrden">Identificador de la orden.</param>
        /// <param name="cancellationToken">Token de cancelación opcional.</param>
        /// <returns>Un listado de entidades <see cref="Usuario"/> asignados a la orden.</returns>
        public async Task<IEnumerable<Usuario>> GetOrdenUsers(int idOrden, CancellationToken cancellationToken = default)
        {
            var users = await _context.Users
                .Where(u => _context.Usuarios_Ordenes
                    .Where(uo => uo.IdOrden == idOrden)
                    .Select(uo => uo.IdUsuario).Contains(u.Id)
                )
                .ToListAsync(cancellationToken);

            return users;
        }

        /// <summary>
        /// Obtiene un listado de usuarios disponibles para asignar a una orden, filtrados por rol y estado.
        /// </summary>
        /// <param name="idUser">Identificador del usuario que solicita la información.</param>
        /// <param name="idOrden">Identificador de la orden.</param>
        /// <param name="cancellationToken">Token de cancelación opcional.</param>
        /// <returns>Un listado de usuarios disponibles para la asignación a la orden.</returns>
        public async Task<IEnumerable<Usuario>> GetOrdenUsersAvailable(string idUser, int idOrden, CancellationToken cancellationToken = default)
        {
            var user = await _context.Users.Where(u => u.Id == idUser).Include(u => u.Rol).FirstOrDefaultAsync(cancellationToken);

            var query = _context.Users.Where(u => !_context.Usuarios_Ordenes.Where(uo => uo.IdOrden == idOrden).Select(uo => uo.IdUsuario).Contains(u.Id));

            if (user!.Rol.Name!.Contains("ADMINISTRADOR"))
            {
                var rolAdministrador = await _context.Roles.Where(r => r.Name == "ADMINISTRADOR").Select(r => r.Orden).FirstOrDefaultAsync();
                query = query.Where(u => u.Rol.Orden > rolAdministrador && u.EstadoUsuario.Name != "Borrado");

                if (user.Rol.Name == "ADMINISTRADOR") query = query.Where(u => u.IdEmpresa == user!.IdEmpresa);
            }
            else
            {
                query = query.Where(u => u.IdPlanta == user!.IdPlanta && u.EstadoUsuario.Name == "Activo");

                if (user.Rol.Name!.Contains("RESPONSABLE")) {
                    var rolResponsable = await _context.Roles.Where(r => r.Name == "RESPONSABLE").Select(r => r.Orden).FirstOrDefaultAsync();
                    query = query.Where(u => u.Rol.Orden >= rolResponsable);
                }
                else
                {
                    query = query.Where(u => u.Rol.Orden >= user.Rol.Orden);
                }
            }

            var datos = await query.ToListAsync(cancellationToken);

            return datos;
        }

        /// <summary>
        /// Obtiene un listado de usuarios responsables de materiales y otros roles relacionados.
        /// </summary>
        /// <param name="cancellationToken">Token de cancelación opcional.</param>
        /// <returns>Un listado de usuarios con roles relacionados con materiales.</returns>
        public async Task<IEnumerable<Usuario>> GetMaterialesUsersAsync(CancellationToken cancellationToken = default)
        {
            var usuariosMateriales = new[] { "responsable materiales", "responsable taller", "personal royse" };

            //var users = await _context.Users
            //    .Where(u => usuariosMateriales.Any(usuario => EF.Functions.Like((u.Nombre + " " + u.Apellidos).ToLower(), $"%{usuario}%")))
            //    .ToListAsync(cancellationToken);

            var users = await _context.Users.ToListAsync(cancellationToken);

            var filteredUsers = users
                                .Where(u => usuariosMateriales.Any(usuario =>
                                    (u.Nombre + " " + u.Apellidos).ToLower().Contains(usuario.ToLower())))
                                .ToList();

            return filteredUsers;
        }
    }
}
