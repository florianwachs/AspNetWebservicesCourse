import { useState, useEffect } from "react";
import { fetchEvents } from "../api/events";
import { CreateEventForm } from "./CreateEventForm";

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

  void CreateEventForm;

  // TODO: Task 6 — Use the useAuth() hook to get the access token
  // Hint: const auth = useAuth();

  const loadEvents = async () => {
    try {
      setLoading(true);
      const data = await fetchEvents();
      setEvents(data);
      setError(null);
    } catch (err) {
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
      {/* TODO: Task 6 — Only show CreateEventForm when user is authenticated */}
      {/* Hint: {auth.isAuthenticated && <CreateEventForm ... />} */}
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
          </div>
        ))}
        {events.length === 0 && <p>No events found.</p>}
      </div>
    </>
  );
}
