using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddProiectSantierEchipaAngajatLucrare : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Proiecte",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nume = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Client = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataSfarsit = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Stare = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Proiecte", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Santier",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProiectId = table.Column<int>(type: "int", nullable: false),
                    Adresa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descriere = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Santier", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Santier_Proiecte_ProiectId",
                        column: x => x.ProiectId,
                        principalTable: "Proiecte",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Angajati",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EchipaId = table.Column<int>(type: "int", nullable: true),
                    Nume = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rol = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Competente = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Angajati", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Echipe",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nume = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SefEchipaId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Echipe", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Echipe_Angajati_SefEchipaId",
                        column: x => x.SefEchipaId,
                        principalTable: "Angajati",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Lucrari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SantierId = table.Column<int>(type: "int", nullable: false),
                    EchipaId = table.Column<int>(type: "int", nullable: true),
                    Descriere = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Termen = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Stare = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lucrari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Lucrari_Echipe_EchipaId",
                        column: x => x.EchipaId,
                        principalTable: "Echipe",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Lucrari_Santier_SantierId",
                        column: x => x.SantierId,
                        principalTable: "Santier",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Angajati_EchipaId",
                table: "Angajati",
                column: "EchipaId");

            migrationBuilder.CreateIndex(
                name: "IX_Echipe_SefEchipaId",
                table: "Echipe",
                column: "SefEchipaId");

            migrationBuilder.CreateIndex(
                name: "IX_Lucrari_EchipaId",
                table: "Lucrari",
                column: "EchipaId");

            migrationBuilder.CreateIndex(
                name: "IX_Lucrari_SantierId",
                table: "Lucrari",
                column: "SantierId");

            migrationBuilder.CreateIndex(
                name: "IX_Santier_ProiectId",
                table: "Santier",
                column: "ProiectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Angajati_Echipe_EchipaId",
                table: "Angajati",
                column: "EchipaId",
                principalTable: "Echipe",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Angajati_Echipe_EchipaId",
                table: "Angajati");

            migrationBuilder.DropTable(
                name: "Lucrari");

            migrationBuilder.DropTable(
                name: "Santier");

            migrationBuilder.DropTable(
                name: "Proiecte");

            migrationBuilder.DropTable(
                name: "Echipe");

            migrationBuilder.DropTable(
                name: "Angajati");
        }
    }
}
