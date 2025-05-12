using GSMAO.Server.Database.Tables;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GSMAO.Server.Database.DAOs
{
    /// <summary>
    /// Proporciona acceso a los datos de empresas en la base de datos.
    /// </summary>
    public class EmpresaDAO : BaseDAO<Empresa, int>
    {
        private readonly UsersDAO _userDAO;

        public EmpresaDAO(ApplicationDbContext context, ILogger<EmpresaDAO> logger, UsersDAO userDAO) : base(context, logger)
        {
            _userDAO = userDAO;
        }

        /// <summary>
        /// Obtiene una empresa según el identificador del usuario indicado.
        /// </summary>
        /// <param name="idUser">El identificador del usuario.</param>
        /// <returns>La entidad <see cref="Empresa"/> encontrada.</returns>
        /// <exception cref="KeyNotFoundException">Cuando no se encuentra la entidad <see cref="Empresa"/> del usuario.</exception>
        public async Task<Empresa> GetEmpresaAsync(string idUser)
        {
            var user = await _userDAO.GetByUserOrEmailOrIdAsync(idUser);

            if (user != null && user.Empresa == null)
                throw new KeyNotFoundException($"No se encontró la empresa del usuario.");

            return user!.Empresa!;
        }
    }
}
