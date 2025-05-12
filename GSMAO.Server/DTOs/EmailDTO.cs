using System.ComponentModel.DataAnnotations;

namespace GSMAO.Server.DTOs
{
    public class EmailDTO
    {
        public required string Asunto { get; set; }
        public required string Mensaje { get; set; }
        public required string Nombre { get; set; }
        public string? Telefono { get; set; }
    }
}
