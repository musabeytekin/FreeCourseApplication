using System;
using FreeCourse.Services.Catalog.DTOs;
using FreeCourse.Shared.DTOs;

namespace FreeCourse.Services.Catalog.Services
{
	public interface ICourseService
	{
		Task<Response<List<CourseDto>>> GetAllAsync();
		Task<Response<CourseDto>> GetByIdAsync(string id);
		Task<Response<List<CourseDto>>> GetAllByUserIdAsync(string userId);
		Task<Response<CourseDto>> CreateCourse(CourseCreateDto course);
		Task<Response<NoContent>> UpdateCourse(CourseUpdateDto course);
		Task<Response<NoContent>> DeleteCourse(string id);
    }
}

