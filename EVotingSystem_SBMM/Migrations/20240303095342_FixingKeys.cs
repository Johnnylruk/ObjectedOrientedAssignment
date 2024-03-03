using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EVotingSystem_SBMM.Migrations
{
    public partial class FixingKeys : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VotePreferences_Events_EventId",
                table: "VotePreferences");

            migrationBuilder.AlterColumn<int>(
                name: "EventId",
                table: "VotePreferences",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_VotePreferences_Events_EventId",
                table: "VotePreferences",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "EventId",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VotePreferences_Events_EventId",
                table: "VotePreferences");

            migrationBuilder.AlterColumn<int>(
                name: "EventId",
                table: "VotePreferences",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_VotePreferences_Events_EventId",
                table: "VotePreferences",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "EventId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
