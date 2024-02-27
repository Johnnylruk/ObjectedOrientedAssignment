using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EVotingSystem_SBMM.Migrations
{
    public partial class VoterIdRelationship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "VoterId",
                table: "VotePreferences",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_VotePreferences_VoterId",
                table: "VotePreferences",
                column: "VoterId");

            migrationBuilder.AddForeignKey(
                name: "FK_VotePreferences_Voters_VoterId",
                table: "VotePreferences",
                column: "VoterId",
                principalTable: "Voters",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VotePreferences_Voters_VoterId",
                table: "VotePreferences");

            migrationBuilder.DropIndex(
                name: "IX_VotePreferences_VoterId",
                table: "VotePreferences");

            migrationBuilder.DropColumn(
                name: "VoterId",
                table: "VotePreferences");
        }
    }
}
