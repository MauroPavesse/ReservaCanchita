using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReservaCanchita.Migrations
{
    /// <inheritdoc />
    public partial class MenuComidasConfiguraciones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comidas_ComidasCategorias_ComidaCategoriaId",
                table: "Comidas");

            migrationBuilder.AlterColumn<string>(
                name: "Descripcion",
                table: "Comidas",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "ComidaCategoriaId",
                table: "Comidas",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Comidas_ComidasCategorias_ComidaCategoriaId",
                table: "Comidas",
                column: "ComidaCategoriaId",
                principalTable: "ComidasCategorias",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comidas_ComidasCategorias_ComidaCategoriaId",
                table: "Comidas");

            migrationBuilder.UpdateData(
                table: "Comidas",
                keyColumn: "Descripcion",
                keyValue: null,
                column: "Descripcion",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "Descripcion",
                table: "Comidas",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "ComidaCategoriaId",
                table: "Comidas",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Comidas_ComidasCategorias_ComidaCategoriaId",
                table: "Comidas",
                column: "ComidaCategoriaId",
                principalTable: "ComidasCategorias",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
