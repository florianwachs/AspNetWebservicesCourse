using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;

namespace aspnetcore1.JokesApi
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Route("api/v2/schnitzel")]
    public class StudentsController : ControllerBase
    {


        [HttpGet("random")]
        public ActionResult<Student> GetStudent()
        {

            var student = new Student { FistName = "Jason", Name = "Bourne" };

            //return new OkObjectResult(student);
            return Ok(student);
        }
    }

    public class Student
    {
        public string Name { get; set; }
        public string FistName { get; set; }
    }
}
