using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GSMAO.Server.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddedRestTablesModuleCorrectivos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "UltimoAcceso",
                table: "Users",
                type: "datetime(6)",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.CreateTable(
                name: "Componentes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Denominacion = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DescripcionES = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DescripcionEN = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IdComponentePadre = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Componentes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Componentes_Componentes_IdComponentePadre",
                        column: x => x.IdComponentePadre,
                        principalTable: "Componentes",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "IncidenciasOrdenes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    FechaInsercion = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    FechaDeteccion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    FechaResolucion = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    IdOrden = table.Column<int>(type: "int", nullable: false),
                    IdComponente = table.Column<int>(type: "int", nullable: false),
                    IdIncidencia = table.Column<int>(type: "int", nullable: false),
                    IdResolucion = table.Column<int>(type: "int", nullable: true),
                    ParoMaquina = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CambioPieza = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    AfectaProduccion = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncidenciasOrdenes", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Ordenes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IdSAP = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FechaCreacion = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    FechaApertura = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    FechaCierre = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ComentarioOrden = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ComentarioResolucion = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Materiales = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TiempoParada = table.Column<float>(type: "float", nullable: true),
                    Confirmada = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IdActivo = table.Column<int>(type: "int", nullable: false),
                    IdEstadoOrden = table.Column<int>(type: "int", nullable: false),
                    IdUsuarioCreador = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IdTipoOrden = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ordenes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ordenes_Activos_IdActivo",
                        column: x => x.IdActivo,
                        principalTable: "Activos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Ordenes_EstadosOrden_IdEstadoOrden",
                        column: x => x.IdEstadoOrden,
                        principalTable: "EstadosOrden",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Ordenes_TiposOrden_IdTipoOrden",
                        column: x => x.IdTipoOrden,
                        principalTable: "TiposOrden",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Ordenes_Users_IdUsuarioCreador",
                        column: x => x.IdUsuarioCreador,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Activo_Componentes",
                columns: table => new
                {
                    IdActivo = table.Column<int>(type: "int", nullable: false),
                    IdComponente = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Activo_Componentes", x => new { x.IdActivo, x.IdComponente });
                    table.ForeignKey(
                        name: "FK_Activo_Componentes_Activos_IdActivo",
                        column: x => x.IdActivo,
                        principalTable: "Activos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Activo_Componentes_Componentes_IdComponente",
                        column: x => x.IdComponente,
                        principalTable: "Componentes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "HistorialCambiosUsuariosOrdenes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    FechaCambio = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IdOrden = table.Column<int>(type: "int", nullable: false),
                    IdUsuarioOrigen = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IdUsuarioDestino = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistorialCambiosUsuariosOrdenes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistorialCambiosUsuariosOrdenes_Ordenes_IdOrden",
                        column: x => x.IdOrden,
                        principalTable: "Ordenes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HistorialCambiosUsuariosOrdenes_Users_IdUsuarioDestino",
                        column: x => x.IdUsuarioDestino,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HistorialCambiosUsuariosOrdenes_Users_IdUsuarioOrigen",
                        column: x => x.IdUsuarioOrigen,
                        principalTable: "Users",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Usuarios_Ordenes",
                columns: table => new
                {
                    IdUsuario = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IdOrden = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios_Ordenes", x => new { x.IdUsuario, x.IdOrden });
                    table.ForeignKey(
                        name: "FK_Usuarios_Ordenes_Ordenes_IdOrden",
                        column: x => x.IdOrden,
                        principalTable: "Ordenes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Usuarios_Ordenes_Users_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Activo_Componentes_IdComponente",
                table: "Activo_Componentes",
                column: "IdComponente");

            migrationBuilder.CreateIndex(
                name: "IX_Componentes_IdComponentePadre",
                table: "Componentes",
                column: "IdComponentePadre");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialCambiosUsuariosOrdenes_IdOrden",
                table: "HistorialCambiosUsuariosOrdenes",
                column: "IdOrden");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialCambiosUsuariosOrdenes_IdUsuarioDestino",
                table: "HistorialCambiosUsuariosOrdenes",
                column: "IdUsuarioDestino");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialCambiosUsuariosOrdenes_IdUsuarioOrigen",
                table: "HistorialCambiosUsuariosOrdenes",
                column: "IdUsuarioOrigen");

            migrationBuilder.CreateIndex(
                name: "IX_Ordenes_IdActivo",
                table: "Ordenes",
                column: "IdActivo");

            migrationBuilder.CreateIndex(
                name: "IX_Ordenes_IdEstadoOrden",
                table: "Ordenes",
                column: "IdEstadoOrden");

            migrationBuilder.CreateIndex(
                name: "IX_Ordenes_IdTipoOrden",
                table: "Ordenes",
                column: "IdTipoOrden");

            migrationBuilder.CreateIndex(
                name: "IX_Ordenes_IdUsuarioCreador",
                table: "Ordenes",
                column: "IdUsuarioCreador");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Ordenes_IdOrden",
                table: "Usuarios_Ordenes",
                column: "IdOrden");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Activo_Componentes");

            migrationBuilder.DropTable(
                name: "HistorialCambiosUsuariosOrdenes");

            migrationBuilder.DropTable(
                name: "IncidenciasOrdenes");

            migrationBuilder.DropTable(
                name: "Usuarios_Ordenes");

            migrationBuilder.DropTable(
                name: "Componentes");

            migrationBuilder.DropTable(
                name: "Ordenes");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UltimoAcceso",
                table: "Users",
                type: "datetime(6)",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true)
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);
        }
    }
}
