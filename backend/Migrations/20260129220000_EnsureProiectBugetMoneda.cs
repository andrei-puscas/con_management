using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    [Migration("20260129220000_EnsureProiectBugetMoneda")]
    public partial class EnsureProiectBugetMoneda : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'Proiecte') AND name = N'Buget')
    ALTER TABLE [Proiecte] ADD [Buget] decimal(18,2) NULL;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'Proiecte') AND name = N'Moneda')
    ALTER TABLE [Proiecte] ADD [Moneda] nvarchar(10) NULL;
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'Proiecte') AND name = N'Buget')
    ALTER TABLE [Proiecte] DROP COLUMN [Buget];
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'Proiecte') AND name = N'Moneda')
    ALTER TABLE [Proiecte] DROP COLUMN [Moneda];
");
        }
    }
}
