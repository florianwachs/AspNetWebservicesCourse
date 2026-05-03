using System.Threading.Channels;

namespace TechConf.Api.Workers;

public sealed record AcceptedProposalMessage(
    int ProposalId,
    string SpeakerName,
    string SpeakerEmail,
    string ProposalTitle,
    string EventName);

public sealed class AcceptedProposalWorker(
    Channel<AcceptedProposalMessage> channel,
    ILogger<AcceptedProposalWorker> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Accepted proposal worker started.");

        await foreach (var message in channel.Reader.ReadAllAsync(stoppingToken))
        {
            logger.LogInformation(
                "Sending acceptance mail for proposal {ProposalId}: {ProposalTitle} -> {SpeakerName} ({SpeakerEmail}) for {EventName}",
                message.ProposalId,
                message.ProposalTitle,
                message.SpeakerName,
                message.SpeakerEmail,
                message.EventName);

            await Task.Delay(150, stoppingToken);
        }
    }
}
