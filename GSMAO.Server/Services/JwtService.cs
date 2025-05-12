using GSMAO.Server.Database.Tables;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GSMAO.Server.Services
{
    /// <summary>
    /// Service para la gestión de tokens JWT.
    /// </summary>
    public class JwtService
    {
        private readonly IConfiguration _configuration;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Método que genera un token válido para un usuario.
        /// </summary>
        /// <param name="usuario">La entidad del <see cref="Usuario"/>.</param>
        /// <returns>El token generado a partir de la entidad <see cref="Usuario"/>.</returns>
        public string GenerateToken(Usuario usuario)
        {
            // Generate a JWT token
            var claims = new List<Claim>
            {
                new("user.id", usuario.Id),
                new("user.UserName", usuario.Email!),
                new("user.name", usuario.Nombre),
                new("user.lastname", usuario.Apellidos),
                new("user.rol.orden", usuario.Rol.Orden.ToString()),
                new(ClaimTypes.Role, usuario.Rol.Name!),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                //new("user.notify", user.AcceptsNotifications.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("Jwt")["Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
              issuer: _configuration.GetSection("Jwt")["Issuer"]!,
              audience: _configuration.GetSection("Jwt")["Audience"]!,
              claims: claims,
              expires: DateTime.Now.AddMinutes(int.Parse(_configuration.GetSection("Jwt")["Expiration"]!)),
              signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
