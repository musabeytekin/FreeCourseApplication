using FreeCourse.Services.Order.Domain.Core;

namespace FreeCourse.Services.Order.Domain.OrderAggregate;

public class Order : Entity, IAggregateRoot
{
    public DateTime Type { get; set; }
    public DateTime CreatedDate { get; set; }
    public Address Address { get; set; }
    public string BuyerId { get; set; }
    private readonly List<OrderItem> _orderItems;
    public IReadOnlyCollection<OrderItem> OrderItems => _orderItems;

    public Order(Address address, string buyerId)
    {
        _orderItems = new List<OrderItem>();
        CreatedDate = DateTime.Now;
        Address = address;
        BuyerId = buyerId;
    }

    public void AddOrderItem(string productId, string productName, string pictureUrl, decimal price)
    {
        var existProduct = _orderItems.Any(x => x.ProductId == productId);
        if (!existProduct)
        {
            _orderItems.Add(new OrderItem(productId, productName, pictureUrl, price));
        }
    }

    public decimal GetTotalPrice => _orderItems.Sum(x => x.Price);
}