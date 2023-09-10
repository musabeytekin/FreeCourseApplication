using FreeCourse.Web.Models;
using FreeCourse.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FreeCourse.Web.Controllers;

public class AuthController : Controller
{
    private readonly IIdentityService _identityService;

    public AuthController(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    [HttpGet]
    public IActionResult SignIn()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> SignIn(SignInInput signInInput)
    {
        if (!ModelState.IsValid)
            return View();

        var response = await _identityService.SignIn(signInInput);

        if (!response.IsSuccess)
        {
            response.Errors.ForEach(error => ModelState.AddModelError("", error));
            return View();
        }

        return RedirectToAction(nameof(Index), "Home");
    }
}