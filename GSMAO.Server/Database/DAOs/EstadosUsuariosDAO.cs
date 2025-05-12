using GSMAO.Server.Database.Tables;
using Microsoft.EntityFrameworkCore;

namespace GSMAO.Server.Database.DAOs
{
    /// <summary>
    /// Proporciona acceso a los datos de estados en la base de datos.
    /// </summary>
    public class EstadosUsuariosDAO : BaseDAO<EstadoUsuario, int>
    {
        public EstadosUsuariosDAO(ApplicationDbContext context, ILogger<EstadosUsuariosDAO> logger) : base(context, logger) { }
    }
}
