using FreeCourse.Shared.Services;
using FreeCourse.Web.Models.Catalog;
using FreeCourse.Web.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FreeCourse.Web.Controllers;

[Authorize]
public class CoursesController : Controller
{
    private readonly ICatalogService _catalogService;
    private readonly ISharedIdentityService _sharedIdentityService;

    public CoursesController(ICatalogService catalogService, ISharedIdentityService sharedIdentityService)
    {
        _catalogService = catalogService;
        _sharedIdentityService = sharedIdentityService;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _catalogService.GetAllCourseByUserIdAsync(_sharedIdentityService.GetUserId));
    }

    public async Task<IActionResult> Create()
    {
        var categories = await _catalogService.GetAllCategoryAsync();

        ViewBag.Categories = new SelectList(categories, "Id", "Name");

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(CourseCreateInput input)
    {
        var categories = await _catalogService.GetAllCategoryAsync();

        ViewBag.Categories = new SelectList(categories, "Id", "Name");

        if (!ModelState.IsValid)
        {
            return View();
        }

        input.UserId = _sharedIdentityService.GetUserId;
        var result = await _catalogService.CreateCourseAsync(input);

        return RedirectToAction(nameof(Index));
    }
    

    public async Task<IActionResult> Update(string id)
    {
        var course = await _catalogService.GetByCourseId(id);
        var categories = await _catalogService.GetAllCategoryAsync();

        if (course is null)
        {
            RedirectToAction(nameof(Index));
        }
        
        ViewBag.Categories = new SelectList(categories, "Id", "Name", course!.Id);
        
        CourseUpdateInput courseUpdateInput = new()
        {
            Id = course.Id,
            Name = course.Name,
            Description = course.Description,
            Price = course.Price,
            Feature = course.Feature,
            CategoryId = course.CategoryId,
            UserId = course.UserId,
            Picture = course.Picture
        };

        return View(courseUpdateInput);
    }
    
    [HttpPost]
    public async Task<IActionResult> Update(CourseUpdateInput input)
    {
        var categories = await _catalogService.GetAllCategoryAsync();

        ViewBag.Categories = new SelectList(categories, "Id", "Name", input.Id);

        if (!ModelState.IsValid)
        {
            return View();
        }

        var result = await _catalogService.UpdateCourseAsync(input);

        return RedirectToAction(nameof(Index));
    }
    
    
}