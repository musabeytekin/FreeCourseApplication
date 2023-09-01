using System;
using AutoMapper;
using FreeCourse.Services.Catalog.DTOs;
using FreeCourse.Services.Catalog.Models;
using FreeCourse.Services.Catalog.Settings;
using FreeCourse.Shared.DTOs;
using MongoDB.Bson;
using MongoDB.Driver;

namespace FreeCourse.Services.Catalog.Services
{
	public class CourseService : ICourseService
	{

		private readonly IMongoCollection<Course> _courseCollection;
		private readonly IMongoCollection<Category> _categoryCollection;
		private readonly IMapper _mapper;

        public CourseService(IMapper mapper, IDatabaseSettings databaseSettings)
        {
            var client = new MongoClient(databaseSettings.ConnectionString);
            var database = client.GetDatabase(databaseSettings.DatabaseName);

            _courseCollection = database.GetCollection<Course>(databaseSettings.CourseCollectionName);
            _categoryCollection = database.GetCollection<Category>(databaseSettings.CategoryCollectionName);
            _mapper = mapper;
        }

        public async Task<Response<List<CourseDto>>> GetAllAsync()
        {
            var courses = await _courseCollection.Find(course => true).ToListAsync();

            if(courses.Any())
            {
                foreach (var course in courses)
                {
                    course.Category = await _categoryCollection.Find<Category>(category => category.Id == course.CategoryId).FirstAsync();
                }
            }
            else
            {
                courses = new List<Course>();
            }

            return Response<List<CourseDto>>.Success(_mapper.Map<List<CourseDto>>(courses), 200);
        }


        public async Task<Response<CourseDto>> GetByIdAsync(string id)
        {
            var course = await _courseCollection.Find<Course>(course => course.Id == id).FirstAsync();

            if(course is not null)
            {
                return Response<CourseDto>.Fail($"Course id ({id}) not found", 404);
            }

            course!.Category = await _categoryCollection.Find<Category>(category => category.Id == course.CategoryId).FirstAsync();

            return Response<CourseDto>.Success(_mapper.Map<CourseDto>(course), 200);
        }


        public async Task<Response<List<CourseDto>>> GetAllByUserIdAsync(string userId)
        {
            var courses = await _courseCollection.Find<Course>(course => course.UserId == userId).ToListAsync();

            if (courses.Any())
            {
                foreach (var course in courses)
                {
                    course.Category = await _categoryCollection.Find<Category>(category => category.Id == course.CategoryId).FirstAsync();
                }
            }
            else
            {
                courses = new List<Course>();
            }

            return Response<List<CourseDto>>.Success(_mapper.Map<List<CourseDto>>(courses), 200);
        }

        public async Task<Response<CourseDto>> CreateCourseAsync(CourseCreateDto course)
        {
            var newCourse = _mapper.Map<Course>(course);

            newCourse.CreatedTime = DateTime.Now;

            await _courseCollection.InsertOneAsync(newCourse);

            return Response<CourseDto>.Success(_mapper.Map<CourseDto>(newCourse),200);

        }

        public async Task<Response<NoContent>> UpdateCourseAsync(CourseUpdateDto course)
        {
            var updateCourse = _mapper.Map<Course>(course);

            var result = await _courseCollection.FindOneAndReplaceAsync(c => c.Id == course.Id, updateCourse);

            if(result is null)
            {
                return Response<NoContent>.Fail("Course not found", 404);
            }

            return Response<NoContent>.Success(200);
           
        }

        public async Task<Response<NoContent>> DeleteCourseAsync(string id)
        {
            var result = await _courseCollection.DeleteOneAsync(course => course.Id == id);

            if(result.DeletedCount > 0)
            {
                return Response<NoContent>.Success(204);
            }
            else
            {
                return Response<NoContent>.Fail($"Course ({id}) not found", 404);
            }
        }

    }
}

