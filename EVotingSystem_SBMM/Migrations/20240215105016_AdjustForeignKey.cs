using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EVotingSystem_SBMM.Migrations
{
    public partial class AdjustForeignKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EndData",
                table: "Events",
                newName: "EndDate");

            migrationBuilder.AddColumn<int>(
                name: "EventId",
                table: "Votes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Votes_EventId",
                table: "Votes",
                column: "EventId");

            migrationBuilder.AddForeignKey(
                name: "FK_Votes_Events_EventId",
                table: "Votes",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "EventId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Votes_Events_EventId",
                table: "Votes");

            migrationBuilder.DropIndex(
                name: "IX_Votes_EventId",
                table: "Votes");

            migrationBuilder.DropColumn(
                name: "EventId",
                table: "Votes");

            migrationBuilder.RenameColumn(
                name: "EndDate",
                table: "Events",
                newName: "EndData");
        }
    }
}
