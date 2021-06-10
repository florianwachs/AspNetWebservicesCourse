using Akka.Actor;
using Akka.DI.Core;
using Akka.Event;
using Akka.Logger.Serilog;
using Akka.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkkaDocumentGenerator.Actors
{
    public class ReportGenerationControllerActor : ReceiveActor
    {
        private ILoggingAdapter Log { get; }
        private IActorRef ReportGeneratorActorRef { get; }

        private Dictionary<Guid, ReportGeneratorActor.GenerateReportResult> Results { get; } = new();
        private Dictionary<Guid, ReportStatusResult> Status { get; } = new();

        public ReportGenerationControllerActor()
        {
            Log = Context.GetLogger<SerilogLoggingAdapter>();
            Log.Info($"Creating {nameof(ReportGenerationControllerActor)}");

            var reportGeneratorActorProps = ReportGeneratorActor.CreateProps(Context.System).WithRouter(new RoundRobinPool(10, new DefaultResizer(10, 30)));
            ReportGeneratorActorRef = Context.ActorOf(reportGeneratorActorProps, "ReportGenerator");

            Receive<GenerateReport>(HandleGenerateReport);
            Receive<ReportStatus>(HandleReportStatus);
            Receive<ReportGeneratorActor.GenerateReportSuccess>(HandleGenerateReportSuccess);
            Receive<ReportGeneratorActor.GenerateReportFailed>(HandleGenerateReportFailed);
        }

        public static Props CreateProps(ActorSystem actorSystem) => actorSystem.DI().Props<ReportGenerationControllerActor>();

        #region Handlers
        private void HandleGenerateReportFailed(ReportGeneratorActor.GenerateReportFailed request)
        {
            Results[request.GenerationId] = request;
            var status = new ReportStatusResult(request.GenerationId, ReportGenerationStatus.Failed);
            Log.Error("Failed to generate {@Report}", status);
            Status[request.GenerationId] = status;
        }

        private void HandleGenerateReportSuccess(ReportGeneratorActor.GenerateReportSuccess request)
        {
            Results[request.GenerationId] = request;
            var status = new ReportStatusResult(request.GenerationId, ReportGenerationStatus.Completed);
            Log.Info("Success to generate {@Report}", status);
            Status[request.GenerationId] = status;
        }

        private void HandleGenerateReport(GenerateReport request)
        {
            var status = new ReportStatusResult(Guid.NewGuid(), ReportGenerationStatus.InQueue);
            Status[status.GenerationId] = status;
            var generateReportRequest = new ReportGeneratorActor.GenerateReport(status.GenerationId, request.ReportName, request.ReportData);

            Log.Info("Created new Report Generation Request {GenerationId} for {Report}", generateReportRequest.GenerationId, generateReportRequest.ReportName);
            ReportGeneratorActorRef.Tell(generateReportRequest);
            Sender.Tell(status);
        }

        private void HandleReportStatus(ReportStatus request)
        {
            ReportStatusResult status = default;
            if (!Status.TryGetValue(request.GenerationId, out status))
            {
                status = new ReportStatusResult(request.GenerationId, ReportGenerationStatus.Unknown);
            }

            Sender.Tell(status);
        }

        protected override void Unhandled(object message)
        {
            Log.Warning("Message {Message} was unhandled", message);
        }

        #endregion


        #region Messages

        public record GenerateReport(string ReportName, object ReportData);
        public record ReportStatus(Guid GenerationId);
        public record ReportStatusResult(Guid GenerationId, ReportGenerationStatus Status);

        public enum ReportGenerationStatus
        {
            Unknown,
            InQueue,
            Completed,
            Failed,
        }

        #endregion
    }
}
