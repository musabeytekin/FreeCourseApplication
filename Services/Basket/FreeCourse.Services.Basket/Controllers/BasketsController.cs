using Microsoft.AspNetCore.Mvc;

namespace FreeCourse.Services.Basket.Controllers;

public class BasketsController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}