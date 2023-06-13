using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JupebPortal.Migrations
{
    /// <inheritdoc />
    public partial class addSubjectsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Subjects",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subjects", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ApplicantOLevels",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Grade = table.Column<int>(type: "int", nullable: false),
                    ExamNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExamBody = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExamYear = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Sitting = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubjectId = table.Column<int>(type: "int", nullable: false),
                    ApplicationFormFK = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicantOLevels", x => x.id);
                    table.ForeignKey(
                        name: "FK_ApplicantOLevels_ApplicationForms_ApplicationFormFK",
                        column: x => x.ApplicationFormFK,
                        principalTable: "ApplicationForms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicantOLevels_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicantOLevels_ApplicationFormFK",
                table: "ApplicantOLevels",
                column: "ApplicationFormFK");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicantOLevels_SubjectId",
                table: "ApplicantOLevels",
                column: "SubjectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicantOLevels");

            migrationBuilder.DropTable(
                name: "Subjects");
        }
    }
}
