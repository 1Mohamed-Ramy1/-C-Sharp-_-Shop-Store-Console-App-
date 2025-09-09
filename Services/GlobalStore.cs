using App.Models;
namespace App.Services;
public class GlobalStore
{
    private static GlobalStore? _instance;

    public User? CurrentUser { get; set; }
    public static GlobalStore Instance
    {
        get
        {
            _instance ??= new GlobalStore();
            return _instance;
        }
    }
    private GlobalStore() { }
}