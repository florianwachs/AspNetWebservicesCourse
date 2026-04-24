# Lab: Signal Showdown with SignalR, React, Avalonia and Aspire

## Overview

In this optional lab you build **Signal Showdown**, a small real-time tech quiz where players join a lobby, answer the same multiple-choice questions, and watch the leaderboard update after every round.

The lab uses:

- **SignalR** for live updates
- **React** for a browser client
- **Avalonia** for a cross-platform desktop client
- **.NET Aspire** to orchestrate the API and the React client

## Learning Objectives

1. Build a SignalR hub that broadcasts a shared quiz snapshot to every client.
2. Model round-based shared game state with seeded questions, answers, scores, and winner detection.
3. Implement a lightweight React client using the SignalR JavaScript client.
4. Implement an Avalonia desktop client using `Microsoft.AspNetCore.SignalR.Client`.
5. Run the solution via Aspire AppHost and inspect service wiring.

## Project Structure

```text
exercise/
  SignalGame.AppHost/        # Aspire orchestration
  SignalGame.Api/            # SignalR hub + game state
  SignalGame.ReactClient/    # Browser client
  SignalGame.AvaloniaClient/ # Desktop client

solution/
  SignalGame.AppHost/
  SignalGame.Api/
  SignalGame.ReactClient/
  SignalGame.AvaloniaClient/
```

## Getting Started

### 1) Run with Aspire (API + React client)

```bash
cd exercise/SignalGame.AppHost
dotnet run
```

Open the Aspire dashboard and launch the `react` endpoint.

### 2) Run the Avalonia desktop client

In a second terminal:

```bash
cd exercise/SignalGame.AvaloniaClient
# Optional if your API runs on a custom URL
# export SIGNALGAME_API_BASEURL="https://localhost:7111"
dotnet run
```

## Tasks

### Task 1 — Hub contract

In `SignalGame.Api/GameHub.cs`:

- validate input player names
- implement `JoinGame`, `StartGame`, `SubmitAnswer`, `NextQuestion`, and `ResetGame`
- broadcast the full quiz snapshot through the `StateChanged` event
- handle reconnects so players can rejoin with the same nickname

### Task 2 — Shared quiz state

In `SignalGame.Api/GameState.cs`:

- keep the quiz state thread-safe
- seed a small in-memory question bank
- track joined players, submitted answers, round results, and the final winner
- reset scores and round state for a rematch

### Task 3 — React real-time quiz UI

In `SignalGame.ReactClient/wwwroot/app.js`:

- connect to the hub endpoint from `/config`
- join the shared lobby with a player name
- implement start, answer, next-question, and reset actions
- render the question card, round results, and live leaderboard

### Task 4 — Avalonia desktop client

In `SignalGame.AvaloniaClient/MainWindow.axaml.cs`:

- connect to SignalR hub
- subscribe to `StateChanged` and `PlayerJoined`
- invoke `JoinGame`, `StartGame`, `SubmitAnswer`, `NextQuestion`, and `ResetGame`
- update UI controls from the UI thread
- rejoin automatically after reconnects

### Task 5 — Nice game polish

Improve the quiz experience:

- show clear round and final winner state
- reveal the correct answer with a short explanation after each round
- keep both clients responsive during reconnects

## Solution

A full reference implementation is available in the `solution/` directory.
