using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EVotingSystem_SBMM.Migrations
{
    public partial class EventIdToPreferenceTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EventId",
                table: "VotePreferences",
                type: "int",
                nullable: true); // Change to nullable

            migrationBuilder.CreateIndex(
                name: "IX_VotePreferences_EventId",
                table: "VotePreferences",
                column: "EventId");

            migrationBuilder.AddForeignKey(
                name: "FK_VotePreferences_Events_EventId",
                table: "VotePreferences",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "EventId",
                onDelete: ReferentialAction.Restrict); // or ReferentialAction.SetNull, depending on your requirements
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VotePreferences_Events_EventId",
                table: "VotePreferences");

            migrationBuilder.DropIndex(
                name: "IX_VotePreferences_EventId",
                table: "VotePreferences");

            migrationBuilder.DropColumn(
                name: "EventId",
                table: "VotePreferences");
        }
    }
}
