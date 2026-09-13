using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuditDataPlatform.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddRowNumberToFinding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RowNumber",
                table: "Findings",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RowNumber",
                table: "Findings");
        }
    }
}
