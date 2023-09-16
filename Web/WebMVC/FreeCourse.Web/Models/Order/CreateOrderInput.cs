namespace FreeCourse.Web.Models.Order;

public class CreateOrderInput
{

    public CreateOrderInput()
    {
        OrderItems = new List<OrderItemCreateInput>();
    }
    
    public string? BuyerId { get; set; }
    public List<OrderItemCreateInput> OrderItems { get; set; }
    public AddressCreateInput Address { get; set; }
}