using WorkshopPlanner.Api.Data;
using WorkshopPlanner.Api.Models;

namespace WorkshopPlanner.Api.Endpoints;

public static class WorkshopEndpoints
{
    public static void MapWorkshopEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/workshops").WithTags("Workshops");

        group.MapGet("/", (WorkshopStore store) =>
            TypedResults.Ok(store.Workshops.Select(workshop => new WorkshopSummaryResponse(
                workshop.Id,
                workshop.Title,
                workshop.City,
                workshop.MaxAttendees,
                workshop.Sessions.Count,
                workshop.Registrations.Count(registration => registration.Status is RegistrationStatus.Confirmed or RegistrationStatus.CheckedIn),
                workshop.Registrations.Count(registration => registration.Status == RegistrationStatus.Waitlisted),
                workshop.CanceledOnUtc is not null,
                workshop.PublishedOnUtc is not null))));

        group.MapGet("/{id:int}", (int id, WorkshopStore store) =>
        {
            var workshop = store.Workshops.FirstOrDefault(item => item.Id == id);
            return workshop is not null
                ? Results.Ok(workshop)
                : Results.NotFound(new { error = $"Workshop {id} was not found." });
        });

        group.MapPost("/", (CreateWorkshopRequest request, WorkshopStore store) =>
        {
            if (string.IsNullOrWhiteSpace(request.Title))
                return Results.BadRequest(new { error = "Title is required." });

            if (string.IsNullOrWhiteSpace(request.City))
                return Results.BadRequest(new { error = "City is required." });

            if (request.MaxAttendees < 5)
                return Results.BadRequest(new { error = "Max attendees must be at least 5." });

            if (store.Workshops.Any(item => item.Title.Equals(request.Title, StringComparison.OrdinalIgnoreCase)))
                return Results.Conflict(new { error = $"A workshop named '{request.Title}' already exists." });

            var workshop = new Workshop(
                store.GetNextWorkshopId(),
                request.Title.Trim(),
                request.City.Trim(),
                request.MaxAttendees);

            store.Workshops.Add(workshop);

            return Results.Created($"/api/workshops/{workshop.Id}", workshop);
        });

        group.MapPut("/{id:int}", (int id, UpdateWorkshopRequest request, WorkshopStore store) =>
        {
            var workshop = store.Workshops.FirstOrDefault(item => item.Id == id);
            if (workshop is null)
                return Results.NotFound(new { error = $"Workshop {id} was not found." });

            if (workshop.CanceledOnUtc is not null)
                return Results.Conflict(new { error = "Canceled workshops cannot be changed." });

            if (workshop.PublishedOnUtc is not null)
                return Results.Conflict(new { error = "Published workshops cannot be changed." });

            if (string.IsNullOrWhiteSpace(request.Title))
                return Results.BadRequest(new { error = "Title is required." });

            if (string.IsNullOrWhiteSpace(request.City))
                return Results.BadRequest(new { error = "City is required." });

            if (request.MaxAttendees < 5)
                return Results.BadRequest(new { error = "Max attendees must be at least 5." });

            if (store.Workshops.Any(item => item.Id != id && item.Title.Equals(request.Title, StringComparison.OrdinalIgnoreCase)))
                return Results.Conflict(new { error = $"A workshop named '{request.Title}' already exists." });

            var activeRegistrations = workshop.Registrations.Count(registration => registration.Status is RegistrationStatus.Confirmed or RegistrationStatus.CheckedIn);
            if (request.MaxAttendees < activeRegistrations)
                return Results.Conflict(new { error = "Max attendees cannot be reduced below the number of active registrations." });

            workshop.Title = request.Title.Trim();
            workshop.City = request.City.Trim();
            workshop.MaxAttendees = request.MaxAttendees;

            return Results.Ok(workshop);
        });

        group.MapPost("/{id:int}/sessions", (int id, AddSessionRequest request, WorkshopStore store) =>
        {
            var workshop = store.Workshops.FirstOrDefault(item => item.Id == id);
            if (workshop is null)
                return Results.NotFound(new { error = $"Workshop {id} was not found." });

            if (workshop.CanceledOnUtc is not null)
                return Results.Conflict(new { error = "Canceled workshops cannot be changed." });

            if (workshop.PublishedOnUtc is not null)
                return Results.Conflict(new { error = "Published workshops cannot be changed." });

            if (string.IsNullOrWhiteSpace(request.Title))
                return Results.BadRequest(new { error = "Session title is required." });

            if (string.IsNullOrWhiteSpace(request.SpeakerName))
                return Results.BadRequest(new { error = "Speaker name is required." });

            if (request.DurationMinutes is < 30 or > 180)
                return Results.BadRequest(new { error = "Session duration must be between 30 and 180 minutes." });

            if (workshop.Sessions.Any(session => session.Title.Equals(request.Title, StringComparison.OrdinalIgnoreCase)))
                return Results.Conflict(new { error = $"A session named '{request.Title}' already exists in this workshop." });

            var totalDuration = workshop.Sessions.Sum(session => session.DurationMinutes) + request.DurationMinutes;
            if (totalDuration > 480)
                return Results.BadRequest(new { error = "A workshop cannot exceed 480 total session minutes." });

            var session = new WorkshopSession(
                store.GetNextSessionId(),
                request.Title.Trim(),
                request.SpeakerName.Trim(),
                request.DurationMinutes);

            workshop.Sessions.Add(session);

            return Results.Created($"/api/workshops/{workshop.Id}", new { workshopId = workshop.Id, sessionId = session.Id });
        });

        group.MapGet("/{id:int}/sessions", (int id, WorkshopStore store) =>
        {
            var workshop = store.Workshops.FirstOrDefault(item => item.Id == id);
            return workshop is not null
                ? Results.Ok(workshop.Sessions.OrderBy(session => session.Title))
                : Results.NotFound(new { error = $"Workshop {id} was not found." });
        });

        group.MapPut("/{id:int}/sessions/{sessionId:int}", (int id, int sessionId, UpdateSessionRequest request, WorkshopStore store) =>
        {
            var workshop = store.Workshops.FirstOrDefault(item => item.Id == id);
            if (workshop is null)
                return Results.NotFound(new { error = $"Workshop {id} was not found." });

            if (workshop.CanceledOnUtc is not null)
                return Results.Conflict(new { error = "Canceled workshops cannot be changed." });

            if (workshop.PublishedOnUtc is not null)
                return Results.Conflict(new { error = "Published workshops cannot be changed." });

            var session = workshop.Sessions.FirstOrDefault(item => item.Id == sessionId);
            if (session is null)
                return Results.NotFound(new { error = $"Session {sessionId} was not found." });

            if (string.IsNullOrWhiteSpace(request.Title))
                return Results.BadRequest(new { error = "Session title is required." });

            if (string.IsNullOrWhiteSpace(request.SpeakerName))
                return Results.BadRequest(new { error = "Speaker name is required." });

            if (request.DurationMinutes is < 30 or > 180)
                return Results.BadRequest(new { error = "Session duration must be between 30 and 180 minutes." });

            if (workshop.Sessions.Any(item => item.Id != sessionId && item.Title.Equals(request.Title, StringComparison.OrdinalIgnoreCase)))
                return Results.Conflict(new { error = $"A session named '{request.Title}' already exists in this workshop." });

            var otherDuration = workshop.Sessions.Where(item => item.Id != sessionId).Sum(item => item.DurationMinutes);
            if (otherDuration + request.DurationMinutes > 480)
                return Results.BadRequest(new { error = "A workshop cannot exceed 480 total session minutes." });

            session.Title = request.Title.Trim();
            session.SpeakerName = request.SpeakerName.Trim();
            session.DurationMinutes = request.DurationMinutes;

            return Results.Ok(new { workshopId = workshop.Id, sessionId = session.Id });
        });

        group.MapDelete("/{id:int}/sessions/{sessionId:int}", (int id, int sessionId, WorkshopStore store) =>
        {
            var workshop = store.Workshops.FirstOrDefault(item => item.Id == id);
            if (workshop is null)
                return Results.NotFound(new { error = $"Workshop {id} was not found." });

            if (workshop.CanceledOnUtc is not null)
                return Results.Conflict(new { error = "Canceled workshops cannot be changed." });

            if (workshop.PublishedOnUtc is not null)
                return Results.Conflict(new { error = "Published workshops cannot be changed." });

            var session = workshop.Sessions.FirstOrDefault(item => item.Id == sessionId);
            if (session is null)
                return Results.NotFound(new { error = $"Session {sessionId} was not found." });

            workshop.Sessions.Remove(session);
            return Results.Ok(new { workshopId = workshop.Id, sessionId });
        });

        group.MapPost("/{id:int}/publish", (int id, WorkshopStore store) =>
        {
            var workshop = store.Workshops.FirstOrDefault(item => item.Id == id);
            if (workshop is null)
                return Results.NotFound(new { error = $"Workshop {id} was not found." });

            if (workshop.CanceledOnUtc is not null)
                return Results.Conflict(new { error = "Canceled workshops cannot be published." });

            if (workshop.PublishedOnUtc is not null)
                return Results.Conflict(new { error = "The workshop is already published." });

            if (workshop.Sessions.Count == 0)
                return Results.BadRequest(new { error = "Add at least one session before publishing." });

            if (workshop.Sessions.Sum(session => session.DurationMinutes) < 60)
                return Results.BadRequest(new { error = "A published workshop needs at least 60 total minutes of sessions." });

            workshop.PublishedOnUtc = DateTime.UtcNow;

            return Results.Ok(new
            {
                workshop.Id,
                workshop.Title,
                Status = "Published",
                workshop.PublishedOnUtc
            });
        });

        group.MapPost("/{id:int}/cancel", (int id, WorkshopStore store) =>
        {
            var workshop = store.Workshops.FirstOrDefault(item => item.Id == id);
            if (workshop is null)
                return Results.NotFound(new { error = $"Workshop {id} was not found." });

            if (workshop.CanceledOnUtc is not null)
                return Results.Conflict(new { error = "The workshop is already canceled." });

            workshop.CanceledOnUtc = DateTime.UtcNow;

            return Results.Ok(new
            {
                workshop.Id,
                workshop.Title,
                Status = "Canceled",
                workshop.CanceledOnUtc
            });
        });

        group.MapGet("/{id:int}/registrations", (int id, WorkshopStore store) =>
        {
            var workshop = store.Workshops.FirstOrDefault(item => item.Id == id);
            return workshop is not null
                ? Results.Ok(workshop.Registrations.OrderBy(registration => registration.Id))
                : Results.NotFound(new { error = $"Workshop {id} was not found." });
        });

        group.MapPost("/{id:int}/registrations", (int id, CreateRegistrationRequest request, WorkshopStore store) =>
        {
            var workshop = store.Workshops.FirstOrDefault(item => item.Id == id);
            if (workshop is null)
                return Results.NotFound(new { error = $"Workshop {id} was not found." });

            if (workshop.CanceledOnUtc is not null)
                return Results.Conflict(new { error = "Canceled workshops do not accept registrations." });

            if (workshop.PublishedOnUtc is null)
                return Results.Conflict(new { error = "Only published workshops accept registrations." });

            if (string.IsNullOrWhiteSpace(request.AttendeeName))
                return Results.BadRequest(new { error = "Attendee name is required." });

            if (string.IsNullOrWhiteSpace(request.AttendeeEmail))
                return Results.BadRequest(new { error = "Attendee email is required." });

            var normalizedEmail = request.AttendeeEmail.Trim();
            if (!normalizedEmail.Contains('@'))
                return Results.BadRequest(new { error = "Attendee email must be a valid email address." });

            if (workshop.Registrations.Any(registration =>
                    registration.Status != RegistrationStatus.Cancelled &&
                    registration.AttendeeEmail.Equals(normalizedEmail, StringComparison.OrdinalIgnoreCase)))
            {
                return Results.Conflict(new { error = $"A registration for '{normalizedEmail}' already exists in this workshop." });
            }

            var status = workshop.Registrations.Count(registration => registration.Status is RegistrationStatus.Confirmed or RegistrationStatus.CheckedIn) >= workshop.MaxAttendees
                ? RegistrationStatus.Waitlisted
                : RegistrationStatus.Confirmed;

            var registration = new WorkshopRegistration(
                store.GetNextRegistrationId(),
                request.AttendeeName.Trim(),
                normalizedEmail,
                status);

            workshop.Registrations.Add(registration);

            return Results.Created(
                $"/api/workshops/{id}/registrations/{registration.Id}",
                new { workshopId = workshop.Id, registrationId = registration.Id, status = registration.Status.ToString() });
        });

        group.MapDelete("/{id:int}/registrations/{registrationId:int}", (int id, int registrationId, WorkshopStore store) =>
        {
            var workshop = store.Workshops.FirstOrDefault(item => item.Id == id);
            if (workshop is null)
                return Results.NotFound(new { error = $"Workshop {id} was not found." });

            var registration = workshop.Registrations.FirstOrDefault(item => item.Id == registrationId);
            if (registration is null)
                return Results.NotFound(new { error = $"Registration {registrationId} was not found." });

            if (registration.Status == RegistrationStatus.Cancelled)
                return Results.Conflict(new { error = "The registration is already canceled." });

            if (registration.Status == RegistrationStatus.CheckedIn)
                return Results.Conflict(new { error = "Checked-in registrations cannot be canceled." });

            var wasSeatHolding = registration.Status == RegistrationStatus.Confirmed;
            registration.Status = RegistrationStatus.Cancelled;

            WorkshopRegistration? promoted = null;
            if (wasSeatHolding)
            {
                promoted = workshop.Registrations.FirstOrDefault(item => item.Status == RegistrationStatus.Waitlisted);
                if (promoted is not null)
                    promoted.Status = RegistrationStatus.Confirmed;
            }

            return Results.Ok(new
            {
                workshopId = workshop.Id,
                registrationId,
                Status = "Cancelled",
                promotedRegistrationId = promoted?.Id
            });
        });

        group.MapPost("/{id:int}/registrations/{registrationId:int}/check-in", (int id, int registrationId, WorkshopStore store) =>
        {
            var workshop = store.Workshops.FirstOrDefault(item => item.Id == id);
            if (workshop is null)
                return Results.NotFound(new { error = $"Workshop {id} was not found." });

            if (workshop.CanceledOnUtc is not null)
                return Results.Conflict(new { error = "Canceled workshops do not allow check-in." });

            if (workshop.PublishedOnUtc is null)
                return Results.Conflict(new { error = "Only published workshops allow check-in." });

            var registration = workshop.Registrations.FirstOrDefault(item => item.Id == registrationId);
            if (registration is null)
                return Results.NotFound(new { error = $"Registration {registrationId} was not found." });

            if (registration.Status == RegistrationStatus.Waitlisted)
                return Results.Conflict(new { error = "Waitlisted registrations cannot be checked in." });

            if (registration.Status == RegistrationStatus.Cancelled)
                return Results.Conflict(new { error = "Canceled registrations cannot be checked in." });

            if (registration.Status == RegistrationStatus.CheckedIn)
                return Results.Conflict(new { error = "The registration is already checked in." });

            registration.Status = RegistrationStatus.CheckedIn;

            return Results.Ok(new
            {
                workshopId = workshop.Id,
                registrationId,
                Status = "CheckedIn"
            });
        });
    }
}
