using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace server1._1.Migrations
{
    /// <inheritdoc />
    public partial class AddRelatedCourseToTest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RelatedCourseId",
                table: "Tests",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tests_RelatedCourseId",
                table: "Tests",
                column: "RelatedCourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tests_Courses_RelatedCourseId",
                table: "Tests",
                column: "RelatedCourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tests_Courses_RelatedCourseId",
                table: "Tests");

            migrationBuilder.DropIndex(
                name: "IX_Tests_RelatedCourseId",
                table: "Tests");

            migrationBuilder.DropColumn(
                name: "RelatedCourseId",
                table: "Tests");
        }
    }
}
