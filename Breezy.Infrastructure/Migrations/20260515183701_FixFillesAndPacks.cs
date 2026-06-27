using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Breezy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixFillesAndPacks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SoundPackFile_SoundPacks_SoundPackId",
                table: "SoundPackFile");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SoundPackFile",
                table: "SoundPackFile");

            migrationBuilder.RenameTable(
                name: "SoundPackFile",
                newName: "SoundPackFiles");

            migrationBuilder.RenameIndex(
                name: "IX_SoundPackFile_SoundPackId",
                table: "SoundPackFiles",
                newName: "IX_SoundPackFiles_SoundPackId");

            migrationBuilder.AlterColumn<string>(
                name: "LastExport_Format",
                table: "SoundPacks",
                type: "character varying(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastExport_ExportedAt",
                table: "SoundPacks",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "LastExport_BlobUrl",
                table: "SoundPacks",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Key",
                table: "SoundPackFiles",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "FileName",
                table: "SoundPackFiles",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Category",
                table: "SoundPackFiles",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SoundPackFiles",
                table: "SoundPackFiles",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SoundPackFiles_SoundPacks_SoundPackId",
                table: "SoundPackFiles",
                column: "SoundPackId",
                principalTable: "SoundPacks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SoundPackFiles_SoundPacks_SoundPackId",
                table: "SoundPackFiles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SoundPackFiles",
                table: "SoundPackFiles");

            migrationBuilder.RenameTable(
                name: "SoundPackFiles",
                newName: "SoundPackFile");

            migrationBuilder.RenameIndex(
                name: "IX_SoundPackFiles_SoundPackId",
                table: "SoundPackFile",
                newName: "IX_SoundPackFile_SoundPackId");

            migrationBuilder.AlterColumn<string>(
                name: "LastExport_Format",
                table: "SoundPacks",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastExport_ExportedAt",
                table: "SoundPacks",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastExport_BlobUrl",
                table: "SoundPacks",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Key",
                table: "SoundPackFile",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<string>(
                name: "FileName",
                table: "SoundPackFile",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<int>(
                name: "Category",
                table: "SoundPackFile",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AddPrimaryKey(
                name: "PK_SoundPackFile",
                table: "SoundPackFile",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SoundPackFile_SoundPacks_SoundPackId",
                table: "SoundPackFile",
                column: "SoundPackId",
                principalTable: "SoundPacks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
