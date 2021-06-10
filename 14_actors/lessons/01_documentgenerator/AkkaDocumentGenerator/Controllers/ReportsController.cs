using Akka.Actor;
using AkkaDocumentGenerator.Actors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AkkaDocumentGenerator.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReportsController : ControllerBase
    {

        private readonly ILogger<ReportsController> _logger;
        private IActorRef ReportControllerActor { get; }

        public ReportsController(ILogger<ReportsController> logger, ActorReferences.ReportGenerationControllerActorProvider reportGenerationControllerActorProvider)
        {
            _logger = logger;
            ReportControllerActor = reportGenerationControllerActorProvider();
        }

        [HttpPost]
        public async Task<ActionResult<ReportGenerationControllerActor.ReportStatusResult>> GenerateReport(ReportGenerationControllerActor.GenerateReport generateReport)
        {
            var result = await ReportControllerActor.Ask<ReportGenerationControllerActor.ReportStatusResult>(generateReport);
            return result;
        }

        [HttpGet("status/{id:guid}")]
        public async Task<ActionResult<ReportGenerationControllerActor.ReportStatusResult>> GetStatus(Guid id)
        {
            var result = await ReportControllerActor.Ask<ReportGenerationControllerActor.ReportStatusResult>(new ReportGenerationControllerActor.ReportStatus(id));
            return result;
        }
    }
}
