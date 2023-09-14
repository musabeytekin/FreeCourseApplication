using FreeCourse.Web.Models.Basket;
using FreeCourse.Web.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FreeCourse.Web.Controllers;

[Authorize]
public class BasketController : Controller
{
    public ICatalogService _catalogService;
    public IBasketService _basketService;
    
    public BasketController(ICatalogService catalogService, IBasketService basketService)
    {
        _catalogService = catalogService;
        _basketService = basketService;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _basketService.Get());
    }
    
    
    public async Task<IActionResult> AddBasketItem(BasketItemViewModel basketItemViewModel)
    {
        var course = await _catalogService.GetByCourseId(basketItemViewModel.CourseId);
        
        var basketItem = new BasketItemViewModel
        {
            CourseId = course.Id,
            CourseName = course.Name,
            Price = course.Price,
            Quantity = basketItemViewModel.Quantity
        };
        
        await _basketService.AddBasketItem(basketItem);

        return RedirectToAction(nameof(Index));

    }

    public async Task<IActionResult> RemoveBasketItem(string courseId)
    {
        var result = await _basketService.RemoveBasketItem(courseId);

        return RedirectToAction(nameof(Index));
    }
    
}