using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GSMAO.Server.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddedActivoTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Activos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ActivoSAP = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DescripcionES = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DescripcionEN = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Redundancia = table.Column<int>(type: "int", nullable: true),
                    Hse = table.Column<int>(type: "int", nullable: true),
                    Usabilidad = table.Column<int>(type: "int", nullable: true),
                    Coste = table.Column<int>(type: "int", nullable: true),
                    ValorCriticidad = table.Column<int>(type: "int", nullable: false),
                    IdCriticidad = table.Column<int>(type: "int", nullable: false),
                    IdLocalizacion = table.Column<int>(type: "int", nullable: false),
                    IdCentroCoste = table.Column<int>(type: "int", nullable: false),
                    IdEstadoActivo = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Activos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Activos_CentrosDeCostes_IdCentroCoste",
                        column: x => x.IdCentroCoste,
                        principalTable: "CentrosDeCostes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Activos_Criticidades_IdCriticidad",
                        column: x => x.IdCriticidad,
                        principalTable: "Criticidades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Activos_EstadosActivo_IdEstadoActivo",
                        column: x => x.IdEstadoActivo,
                        principalTable: "EstadosActivo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Activos_Localizaciones_IdLocalizacion",
                        column: x => x.IdLocalizacion,
                        principalTable: "Localizaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Activos_IdCentroCoste",
                table: "Activos",
                column: "IdCentroCoste");

            migrationBuilder.CreateIndex(
                name: "IX_Activos_IdCriticidad",
                table: "Activos",
                column: "IdCriticidad");

            migrationBuilder.CreateIndex(
                name: "IX_Activos_IdEstadoActivo",
                table: "Activos",
                column: "IdEstadoActivo");

            migrationBuilder.CreateIndex(
                name: "IX_Activos_IdLocalizacion",
                table: "Activos",
                column: "IdLocalizacion");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Activos");
        }
    }
}
