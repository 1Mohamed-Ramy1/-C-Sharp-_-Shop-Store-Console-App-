namespace App.Models;
using App.Services;
public class Order(int userId, string paymentMethod) : IIdentifiable
{
    public int Id { get; set; }
    public int UserId { get; set; } = userId;
    public List<OrderItem> Items { get; set; } = [];
    public string PaymentMethod { get; set; } = paymentMethod;
    public DateTime Date { get; set; } = new DateTime();
    public override string ToString()
    {
        return $"Order(id:{Id}, userId:{UserId})";
    }
}
public class OrderItem(int productId, int quantity, SizeUnit size)
{
    public int ProductId { get; set; } = productId;
    public int Quantity { get; set; } = quantity;

    public SizeUnit Size { get; set; } = size;
}