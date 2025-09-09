using App.Pages;
namespace App.Routes;
public class Route
{
    public required Func<Page> PageFactory { get; set; }
}