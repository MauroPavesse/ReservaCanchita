using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReservaCanchita.Migrations
{
    /// <inheritdoc />
    public partial class ArreglosTablas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservas_Horarios_HorarioId",
                table: "Reservas");

            migrationBuilder.DropTable(
                name: "Horarios");

            migrationBuilder.DropIndex(
                name: "IX_Reservas_HorarioId",
                table: "Reservas");

            migrationBuilder.RenameColumn(
                name: "HorarioId",
                table: "Reservas",
                newName: "HorarioDisponibleId");

            migrationBuilder.CreateTable(
                name: "HorariosBase",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CanchaId = table.Column<int>(type: "int", nullable: false),
                    HoraInicio = table.Column<TimeSpan>(type: "time(6)", nullable: false),
                    HoraFin = table.Column<TimeSpan>(type: "time(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HorariosBase", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HorariosBase_Canchas_CanchaId",
                        column: x => x.CanchaId,
                        principalTable: "Canchas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "HorariosDisponibles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CanchaId = table.Column<int>(type: "int", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    HoraInicio = table.Column<TimeSpan>(type: "time(6)", nullable: false),
                    HoraFin = table.Column<TimeSpan>(type: "time(6)", nullable: false),
                    EstaReservado = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HorariosDisponibles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HorariosDisponibles_Canchas_CanchaId",
                        column: x => x.CanchaId,
                        principalTable: "Canchas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_HorarioDisponibleId",
                table: "Reservas",
                column: "HorarioDisponibleId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HorariosBase_CanchaId",
                table: "HorariosBase",
                column: "CanchaId");

            migrationBuilder.CreateIndex(
                name: "IX_HorariosDisponibles_CanchaId",
                table: "HorariosDisponibles",
                column: "CanchaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservas_HorariosDisponibles_HorarioDisponibleId",
                table: "Reservas",
                column: "HorarioDisponibleId",
                principalTable: "HorariosDisponibles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservas_HorariosDisponibles_HorarioDisponibleId",
                table: "Reservas");

            migrationBuilder.DropTable(
                name: "HorariosBase");

            migrationBuilder.DropTable(
                name: "HorariosDisponibles");

            migrationBuilder.DropIndex(
                name: "IX_Reservas_HorarioDisponibleId",
                table: "Reservas");

            migrationBuilder.RenameColumn(
                name: "HorarioDisponibleId",
                table: "Reservas",
                newName: "HorarioId");

            migrationBuilder.CreateTable(
                name: "Horarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CanchaId = table.Column<int>(type: "int", nullable: false),
                    HoraFin = table.Column<TimeSpan>(type: "time(6)", nullable: false),
                    HoraInicio = table.Column<TimeSpan>(type: "time(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Horarios", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_HorarioId",
                table: "Reservas",
                column: "HorarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservas_Horarios_HorarioId",
                table: "Reservas",
                column: "HorarioId",
                principalTable: "Horarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
