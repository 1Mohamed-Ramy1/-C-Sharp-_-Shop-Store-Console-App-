using App.Models;
using App.Routes;
using App.Services;
using App.Utils;
using Spectre.Console;
using System.Media;

namespace App.Pages;

public class ShopPage : Page
{
    private Dictionary<int, (ProductSize Size, int Quantity)> Cart = new();

    public override void Display()
    {
        Print.OutLine($"=== Welcome ✨{GlobalStore.Instance.CurrentUser?.Username}✨ to ELOSTORA's Shops🏪 ===", ConsoleColor.Cyan);
    }

    public override void HandleInput(Router router)
    {
        while (true)
        {
            Console.Clear();
            Display();

            Print.OutLine("\n══════════════════════", ConsoleColor.DarkCyan);
            Print.OutLine("📋 Select an option:", ConsoleColor.Cyan);
            Print.OutLine("══════════════════════", ConsoleColor.DarkCyan);

            var choice = Print.AskChoice("", new()
        {
            "📁  Browse Products",
            "🛒  View Cart",
            "📜  View History",
            "💳  Checkout",
            "🚪  Log Out"
        }, allowBack: false);

            Print.OutLine("\n═══════════════════════════════════════════════", ConsoleColor.DarkCyan);
            switch (choice)
            {
                case "📁  Browse Products":
                    ListProducts();
                    break;

                case "🛒  View Cart":
                    ViewCart();
                    Print.Pause();
                    break;

                case "📜  View History":
                    ViewHistory();
                    Print.Pause();
                    break;

                case "💳  Checkout":
                    Checkout();
                    break;

                case "🚪  Log Out":
                    while (true)
                    {
                        Print.Out("\n⚠️  Are you sure you want to log out? All unsaved changes will be lost. (Y/N): ", ConsoleColor.Yellow);
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
                    Print.OutLine("❌ Invalid option. Try again.", ConsoleColor.Red);
                    Print.Pause();
                    break;
            }
        }
    }
    private void ListProducts()
    {
        var products = DataManger.ProductDB.GetAll();
        var orderedProducts = products.OrderBy(p =>
        {
            return p.Category?.ToLower() switch
            {
                "men" => 0,
                "women" => 1,
                "kids" => 2,
                _ => 3
            };
        }).ThenBy(p => p.Category).ThenBy(p => p.Name).ThenBy(p => p.Id).ToList();
        while (true)
        {
            Console.Clear();
            Print.PrintFixedESCMessage();
            var table = new Table()
                .Border(TableBorder.Rounded)
                .BorderColor(Color.Grey37)
                .Centered();
            table.AddColumn("[bold yellow]ID[/]");
            table.AddColumn("[bold yellow]Name[/]");
            table.AddColumn("[bold yellow]Category[/]");
            table.AddColumn("[bold yellow]Price[/]");
            table.AddColumn("[bold yellow]Sizes[/]");
            foreach (var product in orderedProducts)
            {
                var orderedSizes = product.Sizes
                    .OrderBy(s => s.Size)
                    .Select(s =>
                    {
                        var cartItem = Cart.FirstOrDefault(c => c.Key == product.Id && c.Value.Size.Size == s.Size);
                        int remaining = s.Quantity;
                        if (cartItem.Key != 0) remaining -= cartItem.Value.Quantity;
                        remaining = Math.Max(0, remaining);
                        var sizeDisplay = remaining == 0
                            ? $"[red]{s.Size} (0)[/]"
                            : $"[green]{s.Size} ({remaining})[/]";

                        return sizeDisplay;
                    });
                table.AddRow(
                    $"[white]{product.Id}[/]",
                    $"[aqua]{product.Name}[/]",
                    $"[orange1]{product.Category ?? "Unknown"}[/]",
                    $"[lime]${product.Price:F2}[/]",
                    string.Join(" | ", orderedSizes)
                );
            }
            AnsiConsole.Write(table);
            if (!AddToCart(orderedProducts)) break;
            Console.ForegroundColor = ConsoleColor.Yellow;
            var again = Print.AskYesNo("🛒 Do you want to add another product..?[/]");
            Console.ResetColor();
            if (!again) break;
        }
    }
    //✅ Add TO Cart
    private bool AddToCart(List<Product> products)
    {
        Print.OutLine("\n🛒 Add Product to Cart", ConsoleColor.Yellow);
        Print.OutLine("══════════════════════════════════════════════════════", ConsoleColor.DarkYellow);
        if (Print.CancelableNumber<int>(out var productId, "🔢 Enter product ID: "))
            return false;
        Print.OutLine($"🆔 You selected product ID: {productId}", ConsoleColor.Cyan);
        var product = products.FirstOrDefault(p => p.Id == productId);
        if (product == null)
        {
            if (OperatingSystem.IsWindows())
            {
                Console.Beep(500, 400);
            }
            Print.OutLine("❌ Invalid product ID.", ConsoleColor.Red);
            return true;
        }
        var sizeOptions = Enum.GetNames(typeof(SizeUnit)).ToList();
        var sizeChoice = Print.AskChoice("📏 Choose a size:", sizeOptions, allowBack: false); // 🔁 بدون Back
        Enum.TryParse(sizeChoice, out SizeUnit size);
        Print.OutLine($"📦 You selected size: {size}", ConsoleColor.Cyan);
        int quantity;
        while (true)
        {
            if (Print.CancelableNumber<int>(out quantity, "🔢 Enter quantity: "))
                return false;

            if (quantity <= 0)
            {
                Print.OutLine("⚠️ Quantity must be greater than 0. Please enter again.", ConsoleColor.Red);
                continue;
            }
            break;
        }
        Print.OutLine($"📦 You entered quantity: {quantity}", ConsoleColor.Cyan);

        Print.Out("🔎 Checking stock", ConsoleColor.Gray);
        for (int i = 0; i < 3; i++)
        {
            Thread.Sleep(300);
            Print.Out(".", ConsoleColor.Gray);
        }
        Console.WriteLine();
        var productSize = product.Sizes.FirstOrDefault(s => s.Size == size);
        if (productSize == null || productSize.Quantity < quantity)
        {
            if (OperatingSystem.IsWindows())
            {
                Console.Beep(400, 600);
            }
            Print.OutLine($"🚫 Out of stock for size ({size}) in product \"{product.Name}\". Please try another size or check back later.", ConsoleColor.Red);
            return true;
        }
        var existingCartItem = Cart.FirstOrDefault(c => c.Key == productId && c.Value.Size.Size == size);
        if (existingCartItem.Key != 0)
        {
            Cart[existingCartItem.Key] = (existingCartItem.Value.Size, existingCartItem.Value.Quantity + quantity);
        }
        else
        {
            Cart.Add(productId, (new ProductSize(size, quantity), quantity));
        }
        productSize.Quantity -= quantity;
        DataManger.ProductDB.Update(product.Id, product);
        Print.OutLine("\n╔══════════════════════════════════════════════════════╗", ConsoleColor.Green);
        Print.OutLine($"║ ✅ {product.Name} ({size}) has been added to your cart.", ConsoleColor.Green);
        Print.OutLine("╚══════════════════════════════════════════════════════╝\n", ConsoleColor.Green);
        if (OperatingSystem.IsWindows())
        {
            Console.Beep(1100, 120);
        }
        return true;
    }
    //✅ View Cart
    private void ViewCart()
    {
        Console.Clear();
        Print.OutLine("\n🛒 Your Shopping Cart", ConsoleColor.Yellow);
        Print.OutLine("══════════════════════════════════════════════════════", ConsoleColor.DarkYellow);
        if (!Cart.Any())
        {
            Print.OutLine("⚠️ Your cart is currently empty.", ConsoleColor.Yellow);
            return;
        }
        var table = new Table()
            .Border(TableBorder.Rounded)
            .BorderColor(Color.Grey)
            .AddColumn("[bold lime]📦 Product[/]")
            .AddColumn("[bold lime]📏 Size[/]")
            .AddColumn("[bold lime]🔢 Quantity[/]")
            .AddColumn("[bold lime]💵 Price[/]");
        foreach (var item in Cart)
        {
            var product = DataManger.ProductDB.GetById(item.Key);
            if (product == null)
            {
                Print.OutLine("❌ Product not found in the database.", ConsoleColor.Red);
                return;
            }
            table.AddRow(
                $"[aqua]{product.Name}[/]",
                $"[green]{item.Value.Size.Size}[/]",
                $"[orange1]{item.Value.Quantity}[/]",
                $"[yellow]${(double)product.Price * item.Value.Quantity:F2}[/]"
            );
        }
        AnsiConsole.Write(table);
        double total = Cart.Sum(item => (double)(DataManger.ProductDB.GetById(item.Key)?.Price ?? 0) * item.Value.Quantity);
        Print.OutLine("\n══════════════════════════════════════════════════════", ConsoleColor.DarkGray);
        Print.OutLine($"🧾 [Total] Total amount: ${total:F2}", ConsoleColor.Cyan);
        Print.OutLine("══════════════════════════════════════════════════════\n", ConsoleColor.DarkGray);
    }
    // ✅ View History
    private void ViewHistory()
    {
        Console.Clear();
        Print.OutLine("\n📜 Order History", ConsoleColor.Yellow);
        Print.OutLine("══════════════════════════════════════════════════════", ConsoleColor.DarkYellow);
        var orders = DataManger.OrderDB.GetAll();
        var userOrders = orders.Where(order => order.UserId == GlobalStore.Instance.CurrentUser?.Id);
        if (!userOrders.Any())
        {
            Print.OutLine("🗂️ No order history found for your account.", ConsoleColor.Yellow);
            return;
        }
        var table = new Table()
            .Border(TableBorder.Rounded)
            .BorderColor(Color.Grey)
            .AddColumn("[bold lime]🆔 Order ID[/]")
            .AddColumn("[bold lime]📅 Date[/]")
            .AddColumn("[bold lime]💰 Total[/]");
        foreach (var order in userOrders)
        {
            double total = order.Items.Sum(item => (double)(DataManger.ProductDB.GetById(item.ProductId)?.Price ?? 0) * item.Quantity);
            table.AddRow(
                $"[aqua]{order.Id}[/]",
                $"[green]{order.Date:yyyy-MM-dd}[/]",
                $"[yellow]${total:F2}[/]"
            );
        }
        AnsiConsole.Write(table);
        Print.OutLine("══════════════════════════════════════════════════════\n", ConsoleColor.DarkGray);
    }
    //✅ CheckOut
    private void Checkout()
    {
        Console.Clear();
        if (!Cart.Any())
        {
            Print.OutLine("⚠️ Your cart is currently empty.", ConsoleColor.Yellow);
            return;
        }
        double total = Cart.Sum(item => (double)(DataManger.ProductDB.GetById(item.Key)?.Price ?? 0) * item.Value.Quantity);
        Print.OutLine($"\n💰 [Total] Total amount due: ${total:F2}", ConsoleColor.Cyan);

        while (true)
        {
            var paymentMethods = new List<string> { "PayPal", "Visa", "CashOnDelivery", "Back" };
            var paymentMethod = Print.AskChoice("💳 Select your payment method:", paymentMethods);
            if (paymentMethod == "Back") return;
            bool isPaid = false;
            while (!isPaid)
            {
                double amountPaid = 0;
                //PayPal
                if (paymentMethod == "PayPal")
                {
                    var payOptions = new List<string> { "Balance", "Visa", "BankAccount", "PayPalCredit", "Back" };
                    var payType = Print.AskChoice("💼 Select PayPal payment type:", payOptions);
                    if (payType == "Back") break;
                    if (payType == "Balance" || payType == "PayPalCredit" || payType == "BankAccount")
                    {
                        var input = Print.Ask("💵 Enter amount to pay (or 'back' to return): ");
                        if (input.ToLower() == "back") break;
                        if (!double.TryParse(input, out amountPaid))
                        {
                            Print.OutLine("❌ Invalid amount. Try again.", ConsoleColor.Red);
                            continue;
                        }
                    }
                    // PayPal Visa
                    else if (payType == "Visa")
                    {
                        var cardNumber = Print.Ask("💳 Enter 8-digit Visa card number (or 'back' to return): ");
                        if (cardNumber.ToLower() == "back") break;
                        if (!cardNumber.All(char.IsDigit) || cardNumber.Length != 8)
                        {
                            Print.OutLine("❌ Invalid card number. Please enter exactly 8 digits.", ConsoleColor.Red);
                            continue;
                        }
                        var password = Print.AskHidden("🔑 Enter your password card (or 'back' to return): ");
                        if (password.ToLower() == "back") break;
                        if (!password.All(char.IsDigit))
                        {
                            Print.OutLine("❌ Invalid password. Only digits allowed.", ConsoleColor.Red);
                            continue;
                        }
                        var input = Print.Ask("💵 Enter amount to pay (or 'back' to return): ");
                        if (input.ToLower() == "back") break;
                        if (!double.TryParse(input, out amountPaid))
                        {
                            Print.OutLine("❌ Invalid amount.", ConsoleColor.Red);
                            continue;
                        }
                    }
                }
                //Visa
                else if (paymentMethod == "Visa")
                {
                    var cardNumber = Print.Ask("💳 Enter 8-digit Visa card number (or 'back' to return): ");
                    if (cardNumber.ToLower() == "back") break;
                    var password = Print.Ask("🔑 Enter your password card (or 'back' to return): ");
                    if (password.ToLower() == "back") break;
                    if (!cardNumber.All(char.IsDigit) || cardNumber.Length != 8 || !password.All(char.IsDigit))
                    {
                        Print.OutLine("❌ Invalid card number or password.", ConsoleColor.Red);
                        continue;
                    }
                    var input = Print.Ask("💵 Enter amount to pay (or 'back' to return): ");
                    if (input.ToLower() == "back") break;
                    if (!double.TryParse(input, out amountPaid))
                    {
                        Print.OutLine("❌ Invalid amount.", ConsoleColor.Red);
                        continue;
                    }
                }
                // Cash
                else if (paymentMethod == "CashOnDelivery")
                {
                    var input = Print.Ask("💵 Enter amount to pay (or 'back' to return): ");
                    if (input.ToLower() == "back") break;
                    if (!double.TryParse(input, out amountPaid))
                    {
                        Print.OutLine("❌ Invalid amount.", ConsoleColor.Red);
                        continue;
                    }
                }
                if (amountPaid < total)
                {
                    Print.OutLine("❌ Amount is less than total. Enter the required quantity, ya poor...", ConsoleColor.Red);
                    continue;
                }
                if (amountPaid > total)
                {
                    double tip = amountPaid - total;
                    Print.OutLine($"💰 Thank you for the tip! Extra paid: ${tip:F2}", ConsoleColor.Cyan);
                }
                isPaid = true;
            }
            if (!isPaid) return;
            var orderItems = Cart.Select(item => new OrderItem(item.Key, item.Value.Quantity, item.Value.Size.Size)).ToList();
            var order = new Order(GlobalStore.Instance.CurrentUser?.Id ?? 0, paymentMethod)
            {
                Items = orderItems,
                Date = DateTime.Now
            };
            DataManger.OrderDB.Add(order);
            foreach (var item in Cart)
            {
                var product = DataManger.ProductDB.GetById(item.Key);
                if (product != null)
                {
                    var productSize = product.Sizes.FirstOrDefault(s => s.Size == item.Value.Size.Size);
                    if (productSize != null)
                    {
                        productSize.Quantity -= item.Value.Quantity;
                        DataManger.ProductDB.Update(product.Id, product);
                    }
                }
            }
            Cart.Clear();
            PlayCashierSound();
            Print.OutLine("\n══════════════════════════════════════════════════════════════", ConsoleColor.DarkCyan);
            Print.OutLine("🎉 Your order has been placed successfully! 🎉", ConsoleColor.Green);
            Print.OutLine($"✅ Thank you, {GlobalStore.Instance.CurrentUser?.Username}, for shopping with us!", ConsoleColor.Cyan);
            Print.OutLine("🛍️  We hope to see you again soon!", ConsoleColor.Magenta);
            Print.OutLine("══════════════════════════════════════════════════════════════\n", ConsoleColor.DarkCyan);
            Print.Pausecounter();
            break;
        }
    }
    // Cashier Sound
    public void PlayCashierSound()
    {
        try
        {
            var filePath = "Utils/cashier.wav";

            // Console.WriteLine("File path: " + filePath);
            if (File.Exists(filePath))
            {
#pragma warning disable CA1416
                SoundPlayer player = new(filePath);
                player.PlaySync();
#pragma warning restore CA1416
            }
            else
            {
                Console.WriteLine("The sound file does not exist at the specified path.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error playing sound: " + ex.Message);
        }
    }
}