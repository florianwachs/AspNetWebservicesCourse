namespace SignalGame.Api;

public sealed class GameState
{
    private readonly Lock _gate = new();
    private readonly List<QuizQuestion> _questions = CreateQuestions();
    private readonly Dictionary<string, PlayerState> _players = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, string> _connections = new(StringComparer.Ordinal);
    private IReadOnlyList<RoundResult> _roundResults = [];
    private int _currentQuestionIndex = -1;
    private QuizPhase _phase = QuizPhase.Lobby;
    private string? _winner;

    public QuizSnapshot Snapshot()
    {
        lock (_gate)
        {
            return BuildSnapshot();
        }
    }

    public QuizSnapshot Join(string playerName, string connectionId)
    {
        lock (_gate)
        {
            if (_players.TryGetValue(playerName, out var existingPlayer) &&
                existingPlayer.IsConnected &&
                (!_connections.TryGetValue(connectionId, out var currentPlayerName) ||
                 !string.Equals(currentPlayerName, playerName, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException("That player name is already in use.");
            }

            if (_connections.TryGetValue(connectionId, out var previousPlayerName) &&
                _players.TryGetValue(previousPlayerName, out var previousPlayer) &&
                !string.Equals(previousPlayerName, playerName, StringComparison.OrdinalIgnoreCase))
            {
                previousPlayer.IsConnected = false;
            }

            _connections[connectionId] = playerName;

            if (!_players.TryGetValue(playerName, out var player))
            {
                player = new PlayerState(playerName);
                _players[playerName] = player;
            }

            player.IsConnected = true;
            return BuildSnapshot();
        }
    }

    public QuizSnapshot? Disconnect(string connectionId)
    {
        lock (_gate)
        {
            if (!_connections.Remove(connectionId, out var playerName) ||
                !_players.TryGetValue(playerName, out var player))
            {
                return null;
            }

            player.IsConnected = false;

            if (_phase == QuizPhase.Question && ConnectedPlayerCount() > 0 && AllConnectedPlayersAnswered())
            {
                CompleteRound();
            }

            return BuildSnapshot();
        }
    }

    public QuizSnapshot StartGame()
    {
        lock (_gate)
        {
            if (ConnectedPlayerCount() == 0)
            {
                throw new InvalidOperationException("Join the lobby before starting the quiz.");
            }

            if (_phase != QuizPhase.Lobby)
            {
                throw new InvalidOperationException("Reset the quiz before starting a new match.");
            }

            PrepareQuestion(0, resetScores: true);
            return BuildSnapshot();
        }
    }

    public QuizSnapshot SubmitAnswer(string connectionId, int optionIndex)
    {
        lock (_gate)
        {
            if (_phase != QuizPhase.Question)
            {
                throw new InvalidOperationException("There is no active question right now.");
            }

            if (!_connections.TryGetValue(connectionId, out var playerName) ||
                !_players.TryGetValue(playerName, out var player) ||
                !player.IsConnected)
            {
                throw new InvalidOperationException("Join the quiz before submitting an answer.");
            }

            var question = _questions[_currentQuestionIndex];
            if ((uint)optionIndex >= (uint)question.Options.Count)
            {
                throw new InvalidOperationException("That answer option does not exist.");
            }

            if (player.HasAnswered)
            {
                throw new InvalidOperationException("You already locked in an answer for this round.");
            }

            player.HasAnswered = true;
            player.SelectedOptionIndex = optionIndex;

            if (AllConnectedPlayersAnswered())
            {
                CompleteRound();
            }

            return BuildSnapshot();
        }
    }

    public QuizSnapshot NextQuestion()
    {
        lock (_gate)
        {
            if (_phase != QuizPhase.Results)
            {
                throw new InvalidOperationException("There is no next question to advance to right now.");
            }

            PrepareQuestion(_currentQuestionIndex + 1, resetScores: false);
            return BuildSnapshot();
        }
    }

    public QuizSnapshot ResetGame()
    {
        lock (_gate)
        {
            foreach (var player in _players.Values)
            {
                player.Score = 0;
                ResetRoundState(player);
            }

            _currentQuestionIndex = -1;
            _phase = QuizPhase.Lobby;
            _roundResults = [];
            _winner = null;

            return BuildSnapshot();
        }
    }

    private QuizSnapshot BuildSnapshot()
    {
        var currentQuestion = _currentQuestionIndex >= 0 && _currentQuestionIndex < _questions.Count
            ? _questions[_currentQuestionIndex]
            : null;
        var revealAnswer = _phase is QuizPhase.Results or QuizPhase.Finished;

        return new QuizSnapshot(
            Phase: _phase switch
            {
                QuizPhase.Lobby => "lobby",
                QuizPhase.Question => "question",
                QuizPhase.Results => "results",
                QuizPhase.Finished => "finished",
                _ => "lobby"
            },
            StatusMessage: BuildStatusMessage(),
            QuestionNumber: currentQuestion is null ? 0 : _currentQuestionIndex + 1,
            TotalQuestions: _questions.Count,
            CurrentQuestion: currentQuestion is null
                ? null
                : new QuizQuestionView(
                    currentQuestion.Category,
                    currentQuestion.Prompt,
                    currentQuestion.Options.ToArray(),
                    revealAnswer ? currentQuestion.CorrectOptionIndex : null,
                    revealAnswer ? currentQuestion.Explanation : null),
            Players: _players.Values
                .OrderByDescending(player => player.Score)
                .ThenBy(player => player.Name, StringComparer.OrdinalIgnoreCase)
                .Select(player => new PlayerSummary(
                    player.Name,
                    player.Score,
                    player.IsConnected,
                    player.HasAnswered,
                    player.LastScoreDelta))
                .ToArray(),
            RoundResults: _roundResults,
            Winner: _winner,
            CanStart: _phase == QuizPhase.Lobby && ConnectedPlayerCount() > 0,
            CanAdvance: _phase == QuizPhase.Results,
            CanReset: _players.Count > 0,
            ConnectedPlayerCount: ConnectedPlayerCount(),
            AnsweredPlayerCount: AnsweredPlayerCount());
    }

    private string BuildStatusMessage()
    {
        var connectedPlayers = ConnectedPlayerCount();
        return _phase switch
        {
            QuizPhase.Lobby when connectedPlayers == 0 => "Join the lobby to kick off the quiz.",
            QuizPhase.Lobby when connectedPlayers == 1 => "1 player ready. Start the quiz whenever you are.",
            QuizPhase.Lobby => $"{connectedPlayers} players ready. Start when everyone has joined.",
            QuizPhase.Question when connectedPlayers == 0 => "Waiting for a player to reconnect.",
            QuizPhase.Question => $"{AnsweredPlayerCount()} of {connectedPlayers} connected players locked in an answer.",
            QuizPhase.Results => "Round complete. Review the explanation and continue.",
            QuizPhase.Finished when _winner is null => "Quiz finished.",
            QuizPhase.Finished when _winner.Contains(" & ", StringComparison.Ordinal) => $"{_winner} share the win!",
            QuizPhase.Finished => $"{_winner} wins the quiz!",
            _ => "Ready."
        };
    }

    private void CompleteRound()
    {
        var question = _questions[_currentQuestionIndex];
        _winner = null;

        foreach (var player in _players.Values)
        {
            player.LastScoreDelta = 0;
            player.WasLastAnswerCorrect = null;

            if (player.SelectedOptionIndex is not int selectedOptionIndex)
            {
                continue;
            }

            var isCorrect = selectedOptionIndex == question.CorrectOptionIndex;
            player.WasLastAnswerCorrect = isCorrect;

            if (isCorrect)
            {
                player.LastScoreDelta = 100;
                player.Score += 100;
            }
        }

        _roundResults = _players.Values
            .OrderByDescending(player => player.Score)
            .ThenBy(player => player.Name, StringComparer.OrdinalIgnoreCase)
            .Select(player => new RoundResult(
                player.Name,
                player.SelectedOptionIndex is int selectedOptionIndex ? question.Options[selectedOptionIndex] : "No answer",
                player.WasLastAnswerCorrect is true,
                player.LastScoreDelta))
            .ToArray();

        if (_currentQuestionIndex == _questions.Count - 1)
        {
            _phase = QuizPhase.Finished;
            _winner = BuildWinnerName();
            return;
        }

        _phase = QuizPhase.Results;
    }

    private void PrepareQuestion(int questionIndex, bool resetScores)
    {
        _currentQuestionIndex = questionIndex;
        _phase = QuizPhase.Question;
        _roundResults = [];
        _winner = null;

        foreach (var player in _players.Values)
        {
            if (resetScores)
            {
                player.Score = 0;
            }

            ResetRoundState(player);
        }
    }

    private static void ResetRoundState(PlayerState player)
    {
        player.HasAnswered = false;
        player.SelectedOptionIndex = null;
        player.WasLastAnswerCorrect = null;
        player.LastScoreDelta = 0;
    }

    private int ConnectedPlayerCount() => _players.Values.Count(player => player.IsConnected);

    private int AnsweredPlayerCount() => _players.Values.Count(player => player.IsConnected && player.HasAnswered);

    private bool AllConnectedPlayersAnswered() =>
        _players.Values.Where(player => player.IsConnected).All(player => player.HasAnswered);

    private string? BuildWinnerName()
    {
        var leaders = _players.Values
            .OrderByDescending(player => player.Score)
            .ThenBy(player => player.Name, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        if (leaders.Length == 0)
        {
            return null;
        }

        var topScore = leaders[0].Score;
        var winners = leaders
            .Where(player => player.Score == topScore)
            .Select(player => player.Name)
            .ToArray();

        return winners.Length == 1 ? winners[0] : string.Join(" & ", winners);
    }

    private static List<QuizQuestion> CreateQuestions() =>
    [
        new(
            "HTTP",
            "Which status code usually means a server created a new resource successfully?",
            ["200 OK", "201 Created", "204 No Content", "302 Found"],
            1,
            "201 Created is the standard response when the server creates a resource."),
        new(
            "Minimal APIs",
            "What is the defining style of Minimal APIs compared to controllers?",
            ["Attribute-based routing in controller classes", "Explicit endpoint mapping in Program.cs", "XML-based route configuration", "Automatic SOAP contract generation"],
            1,
            "Minimal APIs wire endpoints explicitly with calls like `app.MapGet(...)` in Program.cs."),
        new(
            "HTTP",
            "Which HTTP method is usually the right choice when the server generates the resource ID?",
            ["GET", "POST", "PUT", "DELETE"],
            2,
            "POST is the usual choice for creation when the server assigns the new resource ID."),
        new(
            "Dependency Injection",
            "Which DI lifetime is the recommended default for DbContext and repositories?",
            ["Transient", "Scoped", "Singleton", "Static"],
            1,
            "Scoped creates one instance per HTTP request, which matches DbContext and repository usage."),
        new(
            "Web APIs",
            "Which HTTP header tells the server which response media types the client can handle?",
            ["Content-Type", "Authorization", "Accept", "Origin"],
            2,
            "Accept is used for content negotiation on the response."),
        new(
            "Minimal APIs",
            "In a Minimal API handler, which parameter source is auto-detected for a registered service like `IEventService`?",
            ["Route", "Query string", "Dependency injection", "Form data"],
            2,
            "Registered services are automatically resolved from dependency injection in Minimal API handlers."),
        new(
            "Minimal APIs",
            "How many parameters can be bound from the JSON request body in a single Minimal API endpoint?",
            ["Unlimited", "Two", "Only one", "None unless `[FromBody]` is repeated"],
            2,
            "Minimal APIs allow one body-bound parameter per endpoint."),
        new(
            "OpenAPI",
            "Which two calls are the built-in .NET 10 baseline for serving an OpenAPI document?",
            ["`AddSwaggerGen()` and `UseSwaggerUI()`", "`AddOpenApi()` and `MapOpenApi()`", "`AddEndpointsApiExplorer()` and `UseScalar()`", "`AddOpenApi()` and `MapControllers()`"],
            1,
            "Built-in OpenAPI in modern ASP.NET Core starts with `builder.Services.AddOpenApi()` and `app.MapOpenApi()`."),
        new(
            "OpenAPI",
            "Why are `TypedResults` preferred over `Results` in Minimal APIs?",
            ["They always return XML", "They disable OpenAPI generation", "They provide compile-time safety and automatic metadata", "They only work in controllers"],
            2,
            "`TypedResults` improve compile-time safety and automatically contribute response metadata to OpenAPI."),
        new(
            "Entity Framework",
            "What is `DbContext` in EF Core primarily described as?",
            ["An HTTP middleware", "A session with the database", "A logging adapter", "A background worker"],
            1,
            "`DbContext` represents a session with the database and coordinates querying plus `SaveChangesAsync()`."),
        new(
            "Entity Framework",
            "What does `modelBuilder.ApplyConfigurationsFromAssembly(...)` help with?",
            ["Generating SQL scripts for migrations", "Finding and applying `IEntityTypeConfiguration<T>` classes automatically", "Turning entities into controllers", "Registering validators in DI"],
            1,
            "It scans the assembly and applies all Fluent API configuration classes automatically."),
        new(
            "Validation",
            "What does `builder.Services.AddValidation()` enable in .NET 10?",
            ["Automatic Data Annotation validation", "OpenAPI document generation", "Database migrations", "JWT authentication"],
            2,
            "`AddValidation()` registers the built-in validation system for annotated request models."),
        new(
            "Validation",
            "Which FluentValidation API is typically used for async database-backed rules like uniqueness checks?",
            ["MustAsync()", "WithSeverity()", "NotEmpty()", "Include()"],
            1,
            "`MustAsync()` is used for asynchronous rules such as checking the database for duplicates."),
        new(
            "Problem Details",
            "What is the standard content type for RFC 9457 Problem Details responses?",
            ["application/json", "application/problem+json", "text/problem", "application/errors+json"],
            1,
            "Problem Details responses should use `application/problem+json` so clients can recognize them reliably."),
        new(
            "Problem Details",
            "Which middleware call is still required after `AddExceptionHandler<T>()` to actually activate the exception handler pipeline?",
            ["`UseRouting()`", "`UseEndpoints()`", "`UseExceptionHandler()`", "`UseAuthorization()`"],
            2,
            "`AddExceptionHandler<T>()` only registers the handler; `app.UseExceptionHandler()` wires it into the middleware pipeline.")
    ];

    private sealed class PlayerState(string name)
    {
        public string Name { get; } = name;

        public bool HasAnswered { get; set; }

        public bool IsConnected { get; set; }

        public int LastScoreDelta { get; set; }

        public int Score { get; set; }

        public int? SelectedOptionIndex { get; set; }

        public bool? WasLastAnswerCorrect { get; set; }
    }

    private sealed record QuizQuestion(
        string Category,
        string Prompt,
        IReadOnlyList<string> Options,
        int CorrectOptionIndex,
        string Explanation);

    private enum QuizPhase
    {
        Lobby,
        Question,
        Results,
        Finished
    }
}
