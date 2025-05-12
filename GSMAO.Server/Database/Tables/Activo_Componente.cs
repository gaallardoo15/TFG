using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace GSMAO.Server.Database.Tables
{
    /// <summary>
    /// Representa la entidad activo_componente de la base de datos.
    /// 
    /// Esta entidad es la relación entre activos y componentes,
    /// indica que componentes de primer nivel pertenecen a un activo.
    /// </summary>
    [PrimaryKey(nameof(IdActivo), nameof(IdComponente))]
    public class Activo_Componente
    {
        /// <summary>
        /// Identificador del activo
        /// </summary>
        public required int IdActivo { get; set; }

        /// <summary>
        /// Identificador del componente
        /// </summary>
        public required int IdComponente { get; set; }

        /// <summary>
        /// Activo al que pertenece el componente. Relación de clave externa con la tabla <see cref="Activo"/>
        /// </summary>
        [ForeignKey("IdActivo")]
        public required virtual Activo Activo { get; set; }

        /// <summary>
        /// Componente del activo. Relación de clave externa con la tabla <see cref="Componente"/>
        /// </summary>
        [ForeignKey("IdComponente")]
        public required virtual Componente Componente { get; set; }
    }
}
