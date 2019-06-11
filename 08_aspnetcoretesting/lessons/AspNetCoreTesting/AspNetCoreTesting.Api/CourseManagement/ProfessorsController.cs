using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AspNetCoreTesting.Api.CourseManagement.ViewModels;
using AspNetCoreTesting.Domain.Domain;
using AspNetCoreTesting.Infrastructure.DataAccess;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreTesting.Api.CourseManagement
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfessorsController : ControllerBase
    {
        private readonly UniversityDbContext _dbContext;
        private readonly IMapper _mapper;

        public ProfessorsController(UniversityDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Professor>>> GetAllProfessors()
        {
            var courses = await _dbContext.Professors.ToListAsync();
            return Ok(courses);
        }

        [HttpGet("{professorId}/assignedcourses")]
        public async Task<ActionResult<IEnumerable<AssignedCourseVm>>> GetAllAssignedCourses(string professorId)
        {
            var professor = await _dbContext.Professors
                .Include(p => p.AssignedCourses)
                .Where(p => p.Id == professorId)
                .FirstOrDefaultAsync();

            if (professor == null)
                return NotFound();

            return Ok(_mapper.Map<IEnumerable<AssignedCourseVm>>(professor.AssignedCourses));
        }
    }
}
