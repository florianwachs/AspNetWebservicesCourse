using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AspNetCoreUnitTests.Api.ContainerManagement.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreUnitTests.Api.ContainerManagement
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContainersController : ControllerBase
    {
        [HttpGet("infos")]
        public ActionResult<IEnumerable<ContainerInfo>> GetInfosForAllContainers()
        {
            var containerInfos = new[]
            {
                new ContainerInfo{ Id="123", TotalWeightInKg=200}
            };

            return Ok(containerInfos);
        }
    }
}
