using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonnelTransferRequest.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class addedNewPopIsApproved : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "TransferPreferences",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "TransferPreferences");
        }
    }
}
