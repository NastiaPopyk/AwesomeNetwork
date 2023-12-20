using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AwesomeNetwork.Migrations
{
    /// <inheritdoc />
    public partial class AddPostType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PostType",
                table: "Post",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PostType",
                table: "Post");
        }
    }
}
