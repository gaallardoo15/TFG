using GSMAO.Server.Database.Tables;

namespace GSMAO.Server.Database.DAOs
{
    public class ActivoComponenteDAO : BaseDAO<Activo_Componente, (int, int)>
    {
        public ActivoComponenteDAO(ApplicationDbContext context, ILogger<ActivoComponenteDAO> logger) : base(context, logger) { }
    }
}
