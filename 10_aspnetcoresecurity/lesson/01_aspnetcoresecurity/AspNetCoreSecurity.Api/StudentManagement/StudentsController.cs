using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AspNetCoreSecurity.Domain.Domain;
using AspNetCoreSecurity.Infrastructure.DataAccess;
using AspNetCoreSecurity.Infrastructure.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreSecurity.Api.StudentManagement
{
    [Authorize]
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
        [RequireClaim("CanReadAllStudents")]
        public async Task<ActionResult<IEnumerable<Student>>> GetAllStudents()
        {
            var students = await _dbContext.Students.ToListAsync();
            return Ok(students);
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
