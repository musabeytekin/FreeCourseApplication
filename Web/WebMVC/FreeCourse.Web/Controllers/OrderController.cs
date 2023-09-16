using FreeCourse.Web.Models.Order;
using FreeCourse.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FreeCourse.Web.Controllers;

public class OrderController : Controller
{
    private readonly IBasketService _basketService;
    private readonly IOrderService _orderService;

    public OrderController(IBasketService basketService, IOrderService orderService)
    {
        _basketService = basketService;
        _orderService = orderService;
    }

    public async Task<IActionResult> Checkout()
    {
        var basket = await _basketService.Get();

        ViewBag.Basket = basket;
        return View(new CheckoutInfoInput());
    }

    [HttpPost]
    public async Task<IActionResult> Checkout(CheckoutInfoInput checkoutInfoInput)
    {
        //var orderStatus = await _orderService.CreateOrder(checkoutInfoInput);
        var orderStatus = await _orderService.SuspendOrder(checkoutInfoInput);
        if (!orderStatus.IsSuccessful)
        {
            var basket = await _basketService.Get();
            ViewBag.Basket = basket;
            ViewBag.Error = orderStatus.Error;
            return View();
        }

        
        return RedirectToAction(nameof(SuccessfullCheckout), new { orderId = new Random().Next(1, 1000) }); // this is random for now
    }

    public IActionResult SuccessfullCheckout(int orderId)
    {
        ViewBag.OrderId = orderId;
        return View();
    }
    
    public async Task<IActionResult> CheckoutHistory()
    {
        return View(await _orderService.GetOrder());
    }
}