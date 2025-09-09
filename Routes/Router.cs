using App.Pages;

namespace App.Routes;

public class Router
{
    private readonly Dictionary<string, Route> _routes = new();
    private string _currentPath = "";

    public void Register(string path, Func<Page> factory)
    {
        _routes[path] = new Route
        {
            PageFactory = factory,
        };
    }

    public void Navigate(string path)
    {
        if (!_routes.TryGetValue(path, out var route))
        {
            Console.WriteLine($"Page {path} not found");
            RerouteCurrent();
            return;
        }
        _currentPath = path;
        var page = route.PageFactory();
        Console.Clear();
        page.Display();
        page.HandleInput(this);
    }

    public void Start(string startPath)
    {
        Navigate(startPath);
    }

    public void RerouteCurrent()
    {
        Navigate(_currentPath);
    }

    public void Exit()
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.WriteLine("╔════════════════════════════════════════════════════════════════════════════════════╗");
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("║                                                                                    ║  ");
        Console.WriteLine("║        ✨ Thank you for visiting ELOSTORA's Shops — Where Legends Shop! ✨         ║   ");
        Console.WriteLine("║                                                                                    ║ ");
        Console.WriteLine("║        🛍️ Fashion. 🧠 Innovation. 💎 Elegance. All in one legendary place.          ║  ");
        Console.WriteLine("║                                                                                    ║");
        Console.WriteLine("║         We hope your shopping journey was as legendary as your style.              ║ ");
        Console.WriteLine("║                                                                                    ║  ");
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine("║        Developed With everything I've learned💻| Powered by Eng/ Mo7amed Ramy      ║  ");
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("║                                                                                    ║ ");
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.WriteLine("╚════════════════════════════════════════════════════════════════════════════════════╝");
        Environment.Exit(0);
    }
}