using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EVotingSystem_SBMM.Migrations
{
    public partial class AddingVoterProfile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Profile",
                table: "Voters",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Profile",
                table: "Voters");
        }
    }
}
