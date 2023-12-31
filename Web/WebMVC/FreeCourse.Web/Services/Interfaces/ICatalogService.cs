using FreeCourse.Web.Models.Catalog;

namespace FreeCourse.Web.Services.Interfaces;

public interface ICatalogService
{
    Task<List<CourseViewModel>> GetAllCourseAsync();
    Task<List<CourseViewModel>> GetAllCourseByUserIdAsync(string userId);
    Task<CourseViewModel> GetByCourseId(string courseId);
    Task<List<CategoryViewModel>> GetAllCategoryAsync();
    Task<bool> DeleteCourseAsync(string courseId);
    Task<bool> UpdateCourseAsync(CourseUpdateInput courseUpdateInput);
    Task<bool> CreateCourseAsync(CourseCreateInput courseCreateInput);
}