using FreeCourse.Shared.DTOs;
using FreeCourse.Web.Models.Basket;
using FreeCourse.Web.Services.Interfaces;

namespace FreeCourse.Web.Services;

public class BasketService : IBasketService
{
    private readonly HttpClient _httpClient;

    public BasketService(HttpClient httpClient)
    {
        _httpClient = httpClient;
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
            if (basket.BasketItems.Any(basketItem => basketItem.CourseId != basketItemViewModel.CourseId))
            {
                basket.BasketItems.Add(basketItemViewModel);
            }
            else
            {
                var updateBasketItem =
                    basket.BasketItems.FirstOrDefault(x => x.CourseId == basketItemViewModel.CourseId);
                updateBasketItem.Quantity += basketItemViewModel.Quantity;
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
        var basket = await Get();

        if (basket is null)
            return false;

        basket.DiscountCode = discountCode;

        return await SaveOrUpdate(basket);
    }

    public async Task<bool> CancelApplyDiscount()
    {
        var basket = await Get();

        if (basket is null)
            return false;

        basket.DiscountCode = null;

        return await SaveOrUpdate(basket);
    }
}