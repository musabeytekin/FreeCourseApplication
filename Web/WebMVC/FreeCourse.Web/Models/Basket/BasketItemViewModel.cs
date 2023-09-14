namespace FreeCourse.Web.Models.Basket;

public class BasketItemViewModel
{
    public int Quantity { get; set; }
    
    public string CourseId { get; set; }
    
    public string CourseName { get; set; }
    
    public decimal Price { get; set; }
    
    public decimal CurrentPrice
    {
        get => DiscountAppliedPrice is not null ? DiscountAppliedPrice.Value : Price;
    }
    private decimal? DiscountAppliedPrice { get; set; }
    
    public void ApplyDiscount(decimal discountPrice)
    {
        DiscountAppliedPrice = discountPrice;
    }
    
    
}