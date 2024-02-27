using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EVotingSystem_SBMM.Migrations
{
    public partial class UpdatingPreferenceModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop foreign key constraint
            migrationBuilder.DropForeignKey(
                name: "FK_VotePreferences_Events_EventId",
                table: "VotePreferences");

            // Drop index
            migrationBuilder.DropIndex(
                name: "IX_VotePreferences_EventId",
                table: "VotePreferences");

            // Drop the EventId column
            migrationBuilder.DropColumn(
                name: "EventId",
                table: "VotePreferences");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Add the EventId column again as nullable
            migrationBuilder.AddColumn<int>(
                name: "EventId",
                table: "VotePreferences",
                type: "int",
                nullable: true); // Change to nullable

            // Create index for the new EventId column
            migrationBuilder.CreateIndex(
                name: "IX_VotePreferences_EventId",
                table: "VotePreferences",
                column: "EventId");

            // Add foreign key constraint to the new EventId column
            migrationBuilder.AddForeignKey(
                name: "FK_VotePreferences_Events_EventId",
                table: "VotePreferences",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "EventId",
                onDelete: ReferentialAction.Restrict); // or ReferentialAction.SetNull, depending on your requirements
        }
    }
}
