using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class UtilizatorAngajatRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AngajatId",
                table: "Utilizatori",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Utilizatori_AngajatId",
                table: "Utilizatori",
                column: "AngajatId",
                unique: true,
                filter: "[AngajatId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Utilizatori_Angajati_AngajatId",
                table: "Utilizatori",
                column: "AngajatId",
                principalTable: "Angajati",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Utilizatori_Angajati_AngajatId",
                table: "Utilizatori");

            migrationBuilder.DropIndex(
                name: "IX_Utilizatori_AngajatId",
                table: "Utilizatori");

            migrationBuilder.DropColumn(
                name: "AngajatId",
                table: "Utilizatori");
        }
    }
}
