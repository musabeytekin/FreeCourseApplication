namespace FreeCourse.Web.Models.Basket;

public class BasketViewModel
{
    public BasketViewModel()
    {
        _basketItems = new List<BasketItemViewModel>();
    }

    public string UserId { get; set; }
    public string? DiscountCode { get; set; }

    public int? DiscountRate { get; set; }

    private List<BasketItemViewModel> _basketItems { get; set; }

    public List<BasketItemViewModel> BasketItems
    {
        get
        {
            if (HasDiscount)
            {
                _basketItems.ForEach(basketItem =>
                {
                    var discountPrice = basketItem.Price * ((decimal)DiscountRate! / 100);
                    basketItem.ApplyDiscount(Math.Round(basketItem.Price - discountPrice, 2));
                });
            }

            return _basketItems;
        }

        set => _basketItems = value;
    }

    public decimal TotalPrice => BasketItems.Sum(x => x.CurrentPrice * x.Quantity);

    public bool HasDiscount => !string.IsNullOrEmpty(DiscountCode) && DiscountRate.HasValue;

    public void CancelDiscount()
    {
        DiscountCode = null;
        DiscountRate = null;
    }
    
    public void ApplyDiscount(string discountCode, int discountRate)
    {
        DiscountCode = discountCode;
        DiscountRate = discountRate;
    }
}