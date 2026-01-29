using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class LucrareEchipaManyToMany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lucrari_Echipe_EchipaId",
                table: "Lucrari");

            migrationBuilder.DropIndex(
                name: "IX_Lucrari_EchipaId",
                table: "Lucrari");

            migrationBuilder.DropColumn(
                name: "EchipaId",
                table: "Lucrari");

            migrationBuilder.CreateTable(
                name: "LucrareEchipa",
                columns: table => new
                {
                    EchipeId = table.Column<int>(type: "int", nullable: false),
                    LucrariId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LucrareEchipa", x => new { x.EchipeId, x.LucrariId });
                    table.ForeignKey(
                        name: "FK_LucrareEchipa_Echipe_EchipeId",
                        column: x => x.EchipeId,
                        principalTable: "Echipe",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LucrareEchipa_Lucrari_LucrariId",
                        column: x => x.LucrariId,
                        principalTable: "Lucrari",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LucrareEchipa_LucrariId",
                table: "LucrareEchipa",
                column: "LucrariId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LucrareEchipa");

            migrationBuilder.AddColumn<int>(
                name: "EchipaId",
                table: "Lucrari",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Lucrari_EchipaId",
                table: "Lucrari",
                column: "EchipaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Lucrari_Echipe_EchipaId",
                table: "Lucrari",
                column: "EchipaId",
                principalTable: "Echipe",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
