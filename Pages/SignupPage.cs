using App.Models;
using App.Routes;
using App.Services;
using App.Utils;
using Spectre.Console;
namespace App.Pages;
public class SignupPage : Page
{
    public override void Display()
    {
        Print.OutLine("📝 CREATE YOUR ACCOUNT", ConsoleColor.DarkCyan);
        Console.WriteLine(new string('═', 40));
    }
    public override void HandleInput(Router router)
    {
        while (true)
        {
            Console.Clear();
            Display();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("⚠️  NOTE: When you register the account, don't forget to include '@gmail.com' at the end of your email.⚠️");
            Console.WriteLine();
            Console.WriteLine("⚠️  Password must be at least 8 characters and include: uppercase, lowercase, and number.⚠️");
            Console.ResetColor();
            Console.WriteLine();
            var option = Print.AskChoice("", new List<string>
        {
            "Start Sign Up"
        }, allowBack: true);

            if (option == "⬅ Back")
            {
                router.Navigate("home");
                return;
            }
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("⚠️  NOTE: When you register the account, don't forget to include '@gmail.com' at the end of your email.⚠️");
            Console.WriteLine();
            Console.WriteLine("⚠️  Password must be at least 8 characters and include: uppercase, lowercase, and number.⚠️");
            Console.ResetColor();
            Console.WriteLine("\n════════════════════════════════════════");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("Username => (Your Name) : ");
            Console.ResetColor();
            var username = (Console.ReadLine() ?? "").Trim();
            Console.WriteLine();
            if (string.IsNullOrWhiteSpace(username))
            {
                Print.ErrorMsg("Username cannot be empty ❌");
                continue;
            }
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("Email: ");
            Console.ResetColor();
            var email = (Console.ReadLine() ?? "").Trim();
            Console.WriteLine();
            if (!email.EndsWith("@gmail.com"))
            {
                Print.ErrorMsg("Email must end with @gmail.com ❌");
                continue;
            }
            var users = DataManger.UserDB.GetAll();
            var user = users.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
            if (user != null)
            {
                Print.ErrorMsg("⚠️ Account already exists");
                var reSignup = Print.AskYesNo("Do you want to try signup again?");
                if (reSignup) continue;
                router.Navigate("home");
                return;
            }
            string password;
            while (true)
            {
                password = AnsiConsole.Prompt(
                    new TextPrompt<string>("[green]Password:[/]")
                        .PromptStyle("red")
                        .Secret());
                Console.WriteLine();
                if (password.Length < 8 ||
                    !password.Any(char.IsUpper) ||
                    !password.Any(char.IsLower) ||
                    !password.Any(char.IsDigit))
                {
                    Print.ErrorMsg("❌ Password must be at least 8 characters and include: uppercase, lowercase, and number.");
                    continue;
                }
                var confirm = AnsiConsole.Prompt(
                    new TextPrompt<string>("[green]Confirm Password:[/]")
                        .PromptStyle("red")
                        .Secret());
                Console.WriteLine();
                if (confirm != password)
                {
                    Print.ErrorMsg("❌ Passwords do not match.");
                    continue;
                }
                if (Print.AskYesNo("👁️ Do you want to view your password before saving?"))
                {
                    Console.WriteLine();
                    Console.WriteLine($"🔐 Your password: {password}");
                    Console.WriteLine();
                    if (!Print.AskYesNo("Is this correct?"))
                    {
                        Print.OutLine("🔁 Let's try again...", ConsoleColor.Yellow);
                        Console.WriteLine();
                        continue;
                    }
                }
                break;
            }
            DataManger.UserDB.Add(new User(username, email, password));
            Console.WriteLine();
            Print.SuccessMsg("✅ Account created successfully 🎉");
            Console.WriteLine();
            var goToLogin = Print.AskYesNo("Do you want to login now?");
            Console.WriteLine();
            if (goToLogin)
            {
                router.Navigate("login");
                return;
            }
            router.Navigate("home");
            return;
        }
    }

}