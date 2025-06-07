using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace server1._1.Migrations
{
    /// <inheritdoc />
    public partial class AddMaxAttemptsToEmployeeTestAssignment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaxAttempts",
                table: "TestAssignments",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxAttempts",
                table: "TestAssignments");
        }
    }
}
