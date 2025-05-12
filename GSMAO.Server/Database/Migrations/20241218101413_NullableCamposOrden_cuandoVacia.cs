using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GSMAO.Server.Database.Migrations
{
    /// <inheritdoc />
    public partial class NullableCamposOrden_cuandoVacia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ordenes_Activos_IdActivo",
                table: "Ordenes");

            migrationBuilder.DropForeignKey(
                name: "FK_Ordenes_EstadosOrden_IdEstadoOrden",
                table: "Ordenes");

            migrationBuilder.DropForeignKey(
                name: "FK_Ordenes_TiposOrden_IdTipoOrden",
                table: "Ordenes");

            migrationBuilder.AlterColumn<int>(
                name: "IdTipoOrden",
                table: "Ordenes",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "IdEstadoOrden",
                table: "Ordenes",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "IdActivo",
                table: "Ordenes",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaApertura",
                table: "Ordenes",
                type: "datetime(6)",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AddForeignKey(
                name: "FK_Ordenes_Activos_IdActivo",
                table: "Ordenes",
                column: "IdActivo",
                principalTable: "Activos",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Ordenes_EstadosOrden_IdEstadoOrden",
                table: "Ordenes",
                column: "IdEstadoOrden",
                principalTable: "EstadosOrden",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Ordenes_TiposOrden_IdTipoOrden",
                table: "Ordenes",
                column: "IdTipoOrden",
                principalTable: "TiposOrden",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ordenes_Activos_IdActivo",
                table: "Ordenes");

            migrationBuilder.DropForeignKey(
                name: "FK_Ordenes_EstadosOrden_IdEstadoOrden",
                table: "Ordenes");

            migrationBuilder.DropForeignKey(
                name: "FK_Ordenes_TiposOrden_IdTipoOrden",
                table: "Ordenes");

            migrationBuilder.AlterColumn<int>(
                name: "IdTipoOrden",
                table: "Ordenes",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "IdEstadoOrden",
                table: "Ordenes",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "IdActivo",
                table: "Ordenes",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaApertura",
                table: "Ordenes",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Ordenes_Activos_IdActivo",
                table: "Ordenes",
                column: "IdActivo",
                principalTable: "Activos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Ordenes_EstadosOrden_IdEstadoOrden",
                table: "Ordenes",
                column: "IdEstadoOrden",
                principalTable: "EstadosOrden",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Ordenes_TiposOrden_IdTipoOrden",
                table: "Ordenes",
                column: "IdTipoOrden",
                principalTable: "TiposOrden",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
