using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GSMAO.Server.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddedFK_PlantaEmpresa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Plantas_Empresas_EmpresaId",
                table: "Plantas");

            migrationBuilder.RenameColumn(
                name: "EmpresaId",
                table: "Plantas",
                newName: "IdEmpresa");

            migrationBuilder.RenameIndex(
                name: "IX_Plantas_EmpresaId",
                table: "Plantas",
                newName: "IX_Plantas_IdEmpresa");

            migrationBuilder.AddForeignKey(
                name: "FK_Plantas_Empresas_IdEmpresa",
                table: "Plantas",
                column: "IdEmpresa",
                principalTable: "Empresas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Plantas_Empresas_IdEmpresa",
                table: "Plantas");

            migrationBuilder.RenameColumn(
                name: "IdEmpresa",
                table: "Plantas",
                newName: "EmpresaId");

            migrationBuilder.RenameIndex(
                name: "IX_Plantas_IdEmpresa",
                table: "Plantas",
                newName: "IX_Plantas_EmpresaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Plantas_Empresas_EmpresaId",
                table: "Plantas",
                column: "EmpresaId",
                principalTable: "Empresas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
