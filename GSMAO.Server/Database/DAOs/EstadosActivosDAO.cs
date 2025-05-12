using GSMAO.Server.Database.Tables;

namespace GSMAO.Server.Database.DAOs
{
    public class EstadosActivosDAO : BaseDAO<EstadoActivo, int>
    {
        public EstadosActivosDAO(ApplicationDbContext context, ILogger<EstadosActivosDAO> logger) : base(context, logger) { }
    }
}
