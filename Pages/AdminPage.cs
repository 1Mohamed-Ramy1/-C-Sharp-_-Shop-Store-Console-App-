using System.Text;
using System.Transactions;
using App.Models;
using App.Routes;
using App.Services;
using App.Utils;
using Spectre.Console;
namespace App.Pages;
public class AdminPage : Page
{
    public override void Display()
    {
        Print.OutLine($"=== 👑 Welcome My Lord ({GlobalStore.Instance.CurrentUser?.Username}) To Admin Panel ===", ConsoleColor.Yellow);
    }
    public override void HandleInput(Router router)
    {
        while (true)
        {
            Console.Clear();
            Display();
            var input = Print.AskChoice("[bold aqua]📋 At your service:[/]", new List<string>
            {
                "👥 View Users",
                "➕ Add User",
                "❌ Delete User",
                "⚠️ Warn User",
                "🧾 View Products",
                "➕ Add Product",
                "✏️ Edit Product",
                "🗑️ Delete Product",
                "🚪 Log out"
            });
            switch (input)
            {
                case "👥 View Users": ListUsers(); Print.Pause(); break;
                case "➕ Add User": AddUser(); break;
                case "❌ Delete User": DeleteUser(); break;
                case "⚠️ Warn User": WarnUser(); break;
                case "🧾 View Products": ListProducts(); Print.Pause(); break;
                case "➕ Add Product": AddProduct(); break;
                case "✏️ Edit Product": EditProduct(); break;
                case "🗑️ Delete Product": DeleteProduct(); break;
                case "🚪 Log out":
                    while (true)
                    {
                        Print.Out("⚠️ Are you sure you want to log out My Lord👑? (Y/N): ", ConsoleColor.Yellow);
                        var confirm = Console.ReadLine()?.Trim().ToLower();

                        if (confirm == "y")
                        {
                            Print.OutLine("\n🔄 Logging out...", ConsoleColor.Cyan);
                            Thread.Sleep(800);
                            router.Navigate("home");
                            return;
                        }
                        else if (confirm == "n")
                        {
                            Print.OutLine("\n🔙 Back to menu.", ConsoleColor.Green);
                            Thread.Sleep(500);
                            break;
                        }
                        else
                        {
                            Print.OutLine("❌ Invalid input. Please enter Y or N.", ConsoleColor.Red);
                        }
                    }
                    break;
                default:
                    Print.OutLine("Invalid option. Try again.", ConsoleColor.Red);
                    Print.Pause();
                    break;
            }
        }
    }
    private string AskWithEscInline(string prompt)
{
    Console.Write(prompt + " ");
    var input = new StringBuilder();
    while (true)
    {
        var key = Console.ReadKey(intercept: true);

        if (key.Key == ConsoleKey.Escape)
        {
            throw new OperationCanceledException();
        }

        if (key.Key == ConsoleKey.Enter)
        {
            Console.WriteLine();
            break;
        }
        if (key.Key == ConsoleKey.Backspace)
        {
            if (input.Length > 0)
            {
                input.Remove(input.Length - 1, 1);
                // Move cursor back, overwrite char with space, move back again
                Console.Write("\b \b");
            }
        }
        else if (!char.IsControl(key.KeyChar)) // ignore other control chars
        {
            input.Append(key.KeyChar);
            Console.Write(key.KeyChar);
        }
    }
    return input.ToString().Trim();
}
    private void AddProduct()
{
    try
    {
        Console.Clear();
        Print.PrintFixedESCMessage();
        var name = AskWithEscInline("📝 Name product : ");
        if (string.IsNullOrWhiteSpace(name))
        {
            Print.OutLine("❌ Product name cannot be empty.", ConsoleColor.Red);
            Print.Pause();
            return;
        }
        var existingProducts = DataManger.ProductDB.GetAll();
        if (existingProducts.Any(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
        {
            Print.OutLine("❌ A product with this name already exists.", ConsoleColor.Red);
            Print.Pause();
            return;
        }
        var categoryChoice = Print.AskChoice("📂 Choose a category :", new List<string> { "Men", "Women", "Kids", "Other (type manually)" });
        string category = categoryChoice == "Other (type manually)"
            ? AskWithEscInline("📁 Enter new category : ")
            : categoryChoice;
        if (string.IsNullOrWhiteSpace(category))
        {
            Print.OutLine("❌ Category cannot be empty.", ConsoleColor.Red);
            Print.Pause();
            return;
        }
        var price = Print.AskNumber("💵 Price : ");
        if (price < 0)
        {
            Print.OutLine("❌ Price cannot be negative.", ConsoleColor.Red);
            Print.Pause();
            return;
        }
        var newProduct = new Product(name, category, price);

        foreach (var size in Enum.GetValues<SizeUnit>())
        {
            int quantity = Print.AskNumber($"📏 Stock Quantity for {size} : ");
            if (quantity < 0)
            {
                Print.OutLine($"❌ Quantity for {size} cannot be negative.", ConsoleColor.Red);
                Print.Pause();
                return;
            }
            newProduct.Sizes.Add(new(size, quantity));
        }
        DataManger.ProductDB.Add(newProduct);
        Print.OutLine("✅ Product added successfully.", ConsoleColor.Green);
        Print.Pause();
    }
    catch (OperationCanceledException)
    {
        Print.OutLine("❌ Operation cancelled. Returning to previous menu...", ConsoleColor.Yellow);
        Print.Pause();
    }
    catch (Exception ex)
    {
        Print.OutLine($"❌ An error occurred: {ex.Message}", ConsoleColor.Red);
        Print.Pause();
    }
}
    private void EditProduct()
{
    try
    {
        Console.Clear();
        Print.PrintFixedESCMessage();
        var products = ListProducts();
        if (products == null || products.Count == 0)
        {
            Print.OutLine("⚠️ No products available to edit.", ConsoleColor.Yellow);
            Print.Pause();
            return;
        }
        var input = AskWithEscInline("🔎 Select product ID to edit : ");
        if (string.IsNullOrWhiteSpace(input) || !int.TryParse(input, out int id))
        {
            Print.OutLine("❌ Invalid ID. Please enter a numeric value.", ConsoleColor.Red);
            Print.Pause();
            return;
        }
        var product = products.FirstOrDefault(pr => pr.Id == id);
        if (product == null)
        {
            Print.OutLine("❌ Product not found.", ConsoleColor.Red);
            Print.Pause();
            return;
        }
        var name = AskWithEscInline($"📝 Name ({product.Name}) : ");
        var category = AskWithEscInline($"📂 Category ({product.Category}) : ");
        var price = Print.AskNumber($"💵 Price ({product.Price}) : ");

        if (!string.IsNullOrWhiteSpace(name)) product.Name = name;
        if (!string.IsNullOrWhiteSpace(category)) product.Category = category;
        product.Price = price;

        foreach (var size in product.Sizes)
        {
            int q = Print.AskNumber($"📏 Qty for {size.Size} ({size.Quantity}): ");
            size.Quantity = q;
        }

        Print.OutLine("✅ Product updated successfully.", ConsoleColor.Green);
        Print.Pause();
    }
    catch (OperationCanceledException)
    {
        Print.OutLine("❌ Operation cancelled. Returning to previous menu...", ConsoleColor.Yellow);
        Print.Pause();
    }
    catch (Exception ex)
    {
        Print.OutLine($"❌ An error occurred: {ex.Message}", ConsoleColor.Red);
        Print.Pausecounteradmin();
    }
}
    private void DeleteProduct()
    {
        try
        {
            Console.Clear();
            Print.PrintFixedESCMessage();
            var products = ListProducts();
            var input = AskWithEscInline("🗑️ Select product ID to delete :");
            if (!int.TryParse(input, out int id))
            {
                Print.OutLine("❌ Invalid ID", ConsoleColor.Red);
                return;
            }

            var product = products.FirstOrDefault(pr => pr.Id == id);
            if (product == null)
            {
                Print.OutLine("❌ Product not found", ConsoleColor.Red);
                return;
            }

            DataManger.ProductDB.Delete(id);
            Print.OutLine("🗑️ Product deleted", ConsoleColor.Green);
        }
        catch (OperationCanceledException) { }
    }

    private void AddUser()
{
    try
    {
    Console.Clear();
        Print.PrintFixedESCMessage();
        var username = AskWithEscInline("👤 Username : ");
        var email = AskWithEscInline("📧 Email: ");
        var password = AskWithEscInline("🔒 Password : ");
        if (string.IsNullOrWhiteSpace(username) ||
            string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(password))
        {
            Print.OutLine("❌ All fields are required. Please try again.", ConsoleColor.Red);
            Print.Pause();
            return;
        }
        var user = new User(username, email, password);
        DataManger.UserDB.Add(user);
        Print.OutLine("✅ User added successfully", ConsoleColor.Green);
        Print.Pausecounteradmin();
    }
    catch (OperationCanceledException)
    {
        Print.OutLine("❌ Operation cancelled. Returning to previous menu...", ConsoleColor.Yellow);
        Print.Pause();
    }
    catch (Exception ex)
    {
        Print.OutLine($"❌ An error occurred: {ex.Message}", ConsoleColor.Red);
        Print.Pause();
    }
}
    private void DeleteUser()
{
    try
    {
        Console.Clear();
        Print.PrintFixedESCMessage();
        var users = ListUsers();
        if (users == null || users.Count == 0)
        {
            Print.OutLine("⚠️ No users available to delete.", ConsoleColor.Yellow);
            Print.Pause();
            return;
        }
        var input = AskWithEscInline("❌ Select user ID to delete : ");
        if (string.IsNullOrWhiteSpace(input) || !int.TryParse(input, out int id))
        {
            Print.OutLine("❌ Invalid ID. Please enter a numeric ID.", ConsoleColor.Red);
            Print.Pause();
            return;
        }
        var user = users.FirstOrDefault(u => u.Id == id);
        if (user == null)
        {
            Print.OutLine("❌ User not found.", ConsoleColor.Red);
            Print.Pause();
            return;
        }
        DataManger.UserDB.Delete(id);
        Print.OutLine("✅ User deleted successfully.", ConsoleColor.Green);
        Print.Pausecounteradmin();
    }
    catch (OperationCanceledException)
    {
        Print.OutLine("❌ Operation cancelled. Returning to previous menu...", ConsoleColor.Yellow);
        Print.Pause();
    }
    catch (Exception ex)
    {
        Print.OutLine($"❌ An error occurred: {ex.Message}", ConsoleColor.Red);
        Print.Pause();
    }
}
    private void WarnUser()
{
    try
    {
        Console.Clear();
        Print.PrintFixedESCMessage();
        var users = ListUsers();
        if (users == null || users.Count == 0)
        {
            Print.OutLine("⚠️ No users available to warn.", ConsoleColor.Yellow);
            Print.Pause();
            return;
        }
        var input = AskWithEscInline("⚠️ Select user ID to warn : ");
        if (string.IsNullOrWhiteSpace(input) || !int.TryParse(input, out int id))
        {
            Print.OutLine("❌ Invalid ID. Please enter a numeric ID.", ConsoleColor.Red);
            Print.Pause();
            return;
        }
        var user = users.FirstOrDefault(u => u.Id == id);
        if (user == null)
        {
            Print.OutLine("❌ User not found.", ConsoleColor.Red);
            Print.Pause();
            return;
        }
        user.IsWarned = true;
        Print.OutLine($"⚠️ User \"{user.Username}\" has been warned.", ConsoleColor.Yellow);
        Print.Pausecounteradmin();
    }
    catch (OperationCanceledException)
    {
        Print.OutLine("❌ Operation cancelled. Returning to previous menu...", ConsoleColor.Yellow);
        Print.Pause();
    }
    catch (Exception ex)
    {
        Print.OutLine($"❌ An error occurred : {ex.Message}", ConsoleColor.Red);
        Print.Pause();
    }
}
    private List<Product> ListProducts()
    {
        var products = DataManger.ProductDB.GetAll();
        var table = new Table()
            .Border(TableBorder.Rounded)
            .BorderColor(Color.Grey37)
            .Centered()
            .AddColumn("[bold lime]🆔 ID[/]")
            .AddColumn("[bold lime]📦 Name[/]")
            .AddColumn("[bold lime]📁 Category[/]")
            .AddColumn("[bold lime]💰 Price[/]")
            .AddColumn("[bold lime]📏 Sizes[/]");
        foreach (var product in products)
        {
            var sizes = product.Sizes
                .OrderBy(s => s.Size)
                .Select(s => Math.Max(0, s.Quantity) == 0
                    ? $"[red]{s.Size} (0)[/]"
                    : $"[springgreen3]{s.Size} ({s.Quantity})[/]");

            table.AddRow(
                $"[white]{product.Id}[/]",
                $"[aqua]{product.Name}[/]",
                $"[orange1]{product.Category ?? "Unknown"}[/]",
                $"[yellow]${product.Price:F2}[/]",
                string.Join(" | ", sizes));
        }
        AnsiConsole.Write(table);
        return products;
    }
    private List<User> ListUsers()
    {
        var users = DataManger.UserDB.GetAll();
        var table = new Table()
            .Border(TableBorder.Rounded)
            .BorderColor(Color.Grey)
            .AddColumn("[bold lime]🆔 ID[/]")
            .AddColumn("[bold lime]👤 Username[/]")
            .AddColumn("[bold lime]📧 Email[/]")
            .AddColumn("[bold red]⚠️ Warned[/]");
        foreach (var user in users)
        {
            table.AddRow(
                user.Id.ToString(),
                user.Username,
                user.Email,
                user.IsWarned ? "[red]Yes[/]" : "No");
        }
        AnsiConsole.Write(table);
        return users;
    }
}