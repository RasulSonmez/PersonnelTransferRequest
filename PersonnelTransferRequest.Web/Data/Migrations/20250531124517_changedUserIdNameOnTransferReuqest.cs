using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonnelTransferRequest.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class changedUserIdNameOnTransferReuqest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TransferRequests_AspNetUsers_ApplicationUserId",
                table: "TransferRequests");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "TransferRequests");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "TransferRequests",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TransferRequests_AspNetUsers_ApplicationUserId",
                table: "TransferRequests",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TransferRequests_AspNetUsers_ApplicationUserId",
                table: "TransferRequests");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "TransferRequests",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "TransferRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_TransferRequests_AspNetUsers_ApplicationUserId",
                table: "TransferRequests",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
