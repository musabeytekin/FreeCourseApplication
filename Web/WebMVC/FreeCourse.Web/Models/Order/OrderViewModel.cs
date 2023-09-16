namespace FreeCourse.Web.Models.Order;

public class OrderViewModel
{
    public int Id { get; set; }

    public string buyerId { get; set; }

    public List<OrderItemViewModel> OrderItems { get; set; }

    public DateTime CreatedDate { get; set; }
}