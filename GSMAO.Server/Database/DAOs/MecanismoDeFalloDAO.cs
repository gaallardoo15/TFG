using GSMAO.Server.Database.Tables;
namespace GSMAO.Server.Database.DAOs
{
    /// <summary>
    /// Proporciona acceso a los datos de mecanismos de fallo en la base de datos.
    /// </summary>
    public class MecanismoDeFalloDAO: BaseDAO<MecanismoDeFallo, int>
    {
        public MecanismoDeFalloDAO(ApplicationDbContext context, ILogger<MecanismoDeFalloDAO> logger) : base(context, logger) { }
    }
}
