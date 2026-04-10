import { useState, useEffect } from "react";
import { fetchEvents } from "../api/events";
import { LoginForm } from "./LoginForm";

interface Event {
  id: number;
  name: string;
  date: string;
  city: string;
  description: string | null;
}

export function EventList() {
  const [events, setEvents] = useState<Event[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  // TODO: Task 6 — Use the useAuth() hook to get auth state and user roles
  // Hint: const { user, isAuthenticated, isLoading: authLoading } = useAuth();
  // const isOrganizer = user?.roles.includes("Organizer") ?? false;

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

  useEffect(() => {
    loadEvents();
  }, []);

  if (loading) return <div className="loading">Loading events...</div>;
  if (error) return <div className="error-message">{error}</div>;

  return (
    <>
      {/* TODO: Task 6 — Show LoginForm when not authenticated */}
      {/* Hint: {!isAuthenticated && <LoginForm />} */}

      {/* TODO: Task 6 — Show CreateEventForm only for Organizer role */}
      {/* Hint: {isAuthenticated && isOrganizer && <CreateEventForm onCreated={loadEvents} />} */}

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
            {/* TODO: Task 6 — Show delete button only for Organizer role */}
          </div>
        ))}
        {events.length === 0 && <p>No events found.</p>}
      </div>
    </>
  );
}
