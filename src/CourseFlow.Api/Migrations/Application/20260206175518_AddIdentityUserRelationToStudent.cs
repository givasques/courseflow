using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseFlow.Api.Migrations.Application
{
    /// <inheritdoc />
    public partial class AddIdentityUserRelationToStudent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "identity_id",
                schema: "course_flow",
                table: "students",
                type: "TEXT",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<DateTime>(
                name: "enrollment_date_utc",
                schema: "course_flow",
                table: "enrollments",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(2026, 2, 6, 17, 55, 17, 961, DateTimeKind.Utc).AddTicks(200),
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldDefaultValue: new DateTime(2026, 2, 6, 14, 46, 1, 792, DateTimeKind.Utc).AddTicks(6763));

            migrationBuilder.CreateIndex(
                name: "ix_students_identity_id",
                schema: "course_flow",
                table: "students",
                column: "identity_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_students_identity_id",
                schema: "course_flow",
                table: "students");

            migrationBuilder.DropColumn(
                name: "identity_id",
                schema: "course_flow",
                table: "students");

            migrationBuilder.AlterColumn<DateTime>(
                name: "enrollment_date_utc",
                schema: "course_flow",
                table: "enrollments",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(2026, 2, 6, 14, 46, 1, 792, DateTimeKind.Utc).AddTicks(6763),
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldDefaultValue: new DateTime(2026, 2, 6, 17, 55, 17, 961, DateTimeKind.Utc).AddTicks(200));
        }
    }
}
