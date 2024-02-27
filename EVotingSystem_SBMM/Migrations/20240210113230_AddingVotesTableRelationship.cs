using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EVotingSystem_SBMM.Migrations
{
    public partial class AddingVotesTableRelationship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Votes_Candidates_CandidatesId",
                table: "Votes");

            migrationBuilder.DropIndex(
                name: "IX_Votes_CandidatesId",
                table: "Votes");

            migrationBuilder.DropColumn(
                name: "CandidatesId",
                table: "Votes");

            migrationBuilder.CreateIndex(
                name: "IX_Votes_CandidateId",
                table: "Votes",
                column: "CandidateId");

            migrationBuilder.AddForeignKey(
                name: "FK_Votes_Candidates_CandidateId",
                table: "Votes",
                column: "CandidateId",
                principalTable: "Candidates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Votes_Candidates_CandidateId",
                table: "Votes");

            migrationBuilder.DropIndex(
                name: "IX_Votes_CandidateId",
                table: "Votes");

            migrationBuilder.AddColumn<int>(
                name: "CandidatesId",
                table: "Votes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Votes_CandidatesId",
                table: "Votes",
                column: "CandidatesId");

            migrationBuilder.AddForeignKey(
                name: "FK_Votes_Candidates_CandidatesId",
                table: "Votes",
                column: "CandidatesId",
                principalTable: "Candidates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
