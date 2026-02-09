using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseFlow.Api.Migrations.Application
{
    /// <inheritdoc />
    public partial class Add_Instructors : Migration
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
                defaultValue: new DateTime(2026, 2, 9, 15, 17, 54, 536, DateTimeKind.Utc).AddTicks(9921),
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldDefaultValue: new DateTime(2026, 2, 6, 17, 55, 17, 961, DateTimeKind.Utc).AddTicks(200));

            migrationBuilder.AddColumn<string>(
                name: "instructor_id",
                schema: "course_flow",
                table: "courses",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "instructors",
                schema: "course_flow",
                columns: table => new
                {
                    id = table.Column<string>(type: "TEXT", nullable: false),
                    full_name = table.Column<string>(type: "TEXT", maxLength: 250, nullable: false),
                    email = table.Column<string>(type: "TEXT", maxLength: 254, nullable: false),
                    registered_at_utc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    identity_id = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_instructors", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_courses_instructor_id",
                schema: "course_flow",
                table: "courses",
                column: "instructor_id");

            migrationBuilder.CreateIndex(
                name: "ix_instructors_email",
                schema: "course_flow",
                table: "instructors",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_instructors_identity_id",
                schema: "course_flow",
                table: "instructors",
                column: "identity_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_courses_instructors_instructor_id",
                schema: "course_flow",
                table: "courses",
                column: "instructor_id",
                principalSchema: "course_flow",
                principalTable: "instructors",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_courses_instructors_instructor_id",
                schema: "course_flow",
                table: "courses");

            migrationBuilder.DropTable(
                name: "instructors",
                schema: "course_flow");

            migrationBuilder.DropIndex(
                name: "ix_courses_instructor_id",
                schema: "course_flow",
                table: "courses");

            migrationBuilder.DropColumn(
                name: "instructor_id",
                schema: "course_flow",
                table: "courses");

            migrationBuilder.AlterColumn<DateTime>(
                name: "enrollment_date_utc",
                schema: "course_flow",
                table: "enrollments",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(2026, 2, 6, 17, 55, 17, 961, DateTimeKind.Utc).AddTicks(200),
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldDefaultValue: new DateTime(2026, 2, 9, 15, 17, 54, 536, DateTimeKind.Utc).AddTicks(9921));
        }
    }
}
