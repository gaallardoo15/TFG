using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace GSMAO.Server.Database.Tables
{
    /// <summary>
    /// Representa la entidad empresa de la base de datos.
    /// </summary>
    [PrimaryKey(nameof(Id))]
    [Index(nameof(Descripcion), IsUnique = true)]
    public class Empresa
    {
        /// <summary>
        /// Identificador de la empresa
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nombre de la empresa
        /// </summary>
        public required string Descripcion { get; set; }
    }
}
