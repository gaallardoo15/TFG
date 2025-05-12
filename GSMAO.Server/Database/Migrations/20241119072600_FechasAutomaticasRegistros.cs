using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GSMAO.Server.Database.Migrations
{
    /// <inheritdoc />
    public partial class FechasAutomaticasRegistros : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCreacion",
                table: "Users",
                type: "datetime(6)",
                nullable: false)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<DateTime>(
                name: "UltimaModificacion",
                table: "Users",
                type: "datetime(6)",
                nullable: false)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCreacion",
                table: "Resoluciones",
                type: "datetime(6)",
                nullable: false)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<DateTime>(
                name: "UltimaModificacion",
                table: "Resoluciones",
                type: "datetime(6)",
                nullable: false)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCreacion",
                table: "Repuestos",
                type: "datetime(6)",
                nullable: false)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<DateTime>(
                name: "UltimaModificacion",
                table: "Repuestos",
                type: "datetime(6)",
                nullable: false)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCreacion",
                table: "Plantas",
                type: "datetime(6)",
                nullable: false)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<DateTime>(
                name: "UltimaModificacion",
                table: "Plantas",
                type: "datetime(6)",
                nullable: false)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn);

            migrationBuilder.AddColumn<DateTime>(
                name: "UltimaModificacion",
                table: "Ordenes",
                type: "datetime(6)",
                nullable: false)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCreacion",
                table: "MecanismosDeFallo",
                type: "datetime(6)",
                nullable: false)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<DateTime>(
                name: "UltimaModificacion",
                table: "MecanismosDeFallo",
                type: "datetime(6)",
                nullable: false)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCreacion",
                table: "Localizaciones",
                type: "datetime(6)",
                nullable: false)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<DateTime>(
                name: "UltimaModificacion",
                table: "Localizaciones",
                type: "datetime(6)",
                nullable: false)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCreacion",
                table: "Incidencias",
                type: "datetime(6)",
                nullable: false)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<DateTime>(
                name: "UltimaModificacion",
                table: "Incidencias",
                type: "datetime(6)",
                nullable: false)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCreacion",
                table: "Componentes",
                type: "datetime(6)",
                nullable: false)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<DateTime>(
                name: "UltimaModificacion",
                table: "Componentes",
                type: "datetime(6)",
                nullable: false)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCreacion",
                table: "CentrosDeCostes",
                type: "datetime(6)",
                nullable: false)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<DateTime>(
                name: "UltimaModificacion",
                table: "CentrosDeCostes",
                type: "datetime(6)",
                nullable: false)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCreacion",
                table: "Almacenes",
                type: "datetime(6)",
                nullable: false)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<DateTime>(
                name: "UltimaModificacion",
                table: "Almacenes",
                type: "datetime(6)",
                nullable: false)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCreacion",
                table: "Activos",
                type: "datetime(6)",
                nullable: false)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<DateTime>(
                name: "UltimaModificacion",
                table: "Activos",
                type: "datetime(6)",
                nullable: false)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FechaCreacion",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UltimaModificacion",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "FechaCreacion",
                table: "Resoluciones");

            migrationBuilder.DropColumn(
                name: "UltimaModificacion",
                table: "Resoluciones");

            migrationBuilder.DropColumn(
                name: "FechaCreacion",
                table: "Repuestos");

            migrationBuilder.DropColumn(
                name: "UltimaModificacion",
                table: "Repuestos");

            migrationBuilder.DropColumn(
                name: "FechaCreacion",
                table: "Plantas");

            migrationBuilder.DropColumn(
                name: "UltimaModificacion",
                table: "Plantas");

            migrationBuilder.DropColumn(
                name: "UltimaModificacion",
                table: "Ordenes");

            migrationBuilder.DropColumn(
                name: "FechaCreacion",
                table: "MecanismosDeFallo");

            migrationBuilder.DropColumn(
                name: "UltimaModificacion",
                table: "MecanismosDeFallo");

            migrationBuilder.DropColumn(
                name: "FechaCreacion",
                table: "Localizaciones");

            migrationBuilder.DropColumn(
                name: "UltimaModificacion",
                table: "Localizaciones");

            migrationBuilder.DropColumn(
                name: "FechaCreacion",
                table: "Incidencias");

            migrationBuilder.DropColumn(
                name: "UltimaModificacion",
                table: "Incidencias");

            migrationBuilder.DropColumn(
                name: "FechaCreacion",
                table: "Componentes");

            migrationBuilder.DropColumn(
                name: "UltimaModificacion",
                table: "Componentes");

            migrationBuilder.DropColumn(
                name: "FechaCreacion",
                table: "CentrosDeCostes");

            migrationBuilder.DropColumn(
                name: "UltimaModificacion",
                table: "CentrosDeCostes");

            migrationBuilder.DropColumn(
                name: "FechaCreacion",
                table: "Almacenes");

            migrationBuilder.DropColumn(
                name: "UltimaModificacion",
                table: "Almacenes");

            migrationBuilder.DropColumn(
                name: "FechaCreacion",
                table: "Activos");

            migrationBuilder.DropColumn(
                name: "UltimaModificacion",
                table: "Activos");
        }
    }
}
