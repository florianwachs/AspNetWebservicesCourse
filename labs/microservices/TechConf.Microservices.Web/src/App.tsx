import { useEffect, useMemo, useState } from 'react';
import type { FormEvent } from 'react';
import './App.css';

type EventSummary = {
  id: string;
  name: string;
  city: string;
  startsOn: string;
  endsOn: string;
  status: string;
};

type SessionSummary = {
  id: string;
  eventId: string;
  title: string;
  speaker: string;
  startsAt: string;
  seatsAvailable: number;
};

type NotificationSummary = {
  id: string;
  registrationId: string;
  recipient: string;
  subject: string;
  channel: string;
  createdAt: string;
  status: string;
};

type RecommendationSummary = {
  id: string;
  eventId: string;
  title: string;
  reason: string;
  confidence: number;
};

type ServiceSnapshot = {
  name: string;
  capability: string;
  runtime: string;
  dataStore: string;
  communication: string;
  status: string;
};

type DashboardResponse = {
  eventCount: number;
  sessionCount: number;
  registrationCount: number;
  notificationCount: number;
  recommendationCount: number;
  services: ServiceSnapshot[];
};

type RegistrationResponse = {
  registrationId: string;
  attendeeName: string;
  attendeeEmail: string;
  registeredAt: string;
};

type Banner = {
  tone: 'success' | 'error';
  message: string;
};

async function readJson<T>(response: Response): Promise<T> {
  if (!response.ok) {
    let message = `Request failed with status ${response.status}.`;

    try {
      const payload = await response.json() as { error?: string };
      if (payload.error) {
        message = payload.error;
      }
    } catch {
      // Keep the status-based message when the response is not JSON.
    }

    throw new Error(message);
  }

  return await response.json() as T;
}

function formatDate(value: string) {
  return new Intl.DateTimeFormat(undefined, { month: 'short', day: 'numeric', year: 'numeric' }).format(new Date(value));
}

function formatTime(value: string) {
  return new Intl.DateTimeFormat(undefined, { month: 'short', day: 'numeric', hour: '2-digit', minute: '2-digit' }).format(new Date(value));
}

function App() {
  const [dashboard, setDashboard] = useState<DashboardResponse | null>(null);
  const [events, setEvents] = useState<EventSummary[]>([]);
  const [sessions, setSessions] = useState<SessionSummary[]>([]);
  const [notifications, setNotifications] = useState<NotificationSummary[]>([]);
  const [recommendations, setRecommendations] = useState<RecommendationSummary[]>([]);
  const [selectedEventId, setSelectedEventId] = useState<string | null>(null);
  const [selectedSessionId, setSelectedSessionId] = useState<string | null>(null);
  const [attendeeName, setAttendeeName] = useState('Alex Morgan');
  const [attendeeEmail, setAttendeeEmail] = useState('alex.morgan@example.com');
  const [isLoading, setIsLoading] = useState(true);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [banner, setBanner] = useState<Banner | null>(null);

  useEffect(() => {
    void loadInitialData();
  }, []);

  useEffect(() => {
    if (!selectedEventId) {
      setSessions([]);
      setSelectedSessionId(null);
      return;
    }

    void loadSessions(selectedEventId);
    void loadRecommendations(selectedEventId);
  }, [selectedEventId]);

  const selectedEvent = useMemo(
    () => events.find((event) => event.id === selectedEventId) ?? null,
    [events, selectedEventId],
  );

  const selectedSession = useMemo(
    () => sessions.find((session) => session.id === selectedSessionId) ?? null,
    [sessions, selectedSessionId],
  );

  async function loadInitialData() {
    setIsLoading(true);

    try {
      const [dashboardResult, eventsResult, notificationsResult] = await Promise.all([
        readJson<DashboardResponse>(await fetch('/api/dashboard')),
        readJson<EventSummary[]>(await fetch('/api/events')),
        readJson<NotificationSummary[]>(await fetch('/api/notifications/recent')),
      ]);

      setDashboard(dashboardResult);
      setEvents(eventsResult);
      setNotifications(notificationsResult);
      setSelectedEventId((current) => current ?? eventsResult[0]?.id ?? null);
    } catch (error) {
      setBanner({ tone: 'error', message: error instanceof Error ? error.message : 'Failed to load the demo.' });
    } finally {
      setIsLoading(false);
    }
  }

  async function loadSessions(eventId: string) {
    try {
      const nextSessions = await readJson<SessionSummary[]>(await fetch(`/api/events/${eventId}/sessions`));
      setSessions(nextSessions);
      setSelectedSessionId((current) => {
        if (current && nextSessions.some((session) => session.id === current)) {
          return current;
        }

        return nextSessions[0]?.id ?? null;
      });
    } catch (error) {
      setBanner({ tone: 'error', message: error instanceof Error ? error.message : 'Failed to load sessions.' });
      setSessions([]);
      setSelectedSessionId(null);
    }
  }

  async function loadRecommendations(eventId: string) {
    try {
      const nextRecommendations = await readJson<RecommendationSummary[]>(
        await fetch(`/api/recommendations?eventId=${encodeURIComponent(eventId)}`),
      );

      setRecommendations(nextRecommendations);
    } catch (error) {
      setBanner({ tone: 'error', message: error instanceof Error ? error.message : 'Failed to load recommendations.' });
      setRecommendations([]);
    }
  }

  async function refreshDashboard() {
    const [dashboardResult, notificationsResult, recommendationsResult] = await Promise.all([
      readJson<DashboardResponse>(await fetch('/api/dashboard')),
      readJson<NotificationSummary[]>(await fetch('/api/notifications/recent')),
      selectedEventId
        ? readJson<RecommendationSummary[]>(await fetch(`/api/recommendations?eventId=${encodeURIComponent(selectedEventId)}`))
        : Promise.resolve([]),
    ]);

    setDashboard(dashboardResult);
    setNotifications(notificationsResult);
    setRecommendations(recommendationsResult);
  }

  async function handleRegister(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();

    if (!selectedEventId || !selectedSessionId) {
      setBanner({ tone: 'error', message: 'Choose an event and session first.' });
      return;
    }

    setIsSubmitting(true);

    try {
      const created = await readJson<RegistrationResponse>(await fetch('/api/registrations', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          eventId: selectedEventId,
          sessionId: selectedSessionId,
          attendeeName,
          attendeeEmail,
        }),
      }));

      await refreshDashboard();
      setBanner({
        tone: 'success',
        message: `Registered ${created.attendeeName}. The notification service will pick up the event from RabbitMQ.`,
      });
    } catch (error) {
      setBanner({ tone: 'error', message: error instanceof Error ? error.message : 'Registration failed.' });
    } finally {
      setIsSubmitting(false);
    }
  }

  const totals = dashboard ?? {
    eventCount: events.length,
    sessionCount: sessions.length,
    registrationCount: 0,
    notificationCount: notifications.length,
    recommendationCount: recommendations.length,
    services: [],
  };

  return (
    <div className="app-shell">
      <header className="topbar">
        <div>
          <p className="eyebrow">Aspire microservices demo</p>
          <h1>TechConf Distributed Registration</h1>
          <p className="lede">
            A Gateway/BFF composes four independently deployed services with owned data and an asynchronous notification handoff.
          </p>
        </div>
        <button className="icon-button" onClick={() => void loadInitialData()} type="button" aria-label="Refresh dashboard">
          R
        </button>
      </header>

      {banner && (
        <div className={`banner banner-${banner.tone}`} role="status">
          {banner.message}
        </div>
      )}

      <main className="workspace">
        <section className="metrics-grid" aria-label="System totals">
          <article className="metric">
            <span>{totals.eventCount}</span>
            <p>Events</p>
          </article>
          <article className="metric">
            <span>{totals.sessionCount}</span>
            <p>Sessions</p>
          </article>
          <article className="metric">
            <span>{totals.registrationCount}</span>
            <p>Registrations</p>
          </article>
          <article className="metric">
            <span>{totals.notificationCount}</span>
            <p>Notifications</p>
          </article>
          <article className="metric">
            <span>{totals.recommendationCount}</span>
            <p>Node recs</p>
          </article>
        </section>

        <section className="topology-panel">
          <div className="panel-header">
            <div>
              <p className="panel-kicker">Runtime topology</p>
              <h2>Service boundaries</h2>
            </div>
            <span className="status-dot">Healthy</span>
          </div>
          <div className="service-grid">
            {totals.services.map((service) => (
              <article className="service-card" key={service.name}>
                <header>
                  <h3>{service.name}</h3>
                  <span>{service.status}</span>
                </header>
                <p>{service.capability}</p>
                <dl>
                  <div>
                    <dt>Data</dt>
                    <dd>{service.dataStore}</dd>
                  </div>
                  <div>
                    <dt>Comm</dt>
                    <dd>{service.communication}</dd>
                  </div>
                </dl>
              </article>
            ))}
          </div>
        </section>

        <section className="content-grid">
          <div className="panel">
            <div className="panel-header">
              <div>
                <p className="panel-kicker">Catalog service</p>
                <h2>Events</h2>
              </div>
            </div>
            <div className="event-list">
              {isLoading && <p className="muted">Loading events...</p>}
              {events.map((event) => (
                <button
                  className={`event-card ${event.id === selectedEventId ? 'selected' : ''}`}
                  key={event.id}
                  onClick={() => setSelectedEventId(event.id)}
                  type="button"
                >
                  <span>{event.status}</span>
                  <strong>{event.name}</strong>
                  <small>{event.city} - {formatDate(event.startsOn)}</small>
                </button>
              ))}
            </div>
          </div>

          <div className="panel">
            <div className="panel-header">
              <div>
                <p className="panel-kicker">Schedule service</p>
                <h2>{selectedEvent ? selectedEvent.name : 'Sessions'}</h2>
              </div>
            </div>
            <div className="session-list">
              {sessions.map((session) => (
                <button
                  className={`session-card ${session.id === selectedSessionId ? 'selected' : ''}`}
                  key={session.id}
                  onClick={() => setSelectedSessionId(session.id)}
                  type="button"
                >
                  <strong>{session.title}</strong>
                  <span>{session.speaker}</span>
                  <small>{formatTime(session.startsAt)} - {session.seatsAvailable} seats</small>
                </button>
              ))}
              {!sessions.length && <p className="muted">Choose an event to load sessions through the Gateway.</p>}
            </div>
          </div>

          <form className="panel registration-panel" onSubmit={handleRegister}>
            <div className="panel-header">
              <div>
                <p className="panel-kicker">Registration service</p>
                <h2>Register attendee</h2>
              </div>
            </div>
            <label>
              Name
              <input value={attendeeName} onChange={(event) => setAttendeeName(event.target.value)} />
            </label>
            <label>
              Email
              <input value={attendeeEmail} onChange={(event) => setAttendeeEmail(event.target.value)} type="email" />
            </label>
            <div className="selection-summary">
              <span>{selectedEvent?.name ?? 'No event selected'}</span>
              <strong>{selectedSession?.title ?? 'No session selected'}</strong>
            </div>
            <button className="primary-button" disabled={isSubmitting || !selectedSessionId} type="submit">
              {isSubmitting ? 'Registering...' : 'Create registration'}
            </button>
          </form>
        </section>

        <section className="notifications-panel">
          <div className="panel-header">
            <div>
              <p className="panel-kicker">Notifications service</p>
              <h2>Recent messages</h2>
            </div>
            <button className="secondary-button" onClick={() => void refreshDashboard()} type="button">Refresh</button>
          </div>
          <div className="notification-list">
            {notifications.map((notification) => (
              <article className="notification-card" key={notification.id}>
                <strong>{notification.subject}</strong>
                <span>{notification.recipient}</span>
                <small>{notification.channel} - {notification.status} - {formatTime(notification.createdAt)}</small>
              </article>
            ))}
            {!notifications.length && <p className="muted">No notifications yet. Register an attendee to publish a RabbitMQ event.</p>}
          </div>
        </section>

        <section className="recommendations-panel">
          <div className="panel-header">
            <div>
              <p className="panel-kicker">Node.js service</p>
              <h2>Recommendations</h2>
            </div>
          </div>
          <div className="recommendation-list">
            {recommendations.map((recommendation) => (
              <article className="recommendation-card" key={recommendation.id}>
                <span>{recommendation.confidence}% match</span>
                <strong>{recommendation.title}</strong>
                <p>{recommendation.reason}</p>
              </article>
            ))}
            {!recommendations.length && <p className="muted">Choose an event to query the Node service through the Gateway.</p>}
          </div>
        </section>
      </main>
    </div>
  );
}

export default App;
