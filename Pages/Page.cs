using App.Routes;
namespace App.Pages;
public abstract class Page
{
    public abstract void Display();
    public abstract void HandleInput(Router router);
}