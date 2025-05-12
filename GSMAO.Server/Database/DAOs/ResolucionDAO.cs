using GSMAO.Server.Database.Tables;
namespace GSMAO.Server.Database.DAOs
{
    /// <summary>
    /// Proporciona acceso a los datos de resoluciones en la base de datos.
    /// </summary>
    public class ResolucionDAO : BaseDAO<Resolucion, int>
    {
        public ResolucionDAO(ApplicationDbContext context, ILogger<ResolucionDAO> logger) : base(context, logger) { }
    }
}
