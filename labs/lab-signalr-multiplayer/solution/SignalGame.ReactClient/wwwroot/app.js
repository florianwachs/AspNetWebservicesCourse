(() => {
  const { Fragment, useEffect, useMemo, useRef, useState } = React;
  const h = React.createElement;

  const emptyQuiz = {
    phase: "lobby",
    statusMessage: "Connecting...",
    questionNumber: 0,
    totalQuestions: 0,
    currentQuestion: null,
    players: [],
    roundResults: [],
    winner: null,
    canStart: false,
    canAdvance: false,
    canReset: false,
    connectedPlayerCount: 0,
    answeredPlayerCount: 0
  };

  function App() {
    const [name, setName] = useState("");
    const [joinedName, setJoinedName] = useState("");
    const [status, setStatus] = useState("Connecting...");
    const [error, setError] = useState("");
    const [quiz, setQuiz] = useState(emptyQuiz);
    const [connection, setConnection] = useState(null);
    const joinedNameRef = useRef("");

    const joined = joinedName.length > 0;
    const question = quiz.currentQuestion;
    const revealAnswer = quiz.phase === "results" || quiz.phase === "finished";

    useEffect(() => {
      joinedNameRef.current = joinedName;
    }, [joinedName]);

    useEffect(() => {
      let disposed = false;
      let hub = null;

      (async () => {
        const config = await fetch("/config").then(response => response.json());

        hub = new signalR.HubConnectionBuilder()
          .withUrl(config.hubUrl)
          .withAutomaticReconnect()
          .build();

        hub.on("StateChanged", snapshot => {
          if (disposed) {
            return;
          }

          setQuiz(snapshot);
          setStatus(snapshot.statusMessage);
          setError("");
        });

        hub.on("PlayerJoined", player => {
          if (!disposed) {
            setStatus(`${player} joined the lobby.`);
          }
        });

        hub.onreconnecting(() => {
          if (!disposed) {
            setStatus("Connection lost. Reconnecting...");
          }
        });

        hub.onreconnected(async () => {
          if (disposed) {
            return;
          }

          setStatus("Reconnected to the quiz.");

          if (joinedNameRef.current) {
            try {
              await hub.invoke("JoinGame", joinedNameRef.current);
            } catch (reconnectError) {
              setError(reconnectError.message);
            }
          }
        });

        await hub.start();
        if (!disposed) {
          setConnection(hub);
          setStatus("Connected. Join the lobby to play.");
        }
      })().catch(connectionError => {
        if (!disposed) {
          setStatus(`Connection failed: ${connectionError.message}`);
          setError(connectionError.message);
        }
      });

      return () => {
        disposed = true;
        if (hub) {
          hub.stop();
        }
      };
    }, []);

    const currentPlayer = useMemo(() => {
      if (!joinedName) {
        return null;
      }

      const normalizedName = joinedName.toLowerCase();
      return quiz.players.find(player => player.name.toLowerCase() === normalizedName) ?? null;
    }, [joinedName, quiz.players]);

    const canAnswer = Boolean(
      connection &&
      joined &&
      question &&
      quiz.phase === "question" &&
      currentPlayer &&
      !currentPlayer.hasAnswered
    );

    const phaseLabel = {
      lobby: "Lobby",
      question: "Question live",
      results: "Round results",
      finished: "Final results"
    }[quiz.phase] ?? "Quiz";

    const invoke = async (method, ...args) => {
      if (!connection) {
        return false;
      }

      try {
        setError("");
        await connection.invoke(method, ...args);
        return true;
      } catch (invokeError) {
        setStatus(invokeError.message);
        setError(invokeError.message);
        return false;
      }
    };

    const join = async () => {
      const trimmedName = name.trim();
      if (!trimmedName) {
        return;
      }

      const joinedLobby = await invoke("JoinGame", trimmedName);
      if (joinedLobby) {
        setJoinedName(trimmedName);
        setName(trimmedName);
      }
    };

    const answerQuestion = optionIndex => invoke("SubmitAnswer", optionIndex);

    const leaderboardRows = quiz.players.length === 0
      ? [h("tr", { key: "empty" }, h("td", { colSpan: 3, className: "empty-row" }, "No players yet."))]
      : quiz.players.map(player =>
          h("tr", { key: player.name },
            h("td", null, player.name),
            h("td", null, player.score),
            h("td", null,
              player.isConnected
                ? (quiz.phase === "question" && player.hasAnswered ? "Answered" : "Online")
                : "Offline"
            )
          )
        );

    const resultItems = quiz.roundResults.length === 0
      ? h("p", { className: "empty-state" }, "Round results will appear after the active question closes.")
      : h("ul", { className: "results-list" },
          quiz.roundResults.map(result =>
            h("li", { key: result.playerName, className: `result-item ${result.isCorrect ? "correct" : ""}` },
              h("div", { className: "result-copy" },
                h("strong", null, result.playerName),
                h("span", { className: "muted" }, result.selectedOptionText)
              ),
              h("span", { className: `pill ${result.isCorrect ? "success" : "neutral"}` },
                result.isCorrect ? `+${result.scoreDelta}` : "0"
              )
            )
          )
        );

    return h("div", { className: "app" },
      h("section", { className: "card hero" },
        h("div", { className: "hero-copy" },
          h("span", { className: "eyebrow" }, "SignalR multiplayer lab"),
          h("h1", null, "🧠 Signal Showdown"),
          h("p", { className: "lead" }, "A real-time tech quiz where every answer reshapes the leaderboard."),
          h("p", { className: "status" }, status),
          error && h("p", { className: "error" }, error)
        ),
        h("div", { className: "hero-stats" },
          h(StatCard, { label: "Phase", value: phaseLabel }),
          h(StatCard, {
            label: "Players",
            value: `${quiz.connectedPlayerCount}/${quiz.players.length || 0} online`
          }),
          h(StatCard, {
            label: "Progress",
            value: quiz.totalQuestions ? `${quiz.questionNumber || 0}/${quiz.totalQuestions}` : "0/0"
          })
        )
      ),
      h("section", { className: "card controls" },
        h("div", { className: "controls-grid" },
          h("input", {
            placeholder: "Your nickname",
            value: name,
            disabled: joined,
            onChange: event => setName(event.target.value)
          }),
          h("button", {
            className: "primary",
            disabled: joined || !connection,
            onClick: join
          }, joined ? "Joined" : "Join lobby"),
          h("button", {
            className: "secondary",
            disabled: !joined || !quiz.canStart,
            onClick: () => invoke("StartGame")
          }, "Start quiz"),
          h("button", {
            className: "secondary",
            disabled: !joined || !quiz.canAdvance,
            onClick: () => invoke("NextQuestion")
          }, "Next question"),
          h("button", {
            className: "ghost",
            disabled: !joined || !quiz.canReset,
            onClick: () => invoke("ResetGame")
          }, "Reset")
        ),
        joined && h("p", { className: "muted" }, `Playing as ${joinedName}.`)
      ),
      h("section", { className: "card question-card" },
        h("div", { className: "section-header" },
          h("span", { className: "phase-pill" }, phaseLabel),
          question && h("span", { className: "muted" }, `Question ${quiz.questionNumber} of ${quiz.totalQuestions}`)
        ),
        question
          ? h(Fragment, null,
              h("p", { className: "category" }, question.category),
              h("h2", null, question.prompt),
              h("div", { className: "answers" },
                question.options.map((option, index) =>
                  h("button", {
                    key: `${index}-${option}`,
                    className: revealAnswer
                      ? `answer reveal ${index === question.correctOptionIndex ? "correct" : ""}`
                      : "answer",
                    disabled: revealAnswer || !canAnswer,
                    onClick: () => answerQuestion(index)
                  }, option)
                )
              ),
              quiz.phase === "question" && joined && currentPlayer?.hasAnswered &&
                h("p", { className: "muted" }, "Answer locked in. Waiting for the rest of the lobby."),
              revealAnswer && question.explanation &&
                h("p", { className: "explanation" }, question.explanation)
            )
          : h("p", { className: "empty-state" }, "Join the lobby and start the quiz to reveal the first question.")
      ),
      h("section", { className: "grid" },
        h("div", { className: "card" },
          h("div", { className: "section-header" },
            h("h2", null, "Round results"),
            quiz.winner && h("span", { className: "winner-pill" }, `Winner: ${quiz.winner}`)
          ),
          resultItems
        ),
        h("div", { className: "card" },
          h("div", { className: "section-header" },
            h("h2", null, "Leaderboard"),
            quiz.phase === "question" &&
              h("span", { className: "muted" },
                `${quiz.answeredPlayerCount}/${quiz.connectedPlayerCount} answered`
              )
          ),
          h("table", null,
            h("thead", null,
              h("tr", null,
                h("th", null, "Player"),
                h("th", null, "Score"),
                h("th", null, "Status")
              )
            ),
            h("tbody", null, leaderboardRows)
          )
        )
      )
    );
  }

  function StatCard({ label, value }) {
    return h("div", { className: "stat-card" },
      h("span", { className: "muted" }, label),
      h("strong", null, value)
    );
  }

  ReactDOM.createRoot(document.getElementById("root")).render(h(App));
})();
