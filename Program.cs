using NLog;
using System.Linq;
using NWConsole.Model;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

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
        choice = DisplayMenu();
        Console.Clear();
        logger.Info($"Option {choice} selected");

        switch (choice)
        {
            case "1":
                // Display Categories
                DisplayCategories(db);
                break;
            case "2":
                // Add Categories
                AddCategory(db);
                break;
            case "3":
                // Display Category Products
                DisplayCategoryProducts(db);
                break;
            case "4":
                // Display All Categories with Products
                DisplayAllCategoriesWithProducts(db);
                break;
            default:
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("Invalid choice. Please try again.");
                Console.ResetColor();
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

string DisplayMenu()
{
    Console.WriteLine("1) Display Categories");
    Console.WriteLine("2) Add Category");
    Console.WriteLine("3) Display Category and related products");
    Console.WriteLine("4) Display all Categories and their related products");
    Console.WriteLine("\"q\" to quit");

    return Console.ReadLine();
}

void DisplayCategories(NWContext db)
{
    var query = db.Categories.OrderBy(p => p.CategoryName);

    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"{query.Count()} records returned");
    Console.ForegroundColor = ConsoleColor.Magenta;
    foreach (var item in query)
    {
        Console.WriteLine($"{item.CategoryName} - {item.Description}");
    }
    Console.ResetColor();
}

void AddCategory(NWContext db)
{
    Category category = new Category();
    Console.WriteLine("Enter Category Name:");
    category.CategoryName = Console.ReadLine();
    Console.WriteLine("Enter the Category Description:");
    category.Description = Console.ReadLine();
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
            // Save category to db
            db.AddCategory(category);
            
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            logger.Info($"Category '{category.CategoryName}' added to the database.");
            Console.ResetColor();
        }
    }
    if (!isValid)
    {
        foreach (var result in results)
        {
            logger.Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
        }
    }
}

void DisplayCategoryProducts(NWContext db)
{
    var query = db.Categories.OrderBy(p => p.CategoryId);

    Console.WriteLine("Select the category whose products you want to display:");
    Console.ForegroundColor = ConsoleColor.DarkRed;
    foreach (var item in query)
    {
        Console.WriteLine($"{item.CategoryId}) {item.CategoryName}");
    }
    Console.ResetColor();
    int id = int.Parse(Console.ReadLine());
    Console.Clear();
    logger.Info($"CategoryId {id} selected");
    Category category = db.Categories.Include("Products").FirstOrDefault(c => c.CategoryId == id);
    Console.WriteLine($"{category.CategoryName} - {category.Description}");
    foreach (Product p in category.Products)
    {
        Console.WriteLine($"\t{p.ProductName}");
    }
}

void DisplayAllCategoriesWithProducts(NWContext db)
{
    var query = db.Categories.Include("Products").OrderBy(p => p.CategoryId);
    foreach (var item in query)
    {
        Console.WriteLine($"{item.CategoryName}");
        foreach (Product p in item.Products)
        {
            Console.WriteLine($"\t{p.ProductName}");
        }
    }
}
