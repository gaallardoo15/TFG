using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GSMAO.Server.Database.Migrations
{
    /// <inheritdoc />
    public partial class correctFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Planta_Empresa_EmpresaId",
                table: "Planta");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Empresa_IdEmpresa",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_EstadoUsuario_IdEstadoUsuario",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Planta_IdPlanta",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Planta",
                table: "Planta");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EstadoUsuario",
                table: "EstadoUsuario");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Empresa",
                table: "Empresa");

            migrationBuilder.RenameTable(
                name: "Planta",
                newName: "Plantas");

            migrationBuilder.RenameTable(
                name: "EstadoUsuario",
                newName: "EstadosUsuario");

            migrationBuilder.RenameTable(
                name: "Empresa",
                newName: "Empresas");

            migrationBuilder.RenameIndex(
                name: "IX_Planta_EmpresaId",
                table: "Plantas",
                newName: "IX_Plantas_EmpresaId");

            migrationBuilder.RenameIndex(
                name: "IX_Planta_Descripcion",
                table: "Plantas",
                newName: "IX_Plantas_Descripcion");

            migrationBuilder.RenameIndex(
                name: "IX_EstadoUsuario_Name",
                table: "EstadosUsuario",
                newName: "IX_EstadosUsuario_Name");

            migrationBuilder.RenameIndex(
                name: "IX_Empresa_Descripcion",
                table: "Empresas",
                newName: "IX_Empresas_Descripcion");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Plantas",
                table: "Plantas",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EstadosUsuario",
                table: "EstadosUsuario",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Empresas",
                table: "Empresas",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Plantas_Empresas_EmpresaId",
                table: "Plantas",
                column: "EmpresaId",
                principalTable: "Empresas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Empresas_IdEmpresa",
                table: "Users",
                column: "IdEmpresa",
                principalTable: "Empresas",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_EstadosUsuario_IdEstadoUsuario",
                table: "Users",
                column: "IdEstadoUsuario",
                principalTable: "EstadosUsuario",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Plantas_IdPlanta",
                table: "Users",
                column: "IdPlanta",
                principalTable: "Plantas",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Plantas_Empresas_EmpresaId",
                table: "Plantas");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Empresas_IdEmpresa",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_EstadosUsuario_IdEstadoUsuario",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Plantas_IdPlanta",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Plantas",
                table: "Plantas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EstadosUsuario",
                table: "EstadosUsuario");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Empresas",
                table: "Empresas");

            migrationBuilder.RenameTable(
                name: "Plantas",
                newName: "Planta");

            migrationBuilder.RenameTable(
                name: "EstadosUsuario",
                newName: "EstadoUsuario");

            migrationBuilder.RenameTable(
                name: "Empresas",
                newName: "Empresa");

            migrationBuilder.RenameIndex(
                name: "IX_Plantas_EmpresaId",
                table: "Planta",
                newName: "IX_Planta_EmpresaId");

            migrationBuilder.RenameIndex(
                name: "IX_Plantas_Descripcion",
                table: "Planta",
                newName: "IX_Planta_Descripcion");

            migrationBuilder.RenameIndex(
                name: "IX_EstadosUsuario_Name",
                table: "EstadoUsuario",
                newName: "IX_EstadoUsuario_Name");

            migrationBuilder.RenameIndex(
                name: "IX_Empresas_Descripcion",
                table: "Empresa",
                newName: "IX_Empresa_Descripcion");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Planta",
                table: "Planta",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EstadoUsuario",
                table: "EstadoUsuario",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Empresa",
                table: "Empresa",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Planta_Empresa_EmpresaId",
                table: "Planta",
                column: "EmpresaId",
                principalTable: "Empresa",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Empresa_IdEmpresa",
                table: "Users",
                column: "IdEmpresa",
                principalTable: "Empresa",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_EstadoUsuario_IdEstadoUsuario",
                table: "Users",
                column: "IdEstadoUsuario",
                principalTable: "EstadoUsuario",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Planta_IdPlanta",
                table: "Users",
                column: "IdPlanta",
                principalTable: "Planta",
                principalColumn: "Id");
        }
    }
}
