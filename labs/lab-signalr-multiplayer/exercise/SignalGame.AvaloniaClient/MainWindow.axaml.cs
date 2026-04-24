using Avalonia.Controls;
using Avalonia.Threading;
using Microsoft.AspNetCore.SignalR.Client;

namespace SignalGame.AvaloniaClient;

public partial class MainWindow : Window
{
    private readonly HubConnection _connection;
    private readonly Button[] _answerButtons;
    private QuizSnapshot _quiz = QuizSnapshot.Empty;
    private string? _joinedName;
    private readonly string _hubUrl;

    public MainWindow()
    {
        InitializeComponent();

        var apiBaseUrl = ResolveApiBaseUrl();
        _hubUrl = $"{apiBaseUrl.TrimEnd('/')}/hubs/game";

        _connection = new HubConnectionBuilder()
            .WithUrl(_hubUrl)
            .WithAutomaticReconnect()
            .Build();

        _answerButtons = [Option0Button, Option1Button, Option2Button, Option3Button];
        for (var index = 0; index < _answerButtons.Length; index++)
        {
            var optionIndex = index;
            _answerButtons[index].Click += async (_, _) => await InvokeHub("SubmitAnswer", optionIndex);
        }

        _connection.On<QuizSnapshot>("StateChanged", snapshot =>
        {
            Dispatcher.UIThread.Post(() => ApplySnapshot(snapshot));
        });

        _connection.On<string>("PlayerJoined", player =>
        {
            Dispatcher.UIThread.Post(() => StatusText.Text = $"{player} joined the lobby.");
        });

        _connection.Reconnecting += _ =>
        {
            Dispatcher.UIThread.Post(() =>
            {
                StatusText.Text = "Connection lost. Reconnecting...";
                UpdateActionButtons();
            });

            return Task.CompletedTask;
        };

        _connection.Reconnected += async _ =>
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                StatusText.Text = "Reconnected to the quiz.";
                UpdateActionButtons();
            });

            if (!string.IsNullOrWhiteSpace(_joinedName))
            {
                try
                {
                    await _connection.InvokeAsync("JoinGame", _joinedName);
                }
                catch (Exception ex)
                {
                    await Dispatcher.UIThread.InvokeAsync(() => StatusText.Text = $"Rejoin failed: {ToDisplayMessage(ex)}");
                }
            }
        };

        JoinButton.Click += async (_, _) =>
        {
            var player = PlayerNameTextBox.Text?.Trim();
            if (string.IsNullOrWhiteSpace(player))
            {
                StatusText.Text = "Enter a nickname before joining.";
                return;
            }

            try
            {
                await _connection.InvokeAsync("JoinGame", player);
                _joinedName = player;
                PlayerNameTextBox.Text = player;
                PlayerNameTextBox.IsEnabled = false;
                UpdateActionButtons();
            }
            catch (Exception ex)
            {
                StatusText.Text = ToDisplayMessage(ex);
            }
        };

        StartButton.Click += async (_, _) => await InvokeHub("StartGame");
        NextButton.Click += async (_, _) => await InvokeHub("NextQuestion");
        ResetButton.Click += async (_, _) => await InvokeHub("ResetGame");

        Opened += async (_, _) =>
        {
            try
            {
                StatusText.Text = $"Connecting to {_hubUrl}...";

                using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(10));
                await _connection.StartAsync(timeout.Token);
                StatusText.Text = "Connected. Join the lobby to play.";
                UpdateActionButtons();
            }
            catch (Exception ex)
            {
                StatusText.Text = $"Connection failed: {ToDisplayMessage(ex)}";
            }
        };

        Closing += async (_, _) => await _connection.DisposeAsync();
        ApplySnapshot(_quiz);
    }

    private void ApplySnapshot(QuizSnapshot snapshot)
    {
        _quiz = snapshot;
        StatusText.Text = snapshot.StatusMessage;

        var phaseText = snapshot.Phase switch
        {
            "question" => "Question live",
            "results" => "Round results",
            "finished" => "Final results",
            _ => "Lobby"
        };

        PhaseText.Text = phaseText;
        PhasePillText.Text = phaseText;
        PlayersText.Text = $"{snapshot.ConnectedPlayerCount}/{snapshot.Players.Count} online";
        ProgressText.Text = $"{snapshot.QuestionNumber}/{snapshot.TotalQuestions}";
        PlayerStateText.Text = string.IsNullOrWhiteSpace(_joinedName)
            ? "Choose a nickname to enter the lobby."
            : $"Playing as {_joinedName}.";

        var question = snapshot.CurrentQuestion;
        QuestionMetaText.Text = question is null ? string.Empty : $"Question {snapshot.QuestionNumber} of {snapshot.TotalQuestions}";
        QuestionMetaText.IsVisible = question is not null;
        CategoryText.Text = question?.Category ?? string.Empty;
        CategoryBadge.IsVisible = question is not null;
        QuestionText.Text = question?.Prompt ?? "Join the lobby and start the quiz to reveal the first question.";

        CorrectAnswerText.Text = question?.CorrectOptionIndex is int correctOptionIndex
            ? $"Correct answer: {question.Options[correctOptionIndex]}"
            : string.Empty;
        CorrectAnswerCard.IsVisible = !string.IsNullOrWhiteSpace(CorrectAnswerText.Text);

        ExplanationText.Text = question?.Explanation ?? string.Empty;
        ExplanationCard.IsVisible = !string.IsNullOrWhiteSpace(ExplanationText.Text);

        WinnerText.Text = string.IsNullOrWhiteSpace(snapshot.Winner) ? string.Empty : $"Winner: {snapshot.Winner}";
        WinnerCard.IsVisible = !string.IsNullOrWhiteSpace(WinnerText.Text);

        for (var index = 0; index < _answerButtons.Length; index++)
        {
            var button = _answerButtons[index];
            if (question is not null && index < question.Options.Count)
            {
                button.Content = question.Options[index];
                button.IsVisible = true;
            }
            else
            {
                button.Content = string.Empty;
                button.IsVisible = false;
            }

            button.Classes.Remove("correctAnswer");
            if (question is not null &&
                (snapshot.Phase is "results" or "finished") &&
                question.CorrectOptionIndex == index)
            {
                button.Classes.Add("correctAnswer");
            }
        }

        RoundResultsList.ItemsSource = snapshot.RoundResults.Count == 0
            ? ["Round results will appear after the active question closes."]
            : snapshot.RoundResults
                .Select(result => $"{result.PlayerName}: {result.SelectedOptionText} - {(result.IsCorrect ? $"+{result.ScoreDelta}" : "0")}")
                .ToArray();

        LeaderboardList.ItemsSource = snapshot.Players.Count == 0
            ? ["No players yet."]
            : snapshot.Players
                .Select((player, index) => $"{index + 1}. {player.Name} - {player.Score} pts - {FormatPlayerStatus(player, snapshot.Phase)}")
                .ToArray();

        UpdateActionButtons();
    }

    private async Task InvokeHub(string methodName, params object[] arguments)
    {
        try
        {
            await _connection.InvokeCoreAsync(methodName, arguments);
        }
        catch (Exception ex)
        {
            StatusText.Text = ToDisplayMessage(ex);
        }
    }

    private void UpdateActionButtons()
    {
        var isConnected = _connection.State == HubConnectionState.Connected;
        var isJoined = !string.IsNullOrWhiteSpace(_joinedName);

        JoinButton.IsEnabled = isConnected && !isJoined;
        StartButton.IsEnabled = isConnected && isJoined && _quiz.CanStart;
        NextButton.IsEnabled = isConnected && isJoined && _quiz.CanAdvance;
        ResetButton.IsEnabled = isConnected && isJoined && _quiz.CanReset;

        var canAnswer = isConnected && isJoined && _quiz.Phase == "question" && _quiz.CurrentQuestion is not null;
        var currentPlayer = GetCurrentPlayer();
        var currentPlayerCanAnswer = canAnswer && currentPlayer is not null && !currentPlayer.HasAnswered;

        foreach (var button in _answerButtons)
        {
            button.IsEnabled = currentPlayerCanAnswer && button.IsVisible;
        }
    }

    private PlayerSummary? GetCurrentPlayer()
    {
        if (string.IsNullOrWhiteSpace(_joinedName))
        {
            return null;
        }

        return _quiz.Players.FirstOrDefault(player =>
            string.Equals(player.Name, _joinedName, StringComparison.OrdinalIgnoreCase));
    }

    private static string FormatPlayerStatus(PlayerSummary player, string phase) =>
        player.IsConnected
            ? phase == "question" && player.HasAnswered ? "Answered" : "Online"
            : "Offline";

    private static string ResolveApiBaseUrl()
    {
        return FirstNonEmpty(
                   "SIGNALGAME_API_BASEURL",
                   "ApiBaseUrl",
                   "services__api__https__0",
                   "services__api__http__0",
                   "SERVICES__API__HTTPS__0",
                   "SERVICES__API__HTTP__0")
               ?? "http://localhost:5111";
    }

    private static string? FirstNonEmpty(params string[] keys)
    {
        foreach (var key in keys)
        {
            var value = Environment.GetEnvironmentVariable(key);
            if (!string.IsNullOrWhiteSpace(value))
            {
                return value;
            }
        }

        return null;
    }

    private static string ToDisplayMessage(Exception ex) => ex.GetBaseException().Message;
}
