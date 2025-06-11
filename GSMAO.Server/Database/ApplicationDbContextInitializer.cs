using GSMAO.Server.Database.Tables;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GSMAO.Server.Database
{
    /// <summary>
    /// Define la funcionalidad básica para un inicializador de DbContext.
    /// </summary>
    public interface IDefaultDbContextInitializer
    {
        bool EnsureCreated();
        Task SeedAsync();
    }

    public class ApplicationDbContextInitializer : IDefaultDbContextInitializer
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Usuario> _userManager;

        public ApplicationDbContextInitializer(ApplicationDbContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        /// <summary>
        /// Verifica y crea la base de datos si no existe.
        /// </summary>
        public bool EnsureCreated() => _context.Database.EnsureCreated();

        /// <summary>
        /// Añade datos iniciales a la base de datos de manera asíncrona.
        /// </summary>
        public async Task SeedAsync()
        {
            await SeedEstadosUsuarios();
            await SeedRoles();
            await SeedEmpresas();
            await SeedPlantas();
            await SeedEstadosActivos();
            await SeedCriticidades();

            await _context.SaveChangesAsync();

            await SeedUsuarios();
        }

        public async Task SeedUsuarios()
        {
            // Inicializar usuario superadministrador si no existen usuarios.
            if (!_context.Usuarios.Any())
            {
                var activo = await _context.EstadosUsuario.Where(r => r.Name == "Activo").FirstOrDefaultAsync();
                var rolSuperAdmin = await _context.Roles.Where(r => r.Name == "SUPER_ADMINISTRADOR").FirstOrDefaultAsync();
                var usuario = new Usuario
                {
                    Nombre = "Usuario",
                    Apellidos = "GMAO",
                    Email = "usuario@gmao.com",
                    UserName = "usuario@gmao.com",
                    Confirmado = 1,
                    IdRol = rolSuperAdmin!.Id,
                    IdEstadoUsuario = activo!.Id,
                    Rol = rolSuperAdmin,
                    EstadoUsuario = activo
                };
                await _userManager.CreateAsync(usuario, "superadmin@123");
                await _context.SaveChangesAsync();
            }
        }

        public async Task SeedEstadosUsuarios()
        {
            // Inicializar valores predeterminados para EstadoPuesto si no existen.
            if (!_context.EstadosUsuario.Any())
            {
                var estadosIniciales = new List<EstadoUsuario>
                {
                    new EstadoUsuario { Name = "Activo" },
                    new EstadoUsuario { Name = "Inactivo" },
                    new EstadoUsuario { Name = "Borrado" }
                };
                await _context.EstadosUsuario.AddRangeAsync(estadosIniciales);
                await _context.SaveChangesAsync();
            }
        }

        public async Task SeedRoles()
        {
            // Inicializar valores predeterminados para Rol si no existen.
            if (!_context.Roles.Any())
            {
                var estadosIniciales = new List<Rol>
                {
                    new Rol { Name = "SUPER_ADMINISTRADOR", Orden = 1 },
                    new Rol { Name = "ADMINISTRADOR", Orden = 2 },
                    new Rol { Name = "JEFE_MANTENIMIENTO", Orden = 3 },
                    new Rol { Name = "RESPONSABLE", Orden = 4 },
                    new Rol { Name = "OPERARIO", Orden = 7 },
                    new Rol { Name = "RESPONSABLE_TALLER", Orden = 6 },
                    new Rol { Name = "RESPONSABLE_MATERIALES", Orden = 5 }
                };
                await _context.Roles.AddRangeAsync(estadosIniciales);
                await _context.SaveChangesAsync();
            }
        }

        public async Task SeedEmpresas()
        {
            // Inicializar valores predeterminados para Empresas si no existen.
            if (!_context.Empresas.Any())
            {
                var hitachi = new List<Empresa>
                {
                    new Empresa { Descripcion = "GMAO" }
                };
                await _context.Empresas.AddRangeAsync(hitachi);
                await _context.SaveChangesAsync();
            }
        }

        public async Task SeedPlantas()
        {
            // Inicializar valores predeterminados para Plantas si no existen.
            if (!_context.Plantas.Any())
            {
                var empresa = await _context.Empresas.Where(r => r.Descripcion == "GMAO").FirstOrDefaultAsync();

                var hitachi = new List<Planta>
                {
                    new Planta { IdEmpresa = empresa!.Id, Descripcion = "Planta de Córdoba", Empresa = empresa }
                };
                await _context.Plantas.AddRangeAsync(hitachi);
                await _context.SaveChangesAsync();
            }
        }

        public async Task SeedEstadosActivos()
        {
            // Inicializar valores predeterminados para EstadoPuesto si no existen.
            if (!_context.EstadosActivo.Any())
            {
                var estadosIniciales = new List<EstadoActivo>
                {
                    new EstadoActivo { Name = "Activo" },
                    new EstadoActivo { Name = "Inactivo" },
                    new EstadoActivo { Name = "Borrado" }
                };
                await _context.EstadosActivo.AddRangeAsync(estadosIniciales);
                await _context.SaveChangesAsync();
            }
        }

        public async Task SeedCriticidades()
        {
            // Inicializar valores predeterminados para EstadoPuesto si no existen.
            if (!_context.Criticidades.Any())
            {
                var criticidadesIniciales = new List<Criticidad>
                {
                    new Criticidad { Siglas = "MC", Descripcion = "Muy crítico" },
                    new Criticidad { Siglas = "CA", Descripcion = "Criticidad Alta" },
                    new Criticidad { Siglas = "CM", Descripcion = "Criticidad Media" },
                    new Criticidad { Siglas = "CB", Descripcion = "Criticidad Baja" },
                    new Criticidad { Siglas = "SC", Descripcion = "Sin criticidad" }
                };
                await _context.Criticidades.AddRangeAsync(criticidadesIniciales);
                await _context.SaveChangesAsync();
            }
        }
    }
}
