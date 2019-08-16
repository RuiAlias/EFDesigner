using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sandbox_EFCore.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.CreateTable(
                name: "PressReleaseDetails",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PressReleaseDetails", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PressReleases",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PressReleases", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PressRelease_PressReleaseDetailHistory_x_PressReleaseDetail_PressReleases",
                schema: "dbo",
                columns: table => new
                {
                    LHS_Id = table.Column<int>(nullable: false),
                    RHS_Id = table.Column<int>(nullable: false),
                    PressReleaseDetailId = table.Column<int>(nullable: true),
                    PressReleaseId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PressRelease_PressReleaseDetailHistory_x_PressReleaseDetail_PressReleases", x => new { x.LHS_Id, x.RHS_Id });
                    table.ForeignKey(
                        name: "FK_PressRelease_PressReleaseDetailHistory_x_PressReleaseDetail_PressReleases_PressReleases_LHS_Id",
                        column: x => x.LHS_Id,
                        principalSchema: "dbo",
                        principalTable: "PressReleases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PressRelease_PressReleaseDetailHistory_x_PressReleaseDetail_PressReleases_PressReleaseDetails_PressReleaseDetailId",
                        column: x => x.PressReleaseDetailId,
                        principalSchema: "dbo",
                        principalTable: "PressReleaseDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PressRelease_PressReleaseDetailHistory_x_PressReleaseDetail_PressReleases_PressReleases_PressReleaseId",
                        column: x => x.PressReleaseId,
                        principalSchema: "dbo",
                        principalTable: "PressReleases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PressRelease_PressReleaseDetailHistory_x_PressReleaseDetail_PressReleases_PressReleaseDetails_RHS_Id",
                        column: x => x.RHS_Id,
                        principalSchema: "dbo",
                        principalTable: "PressReleaseDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PressRelease_PressReleaseDetails_x_PressReleaseDetail",
                schema: "dbo",
                columns: table => new
                {
                    LHS_Id = table.Column<int>(nullable: false),
                    RHS_Id = table.Column<int>(nullable: false),
                    PressReleaseId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PressRelease_PressReleaseDetails_x_PressReleaseDetail", x => new { x.LHS_Id, x.RHS_Id });
                    table.ForeignKey(
                        name: "FK_PressRelease_PressReleaseDetails_x_PressReleaseDetail_PressReleases_LHS_Id",
                        column: x => x.LHS_Id,
                        principalSchema: "dbo",
                        principalTable: "PressReleases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PressRelease_PressReleaseDetails_x_PressReleaseDetail_PressReleases_PressReleaseId",
                        column: x => x.PressReleaseId,
                        principalSchema: "dbo",
                        principalTable: "PressReleases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PressRelease_PressReleaseDetails_x_PressReleaseDetail_PressReleaseDetails_RHS_Id",
                        column: x => x.RHS_Id,
                        principalSchema: "dbo",
                        principalTable: "PressReleaseDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PressRelease_PressReleaseDetailHistory_x_PressReleaseDetail_PressReleases_PressReleaseDetailId",
                schema: "dbo",
                table: "PressRelease_PressReleaseDetailHistory_x_PressReleaseDetail_PressReleases",
                column: "PressReleaseDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_PressRelease_PressReleaseDetailHistory_x_PressReleaseDetail_PressReleases_PressReleaseId",
                schema: "dbo",
                table: "PressRelease_PressReleaseDetailHistory_x_PressReleaseDetail_PressReleases",
                column: "PressReleaseId");

            migrationBuilder.CreateIndex(
                name: "IX_PressRelease_PressReleaseDetailHistory_x_PressReleaseDetail_PressReleases_RHS_Id",
                schema: "dbo",
                table: "PressRelease_PressReleaseDetailHistory_x_PressReleaseDetail_PressReleases",
                column: "RHS_Id");

            migrationBuilder.CreateIndex(
                name: "IX_PressRelease_PressReleaseDetails_x_PressReleaseDetail_PressReleaseId",
                schema: "dbo",
                table: "PressRelease_PressReleaseDetails_x_PressReleaseDetail",
                column: "PressReleaseId");

            migrationBuilder.CreateIndex(
                name: "IX_PressRelease_PressReleaseDetails_x_PressReleaseDetail_RHS_Id",
                schema: "dbo",
                table: "PressRelease_PressReleaseDetails_x_PressReleaseDetail",
                column: "RHS_Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PressRelease_PressReleaseDetailHistory_x_PressReleaseDetail_PressReleases",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "PressRelease_PressReleaseDetails_x_PressReleaseDetail",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "PressReleases",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "PressReleaseDetails",
                schema: "dbo");
        }
    }
}
