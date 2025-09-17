using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MusicasCatolicasAPI.Migrations
{
    /// <inheritdoc />
    public partial class IdCategoriaNaSub : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoriaId",
                table: "SubCategorias",
                type: "INTEGER",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CategoriaId",
                table: "SubCategorias");
        }
    }
}
