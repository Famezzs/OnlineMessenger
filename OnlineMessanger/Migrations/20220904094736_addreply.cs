using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineMessanger.Migrations
{
    public partial class addreply : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReplyToMessadeId",
                table: "Messages",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReplyToMessadeId",
                table: "Messages");
        }
    }
}
