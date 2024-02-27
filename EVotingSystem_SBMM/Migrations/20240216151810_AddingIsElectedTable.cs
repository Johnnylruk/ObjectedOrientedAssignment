using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EVotingSystem_SBMM.Migrations
{
    public partial class AddingIsElectedTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsElected",
                table: "Candidates",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsElected",
                table: "Candidates");
        }
    }
}
