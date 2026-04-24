namespace SignalGame.Api;

public sealed record QuizSnapshot(
    string Phase,
    string StatusMessage,
    int QuestionNumber,
    int TotalQuestions,
    QuizQuestionView? CurrentQuestion,
    IReadOnlyList<PlayerSummary> Players,
    IReadOnlyList<RoundResult> RoundResults,
    string? Winner,
    bool CanStart,
    bool CanAdvance,
    bool CanReset,
    int ConnectedPlayerCount,
    int AnsweredPlayerCount);

public sealed record QuizQuestionView(
    string Category,
    string Prompt,
    IReadOnlyList<string> Options,
    int? CorrectOptionIndex,
    string? Explanation);

public sealed record PlayerSummary(
    string Name,
    int Score,
    bool IsConnected,
    bool HasAnswered,
    int LastScoreDelta);

public sealed record RoundResult(
    string PlayerName,
    string SelectedOptionText,
    bool IsCorrect,
    int ScoreDelta);
