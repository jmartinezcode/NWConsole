using NLog;
using System.Linq;
using NWConsole.Model;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.Xml.Serialization;

string path = Directory.GetCurrentDirectory() + "\\nlog.config";

// create instance of Logger
var logger = LogManager.LoadConfiguration(path).GetCurrentClassLogger();
logger.Info("Program started");

try
{
    var db = new NWContext();
    string choice;

    do
    {
        choice = DisplayMainMenu();
        Console.Clear();
        logger.Info($"Option {choice} selected");

        switch (choice)
        {
            case "1": // Display SubMenu            
                string displayChoice;
                do
                {
                    displayChoice = DisplaySubMenu();
                    Console.Clear();
                    logger.Info($"Option {displayChoice} selected");

                    switch (displayChoice)
                    {
                        case "1": // Display Categories SubMenu                           
                            string categoriesChoice;
                            do
                            {
                                categoriesChoice = DisplayCategoriesSubMenu();
                                Console.Clear();
                                logger.Info($"Option {categoriesChoice} selected");
                                // Handle categories submenu choices                              
                                switch (categoriesChoice)
                                {
                                    case "1": // Display Categories (CategoryName only)
                                        DisplayCategories(db);
                                        break;
                                    case "2": // Display Single Category and Related Products
                                        DisplayCategoryProducts(db);
                                        break;
                                    case "3": // Display All Categories and their related products
                                        DisplayAllCategoriesWithProducts(db);
                                        break;
                                    case "4": // Return to Display Sub Menu
                                        break;                                    
                                    case "q": // Exit the program
                                        return;
                                }
                            } while (categoriesChoice != "4" && categoriesChoice.ToLower() != "q");
                            break;
                        case "2": // Display Products SubMenu
                            string productsChoice;
                            do
                            {
                                productsChoice = DisplayProductsSubMenu();
                                Console.Clear();
                                logger.Info($"Option {productsChoice} selected");
                                // Handle products submenu choices
                                switch (productsChoice)
                                {
                                    case "1": // Display Products (ProductName only)                                        
                                        DisplayAllProducts(db);
                                        break;
                                    case "2": // Display Active Products (ProductName only)                                        
                                        DisplayActiveProducts(db);
                                        break;
                                    case "3": // Display Discontinued Products (ProductName only)
                                        DisplayDiscontinuedProducts(db);
                                        break;
                                    case "4": // Display Single Product (all fields displayed)
                                        DisplaySingleProduct(db);
                                        break;
                                    case "5": // Return to Previous Menu
                                        break;
                                    case "q": // Exit the program
                                        return; 
                                }
                            } while (productsChoice != "5" && productsChoice.ToLower() != "q");
                            break;
                        case "3": // Returns to Main Menu                            
                            break;
                        case "q": // Exit the program
                            return;
                    }
                } while (displayChoice != "3" && displayChoice.ToLower() != "q");
                break;
            case "2": // Add SubMenu
                break;
            case "3": // Edit SubMenu
                break;
            case "4": // Delete SubMenu                
                break;

            // case "2":
            //     // Add Categories
            //     AddCategory(db);
            //     break;
            // case "5":
            //     // Edit Category
            //     EditCategory(db);
            //     break;
            // case "6":
            //     // Add Product
            //     AddProduct(db);
            //     break;
            
            default:
                if (choice.ToLower() != "q")
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine("Invalid choice. Please try again.");
                    Console.ResetColor();
                }
                break;
        }
        Console.WriteLine();
    } while (choice.ToLower() != "q");
}
catch (Exception ex)
{
    logger.Error(ex.Message);
}

logger.Info("Program ended");

string DisplayMainMenu()
{
    Console.WriteLine("Main Menu");
    Console.WriteLine("=================");
    Console.WriteLine("1. Display");
    Console.WriteLine("2. Add");
    Console.WriteLine("3. Edit");
    Console.WriteLine("4. Delete");
    Console.WriteLine("\"q\" to quit");

    return Console.ReadLine();
}
string DisplaySubMenu()
{
    Console.WriteLine("Display Options");
    Console.WriteLine("========================");
    Console.WriteLine("1. Display Categories");
    Console.WriteLine("2. Display Products");
    Console.WriteLine("3. Return to Main Menu");
    Console.WriteLine("\"q\" to quit");
    return Console.ReadLine();
}
string DisplayCategoriesSubMenu()
{
    Console.WriteLine("Display Categories");
    Console.WriteLine("=====================================================");
    Console.WriteLine("1. Display Categories (CategoryName only)");
    Console.WriteLine("2. Display Single Category and Related Products");
    Console.WriteLine("3. Display All Categories and their related products");
    Console.WriteLine("4. Return to Previous Menu");
    Console.WriteLine("\"q\" to quit");

    return Console.ReadLine();
}
string DisplayProductsSubMenu()
{
    Console.WriteLine("Display Products");
    Console.WriteLine("====================================================");
    Console.WriteLine("1. Display Products (ProductName only)");
    Console.WriteLine("2. Display Active Products (ProductName only)");
    Console.WriteLine("3. Display Discontinued Products (ProductName only)");
    Console.WriteLine("4. Display Single Product (all fields displayed)");
    Console.WriteLine("5. Return to Previous Menu");
    Console.WriteLine("\"q\" to quit");

    return Console.ReadLine();
}

void DisplayCategories(NWContext db)
{
    var query = db.Categories.OrderBy(p => p.CategoryName);

    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"{query.Count()} records returned");
    Console.ForegroundColor = ConsoleColor.DarkGreen;
    foreach (var item in query)
    {
        Console.WriteLine($"{item.CategoryName} - {item.Description}");
    }
    Console.ResetColor();
}
void DisplayCategoryProducts(NWContext db)
{
    int id = GetCategoryID(db, "display");
    logger.Info($"CategoryId {id} selected");
    // Filter for active Products only 
    Category category = db.Categories
        .Include(c => c.Products.Where(p => !p.Discontinued))
        .FirstOrDefault(c => c.CategoryId == id);

    Console.WriteLine($"Displaying {category.CategoryName} - {category.Description}");
    foreach (Product p in category.Products)
    {
        Console.WriteLine($"\t{p.ProductName}");
    }
}
void DisplayAllCategoriesWithProducts(NWContext db)
{
    // Filter for active Products only
    var query = db.Categories
        .Include(c => c.Products.Where(p => !p.Discontinued))
        .OrderBy(p => p.CategoryId);

    foreach (var item in query)
    {
        Console.WriteLine($"{item.CategoryName}");
        foreach (Product p in item.Products)
        {
            Console.WriteLine($"\t{p.ProductName}");
        }
    }
}

void DisplayAllProducts(NWContext db)
{
    var query = db.Products.OrderBy(p => p.ProductName);

    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"{query.Count()} records returned");
    Console.ResetColor();

    foreach (var product in query)
    {
        if (!product.Discontinued)
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine($"{product.ProductName}");            
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"{product.ProductName} (Discontinued)");
        }
        Console.ResetColor();
    }
}
void DisplayActiveProducts(NWContext db)
{
    var query = db.Products.Where(p => !p.Discontinued).OrderBy(p => p.ProductName);

    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"{query.Count()} active products returned");
    Console.ForegroundColor = ConsoleColor.DarkGreen;
    foreach (var product in query)
    {
        Console.WriteLine($"{product.ProductName}");        
    }
    Console.ResetColor();
}
void DisplayDiscontinuedProducts(NWContext db)
{
    var query = db.Products.Where(p => p.Discontinued).OrderBy(p => p.ProductName);

    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"{query.Count()} discontinued products returned");
    Console.ForegroundColor = ConsoleColor.DarkGray;
    foreach (var product in query)
    {
        Console.WriteLine($"{product.ProductName}");
    }
    Console.ResetColor();
}
void DisplaySingleProduct(NWContext db)
{
    int id = GetProductID(db, "display");
    logger.Info($"ProductId {id} selected");

    Console.ForegroundColor = ConsoleColor.DarkGreen;
    Product product = db.Products.FirstOrDefault(p => p.ProductId == id);
    Console.WriteLine(product);
    Console.ResetColor();
}
string AddSubMenu()
{
    Console.WriteLine("Add Options");
    Console.WriteLine("========================");
    Console.WriteLine("1. Add Category");
    Console.WriteLine("2. Add Product");
    Console.WriteLine("3. Return to Main Menu");
    Console.WriteLine("\"q\" to quit");
    return Console.ReadLine();
}

void AddCategory(NWContext db)
{
    Console.ForegroundColor = ConsoleColor.DarkCyan;
    Console.WriteLine("Adding Category");
    Console.ResetColor();
    Category category = ValidateCategory(db);
    if (category != null)
    {
        // Add category
        db.AddCategory(category);

        Console.ForegroundColor = ConsoleColor.DarkGreen;
        logger.Info($"Category '{category.CategoryName}' added to the database.");
        Console.ResetColor();
    }
}
void AddProduct(NWContext db)
{
    Console.ForegroundColor = ConsoleColor.DarkCyan;
    Console.WriteLine("Adding Product");
    Console.ResetColor();
    Product product = ValidateProduct(db);
    if (product != null)
    {
        // Get Product Information
        // SupplierID
        product.SupplierId = GetSupplierID(db, product.ProductName);
        // CategoryID
        product.CategoryId = GetCategoryID(db, $"add {product.ProductName}");
        // QTY/Unit
        Console.Write("Enter Quantity Per Unit: ");
        product.QuantityPerUnit = Console.ReadLine();

        // Discontinued
        product.Discontinued = false;

        // Add product
        db.AddProduct(product);

        Console.ForegroundColor = ConsoleColor.DarkGreen;
        logger.Info($"Product '{product.ProductName}' added to the database.");
        Console.ResetColor();
    }
}

string EditSubMenu()
{
    Console.WriteLine("Edit Options");
    Console.WriteLine("========================");
    Console.WriteLine("1. Edit Category");
    Console.WriteLine("2. Edit Product");
    Console.WriteLine("3. Return to Main Menu");
    Console.WriteLine("\"q\" to quit");
    return Console.ReadLine();
}
void EditCategory(NWContext db) 
{
    // Edit category
    int id = GetCategoryID(db, "edit");
    logger.Info($"CategoryId {id} selected");

    // Get existing category
    Category existingCategory = db.Categories.Find(id);

    if (existingCategory != null)
    {
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.WriteLine("Editing Category");
        Console.ResetColor();
        Category updatedCategory = ValidateCategory(db);
        if (updatedCategory != null)
        {
            updatedCategory.CategoryId = existingCategory.CategoryId;
            db.EditCategory(updatedCategory);
            logger.Info($"Category '{existingCategory.CategoryName}' updated successfully");
        }
    }
    else
        logger.Error($"Category with ID {id} not found.");
}
void EditProduct(NWContext db)
{
    // TODO
}

string DeleteSubMenu()
{
    Console.WriteLine("Delete Options");
    Console.WriteLine("========================");
    Console.WriteLine("1. Delete Category");
    Console.WriteLine("2. Delete Product");
    Console.WriteLine("3. Return to Main Menu");
    Console.WriteLine("\"q\" to quit");
    return Console.ReadLine();
}

void DeleteProduct(NWContext db)
{
    // TODO
}
void DeleteCategory(NWContext db)
{
    // TODO
}

Category ValidateCategory(NWContext db)
{
    Category category = new Category();
    Console.WriteLine("Enter Category Name:");
    category.CategoryName = Console.ReadLine();
    Console.WriteLine("Enter the Category Description:");
    category.Description = Console.ReadLine();
    // Validate
    ValidationContext context = new ValidationContext(category, null, null);
    List<ValidationResult> results = new List<ValidationResult>();

    var isValid = Validator.TryValidateObject(category, context, results, true);
    if (isValid)
    {
        // check for unique name
        if (db.Categories.Any(c => c.CategoryName == category.CategoryName))
        {
            // generate validation error
            isValid = false;
            results.Add(new ValidationResult("Name exists", new string[] { "CategoryName" }));
        }
        else
        {
            logger.Info("Validation passed");
            return category;
        }
    }
    if (!isValid)
    {
        foreach (var result in results)
        {
            logger.Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
        }
    }
    return null;
}
Product ValidateProduct(NWContext db)
{
    Product product = new Product();

    // Validate ProductName
    Console.WriteLine("Enter Product Name:");
    product.ProductName = Console.ReadLine();
    if (!ValidateProperty(product, nameof(Product.ProductName), product.ProductName, out var nameValidationResults))
    {
        DisplayValidationErrors(nameValidationResults);
        return null;
    }
    // Validate Unique ProductName    
    if (db.Products.Any(p => p.ProductName == product.ProductName))
    {
        DisplayValidationErrors(new List<ValidationResult>
        {
            new ValidationResult("Product Name already exists", new[] { nameof(Product.ProductName) })
        });
        return null;
    }
    
    List<ValidationResult> unitPriceValidationResults = new List<ValidationResult>();
    // Validate UnitPrice
    Console.WriteLine("Enter Unit Price:");
    if (!decimal.TryParse(Console.ReadLine(), out decimal unitPrice) || !ValidateProperty(product, nameof(Product.UnitPrice), unitPrice, out unitPriceValidationResults))
    {
        DisplayValidationErrors(unitPriceValidationResults);
        return null;
    }
    product.UnitPrice = unitPrice;

    List<ValidationResult> unitsInStockValidationResults = new List<ValidationResult>();
    // Validate UnitsInStock
    Console.WriteLine("Enter Units In Stock:");
    if (!short.TryParse(Console.ReadLine(), out short unitsInStock) || !ValidateProperty(product, nameof(Product.UnitsInStock), unitsInStock, out unitsInStockValidationResults))
    {
        DisplayValidationErrors(unitsInStockValidationResults);
        return null;
    }
    product.UnitsInStock = unitsInStock;

    List<ValidationResult> unitsOnOrderValidationResults = new List<ValidationResult>();
    // Validate UnitsOnOrder
    Console.WriteLine("Enter Units On Order:");
    if (!short.TryParse(Console.ReadLine(), out short unitsOnOrder) || !ValidateProperty(product, nameof(Product.UnitsOnOrder), unitsOnOrder, out unitsOnOrderValidationResults))
    {
        DisplayValidationErrors(unitsOnOrderValidationResults);
        return null;
    }
    product.UnitsOnOrder = unitsOnOrder;

    List<ValidationResult> reorderLevelValidationResults = new List<ValidationResult>();
    // Validate ReOrderLevel
    Console.WriteLine("Enter Reorder Level:");
    if (!short.TryParse(Console.ReadLine(), out short reorderLevel) || !ValidateProperty(product, nameof(Product.ReorderLevel), reorderLevel, out reorderLevelValidationResults))
    {
        DisplayValidationErrors(reorderLevelValidationResults);
        return null;
    }
    product.ReorderLevel = reorderLevel;

    logger.Info("Validation passed");
    return product;    
}

int GetCategoryID(NWContext db, string mode)
{
    var query = db.Categories.OrderBy(p => p.CategoryId);

    Console.WriteLine("Select the category:");
    Console.ForegroundColor = ConsoleColor.Green;
    foreach (var item in query)
    {
        Console.WriteLine($"{item.CategoryId}) {item.CategoryName}");
    }
    Console.ResetColor();
    // Verify input
    int id;
    while (true)
    {
        Console.Write($"Enter the category ID to {mode}: ");
        string input = Console.ReadLine();

        if (int.TryParse(input, out id))
        {
            if (query.Any(c => c.CategoryId == id))
            {
                Console.Clear();
                return id;
            }
            else
                Console.WriteLine("Invalid category ID. Please try again.");
        }
        else
            Console.WriteLine("Invalid input. Please enter a valid ID.");
    }
}
int GetSupplierID(NWContext db, string mode)
{
    var query = db.Suppliers.OrderBy(s => s.SupplierId);

    Console.WriteLine("Select the Supplier:");
    Console.ForegroundColor = ConsoleColor.Green;
    foreach (var item in query)
    {
        Console.WriteLine($"{item.SupplierId}) {item.CompanyName}");
    }
    Console.ResetColor();
    // Verify input
    int id;
    while (true)
    {
        Console.Write($"Enter the Supplier ID for {mode}: ");
        string input = Console.ReadLine();

        if (int.TryParse(input, out id))
        {
            if (query.Any(s => s.SupplierId == id))
            {
                Console.Clear();
                return id;
            }
            else
                Console.WriteLine("Invalid Supplier ID. Please try again.");
        }
        else
            Console.WriteLine("Invalid input. Please enter a valid ID.");
    }
}
int GetProductID(NWContext db, string mode)
{
    var query = db.Products.OrderBy(p => p.ProductId);

    Console.WriteLine("Select the Product:");
    Console.ForegroundColor = ConsoleColor.Green;
    foreach (var product in query)
    {
        Console.WriteLine($"{product.ProductId} {product.ProductName}");
    }
    Console.ResetColor();
    // Verify input
    int id;
    while (true)
    {
        Console.Write($"Enter the Product ID to {mode}: ");
        string input = Console.ReadLine();

        if (int.TryParse(input, out id))
        {
            if (query.Any(p => p.ProductId == id))
            {
                Console.Clear();
                return id;
            }
            else
                Console.WriteLine("Invalid Product ID. Please try again.");
        }
        else
            Console.WriteLine("Invalid input. Please enter a valid ID.");
    }
}

bool ValidateProperty<T>(object instance, string propertyName, T value, out List<ValidationResult> results)
{
    // modified for reusability
    instance.GetType().GetProperty(propertyName).SetValue(instance, value);

    ValidationContext context = new ValidationContext(instance, null, null)
    {
        MemberName = propertyName
    };

    results = new List<ValidationResult>();
    return Validator.TryValidateProperty(value, context, results);
}
void DisplayValidationErrors(List<ValidationResult> results)
{
    if (results.Count > 0)
    {
        foreach (var result in results)
        {
            logger.Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
        }
    }
    else
    {
        logger.Error("Invalid input. Please enter a valid value.");
    }
}
