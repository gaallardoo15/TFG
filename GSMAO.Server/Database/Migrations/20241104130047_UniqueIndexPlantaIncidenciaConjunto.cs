using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GSMAO.Server.Database.Migrations
{
    /// <inheritdoc />
    public partial class UniqueIndexPlantaIncidenciaConjunto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Plantas_Descripcion",
                table: "Plantas");

            migrationBuilder.DropIndex(
                name: "IX_Incidencias_DescripcionES",
                table: "Incidencias");

            migrationBuilder.CreateIndex(
                name: "IX_Plantas_Descripcion_IdEmpresa",
                table: "Plantas",
                columns: new[] { "Descripcion", "IdEmpresa" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Incidencias_DescripcionES_IdMecanismoFallo",
                table: "Incidencias",
                columns: new[] { "DescripcionES", "IdMecanismoFallo" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Plantas_Descripcion_IdEmpresa",
                table: "Plantas");

            migrationBuilder.DropIndex(
                name: "IX_Incidencias_DescripcionES_IdMecanismoFallo",
                table: "Incidencias");

            migrationBuilder.CreateIndex(
                name: "IX_Plantas_Descripcion",
                table: "Plantas",
                column: "Descripcion",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Incidencias_DescripcionES",
                table: "Incidencias",
                column: "DescripcionES",
                unique: true);
        }
    }
}
