namespace App.Models;
using App.Services;
public class Product(string name, string category, double price) : IIdentifiable
{
    public int Id { get; set; }
    public string Name { get; set; } = name;

    public string Category { get; set; } = category;
    public double Price { get; set; } = price;

    public List<ProductSize> Sizes { get; set; } = [];

    public override string ToString()
    {
        return $"Product(id:{Id}, name:{Name})";
    }
}
public enum SizeUnit
{
    XS,
    SM,
    MD,
    LG,
    XL,
    XXL
}
public class ProductSize(SizeUnit size, int quantity)
{
    public SizeUnit Size { get; set; } = size;
    public int Quantity { get; set; } = quantity;
}