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
                name: "Sources",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sources", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Targets",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Targets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Source_TargetsBi_x_Target_Sources",
                schema: "dbo",
                columns: table => new
                {
                    LHSId = table.Column<int>(nullable: false),
                    RHSId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Source_TargetsBi_x_Target_Sources", x => new { x.LHSId, x.RHSId });
                    table.ForeignKey(
                        name: "FK_Source_TargetsBi_x_Target_Sources_Sources_LHSId",
                        column: x => x.LHSId,
                        principalSchema: "dbo",
                        principalTable: "Sources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Source_TargetsBi_x_Target_Sources_Targets_RHSId",
                        column: x => x.RHSId,
                        principalSchema: "dbo",
                        principalTable: "Targets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Source_TargetsUni_x_Target",
                schema: "dbo",
                columns: table => new
                {
                    LHSId = table.Column<int>(nullable: false),
                    RHSId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Source_TargetsUni_x_Target", x => new { x.LHSId, x.RHSId });
                    table.ForeignKey(
                        name: "FK_Source_TargetsUni_x_Target_Sources_LHSId",
                        column: x => x.LHSId,
                        principalSchema: "dbo",
                        principalTable: "Sources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Source_TargetsUni_x_Target_Targets_RHSId",
                        column: x => x.RHSId,
                        principalSchema: "dbo",
                        principalTable: "Targets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Source_TargetsBi_x_Target_Sources_RHSId",
                schema: "dbo",
                table: "Source_TargetsBi_x_Target_Sources",
                column: "RHSId");

            migrationBuilder.CreateIndex(
                name: "IX_Source_TargetsUni_x_Target_RHSId",
                schema: "dbo",
                table: "Source_TargetsUni_x_Target",
                column: "RHSId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Source_TargetsBi_x_Target_Sources",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Source_TargetsUni_x_Target",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Sources",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Targets",
                schema: "dbo");
        }
    }
}
