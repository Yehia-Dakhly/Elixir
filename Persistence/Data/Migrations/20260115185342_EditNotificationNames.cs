using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Data.Migrations
{
    /// <inheritdoc />
    public partial class EditNotificationNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NotificationBase_NotificationChild_NotificationChildId",
                table: "NotificationBase");

            migrationBuilder.RenameColumn(
                name: "NotificationChildId",
                table: "NotificationBase",
                newName: "NotificationBaseId");

            migrationBuilder.RenameIndex(
                name: "IX_NotificationBase_NotificationChildId",
                table: "NotificationBase",
                newName: "IX_NotificationBase_NotificationBaseId");

            migrationBuilder.AddForeignKey(
                name: "FK_NotificationBase_NotificationChild_NotificationBaseId",
                table: "NotificationBase",
                column: "NotificationBaseId",
                principalTable: "NotificationChild",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NotificationBase_NotificationChild_NotificationBaseId",
                table: "NotificationBase");

            migrationBuilder.RenameColumn(
                name: "NotificationBaseId",
                table: "NotificationBase",
                newName: "NotificationChildId");

            migrationBuilder.RenameIndex(
                name: "IX_NotificationBase_NotificationBaseId",
                table: "NotificationBase",
                newName: "IX_NotificationBase_NotificationChildId");

            migrationBuilder.AddForeignKey(
                name: "FK_NotificationBase_NotificationChild_NotificationChildId",
                table: "NotificationBase",
                column: "NotificationChildId",
                principalTable: "NotificationChild",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
