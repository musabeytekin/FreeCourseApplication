namespace FreeCourse.Web.Models.Order;

public class OrderItemCreateInput
{
    public string? ProductId { get; set; }
    public string? ProductName { get; set; }
    public decimal Price { get; set; }
    public string? PictureUrl { get; set; }
}