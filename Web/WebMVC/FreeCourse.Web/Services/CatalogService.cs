using FreeCourse.Shared.DTOs;
using FreeCourse.Web.Models;
using FreeCourse.Web.Models.Catalog;
using FreeCourse.Web.Services.Interfaces;

namespace FreeCourse.Web.Services;

public class CatalogService : ICatalogService
{
    private readonly HttpClient _httpClient;

    public CatalogService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<CourseViewModel>> GetAllCourseAsync()
    {
        var response = await _httpClient.GetAsync("courses");

        if (!response.IsSuccessStatusCode)
        {
            return new List<CourseViewModel>();
        }

        var responseSuccess = await response.Content.ReadFromJsonAsync<Response<List<CourseViewModel>>>();

        return responseSuccess.Data;
    }

    public async Task<List<CourseViewModel>> GetAllCourseByUserIdAsync(string userId)
    {
        var address = _httpClient.BaseAddress.ToString();
        var response = await _httpClient.GetAsync($"courses/user/{userId}"); 
        //http://localhost:5000/services/catalog/courses/user/1
        //http://localhost:5011/api/courses/user/userid
                        

        if (!response.IsSuccessStatusCode)
        {
            return new List<CourseViewModel>();
        }

        var responseSuccess = await response.Content.ReadFromJsonAsync<Response<List<CourseViewModel>>>();

        return responseSuccess.Data;
    }

    public async Task<CourseViewModel> GetByCourseId(string courseId)
    {
        var response = await _httpClient.GetAsync($"/courses/{courseId}");

        if (!response.IsSuccessStatusCode)
        {
            return new CourseViewModel();
        }

        var responseSuccess = await response.Content.ReadFromJsonAsync<Response<CourseViewModel>>();

        return responseSuccess.Data;
    }

    public async Task<CategoryViewModel> GetAllCategoryAsync()
    {
        var response = await _httpClient.GetAsync("/categories");

        if (!response.IsSuccessStatusCode)
        {
            return new CategoryViewModel();
        }

        var responseSuccess = await response.Content.ReadFromJsonAsync<Response<CategoryViewModel>>();

        return responseSuccess.Data;
    }

    public async Task<bool> DeleteCourseAsync(string courseId)
    {
        var response = await _httpClient.DeleteAsync($"/courses/{courseId}");
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateCourseAsync(CourseUpdateInput courseUpdateInput)
    {
        var response = await _httpClient.PutAsJsonAsync<CourseUpdateInput>("/courses", courseUpdateInput);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> CreateCourseAsync(CourseCreateInput courseCreateInput)
    {
        var response = await _httpClient.PostAsJsonAsync<CourseCreateInput>("/courses", courseCreateInput);
        return response.IsSuccessStatusCode;
    }
}