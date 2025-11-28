using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vistoria_projeto.Migrations
{
    /// <inheritdoc />
    public partial class AddSegundaFoto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CaminhoFoto2",
                table: "ChecklistsVistorias",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CaminhoFoto2",
                table: "ChecklistsVistorias");
        }
    }
}
