using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GSMAO.Server.Database.Migrations
{
    /// <inheritdoc />
    public partial class IndexIncidenciasOrdenes_IdOrden : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_IncidenciasOrdenes_IdComponente",
                table: "IncidenciasOrdenes",
                column: "IdComponente");

            migrationBuilder.CreateIndex(
                name: "IX_IncidenciasOrdenes_IdIncidencia",
                table: "IncidenciasOrdenes",
                column: "IdIncidencia");

            migrationBuilder.CreateIndex(
                name: "IX_IncidenciasOrdenes_IdOrden",
                table: "IncidenciasOrdenes",
                column: "IdOrden");

            migrationBuilder.CreateIndex(
                name: "IX_IncidenciasOrdenes_IdResolucion",
                table: "IncidenciasOrdenes",
                column: "IdResolucion");

            migrationBuilder.AddForeignKey(
                name: "FK_IncidenciasOrdenes_Componentes_IdComponente",
                table: "IncidenciasOrdenes",
                column: "IdComponente",
                principalTable: "Componentes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_IncidenciasOrdenes_Incidencias_IdIncidencia",
                table: "IncidenciasOrdenes",
                column: "IdIncidencia",
                principalTable: "Incidencias",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_IncidenciasOrdenes_Ordenes_IdOrden",
                table: "IncidenciasOrdenes",
                column: "IdOrden",
                principalTable: "Ordenes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_IncidenciasOrdenes_Resoluciones_IdResolucion",
                table: "IncidenciasOrdenes",
                column: "IdResolucion",
                principalTable: "Resoluciones",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IncidenciasOrdenes_Componentes_IdComponente",
                table: "IncidenciasOrdenes");

            migrationBuilder.DropForeignKey(
                name: "FK_IncidenciasOrdenes_Incidencias_IdIncidencia",
                table: "IncidenciasOrdenes");

            migrationBuilder.DropForeignKey(
                name: "FK_IncidenciasOrdenes_Ordenes_IdOrden",
                table: "IncidenciasOrdenes");

            migrationBuilder.DropForeignKey(
                name: "FK_IncidenciasOrdenes_Resoluciones_IdResolucion",
                table: "IncidenciasOrdenes");

            migrationBuilder.DropIndex(
                name: "IX_IncidenciasOrdenes_IdComponente",
                table: "IncidenciasOrdenes");

            migrationBuilder.DropIndex(
                name: "IX_IncidenciasOrdenes_IdIncidencia",
                table: "IncidenciasOrdenes");

            migrationBuilder.DropIndex(
                name: "IX_IncidenciasOrdenes_IdOrden",
                table: "IncidenciasOrdenes");

            migrationBuilder.DropIndex(
                name: "IX_IncidenciasOrdenes_IdResolucion",
                table: "IncidenciasOrdenes");
        }
    }
}
