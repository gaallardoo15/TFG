using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GSMAO.Server.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddedSiglasToCriticidad : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Siglas",
                table: "Criticidades",
                type: "varchar(255)",
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Criticidades_Siglas",
                table: "Criticidades",
                column: "Siglas",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Criticidades_Siglas",
                table: "Criticidades");

            migrationBuilder.DropColumn(
                name: "Siglas",
                table: "Criticidades");
        }
    }
}
