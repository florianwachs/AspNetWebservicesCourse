namespace TechConf.Grpc.Server.Services;

// TODO: Task 1 — After defining session.proto, implement SessionGrpcService
// This service should:
//   - Override GetSessions to return sessions (optionally filtered by eventId)
//   - Override GetSessionById to return a single session
//   - Override CreateSession to create a new session
//
// TODO: Task 4 — Add server streaming
//   - Override StreamSessionUpdates to periodically send session info
//   - Use IServerStreamWriter<SessionMessage> to write responses
//   - Check context.CancellationToken to stop when client disconnects
//   - Use Task.Delay between updates (e.g., every 3 seconds)

public class SessionGrpcService
{
    // Replace with: SessionService.SessionServiceBase once session.proto is defined
}
