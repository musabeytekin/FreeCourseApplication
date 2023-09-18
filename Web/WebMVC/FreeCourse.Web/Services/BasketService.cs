using FreeCourse.Shared.DTOs;
using FreeCourse.Web.Models.Basket;
using FreeCourse.Web.Services.Interfaces;

namespace FreeCourse.Web.Services;

public class BasketService : IBasketService
{
    private readonly HttpClient _httpClient;
    private readonly IDiscountService _discountService;

    public BasketService(HttpClient httpClient, IDiscountService discountService)
    {
        _httpClient = httpClient;
        _discountService = discountService;
    }

    public async Task<bool> SaveOrUpdate(BasketViewModel basketViewModel)
    {
        var response = await _httpClient.PostAsJsonAsync<BasketViewModel>("baskets", basketViewModel);

        return response.IsSuccessStatusCode;
    }

    public async Task<BasketViewModel?> Get()
    {
        var response = await _httpClient.GetAsync("baskets");

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var basketViewModel = await response.Content.ReadFromJsonAsync<Response<BasketViewModel>>();

        return basketViewModel.Data;
    }

    public async Task<bool> Delete()
    {
        var response = await _httpClient.DeleteAsync("baskets");

        return response.IsSuccessStatusCode;
    }

    public async Task AddBasketItem(BasketItemViewModel basketItemViewModel)
    {
        var basket = await Get();

        if (basket is not null)
        {
            if (!basket.BasketItems.Any(x => x.CourseId == basketItemViewModel.CourseId))
            {
                basket.BasketItems.Add(basketItemViewModel);
            }
        }
        else
        {
            basket = new BasketViewModel();
            basket.BasketItems.Add(basketItemViewModel);
        }

        await SaveOrUpdate(basket);
    }

    public async Task<bool> RemoveBasketItem(string courseId)
    {
        var basket = await Get();

        if (basket is null)
            return false;

        var deleteItem = basket.BasketItems.FirstOrDefault(x => x.CourseId == courseId);

        if (deleteItem is null)
            return false;

        var deleteResult = basket.BasketItems.Remove(deleteItem);

        if (!deleteResult)
        {
            return false;
        }

        if (!basket.BasketItems.Any())
        {
            basket.DiscountCode = null;
        }

        return await SaveOrUpdate(basket);
    }

    public async Task<bool> ApplyDiscount(string discountCode)
    {
        await CancelApplyDiscount();
        var basket = await Get();

        if (basket is null)
            return false;

        var discount = await _discountService.GetDiscount(discountCode);

        if (discount is null)
            return false;

        basket.ApplyDiscount(discount.Code, discount.Rate);


        await SaveOrUpdate(basket);

        return true;
    }

    public async Task<bool> CancelApplyDiscount()
    {
        var basket = await Get();

        if (basket is null)
            return false;

        basket.CancelDiscount();

        return await SaveOrUpdate(basket);
    }
}