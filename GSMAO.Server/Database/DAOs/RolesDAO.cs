using GSMAO.Server.Database.Tables;
using Microsoft.EntityFrameworkCore;

namespace GSMAO.Server.Database.DAOs
{
    /// <summary>
    /// Proporciona acceso a los datos de roles en la base de datos.
    /// </summary>
    public class RolesDAO : BaseDAO<Rol, string>
    {
        public RolesDAO(
            ApplicationDbContext context,
            ILogger<RolesDAO> logger
        ) : base(context, logger) { }

        /// <summary>
        /// Obtiene un listado de roles que no sean de administración.
        /// </summary>
        /// <returns>Un listado de entidades <see cref="Rol"/> encontradas.</returns>
        public async Task<IEnumerable<Rol>> GetNotAdminRolesAsync()
        {
            return await _context.Roles.Where(r => r.Name != "SUPER_ADMINISTRADOR" && r.Name != "ADMINISTRADOR").ToListAsync();
        }
    }
}
