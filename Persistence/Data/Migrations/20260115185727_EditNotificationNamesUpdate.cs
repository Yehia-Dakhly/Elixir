using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Data.Migrations
{
    /// <inheritdoc />
    public partial class EditNotificationNamesUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NotificationBase_NotificationChild_NotificationBaseId",
                table: "NotificationBase");

            migrationBuilder.DropForeignKey(
                name: "FK_NotificationBase_Users_UserId",
                table: "NotificationBase");

            migrationBuilder.DropForeignKey(
                name: "FK_NotificationChild_BloodRequests_BloodRequestId",
                table: "NotificationChild");

            migrationBuilder.DropIndex(
                name: "IX_NotificationChild_BloodRequestId",
                table: "NotificationChild");

            migrationBuilder.DropIndex(
                name: "IX_NotificationBase_NotificationBaseId",
                table: "NotificationBase");

            migrationBuilder.DropIndex(
                name: "IX_NotificationBase_UserId",
                table: "NotificationBase");

            migrationBuilder.DropColumn(
                name: "BloodRequestId",
                table: "NotificationChild");

            migrationBuilder.DropColumn(
                name: "Body",
                table: "NotificationChild");

            migrationBuilder.DropColumn(
                name: "Data",
                table: "NotificationChild");

            migrationBuilder.DropColumn(
                name: "SendAt",
                table: "NotificationChild");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "NotificationChild");

            migrationBuilder.DropColumn(
                name: "IsRead",
                table: "NotificationBase");

            migrationBuilder.DropColumn(
                name: "NotificationBaseId",
                table: "NotificationBase");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "NotificationBase");

            migrationBuilder.AddColumn<bool>(
                name: "IsRead",
                table: "NotificationChild",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "NotificationBaseId",
                table: "NotificationChild",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "NotificationChild",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "BloodRequestId",
                table: "NotificationBase",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Body",
                table: "NotificationBase",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Data",
                table: "NotificationBase",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "SendAt",
                table: "NotificationBase",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "NotificationBase",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationChild_NotificationBaseId",
                table: "NotificationChild",
                column: "NotificationBaseId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationChild_UserId",
                table: "NotificationChild",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationBase_BloodRequestId",
                table: "NotificationBase",
                column: "BloodRequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_NotificationBase_BloodRequests_BloodRequestId",
                table: "NotificationBase",
                column: "BloodRequestId",
                principalTable: "BloodRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_NotificationChild_NotificationBase_NotificationBaseId",
                table: "NotificationChild",
                column: "NotificationBaseId",
                principalTable: "NotificationBase",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_NotificationChild_Users_UserId",
                table: "NotificationChild",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NotificationBase_BloodRequests_BloodRequestId",
                table: "NotificationBase");

            migrationBuilder.DropForeignKey(
                name: "FK_NotificationChild_NotificationBase_NotificationBaseId",
                table: "NotificationChild");

            migrationBuilder.DropForeignKey(
                name: "FK_NotificationChild_Users_UserId",
                table: "NotificationChild");

            migrationBuilder.DropIndex(
                name: "IX_NotificationChild_NotificationBaseId",
                table: "NotificationChild");

            migrationBuilder.DropIndex(
                name: "IX_NotificationChild_UserId",
                table: "NotificationChild");

            migrationBuilder.DropIndex(
                name: "IX_NotificationBase_BloodRequestId",
                table: "NotificationBase");

            migrationBuilder.DropColumn(
                name: "IsRead",
                table: "NotificationChild");

            migrationBuilder.DropColumn(
                name: "NotificationBaseId",
                table: "NotificationChild");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "NotificationChild");

            migrationBuilder.DropColumn(
                name: "BloodRequestId",
                table: "NotificationBase");

            migrationBuilder.DropColumn(
                name: "Body",
                table: "NotificationBase");

            migrationBuilder.DropColumn(
                name: "Data",
                table: "NotificationBase");

            migrationBuilder.DropColumn(
                name: "SendAt",
                table: "NotificationBase");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "NotificationBase");

            migrationBuilder.AddColumn<int>(
                name: "BloodRequestId",
                table: "NotificationChild",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Body",
                table: "NotificationChild",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Data",
                table: "NotificationChild",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "SendAt",
                table: "NotificationChild",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "NotificationChild",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsRead",
                table: "NotificationBase",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "NotificationBaseId",
                table: "NotificationBase",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "NotificationBase",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_NotificationChild_BloodRequestId",
                table: "NotificationChild",
                column: "BloodRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationBase_NotificationBaseId",
                table: "NotificationBase",
                column: "NotificationBaseId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationBase_UserId",
                table: "NotificationBase",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_NotificationBase_NotificationChild_NotificationBaseId",
                table: "NotificationBase",
                column: "NotificationBaseId",
                principalTable: "NotificationChild",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_NotificationBase_Users_UserId",
                table: "NotificationBase",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_NotificationChild_BloodRequests_BloodRequestId",
                table: "NotificationChild",
                column: "BloodRequestId",
                principalTable: "BloodRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
