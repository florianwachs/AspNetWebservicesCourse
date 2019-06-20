using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AspNetCoreSecurity.Domain.Domain;
using AspNetCoreSecurity.Infrastructure.DataAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static AspNetCoreSecurity.Api.AuthConfig;

namespace AspNetCoreSecurity.Api.CourseManagement
{
    [Authorize(Policy = AppPolicies.CanReadCourses)]
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly UniversityDbContext _dbContext;
        private readonly IAuthorizationService _authorizationService;

        public CoursesController(UniversityDbContext dbContext, IAuthorizationService authorizationService)
        {
            _dbContext = dbContext;
            _authorizationService = authorizationService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Course>>> GetAllCourses()
        {
            var courses = await _dbContext.Courses.ToListAsync();
            return Ok(courses);
        }

        [Authorize(Policy = AppPolicies.CanReadStudentsEnrolledInCourse)]
        [HttpGet("{courseId}/students")]
        public async Task<ActionResult<IEnumerable<Student>>> GetCourseStudents(string courseId)
        {
            var course = await _dbContext.Courses
                .Include(c => c.Students)
                .ThenInclude(sc => sc.Student)
                .Where(c => c.Id == courseId).FirstOrDefaultAsync();           

            var studentsOfCourse = course.Students.Select(sc => sc.Student);
            return Ok(studentsOfCourse);
        }
               
        [HttpGet("{courseId}/grades")]
        public async Task<ActionResult<IEnumerable<CourseGrade>>> GetCourseGrades(string courseId)
        {
            var course = await _dbContext.Courses
                .Include(c => c.Grades)
                .ThenInclude(sc => sc.Student)
                .Where(c => c.Id == courseId).FirstOrDefaultAsync();

            if(course == null)
            {
                return NotFound();
            }

            var authorizationResult = await _authorizationService.AuthorizeAsync(User, course, new CanReadCourseGradesRequirement());
            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }

            return Ok(course.Grades);
        }
    }
}
