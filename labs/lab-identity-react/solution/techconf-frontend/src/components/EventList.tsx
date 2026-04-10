import { useState, useEffect } from "react";
import { useAuth } from "../auth/AuthProvider";
import { fetchEvents, deleteEvent } from "../api/events";
import { LoginForm } from "./LoginForm";
import { CreateEventForm } from "./CreateEventForm";
import type { EventDto } from "../api/events";

export function EventList() {
  const { user, isAuthenticated, isLoading: authLoading } = useAuth();
  const [events, setEvents] = useState<EventDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const isOrganizer = user?.roles.includes("Organizer") ?? false;

  const loadEvents = async () => {
    try {
      setLoading(true);
      const data = await fetchEvents();
      setEvents(data);
      setError(null);
    } catch (err) {
      console.error("Failed to load events:", err);
      setError("Failed to load events");
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async (id: number) => {
    try {
      await deleteEvent(id);
      await loadEvents();
    } catch (err) {
      console.error("Failed to delete event:", err);
      setError("Failed to delete event");
    }
  };

  useEffect(() => {
    loadEvents();
  }, []);

  if (authLoading) return <div className="loading">Loading...</div>;

  return (
    <>
      {!isAuthenticated && <LoginForm />}

      {isAuthenticated && isOrganizer && (
        <CreateEventForm onCreated={loadEvents} />
      )}

      {loading ? (
        <div className="loading">Loading events...</div>
      ) : error ? (
        <div className="error-message">{error}</div>
      ) : (
        <div className="event-grid">
          {events.map((event) => (
            <div key={event.id} className="event-card">
              <h3>{event.name}</h3>
              <div className="event-meta">
                📅 {new Date(event.date).toLocaleDateString()} · 📍 {event.city}
              </div>
              {event.description && (
                <p className="event-description">{event.description}</p>
              )}
              {isAuthenticated && isOrganizer && (
                <div className="event-actions">
                  <button className="btn btn-danger" onClick={() => handleDelete(event.id)}>
                    Delete
                  </button>
                </div>
              )}
            </div>
          ))}
          {events.length === 0 && <p>No events found.</p>}
        </div>
      )}
    </>
  );
}
