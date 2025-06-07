using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace server1._1.Migrations
{
    /// <inheritdoc />
    public partial class AddMaterialPathToCourseAssignments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MaterialPath",
                table: "CourseAssignments",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaterialPath",
                table: "CourseAssignments");
        }
    }
}
