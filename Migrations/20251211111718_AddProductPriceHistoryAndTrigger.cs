using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineShopping.Migrations
{
    /// <inheritdoc />
    public partial class AddProductPriceHistoryAndTrigger : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductPriceHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProductId = table.Column<int>(type: "INTEGER", nullable: false),
                    ProductName = table.Column<string>(type: "TEXT", nullable: false),
                    OldPrice = table.Column<decimal>(type: "TEXT", nullable: false),
                    NewPrice = table.Column<decimal>(type: "TEXT", nullable: false),
                    ChangedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ChangeType = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductPriceHistory", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "PasswordHash", "PasswordSalt" },
                values: new object[] { "a3xI9eLyk9Vd9oJ0dKNwJ5lzEnLnv/slZRKKC07XeJ0=", "FHfFJ/oNuSxlVd0wwzOonA==" });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "PasswordHash", "PasswordSalt" },
                values: new object[] { "5KU19xwBJBtQrfUYzvODdeb6224NKdDdB6Vjn+oi9FI=", "/fF26FhboeRYYrk9Vgb95A==" });

            migrationBuilder.CreateIndex(
                name: "IX_ProductPriceHistory_ChangedAt",
                table: "ProductPriceHistory",
                column: "ChangedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ProductPriceHistory_ProductId",
                table: "ProductPriceHistory",
                column: "ProductId");

            // Create TRIGGER for INSERT - logs when new products are created
            migrationBuilder.Sql(@"
                CREATE TRIGGER trg_Product_Insert_PriceHistory
                AFTER INSERT ON Products
                FOR EACH ROW
                BEGIN
                    INSERT INTO ProductPriceHistory (ProductId, ProductName, OldPrice, NewPrice, ChangedAt, ChangeType)
                    VALUES (NEW.Id, NEW.Name, 0, NEW.UnitPrice, datetime('now'), 'INSERT');
                END;
            ");

            // Create TRIGGER for UPDATE - logs when product prices change
            migrationBuilder.Sql(@"
                CREATE TRIGGER trg_Product_Update_PriceHistory
                AFTER UPDATE OF UnitPrice ON Products
                FOR EACH ROW
                WHEN OLD.UnitPrice != NEW.UnitPrice
                BEGIN
                    INSERT INTO ProductPriceHistory (ProductId, ProductName, OldPrice, NewPrice, ChangedAt, ChangeType)
                    VALUES (NEW.Id, NEW.Name, OLD.UnitPrice, NEW.UnitPrice, datetime('now'), 'UPDATE');
                END;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop triggers first
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS trg_Product_Insert_PriceHistory;");
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS trg_Product_Update_PriceHistory;");

            migrationBuilder.DropTable(
                name: "ProductPriceHistory");

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "PasswordHash", "PasswordSalt" },
                values: new object[] { "sTOs9gcZxJgvEYdie+9fRNl7rrY8Mlj2h3sRKFq3Vms=", "gN74mPVcpTLtynBIUQq4rA==" });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "PasswordHash", "PasswordSalt" },
                values: new object[] { "xMJtnLRbzFvzSfC3fAabeNgqVWbonm99EDQcryloebY=", "i5mCA6uNI6K1hLUjRl9vDg==" });
        }
    }
}
