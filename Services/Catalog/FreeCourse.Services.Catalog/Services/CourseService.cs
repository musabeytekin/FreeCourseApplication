using System;
using AutoMapper;
using FreeCourse.Services.Catalog.DTOs;
using FreeCourse.Services.Catalog.Models;
using FreeCourse.Services.Catalog.Settings;
using FreeCourse.Shared.DTOs;
using FreeCourse.Shared.Messages;
using Mass = MassTransit;
using MassTransit.RabbitMqTransport;
using MongoDB.Driver;

namespace FreeCourse.Services.Catalog.Services
{
	public class CourseService : ICourseService
	{

		private readonly IMongoCollection<Course> _courseCollection;
		private readonly IMongoCollection<Category> _categoryCollection;
		private readonly IMapper _mapper;
        private readonly Mass.IPublishEndpoint _publishEndpoint;

        public CourseService(IMapper mapper, IDatabaseSettings databaseSettings, Mass.IPublishEndpoint publishEndpoint)
        {
            var client = new MongoClient(databaseSettings.ConnectionString);
            var database = client.GetDatabase(databaseSettings.DatabaseName);

            _courseCollection = database.GetCollection<Course>(databaseSettings.CourseCollectionName);
            _categoryCollection = database.GetCollection<Category>(databaseSettings.CategoryCollectionName);
            _mapper = mapper;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<Shared.DTOs.Response<List<CourseDto>>> GetAllAsync()
        {
            var courses = await _courseCollection.Find<Course>(course => true).ToListAsync();

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

            return Shared.DTOs.Response<List<CourseDto>>.Success(_mapper.Map<List<CourseDto>>(courses), 200);
        }


        public async Task<Shared.DTOs.Response<CourseDto>> GetByIdAsync(string id)
        {
            var course = await _courseCollection.Find<Course>(course => course.Id == id).FirstOrDefaultAsync();

            if(course is null)
            {
                return Shared.DTOs.Response<CourseDto>.Fail($"Course id ({id}) not found", 404);
            }

            course!.Category = await _categoryCollection.Find<Category>(category => category.Id == course.CategoryId).FirstAsync();

            return Shared.DTOs.Response<CourseDto>.Success(_mapper.Map<CourseDto>(course), 200);
        }


        public async Task<Shared.DTOs.Response<List<CourseDto>>> GetAllByUserIdAsync(string userId)
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

            return Shared.DTOs.Response<List<CourseDto>>.Success(_mapper.Map<List<CourseDto>>(courses), 200);
        }

        public async Task<Shared.DTOs.Response<CourseDto>> CreateCourseAsync(CourseCreateDto course)
        {
            var newCourse = _mapper.Map<Course>(course);

            newCourse.CreatedTime = DateTime.Now;

            await _courseCollection.InsertOneAsync(newCourse);

            return Shared.DTOs.Response<CourseDto>.Success(_mapper.Map<CourseDto>(newCourse),200);

        }

        public async Task<Shared.DTOs.Response<NoContent>> UpdateCourseAsync(CourseUpdateDto course)
        {
            var updateCourse = _mapper.Map<Course>(course);

            var result = await _courseCollection.FindOneAndReplaceAsync(c => c.Id == course.Id, updateCourse);

            if(result is null)
            {
                return Shared.DTOs.Response<NoContent>.Fail("Course not found", 404);
            }

            await _publishEndpoint.Publish<CourseNameChangedEvent>(new
            {
                CourseId = course.Id,
                UpdatedName = course.Name
            });
            
            return Shared.DTOs.Response<NoContent>.Success(204);
           
        }

        public async Task<Shared.DTOs.Response<NoContent>> DeleteCourseAsync(string id)
        {
            var result = await _courseCollection.DeleteOneAsync(course => course.Id == id);

            if(result.DeletedCount > 0)
            {
                return Shared.DTOs.Response<NoContent>.Success(204);
            }
            else
            {
                return Shared.DTOs.Response<NoContent>.Fail($"Course ({id}) not found", 404);
            }
        }

    }
}

