using App.Models;
using App.Routes;
using App.Services;
using App.Utils;
using Spectre.Console;
namespace App.Pages;
public class LoginPage : Page
{
    private static string? LastUsedEmail = null;
    public override void Display()
    {
        AnsiConsole.Clear();
        Print.OutLine("🔐 LOGIN TO YOUR ACCOUNT", ConsoleColor.DarkCyan);
        Console.WriteLine();
    }
    public override void HandleInput(Router router)
    {
        while (true)
        {
            AnsiConsole.Clear();
            Display();

            var input = Print.AskChoice(
                "⚠️  Note: When you register the account, do not forget at the end (@gmail.com)",
                new List<string> { "📧 Enter Email:" }, allowBack: true);

            if (input == "⬅ Back")
            {
                router.Navigate("home");
                return;
            }
            Console.WriteLine();
            string email;
            if (LastUsedEmail != null)
            {
                Print.Out("📨 Enter Email => ", ConsoleColor.Cyan);
                Print.Out($"(Press Enter to use last Email : {LastUsedEmail}) : ", ConsoleColor.DarkGray);
                Console.WriteLine();

                var tempInput = Console.ReadLine()?.Trim();
                email = string.IsNullOrWhiteSpace(tempInput) ? LastUsedEmail : tempInput!;
            }
            else
            {
                Print.Out("📨 Email => ", ConsoleColor.Cyan);
                Console.ForegroundColor = ConsoleColor.White;
                email = (Console.ReadLine() ?? "").Trim();
                Console.ResetColor();
            }
            if (string.IsNullOrWhiteSpace(email))
            {
                Print.OutLine("❌ Email cannot be empty. Please try again.", ConsoleColor.Red);
                ContinueOrBack(router);
                continue;
            }
            Console.WriteLine();
            string password;
            try
            {
                password = AnsiConsole.Prompt(
                    new TextPrompt<string>("🔑 [red]Password[/]:").Secret());
            }
            catch
            {
                Print.OutLine("❌ Error reading password. Please try again.", ConsoleColor.Red);
                ContinueOrBack(router);
                continue;
            }
            Console.WriteLine();
            // ✅ Admin
            if (AdminManager.IsAdmin(email, password))
            {
                GlobalStore.Instance.CurrentUser = AdminManager.GetAdmin();
                Print.SuccessMsg("🎉 Welcome Boss..👑");
                Console.WriteLine();
                var option = Print.AskChoice("Choose your next destination:", new List<string>
                {
                    "🛠 Go to Admin Page",
                    "🛒 Go to Shop Page as Admin"
                });

                Console.WriteLine();
                router.Navigate(option == "🛠 Go to Admin Page" ? "admin" : "shop");
                return;
            }
            var usersList = DataManger.UserDB.GetAll();
            var user = usersList.FirstOrDefault(u =>
                u.Email.Equals(email, StringComparison.OrdinalIgnoreCase) &&
                u.Password == password);

            if (user == null)
            {
                Print.OutLine("❌ Email or Password is incorrect. Please try again.", ConsoleColor.Red);
                ContinueOrBack(router);
                continue;
            }
            GlobalStore.Instance.CurrentUser = user;
            Print.SuccessMsg($"👋 Welcome : {user.Username}!");
            Console.WriteLine();

            if (!user.IsAdmin)
                LastUsedEmail = user.Email;

            router.Navigate(user.IsAdmin ? "admin" : "shop");
            return;
        }
    }
    private void ContinueOrBack(Router router)
    {
        var choice = Print.AskChoice("What do you want to do next ❓", new List<string>
        {
            "🔁 Try Again",
            "🏠 Back to Home"
        });

        if (choice == "🏠 Back to Home")
            router.Navigate("home");
    }
}