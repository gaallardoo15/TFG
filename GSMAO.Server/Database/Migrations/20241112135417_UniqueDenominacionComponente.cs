using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GSMAO.Server.Database.Migrations
{
    /// <inheritdoc />
    public partial class UniqueDenominacionComponente : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Denominacion",
                table: "Componentes",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Componentes_Denominacion",
                table: "Componentes",
                column: "Denominacion",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Componentes_Denominacion",
                table: "Componentes");

            migrationBuilder.AlterColumn<string>(
                name: "Denominacion",
                table: "Componentes",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
