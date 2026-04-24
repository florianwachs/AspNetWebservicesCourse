import { useCallback, useEffect, useMemo, useState } from 'react'
import './App.css'
import { getSession, listSessions, releaseSeat, reserveSeat } from './api'
import type { SessionDetails, SessionSummary } from './types'

type ActivityTone = 'error' | 'info' | 'success'

interface ActivityItem {
  id: string
  tone: ActivityTone
  title: string
  detail: string
}

const attendeeIdPattern =
  /^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i

function createAttendeeId(): string {
  return (
    globalThis.crypto?.randomUUID?.() ??
    'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, (character) => {
      const randomValue = Math.floor(Math.random() * 16)
      const nextValue =
        character === 'x' ? randomValue : (randomValue & 0x3) | 0x8
      return nextValue.toString(16)
    })
  )
}

function shortId(value: string) {
  return value.slice(0, 8)
}

function capacityLabel(value: number) {
  return `${value} seat${value === 1 ? '' : 's'}`
}

function App() {
  const [sessions, setSessions] = useState<SessionSummary[]>([])
  const [selectedSessionId, setSelectedSessionId] = useState<string | null>(null)
  const [selectedSession, setSelectedSession] = useState<SessionDetails | null>(null)
  const [attendeeId, setAttendeeId] = useState<string>(() => createAttendeeId())
  const [isLoadingSessions, setIsLoadingSessions] = useState(true)
  const [isLoadingDetails, setIsLoadingDetails] = useState(false)
  const [isSubmitting, setIsSubmitting] = useState(false)
  const [loadError, setLoadError] = useState<string | null>(null)
  const [detailError, setDetailError] = useState<string | null>(null)
  const [activities, setActivities] = useState<ActivityItem[]>([
    {
      id: 'ready',
      tone: 'info',
      title: 'Dashboard primed',
      detail: 'Launch the AppHost to connect the UI to the Akka reservation actor.',
    },
  ])

  const normalizedAttendeeId = attendeeId.trim()
  const selectedSummary = useMemo(
    () => sessions.find((session) => session.id === selectedSessionId) ?? null,
    [selectedSessionId, sessions],
  )
  const totalCapacity = useMemo(
    () => sessions.reduce((sum, session) => sum + session.capacity, 0),
    [sessions],
  )
  const totalReserved = useMemo(
    () => sessions.reduce((sum, session) => sum + session.reservedSeats, 0),
    [sessions],
  )
  const totalAvailable = totalCapacity - totalReserved
  const selectedOccupancy = selectedSummary
    ? Math.round((selectedSummary.reservedSeats / selectedSummary.capacity) * 100)
    : 0
  const attendeeIdLooksValid = attendeeIdPattern.test(normalizedAttendeeId)
  const attendeeHasSeat = selectedSession?.attendeeIds.includes(normalizedAttendeeId) ?? false

  const pushActivity = useCallback(
    (tone: ActivityTone, title: string, detail: string) => {
      setActivities((current) =>
        [
          {
            id: `${Date.now()}-${Math.random().toString(16).slice(2, 8)}`,
            tone,
            title,
            detail,
          },
          ...current,
        ].slice(0, 6),
      )
    },
    [],
  )

  function syncSessionState(session: SessionDetails) {
    setSelectedSession(session)
    setSessions((current) =>
      current.map((summary) =>
        summary.id === session.id
          ? {
              id: session.id,
              title: session.title,
              capacity: session.capacity,
              reservedSeats: session.reservedSeats,
              availableSeats: session.availableSeats,
            }
          : summary,
      ),
    )
  }

  const loadSessions = useCallback(
    async (preferredSessionId?: string | null) => {
      setIsLoadingSessions(true)
      setLoadError(null)

      try {
        const response = await listSessions()
        setSessions(response.sessions)
        setSelectedSession((current) =>
          response.sessions.length === 0 ? null : current,
        )
        setIsLoadingDetails((current) =>
          response.sessions.length === 0 ? false : current,
        )
        setSelectedSessionId((current) => {
          const candidate = preferredSessionId ?? current

          if (candidate && response.sessions.some((session) => session.id === candidate)) {
            return candidate
          }

          return response.sessions[0]?.id ?? null
        })
      } catch (error) {
        const message =
          error instanceof Error
            ? error.message
            : 'The dashboard could not reach the reservation API.'

        setSessions([])
        setSelectedSession(null)
        setSelectedSessionId(null)
        setLoadError(message)
        pushActivity('error', 'Dashboard disconnected', message)
      } finally {
        setIsLoadingSessions(false)
      }
    },
    [pushActivity],
  )

  useEffect(() => {
    const timeoutId = window.setTimeout(() => {
      void loadSessions(null)
    }, 0)

    return () => {
      window.clearTimeout(timeoutId)
    }
  }, [loadSessions])

  useEffect(() => {
    if (!selectedSessionId) {
      return
    }

    let isDisposed = false

    const loadDetails = async () => {
      setIsLoadingDetails(true)
      setDetailError(null)

      try {
        const session = await getSession(selectedSessionId)

        if (!isDisposed) {
          setSelectedSession(session)
        }
      } catch (error) {
        if (isDisposed) {
          return
        }

        const message =
          error instanceof Error
            ? error.message
            : 'The dashboard could not load attendee details for this session.'

        setSelectedSession(null)
        setDetailError(message)
      } finally {
        if (!isDisposed) {
          setIsLoadingDetails(false)
        }
      }
    }

    const timeoutId = window.setTimeout(() => {
      void loadDetails()
    }, 0)

    return () => {
      isDisposed = true
      window.clearTimeout(timeoutId)
    }
  }, [selectedSessionId])

  async function handleSeatAction(action: 'release' | 'reserve') {
    if (!selectedSessionId) {
      return
    }

    if (!attendeeIdLooksValid) {
      setDetailError('Please enter a valid attendee GUID before sending a request.')
      return
    }

    setIsSubmitting(true)
    setDetailError(null)

    try {
      const session =
        action === 'reserve'
          ? await reserveSeat(selectedSessionId, normalizedAttendeeId)
          : await releaseSeat(selectedSessionId, normalizedAttendeeId)

      syncSessionState(session)
      pushActivity(
        'success',
        action === 'reserve' ? 'Seat reserved' : 'Seat released',
        `${shortId(normalizedAttendeeId)} ${
          action === 'reserve' ? 'claimed' : 'released'
        } a seat in ${session.title}.`,
      )
    } catch (error) {
      const message =
        error instanceof Error
          ? error.message
          : action === 'reserve'
            ? 'The reservation request failed.'
            : 'The release request failed.'

      setDetailError(message)
      pushActivity(
        'error',
        action === 'reserve' ? 'Reservation rejected' : 'Release rejected',
        message,
      )
    } finally {
      setIsSubmitting(false)
    }
  }

  return (
    <div className="app-shell">
      <header className="panel hero-panel">
        <div className="hero-copy">
          <div className="eyebrow">Actor-powered seat control</div>
          <h1>TechConf Seat Control Room</h1>
          <p className="hero-text">
            Monitor session capacity, probe edge cases, and let the Akka actor stay
            the only authority that can approve or reject seat changes.
          </p>
          <div className="hero-tags">
            <span>Akka.NET</span>
            <span>Minimal APIs</span>
            <span>Vite + React</span>
            <span>Aspire orchestration</span>
          </div>
        </div>

        <div className="hero-spotlight">
          <div className="spotlight-label">Selected session</div>
          {selectedSummary ? (
            <>
              <h2>{selectedSummary.title}</h2>
              <p className="spotlight-meta">
                {selectedSummary.reservedSeats} reserved of{' '}
                {capacityLabel(selectedSummary.capacity)}
              </p>
              <div className="capacity-meter">
                <div
                  className="capacity-meter-fill"
                  style={{ width: `${selectedOccupancy}%` }}
                />
              </div>
              <div className="spotlight-footer">
                <span>{selectedSummary.availableSeats} seats left</span>
                <button
                  className="ghost-button"
                  onClick={() => void loadSessions(selectedSessionId)}
                  type="button"
                >
                  Refresh snapshot
                </button>
              </div>
            </>
          ) : (
            <p className="spotlight-empty">
              Pick a session to inspect live attendee state and run reservation
              requests.
            </p>
          )}
        </div>
      </header>

      <section className="stats-grid">
        <article className="panel stat-card">
          <span className="stat-label">Tracked sessions</span>
          <strong>{sessions.length}</strong>
          <p>Seeded conference rooms available to the actor.</p>
        </article>
        <article className="panel stat-card">
          <span className="stat-label">Reserved seats</span>
          <strong>{totalReserved}</strong>
          <p>Current reservations accepted by the single state owner.</p>
        </article>
        <article className="panel stat-card">
          <span className="stat-label">Seats remaining</span>
          <strong>{totalAvailable}</strong>
          <p>Capacity left before the actor starts rejecting requests.</p>
        </article>
        <article className="panel stat-card">
          <span className="stat-label">Current attendee</span>
          <strong>{shortId(normalizedAttendeeId || '--------')}</strong>
          <p>Use one GUID to reserve, release, and test repeat calls.</p>
        </article>
      </section>

      <main className="dashboard-grid">
        <section className="panel sessions-panel">
          <div className="section-header">
            <div>
              <div className="section-eyebrow">Live capacity map</div>
              <h2>Sessions</h2>
            </div>
            {isLoadingSessions ? <span className="status-pill">Loading</span> : null}
          </div>

          {loadError ? (
            <div className="empty-state error-state">
              <h3>API not ready yet</h3>
              <p>{loadError}</p>
              <p>
                The frontend is wired up. Start the AppHost or finish the Akka API
                tasks so the dashboard can load live session data.
              </p>
              <button
                className="primary-button"
                onClick={() => void loadSessions(selectedSessionId)}
                type="button"
              >
                Try again
              </button>
            </div>
          ) : (
            <div className="session-list">
              {sessions.map((session) => {
                const occupancy = Math.round(
                  (session.reservedSeats / session.capacity) * 100,
                )
                const isSelected = session.id === selectedSessionId

                return (
                  <button
                    className={`session-card${isSelected ? ' selected' : ''}`}
                    key={session.id}
                    onClick={() => setSelectedSessionId(session.id)}
                    type="button"
                  >
                    <div className="session-card-head">
                      <div>
                        <span className="session-capacity">{capacityLabel(session.capacity)}</span>
                        <h3>{session.title}</h3>
                      </div>
                      <span className="session-availability">
                        {session.availableSeats} left
                      </span>
                    </div>
                    <div className="capacity-meter compact">
                      <div
                        className="capacity-meter-fill"
                        style={{ width: `${occupancy}%` }}
                      />
                    </div>
                    <div className="session-card-footer">
                      <span>{session.reservedSeats} reserved</span>
                      <span>{occupancy}% occupied</span>
                    </div>
                  </button>
                )
              })}
            </div>
          )}
        </section>

        <section className="panel details-panel">
          <div className="section-header">
            <div>
              <div className="section-eyebrow">Manual test console</div>
              <h2>Reservation controls</h2>
            </div>
            {isLoadingDetails ? <span className="status-pill">Syncing</span> : null}
          </div>

          <label className="field">
            <span>Attendee ID</span>
            <input
              onChange={(event) => setAttendeeId(event.target.value)}
              placeholder="8caa4442-0d65-4dfe-9577-9580ab824001"
              type="text"
              value={attendeeId}
            />
          </label>

          <div className="field-row">
            <button
              className="ghost-button"
              onClick={() => setAttendeeId(createAttendeeId())}
              type="button"
            >
              Generate attendee
            </button>
            <span
              className={`inline-hint${
                attendeeIdLooksValid ? ' inline-hint-valid' : ' inline-hint-error'
              }`}
            >
              {attendeeIdLooksValid ? 'GUID format looks good' : 'GUID format required'}
            </span>
          </div>

          <div className="action-row">
            <button
              className="primary-button"
              disabled={
                !selectedSessionId ||
                !attendeeIdLooksValid ||
                attendeeHasSeat ||
                isSubmitting
              }
              onClick={() => void handleSeatAction('reserve')}
              type="button"
            >
              {isSubmitting ? 'Sending...' : 'Reserve seat'}
            </button>
            <button
              className="secondary-button"
              disabled={
                !selectedSessionId ||
                !attendeeIdLooksValid ||
                !attendeeHasSeat ||
                isSubmitting
              }
              onClick={() => void handleSeatAction('release')}
              type="button"
            >
              Release seat
            </button>
          </div>

          {detailError ? <div className="inline-notice error-state">{detailError}</div> : null}

          {selectedSession ? (
            <div className="details-card">
              <div className="details-head">
                <div>
                  <span className="section-eyebrow">Live attendee snapshot</span>
                  <h3>{selectedSession.title}</h3>
                </div>
                <span className="session-availability">
                  {selectedSession.availableSeats} left
                </span>
              </div>

              <div className="detail-stats">
                <div>
                  <span className="detail-label">Capacity</span>
                  <strong>{selectedSession.capacity}</strong>
                </div>
                <div>
                  <span className="detail-label">Reserved</span>
                  <strong>{selectedSession.reservedSeats}</strong>
                </div>
                <div>
                  <span className="detail-label">Available</span>
                  <strong>{selectedSession.availableSeats}</strong>
                </div>
              </div>

              <div className="attendee-list">
                {selectedSession.attendeeIds.length > 0 ? (
                  selectedSession.attendeeIds.map((value) => (
                    <div
                      className={`attendee-pill${
                        value === normalizedAttendeeId ? ' attendee-pill-active' : ''
                      }`}
                      key={value}
                    >
                      <span>{shortId(value)}</span>
                      <small>{value}</small>
                    </div>
                  ))
                ) : (
                  <div className="empty-attendees">
                    No attendee owns a seat yet. Try a reservation to see the actor
                    state change.
                  </div>
                )}
              </div>
            </div>
          ) : (
            <div className="empty-state">
              <h3>No session selected</h3>
              <p>Select a session from the left to inspect attendee details.</p>
            </div>
          )}
        </section>
      </main>

      <section className="secondary-grid">
        <section className="panel activity-panel">
          <div className="section-header">
            <div>
              <div className="section-eyebrow">Recent activity</div>
              <h2>Event log</h2>
            </div>
          </div>

          <div className="activity-list">
            {activities.map((activity) => (
              <article
                className={`activity-item activity-${activity.tone}`}
                key={activity.id}
              >
                <div className="activity-title">{activity.title}</div>
                <p>{activity.detail}</p>
              </article>
            ))}
          </div>
        </section>

        <aside className="panel notes-panel">
          <div className="section-eyebrow">Why this view helps</div>
          <h2>Actor-model reminders</h2>
          <ul className="notes-list">
            <li>The browser only sends commands. The actor decides whether state changes are valid.</li>
            <li>Repeated reserve or release calls make conflicts visible without adding controller logic.</li>
            <li>The attendee list reveals that session state stays local to the actor and updates atomically.</li>
          </ul>
        </aside>
      </section>
    </div>
  )
}

export default App
