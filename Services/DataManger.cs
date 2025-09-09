using App.Models;
namespace App.Services;
public static class DataManger
{
    public static JsonDatabase<User> UserDB { get; } = new("Data/users.json");
    public static JsonDatabase<Product> ProductDB { get; } = new("Data/products.json");

    public static JsonDatabase<Order> OrderDB { get; } = new("Data/orders.json");
}