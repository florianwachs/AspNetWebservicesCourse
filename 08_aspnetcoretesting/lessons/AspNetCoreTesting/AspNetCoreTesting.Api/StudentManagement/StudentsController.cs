using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AspNetCoreTesting.Domain.Domain;
using AspNetCoreTesting.Infrastructure.DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreTesting.Api.StudentManagement
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly UniversityDbContext _dbContext;

        public StudentsController(UniversityDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Student>>> GetAllStudents()
        {
            var courses = await _dbContext.Students.ToListAsync();
            return Ok(courses);
        }

        [HttpGet("{studentId}/enrolledcourses")]
        public async Task<ActionResult<IEnumerable<Course>>> GetAllEnrolledCourses(string studentId)
        {
            var student = await _dbContext.Students.Include(s => s.EnrolledCourses).Where(s => s.Id == studentId).FirstOrDefaultAsync();
            if (student == null)
                return NotFound();

            return Ok(student.EnrolledCourses);
        }

        [HttpGet("{studentId}/courserequests")]
        public async Task<ActionResult<IEnumerable<CourseRequest>>> GetAllCourseRequests(string studentId)
        {
            var student = await _dbContext.Students.Include(s => s.CoursesRequests).Where(s => s.Id == studentId).FirstOrDefaultAsync();
            if (student == null)
                return NotFound();

            return Ok(student.CoursesRequests);
        }

    }
}
