using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseFlow.Api.Migrations.Application
{
    /// <inheritdoc />
    public partial class UpdatingTheCourseEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "enrollment_date_utc",
                schema: "course_flow",
                table: "enrollments",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(2026, 2, 9, 20, 0, 34, 239, DateTimeKind.Utc).AddTicks(5944),
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldDefaultValue: new DateTime(2026, 2, 9, 15, 17, 54, 536, DateTimeKind.Utc).AddTicks(9921));

            migrationBuilder.AlterColumn<string>(
                name: "instructor_id",
                schema: "course_flow",
                table: "courses",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "enrollment_date_utc",
                schema: "course_flow",
                table: "enrollments",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(2026, 2, 9, 15, 17, 54, 536, DateTimeKind.Utc).AddTicks(9921),
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldDefaultValue: new DateTime(2026, 2, 9, 20, 0, 34, 239, DateTimeKind.Utc).AddTicks(5944));

            migrationBuilder.AlterColumn<string>(
                name: "instructor_id",
                schema: "course_flow",
                table: "courses",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);
        }
    }
}
