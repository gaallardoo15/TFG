using Microsoft.AspNetCore.Identity;

namespace GSMAO.Server.Database.Tables
{
    /// <summary>
    /// Representa la entidad rol de la base de datos.
    /// Hereda de <see cref="IdentityRole"/>
    /// </summary>
    public class Rol : IdentityRole
    {
        public required int Orden { get; set; }
    }
}
