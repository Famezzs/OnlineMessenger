using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineMessenger.Migrations
{
    public partial class updatereply : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ReplyToMessadeId",
                table: "Messages",
                newName: "ReplyToMessageId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ReplyToMessageId",
                table: "Messages",
                newName: "ReplyToMessadeId");
        }
    }
}
