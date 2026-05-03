# Lab: Live Event Streaming with gRPC and gRPC-Web

## Overview

This lab now demonstrates a complete live event pipeline for the TechConf domain:

1. A **gRPC server** persists events with Entity Framework Core and SQLite.
2. A **.NET producer client** continually creates new events by calling the server.
3. A **Vite + React dashboard** subscribes to the live event stream from the browser by using **gRPC-Web**.
4. An **Aspire AppHost** orchestrates the server, producer, and frontend together.

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- Node.js + npm
- [Aspire CLI](https://learn.microsoft.com/dotnet/aspire/fundamentals/setup-tooling)

## Learning objectives

1. Extend a protobuf contract with **server streaming** for live browser updates.
2. Implement gRPC endpoints backed by EF Core and SQLite.
3. Enable **gRPC-Web** and CORS so browser clients can connect to ASP.NET Core gRPC services.
4. Build a continuous **producer** that publishes events over time.
5. Generate a browser-friendly TypeScript client from `.proto` files and render a live React UI.
6. Add a Vite application to an **Aspire AppHost** and wire it to the gRPC API.

## Running the lab

From `labs/lab-grpc/exercise`:

```bash
aspire start --isolated --apphost TechConf.Grpc.AppHost/TechConf.Grpc.AppHost.csproj
```

This starts three resources:

- `api` - the ASP.NET Core gRPC server
- `producer` - the .NET console client that keeps creating events
- `web` - the Vite + React dashboard

Open the Aspire dashboard, then launch the `web` endpoint to watch the live event feed update as the producer creates new events.

## Key implementation areas

- `TechConf.Grpc.Server/Protos/event.proto` - unary event APIs plus `StreamEvents`
- `TechConf.Grpc.Server/Services/EventGrpcService.cs` - CRUD + live event broadcasting
- `TechConf.Grpc.Server/Program.cs` - gRPC-Web, CORS, reflection, and Aspire defaults
- `TechConf.Grpc.Client/Program.cs` - continuous event producer
- `TechConf.Grpc.Web/` - Vite React app using a generated gRPC-Web client
- `TechConf.Grpc.AppHost/` - Aspire orchestration for server, producer, and frontend

## Manual validation ideas

1. Watch the producer logs and confirm new events are created every few seconds.
2. Refresh the dashboard and verify it replays existing events, then continues receiving new ones live.
3. Use the filter box in the React app to narrow the streamed event list by title.
4. Inspect `api` with gRPC reflection or `grpcurl` if you want to verify the service contract manually.
