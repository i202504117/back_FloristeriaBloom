using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace back_FloristeriaBloom.Migrations
{
    /// <inheritdoc />
    public partial class AgregarColorAProducto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "Productos",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DetallesPedidoAdicionales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DetallePedidoId = table.Column<int>(type: "int", nullable: false),
                    AdicionalId = table.Column<int>(type: "int", nullable: false),
                    PrecioUnitario = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetallesPedidoAdicionales", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DetallesPedidoAdicionales_Adicionales_AdicionalId",
                        column: x => x.AdicionalId,
                        principalTable: "Adicionales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DetallesPedidoAdicionales_DetallesPedido_DetallePedidoId",
                        column: x => x.DetallePedidoId,
                        principalTable: "DetallesPedido",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DetallesPedidoAdicionales_AdicionalId",
                table: "DetallesPedidoAdicionales",
                column: "AdicionalId");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesPedidoAdicionales_DetallePedidoId",
                table: "DetallesPedidoAdicionales",
                column: "DetallePedidoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DetallesPedidoAdicionales");

            migrationBuilder.DropColumn(
                name: "Color",
                table: "Productos");
        }
    }
}