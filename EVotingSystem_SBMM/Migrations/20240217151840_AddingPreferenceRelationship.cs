using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EVotingSystem_SBMM.Migrations
{
    public partial class AddingPreferenceRelationship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VotePreferences",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VoteId = table.Column<int>(type: "int", nullable: false),
                    CandidateId = table.Column<int>(type: "int", nullable: false),
                    Rank = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VotePreferences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VotePreferences_Candidates_CandidateId",
                        column: x => x.CandidateId,
                        principalTable: "Candidates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VotePreferences_Votes_VoteId",
                        column: x => x.VoteId,
                        principalTable: "Votes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VotePreferences_CandidateId",
                table: "VotePreferences",
                column: "CandidateId");

            migrationBuilder.CreateIndex(
                name: "IX_VotePreferences_VoteId",
                table: "VotePreferences",
                column: "VoteId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VotePreferences");
        }
    }
}
