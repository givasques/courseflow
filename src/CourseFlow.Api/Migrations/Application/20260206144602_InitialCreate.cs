using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseFlow.Api.Migrations.Application
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "course_flow");

            migrationBuilder.CreateTable(
                name: "courses",
                schema: "course_flow",
                columns: table => new
                {
                    id = table.Column<string>(type: "TEXT", nullable: false),
                    title = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    category = table.Column<int>(type: "INTEGER", nullable: false),
                    workload_hours = table.Column<int>(type: "INTEGER", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_courses", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "students",
                schema: "course_flow",
                columns: table => new
                {
                    id = table.Column<string>(type: "TEXT", nullable: false),
                    full_name = table.Column<string>(type: "TEXT", maxLength: 250, nullable: false),
                    email = table.Column<string>(type: "TEXT", maxLength: 254, nullable: false),
                    registered_at_utc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_students", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "enrollments",
                schema: "course_flow",
                columns: table => new
                {
                    course_id = table.Column<string>(type: "TEXT", nullable: false),
                    student_id = table.Column<string>(type: "TEXT", nullable: false),
                    status = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 1),
                    enrollment_date_utc = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValue: new DateTime(2026, 2, 6, 14, 46, 1, 792, DateTimeKind.Utc).AddTicks(6763))
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_enrollments", x => new { x.course_id, x.student_id });
                    table.ForeignKey(
                        name: "fk_enrollments_courses_course_id",
                        column: x => x.course_id,
                        principalSchema: "course_flow",
                        principalTable: "courses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_enrollments_students_student_id",
                        column: x => x.student_id,
                        principalSchema: "course_flow",
                        principalTable: "students",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_courses_category",
                schema: "course_flow",
                table: "courses",
                column: "category");

            migrationBuilder.CreateIndex(
                name: "ix_courses_title",
                schema: "course_flow",
                table: "courses",
                column: "title",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_enrollments_status",
                schema: "course_flow",
                table: "enrollments",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ix_enrollments_student_id",
                schema: "course_flow",
                table: "enrollments",
                column: "student_id");

            migrationBuilder.CreateIndex(
                name: "ix_students_email",
                schema: "course_flow",
                table: "students",
                column: "email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "enrollments",
                schema: "course_flow");

            migrationBuilder.DropTable(
                name: "courses",
                schema: "course_flow");

            migrationBuilder.DropTable(
                name: "students",
                schema: "course_flow");
        }
    }
}
