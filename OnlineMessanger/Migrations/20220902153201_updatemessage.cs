using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineMessanger.Migrations
{
    public partial class updatemessage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeletedForSelf",
                table: "Messages",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeletedForSelf",
                table: "Messages");
        }
    }
}
