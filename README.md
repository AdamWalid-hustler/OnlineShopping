# Online Shopping Management System

A console-based database application demonstrating CRUD operations, data validation, EF Core, and secure password storage.

## Features

✅ **Normalized Database (3NF)** - Customer, Category, Product, Order, OrderLine entities  
✅ **Complete CRUD** - Full Create, Read, Update, Delete for Products and Categories  
✅ **Data Validation** - Mandatory fields, range checks, format validation  
✅ **Error Handling** - Clear error messages for all operations  
✅ **EF Core** - Code-first approach with migrations  
✅ **LINQ Queries** - All data access via LINQ with filtering/searching  
✅ **Password Security** - PBKDF2 hashing with salt (100,000 iterations)  
✅ **Performance Indexes** - Optimized queries on foreign keys and searchable fields  

## Project Structure

```
OnlineShopping/
├── Data/
│   └── AppDbContext.cs          # EF Core DbContext with configurations
├── Models/
│   ├── Category.cs              # Category entity with validation
│   ├── Customer.cs              # Customer entity with password fields
│   ├── Order.cs                 # Order entity
│   ├── OrderLine.cs             # OrderLine entity (junction table)
│   └── Product.cs               # Product entity with validation
├── Services/
│   ├── CategoryService.cs       # Category CRUD operations
│   └── ProductService.cs        # Product CRUD operations
├── Utilities/
│   └── PasswordHasher.cs        # PBKDF2 password hashing
├── Migrations/                  # EF Core migrations
├── Program.cs                   # Console UI with menus
├── PROJECT_REPORT.md            # Comprehensive project documentation
└── README.md                    # This file
```

## Requirements

- .NET 8.0 SDK
- Entity Framework Core 8.0
- SQLite (included)

## Getting Started

### 1. Clone or Open the Project

```powershell
cd C:\Users\Walid\OnlineShopping
```

### 2. Restore Dependencies

```powershell
dotnet restore
```

### 3. Apply Database Migrations

The database is already set up. If you need to recreate it:

```powershell
dotnet ef database update
```

### 4. Run the Application

```powershell
dotnet run
```

## Using the Application

### Main Menu

When you start the application, you'll see:

```
=== Online Shopping Management System ===

=== MAIN MENU ===
1. Product Management
2. Category Management
3. Exit
```

### Product Management

- **List all products** - View all products with prices and categories
- **Search products** - Find products by name
- **View product details** - See full information for a specific product
- **Create new product** - Add a product (requires existing category)
- **Update product** - Modify product name, price, or category
- **Delete product** - Remove a product (blocked if used in orders)

### Category Management

- **List all categories** - View all categories with product counts
- **Search categories** - Find categories by name or description
- **View category details** - See category info and associated products
- **Create new category** - Add a category with name and description
- **Update category** - Modify category details
- **Delete category** - Remove a category (blocked if has products)

## Database Schema

### Customer
- Id (PK), Name, Email (unique), Address, PasswordHash, PasswordSalt

### Category
- Id (PK), Name (unique), Description

### Product
- Id (PK), Name, UnitPrice, CategoryId (FK → Category)

### Order
- Id (PK), CustomerId (FK → Customer), Date, TotalAmount

### OrderLine
- Id (PK), OrderId (FK → Order), ProductId (FK → Product), UnitPrice, Quantity

## Sample Data

The database is seeded with:
- 2 Customers (Sal, Amjad) with hashed passwords
- 2 Categories (Sports Equipment, Summer clothing)
- 2 Products (Basketball, T-Shirt)
- 2 Orders with 3 OrderLines

## Validation Rules

### Product
- Name: 2-100 characters (required)
- Price: $0.01 - $999,999.99 (required)
- Category: Must reference existing category

### Category
- Name: 2-50 characters (required, unique)
- Description: Max 500 characters (optional)

## Security: Password Hashing

Passwords use **PBKDF2** with:
- 100,000 iterations (key stretching)
- 16-byte random salt per user
- SHA256 hashing algorithm
- 32-byte output hash

This approach:
- Prevents rainbow table attacks (unique salts)
- Resists brute-force attempts (100k iterations)
- Follows NIST recommendations
- Built into .NET (no external dependencies)

## Performance Optimizations

**Indexes created on:**
- Customer.Email (unique)
- Category.Name
- Product.Name
- Product.CategoryId (FK)
- Order.CustomerId (FK)
- OrderLine.OrderId (FK)
- OrderLine.ProductId (FK)

**LINQ optimizations:**
- Eager loading with `.Include()` to avoid N+1 queries
- Async operations for all database calls
- Parameterized queries for SQL injection prevention

## Testing the Application

### Test Scenario 1: Create Category and Product

1. Run the app: `dotnet run`
2. Select `2` (Category Management)
3. Select `4` (Create new category)
4. Enter name: "Electronics"
5. Enter description: "Electronic devices and gadgets"
6. Back to main menu (`7`)
7. Select `1` (Product Management)
8. Select `4` (Create new product)
9. Enter name: "Laptop"
10. Enter price: 999.99
11. Enter category ID: 3 (Electronics)

### Test Scenario 2: Search and Update

1. Select `1` (Product Management)
2. Select `2` (Search products)
3. Enter search term: "ball"
4. View results
5. Select `5` (Update product)
6. Enter ID of basketball product
7. Update price or name

### Test Scenario 3: Validation Errors

1. Try to create a product with empty name → Validation error
2. Try to create a product with price -5 → Range validation error
3. Try to delete a category that has products → Referential integrity error

## Documentation

See `PROJECT_REPORT.md` for:
- Detailed ER diagram and normalization explanation
- Complete CRUD flow documentation
- Validation and error handling details
- Security implementation rationale
- Known limitations and future improvements

## Technologies Used

- **Language:** C# 12
- **Framework:** .NET 8.0
- **ORM:** Entity Framework Core 8.0
- **Database:** SQLite 3
- **Validation:** System.ComponentModel.DataAnnotations
- **Security:** System.Security.Cryptography (PBKDF2)

## Author

Walid - Online Shopping Management System for Database Course

## License

Educational project - free to use and modify.
