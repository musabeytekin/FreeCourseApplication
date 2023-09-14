using FreeCourse.Shared.DTOs;
using FreeCourse.Web.Helpers;
using FreeCourse.Web.Models;
using FreeCourse.Web.Models.Catalog;
using FreeCourse.Web.Services.Interfaces;

namespace FreeCourse.Web.Services;

public class CatalogService : ICatalogService
{
    private readonly HttpClient _httpClient;
    private readonly IPhotoStockService _photoStockService;
    private readonly PhotoHelper _photoHelper;

    public CatalogService(HttpClient httpClient, IPhotoStockService photoStockService, PhotoHelper photoHelper)
    {
        _httpClient = httpClient;
        _photoStockService = photoStockService;
        _photoHelper = photoHelper;
    }

    public async Task<List<CourseViewModel>> GetAllCourseAsync()
    {
        var response = await _httpClient.GetAsync("courses");

        if (!response.IsSuccessStatusCode)
        {
            return new List<CourseViewModel>();
        }

        var responseSuccess = await response.Content.ReadFromJsonAsync<Response<List<CourseViewModel>>>();

        responseSuccess.Data.ForEach(x => { x.StockPictureUrl = _photoHelper.GetPhotoStockUrl(x.Picture); });

        return responseSuccess.Data;
    }

    public async Task<List<CourseViewModel>> GetAllCourseByUserIdAsync(string userId)
    {
        var response = await _httpClient.GetAsync($"courses/user/{userId}");
        //http://localhost:5000/services/catalog/courses/user/1
        //http://localhost:5011/api/courses/user/userid


        if (!response.IsSuccessStatusCode)
        {
            return new List<CourseViewModel>();
        }

        var responseSuccess = await response.Content.ReadFromJsonAsync<Response<List<CourseViewModel>>>();

        responseSuccess.Data.ForEach(x => { x.StockPictureUrl = _photoHelper.GetPhotoStockUrl(x.Picture); });
        return responseSuccess.Data;
    }

    public async Task<CourseViewModel> GetByCourseId(string courseId)
    {
        var response = await _httpClient.GetAsync($"courses/{courseId}");

        if (!response.IsSuccessStatusCode)
        {
            return new CourseViewModel();
        }

        var responseSuccess = await response.Content.ReadFromJsonAsync<Response<CourseViewModel>>();
        responseSuccess.Data.StockPictureUrl = _photoHelper.GetPhotoStockUrl(responseSuccess.Data.Picture);
        return responseSuccess.Data;
    }

    public async Task<List<CategoryViewModel>> GetAllCategoryAsync()
    {
        var response = await _httpClient.GetAsync("categories");

        if (!response.IsSuccessStatusCode)
        {
            return new List<CategoryViewModel>();
        }

        var responseSuccess = await response.Content.ReadFromJsonAsync<Response<List<CategoryViewModel>>>();

        return responseSuccess.Data;
    }

    public async Task<bool> DeleteCourseAsync(string courseId)
    {
        var response = await _httpClient.DeleteAsync($"courses/{courseId}");
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateCourseAsync(CourseUpdateInput courseUpdateInput)
    {
        var photoResult = await _photoStockService.UploadPhoto(courseUpdateInput.PhotoFormFile);
        if (photoResult is not null)
        {
            await _photoStockService.DeletePhoto(courseUpdateInput.Picture);
            courseUpdateInput.Picture = photoResult.Url;
        }
        
        var response = await _httpClient.PutAsJsonAsync<CourseUpdateInput>("courses", courseUpdateInput);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> CreateCourseAsync(CourseCreateInput courseCreateInput)
    {
        var photo = await _photoStockService.UploadPhoto(courseCreateInput.PhotoFormFile);
        if (photo is not null)
        {
            courseCreateInput.Picture = photo.Url;
        }

        var response = await _httpClient.PostAsJsonAsync<CourseCreateInput>("courses", courseCreateInput);
        return response.IsSuccessStatusCode;
    }
}