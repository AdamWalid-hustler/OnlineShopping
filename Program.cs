
using System.Runtime.InteropServices;
using Microsoft.EntityFrameworkCore;
using OnlineShopping.Data;
using OnlineShopping.Models;

var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
optionsBuilder.UseSqlite("Data Source=OnlinShopping.db");

using (var DbContext = new AppDbContext(optionsBuilder.Options))


while (true)
{
    Console.WriteLine("")
}