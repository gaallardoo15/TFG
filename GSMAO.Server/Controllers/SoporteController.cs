using GSMAO.Server.Database.Tables;
using GSMAO.Server.DTOs;
using GSMAO.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace GSMAO.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SoporteController(ILoggerFactory loggerFactory, IMyEmailSender emailSender) : ControllerBase
    {
        private readonly ILogger<SoporteController> _logger = loggerFactory.CreateLogger<SoporteController>();

        [HttpPost]
        public async Task<ActionResult> SendEmail(EmailDTO emailDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Obtengo el email del usuario del token
            var userEmail = User.Claims.Where(u => u.Type == "user.UserName").Select(u => u.Value).FirstOrDefault();

            // Construyo y envío el mensaje
            try
            {
                var htmlMessage = "";
                if (emailDTO.Telefono != "")
                {
                    htmlMessage = $@"<h2>Incidencia detectada</h2>
                                <p><b>Nombre: </b>{emailDTO.Nombre}</p>
                                <p><b>Email: </b>{userEmail}</p>
                                <p><b>Teléfono: </b>{emailDTO.Telefono}</p>
                                <br>
                                <p>{emailDTO.Mensaje}</p>";
                }
                else
                {
                    htmlMessage = $@"<h2>Incidencia detectada</h2>
                                <p><b>Nombre: </b>{emailDTO.Nombre}</p>
                                <p><b>Email: </b>{userEmail}</p>
                                <br>
                                <p>{emailDTO.Mensaje}</p>";
                }

                await emailSender.SendEmailAsync("soporte@suitelec.com", emailDTO.Asunto, htmlMessage, emailDTO.Nombre);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ocurrió un problema al enviar el correo electrónico a Soporte Suitelec. {ex}");
                return StatusCode(500, "Ocurrió un problema al enviar el correo electrónico a Soporte Suitelec. Intenta nuevamente. Si continúa, contacta con SUITELEC al correo eléctronico soporte@suitelec.com");
            }
        }
    }
}
