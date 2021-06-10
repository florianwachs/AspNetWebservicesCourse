using Akka.Actor;
using Akka.DI.Core;
using Akka.Event;
using Akka.Logger.Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkkaDocumentGenerator.Actors
{
    public class ReportGeneratorActor : ReceiveActor
    {
        private static Random rnd = new Random();
        private ILoggingAdapter Log { get; }
        public ReportGeneratorActor()
        {
            Log = Context.GetLogger<SerilogLoggingAdapter>();
            Log.Info($"Creating {nameof(ReportGeneratorActor)}");

            ReceiveAsync<GenerateReport>(HandleGenerateReport);
        }

        public static Props CreateProps(ActorSystem actorSystem) => actorSystem.DI().Props<ReportGeneratorActor>();

        private async Task HandleGenerateReport(GenerateReport request)
        {
            // TODO:
            // - ReportViewModel generieren
            // - Report mit Telerik Reporting generieren
            // - Report in Temp-Verzeichnis ablegen
            // - Success / Failure an Sender zurück geben


            Log.Info("Beginning Report Generation for {GenerationId} {Report}", request.GenerationId, request.ReportName);
            await Task.Delay(TimeSpan.FromSeconds(rnd.Next(10, 20)));

            if (rnd.Next(0, 11) > 5)
            {
                Log.Info("Completed Report Generation for {GenerationId} {Report}", request.GenerationId, request.ReportName);
                Sender.Tell(new GenerateReportSuccess(request.GenerationId, $"[PathToReport]-{request.GenerationId}"));
            }
            else
            {
                Log.Error("Failed Report Generation for {GenerationId} {Report}", request.GenerationId, request.ReportName);
                Sender.Tell(new GenerateReportFailed(request.GenerationId, $"[FailReason]-{request.GenerationId}"));
            }
        }

        #region Messages

        public record GenerateReport(Guid GenerationId, string ReportName, object ReportData);

        public record GenerateReportResult;
        public record GenerateReportSuccess(Guid GenerationId, string GeneratedReportPath) : GenerateReportResult;
        public record GenerateReportFailed(Guid GenerationId, string FailReason) : GenerateReportResult;

        #endregion
    }
}
