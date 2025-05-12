using System.ComponentModel.DataAnnotations;
using GSMAO.Server.Database.Tables;

namespace GSMAO.Server.DTOs
{
    /// <summary>
    /// Objeto de transferencia de datos utilizado para representar los datos básicos de una <see cref="Empresa"/>.
    /// </summary>
    public class BaseEmpresaDTO
    {
        /// <summary>
        /// Descripción de la empresa
        /// </summary>
        [Required(ErrorMessage = " El campo 'Descripcion' es obligatorio.")]
        public required string Descripcion { get; set; }
        
    }

    /// <summary>
    /// Objeto de transferencia de datos utilizado para representar los datos básicos para crear una <see cref="Empresa"/>.
    /// </summary>
    public class CreateEmpresaDTO :BaseEmpresaDTO { }

    /// <summary>
    /// Objeto de transferencia de datos utilizado para representar los datos necesarios de una <see cref="Empresa"/>, además de para actualizarla.
    /// </summary>
    public class EmpresaDTO : BaseEmpresaDTO
    {
        /// <summary>
        /// Identificador de la empresa
        /// </summary>
        public int Id { get; set; }
    }
}
