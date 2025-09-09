using App.Routes;
using App.Utils;
using Spectre.Console;

namespace App.Pages;

public class HomePage : Page
{
    public override void Display()
    {
        Console.Clear();
Print.OutLine("╔═══════════════════════════════════════════════════════════════════════════════════════════════════════════════════════════════╗", ConsoleColor.DarkCyan);
Print.OutLine("║                                                   🛍️  W E L C O M E   T O...                                                   ║", ConsoleColor.Cyan);
Print.OutLine("║                                                                                                                               ║", ConsoleColor.Cyan);
Print.OutLine("║   ███████╗  ██╗        ██████╗    ███████╗   ███████╗   ██████╗   ██████╗    ██████╗     ██████    ██   ██   ██████   ██████  ║", ConsoleColor.Cyan);
Print.OutLine("║   ██╔════╝  ██║       ██╔═══██╗   ██╔════╝  ╚══██╔══╝  ██╔═══██╗  ██╔══██╗  ██╔═══██╗    ██        ██   ██  ██    ██  ██  ██  ║", ConsoleColor.Blue);
Print.OutLine("║   █████╗    ██║       ██║   ██║   █████╗       ██║     ██║   ██║  ██████╔╝  ██║   ██║    ██████    ███████  ██    ██  ██████  ║", ConsoleColor.Cyan);
Print.OutLine("║   ██╔══╝    ██║       ██║   ██║   ╚═══██╗      ██║     ██║   ██║  ██╔══██╗  ███████║         ██    ██   ██  ██    ██  ██      ║", ConsoleColor.Blue);
Print.OutLine("║   ███████╗  ███████╗  ╚██████╔╝   ██████║      ██║     ╚██████╔╝  ██║  ██║  ██╔═══██║    ██████    ██   ██   ██████   ██      ║", ConsoleColor.Cyan);
Print.OutLine("║   ╚══════╝  ╚══════╝   ╚═════╝    ╚═════╝      ╚═╝      ╚═════╝   ╚═╝  ╚═╝  ╚══════╝                                          ║", ConsoleColor.Gray);
Print.OutLine("║                                                                                                                               ║", ConsoleColor.Cyan);
Print.OutLine("║                                              Your Fashion Journey Begins with Us!...                                          ║", ConsoleColor.DarkCyan);
Print.OutLine("║                                                🧢.👕.the clothing version.👖.👞                                               ║", ConsoleColor.DarkCyan);
Print.OutLine("╚═══════════════════════════════════════════════════════════════════════════════════════════════════════════════════════════════╝", ConsoleColor.DarkCyan);

Print.OutLine("", ConsoleColor.White);
Print.OutLine("✨ELOSTORA — Where Fashion Meets Elegance✨", ConsoleColor.DarkCyan);
Print.OutLine("", ConsoleColor.White);
Print.OutLine("➡️ Press any key to begin your shopping journey...🧭", ConsoleColor.Gray);
Console.ReadKey();
        AnsiConsole.MarkupLine("\n[bold gold1]✨Your legend of shopping starts here!✨[/]\n");
        AnsiConsole.MarkupLine("[dim gray]ELOSTORA Store - Fashion, Tech, and more...[/]");
    }
    public override void HandleInput(Router router)
    {
        while (true)
        {
            Console.WriteLine();
            var input = Print.AskChoice("[bold cyan]\nSelect an option:[/]", new List<string>
            {
                "📝 Sign up",
                "🔐 Log in",
                "❌ Exit"
            }); 

            switch (input)
            {
                case "📝 Sign up":
                    router.Navigate("signup");
                    return;
                case "🔐 Log in":
                    router.Navigate("login");
                    return;
                case "❌ Exit":
                    router.Exit();
                    return;
            }
        }
    }
}
