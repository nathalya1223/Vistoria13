using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vistoria_projeto.Migrations
{
    /// <inheritdoc />
    public partial class AddCamposAssinatura : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AssinadoPor",
                table: "ChecklistsVistorias",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataAssinatura",
                table: "ChecklistsVistorias",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "LaudoAssinado",
                table: "ChecklistsVistorias",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssinadoPor",
                table: "ChecklistsVistorias");

            migrationBuilder.DropColumn(
                name: "DataAssinatura",
                table: "ChecklistsVistorias");

            migrationBuilder.DropColumn(
                name: "LaudoAssinado",
                table: "ChecklistsVistorias");
        }
    }
}
