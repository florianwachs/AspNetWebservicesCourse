import { useState, type FormEvent } from 'react';
import type { AddSessionRequest, WorkshopDetail as WorkshopDetailModel } from '../types';

type WorkshopDetailProps = {
  busyAction: 'create' | 'session' | 'publish' | null;
  isLoading: boolean;
  workshop: WorkshopDetailModel | null;
  onAddSession: (request: AddSessionRequest) => Promise<boolean>;
  onPublish: () => Promise<boolean>;
};

const initialSessionForm: AddSessionRequest = {
  title: '',
  speakerName: '',
  durationMinutes: 60,
};

export function WorkshopDetail({
  busyAction,
  isLoading,
  workshop,
  onAddSession,
  onPublish,
}: WorkshopDetailProps) {
  const [sessionForm, setSessionForm] = useState(initialSessionForm);

  async function handleSessionSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();

    const wasSuccessful = await onAddSession(sessionForm);
    if (wasSuccessful) {
      setSessionForm(initialSessionForm);
    }
  }

  if (isLoading) {
    return <div className="loading-shell">Loading workshop details...</div>;
  }

  if (!workshop) {
    return (
      <div className="empty-state">
        <h3>Select a workshop</h3>
        <p>Choose a workshop from the left to inspect its read model and trigger commands from the UI.</p>
      </div>
    );
  }

  const totalMinutes = workshop.sessions.reduce((total, session) => total + session.durationMinutes, 0);

  return (
    <div className="stacked-content">
      <section className="form-card detail-shell">
        <div className="detail-top">
          <div>
            <p className="panel-kicker">Selected workshop</p>
            <h3>{workshop.title}</h3>
            <p className="detail-meta">{workshop.city}</p>
          </div>
          <span className={`status-pill ${workshop.isPublished ? 'published' : 'draft'}`}>
            {workshop.isPublished ? 'Published' : 'Draft'}
          </span>
        </div>

        <div className="detail-grid">
          <div className="detail-stat">
            <strong>{workshop.sessions.length}</strong>
            <span className="detail-meta">Sessions</span>
          </div>
          <div className="detail-stat">
            <strong>{totalMinutes} min</strong>
            <span className="detail-meta">Agenda duration</span>
          </div>
          <div className="detail-stat">
            <strong>{workshop.maxAttendees}</strong>
            <span className="detail-meta">Seats</span>
          </div>
        </div>

        <div className="detail-actions">
          <span className="detail-badge">Command handler updates the same in-memory store</span>
          <button
            className="primary-button"
            data-testid="publish-workshop"
            disabled={workshop.isPublished || busyAction === 'publish'}
            onClick={() => void onPublish()}
            type="button"
          >
            {busyAction === 'publish' ? 'Publishing...' : 'Publish workshop'}
          </button>
        </div>
      </section>

      <section className="form-card">
        <div className="panel-header">
          <div>
            <p className="panel-kicker">Query projection</p>
            <h3>Agenda</h3>
          </div>
        </div>
        {workshop.sessions.length === 0 ? (
          <div className="empty-state">
            <h3>No sessions yet</h3>
            <p>Add the first session below, then publish once the workshop reaches at least 60 minutes.</p>
          </div>
        ) : (
          <div className="session-list">
            {workshop.sessions.map((session) => (
              <article className="session-card" key={session.id}>
                <header>
                  <div>
                    <h4>{session.title}</h4>
                    <p className="session-meta">{session.speakerName}</p>
                  </div>
                  <span className="duration-pill">{session.durationMinutes} min</span>
                </header>
              </article>
            ))}
          </div>
        )}
      </section>

      <form className="form-card form-stack" onSubmit={(event) => void handleSessionSubmit(event)}>
        <div className="panel-header">
          <div>
            <p className="panel-kicker">Command form</p>
            <h3>Add session</h3>
          </div>
        </div>

        <div className="field-group">
          <label className="field-label" htmlFor="session-title">
            Session title
          </label>
          <input
            className="field-input"
            data-testid="session-title"
            disabled={workshop.isPublished}
            id="session-title"
            onChange={(event) => setSessionForm({ ...sessionForm, title: event.target.value })}
            placeholder="Validation Behavior in practice"
            required
            type="text"
            value={sessionForm.title}
          />
        </div>

        <div className="field-group">
          <label className="field-label" htmlFor="session-speaker">
            Speaker
          </label>
          <input
            className="field-input"
            data-testid="session-speaker"
            disabled={workshop.isPublished}
            id="session-speaker"
            onChange={(event) => setSessionForm({ ...sessionForm, speakerName: event.target.value })}
            placeholder="Mira Adler"
            required
            type="text"
            value={sessionForm.speakerName}
          />
        </div>

        <div className="field-group">
          <label className="field-label" htmlFor="session-duration">
            Duration in minutes
          </label>
          <input
            className="field-input"
            data-testid="session-duration"
            disabled={workshop.isPublished}
            id="session-duration"
            max={180}
            min={30}
            onChange={(event) =>
              setSessionForm({
                ...sessionForm,
                durationMinutes: Number.parseInt(event.target.value, 10) || 0,
              })
            }
            required
            type="number"
            value={sessionForm.durationMinutes}
          />
          <p className="field-help">Try invalid values and watch the validation behavior reject them consistently.</p>
        </div>

        <button
          className="secondary-button"
          data-testid="add-session"
          disabled={workshop.isPublished || busyAction === 'session'}
          type="submit"
        >
          {busyAction === 'session' ? 'Adding session...' : 'Add session'}
        </button>
      </form>
    </div>
  );
}
