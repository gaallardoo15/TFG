using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GSMAO.Server.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddedIndexUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "TiposOrden",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "DescripcionES",
                table: "Resoluciones",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "DescripcionES",
                table: "MecanismosDeFallo",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "LocalizacionSAP",
                table: "Localizaciones",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "DescripcionES",
                table: "Localizaciones",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "DescripcionES",
                table: "Incidencias",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "EstadosRepuesto",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "EstadosOrden",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "EstadosActivo",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "DescripcionES",
                table: "CentrosDeCostes",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "CentroCosteSAP",
                table: "CentrosDeCostes",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_TiposOrden_Name",
                table: "TiposOrden",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Resoluciones_DescripcionES",
                table: "Resoluciones",
                column: "DescripcionES",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MecanismosDeFallo_DescripcionES",
                table: "MecanismosDeFallo",
                column: "DescripcionES",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Localizaciones_DescripcionES",
                table: "Localizaciones",
                column: "DescripcionES",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Localizaciones_LocalizacionSAP",
                table: "Localizaciones",
                column: "LocalizacionSAP",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Incidencias_DescripcionES",
                table: "Incidencias",
                column: "DescripcionES",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EstadosRepuesto_Name",
                table: "EstadosRepuesto",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EstadosOrden_Name",
                table: "EstadosOrden",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EstadosActivo_Name",
                table: "EstadosActivo",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CentrosDeCostes_CentroCosteSAP",
                table: "CentrosDeCostes",
                column: "CentroCosteSAP",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CentrosDeCostes_DescripcionES",
                table: "CentrosDeCostes",
                column: "DescripcionES",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TiposOrden_Name",
                table: "TiposOrden");

            migrationBuilder.DropIndex(
                name: "IX_Resoluciones_DescripcionES",
                table: "Resoluciones");

            migrationBuilder.DropIndex(
                name: "IX_MecanismosDeFallo_DescripcionES",
                table: "MecanismosDeFallo");

            migrationBuilder.DropIndex(
                name: "IX_Localizaciones_DescripcionES",
                table: "Localizaciones");

            migrationBuilder.DropIndex(
                name: "IX_Localizaciones_LocalizacionSAP",
                table: "Localizaciones");

            migrationBuilder.DropIndex(
                name: "IX_Incidencias_DescripcionES",
                table: "Incidencias");

            migrationBuilder.DropIndex(
                name: "IX_EstadosRepuesto_Name",
                table: "EstadosRepuesto");

            migrationBuilder.DropIndex(
                name: "IX_EstadosOrden_Name",
                table: "EstadosOrden");

            migrationBuilder.DropIndex(
                name: "IX_EstadosActivo_Name",
                table: "EstadosActivo");

            migrationBuilder.DropIndex(
                name: "IX_CentrosDeCostes_CentroCosteSAP",
                table: "CentrosDeCostes");

            migrationBuilder.DropIndex(
                name: "IX_CentrosDeCostes_DescripcionES",
                table: "CentrosDeCostes");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "TiposOrden",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "DescripcionES",
                table: "Resoluciones",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "DescripcionES",
                table: "MecanismosDeFallo",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "LocalizacionSAP",
                table: "Localizaciones",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "DescripcionES",
                table: "Localizaciones",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "DescripcionES",
                table: "Incidencias",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "EstadosRepuesto",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "EstadosOrden",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "EstadosActivo",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "DescripcionES",
                table: "CentrosDeCostes",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "CentroCosteSAP",
                table: "CentrosDeCostes",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
