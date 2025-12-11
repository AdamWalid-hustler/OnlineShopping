using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineShopping.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderSummaryView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Stock",
                table: "Products",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Address", "PasswordHash", "PasswordSalt" },
                values: new object[] { "buoES2WMnT8RSQU=", "sTOs9gcZxJgvEYdie+9fRNl7rrY8Mlj2h3sRKFq3Vms=", "gN74mPVcpTLtynBIUQq4rA==" });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Address", "PasswordHash", "PasswordSalt" },
                values: new object[] { "beIaUWOKKvsRDlZ3TOAR", "xMJtnLRbzFvzSfC3fAabeNgqVWbonm99EDQcryloebY=", "i5mCA6uNI6K1hLUjRl9vDg==" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 101,
                column: "Stock",
                value: 50);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 102,
                column: "Stock",
                value: 100);

            migrationBuilder.CreateIndex(
                name: "IX_Products_Stock",
                table: "Products",
                column: "Stock");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_Date",
                table: "Orders",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_Date_TotalAmount",
                table: "Orders",
                columns: new[] { "Date", "TotalAmount" });

            // Create DATABASE VIEW
            migrationBuilder.Sql(@"
                CREATE VIEW OrderSummaryView AS
                SELECT 
                    o.Id AS OrderId,
                    o.Date AS OrderDate,
                    c.Name AS CustomerName,
                    c.Email AS CustomerEmail,
                    o.TotalAmount,
                    COUNT(ol.Id) AS TotalItems
                FROM Orders o
                INNER JOIN Customers c ON o.CustomerId = c.Id
                LEFT JOIN OrderLines ol ON o.Id = ol.OrderId
                GROUP BY o.Id, o.Date, c.Name, c.Email, o.TotalAmount
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop DATABASE VIEW
            migrationBuilder.Sql("DROP VIEW IF EXISTS OrderSummaryView");

            migrationBuilder.DropIndex(
                name: "IX_Products_Stock",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Orders_Date",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_Date_TotalAmount",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Stock",
                table: "Products");

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Address", "PasswordHash", "PasswordSalt" },
                values: new object[] { "Hamngatan 6", "TgMGCX+6rkXbHdcWHaMfSsTaRP9GW2YPM53yVLeNJVg=", "+S5QGHdISZKThB50yT5i2Q==" });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Address", "PasswordHash", "PasswordSalt" },
                values: new object[] { "Kistagången 22", "6lLI+x0oZooep6PSdTYr9xgFGisPdHp+rdgwASgi3MI=", "n6XUNf9xsjWCw2NJQ+PoMg==" });
        }
    }
}
