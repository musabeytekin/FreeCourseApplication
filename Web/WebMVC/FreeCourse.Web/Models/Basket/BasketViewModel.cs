namespace FreeCourse.Web.Models.Basket;

public class BasketViewModel
{
    public string UserId { get; set; }
    public string? DiscountCode { get; set; }

    public int? discountRate { get; set; }

    private List<BasketItemViewModel> _basketItems { get; set; }

    public List<BasketItemViewModel> BasketItems
    {
        get
        {
            if (HasDiscount)
            {
                _basketItems.ForEach(basketItem =>
                {
                    var discountPrice = basketItem.Price * ((decimal)discountRate! / 100);
                    basketItem.ApplyDiscount(discountPrice);
                });
            }

            return _basketItems;
        }

        set => _basketItems = value;
    }

    public decimal TotalPrice => BasketItems.Sum(x => x.CurrentPrice * x.Quantity);

    public bool HasDiscount => !string.IsNullOrEmpty(DiscountCode);
}