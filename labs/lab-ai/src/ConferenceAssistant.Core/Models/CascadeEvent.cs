namespace ConferenceAssistant.Core.Models;

public record CascadeEvent(
    string Technology,
    CascadeStatus Status,
    string Detail,
    DateTimeOffset Timestamp);

public enum CascadeStatus { Pending, Active, Complete, Error }
