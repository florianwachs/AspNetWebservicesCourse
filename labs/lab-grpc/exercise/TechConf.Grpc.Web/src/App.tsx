import { useEffect, useMemo, useState } from 'react'
import { grpc } from '@improbable-eng/grpc-web'
import {
  EventServiceClientImpl,
  EventStatusEnum,
  GrpcWebImpl,
  type EventMessage,
} from './generated/event'
import './App.css'

type ConnectionState = 'connecting' | 'live' | 'ended' | 'error'

type LiveEvent = {
  id: string
  title: string
  description: string
  startDate: Date
  endDate: Date
  location: string
  maxAttendees: number
  status: EventStatusEnum
}

const grpcBaseUrl = import.meta.env.VITE_GRPC_BASE_URL || 'https://localhost:5001'
const eventClient = new EventServiceClientImpl(
  new GrpcWebImpl(grpcBaseUrl, {
    transport: grpc.CrossBrowserHttpTransport({ withCredentials: false }),
    streamingTransport: grpc.CrossBrowserHttpTransport({ withCredentials: false }),
  }),
)
const dateFormatter = new Intl.DateTimeFormat('en-US', {
  dateStyle: 'medium',
  timeStyle: 'short',
})

function App() {
  const [events, setEvents] = useState<LiveEvent[]>([])
  const [search, setSearch] = useState('')
  const [connectionState, setConnectionState] = useState<ConnectionState>('connecting')
  const [errorMessage, setErrorMessage] = useState<string | null>(null)
  const [lastUpdate, setLastUpdate] = useState<Date | null>(null)

  useEffect(() => {
    let disposed = false
    const readyTimer = window.setTimeout(() => {
      if (!disposed) {
        setConnectionState('live')
      }
    }, 400)

    const subscription = eventClient
      .StreamEvents({
        search: search.trim(),
      })
      .subscribe({
        next: (message: EventMessage) => {
          if (disposed) {
            return
          }

          setEvents((currentEvents) => upsertEvent(currentEvents, mapEvent(message)))
          setLastUpdate(new Date())
          setConnectionState('live')
        },
        error: (error: unknown) => {
          if (disposed) {
            return
          }

          setConnectionState('error')
          setErrorMessage(getErrorMessage(error))
        },
        complete: () => {
          if (disposed) {
            return
          }

          setConnectionState((currentState) =>
            currentState === 'error' ? currentState : 'ended',
          )
        },
      })

    return () => {
      disposed = true
      window.clearTimeout(readyTimer)
      subscription.unsubscribe()
    }
  }, [search])

  const metrics = useMemo(() => {
    const uniqueCities = new Set(events.map((event) => event.location)).size
    const totalCapacity = events.reduce((sum, event) => sum + event.maxAttendees, 0)
    const draftEvents = events.filter(
      (event) => event.status === EventStatusEnum.EVENT_STATUS_DRAFT,
    ).length

    return {
      uniqueCities,
      totalCapacity,
      draftEvents,
    }
  }, [events])

  const featuredEvent = useMemo(
    () =>
      [...events].sort(
        (left, right) => left.startDate.getTime() - right.startDate.getTime(),
      )[0] ?? null,
    [events],
  )

  return (
    <main className="shell">
      <section className="hero-panel">
        <div className="hero-copy">
          <span className={`connection-pill connection-pill--${connectionState}`}>
            {connectionLabel(connectionState)}
          </span>
          <p className="eyebrow">gRPC-Web + Aspire</p>
          <h1>Live TechConf event feed</h1>
          <p className="hero-text">
            The .NET producer keeps creating events, the gRPC server persists them,
            and this React dashboard renders the live stream directly in the browser.
          </p>
        </div>

        <div className="hero-card">
          <div className="hero-card__label">Stream endpoint</div>
          <code>{grpcBaseUrl}</code>
          <div className="hero-card__meta">
            <span>{events.length} events on screen</span>
            <span>
              {lastUpdate ? `Last update ${formatTime(lastUpdate)}` : 'Awaiting first update'}
            </span>
          </div>
        </div>
      </section>

      <section className="toolbar">
        <label className="search-panel" htmlFor="event-search">
          <span>Filter by title</span>
          <input
            id="event-search"
            type="search"
            placeholder="Search TechConf events"
            value={search}
            onChange={(event) => {
              const nextSearch = event.target.value
              setSearch(nextSearch)
              setEvents([])
              setConnectionState('connecting')
              setErrorMessage(null)
              setLastUpdate(null)
            }}
          />
        </label>

        <div className="status-panel" aria-live="polite">
          <strong>{connectionLabel(connectionState)}</strong>
          <span>{statusDescription(connectionState, errorMessage)}</span>
        </div>
      </section>

      <section className="stats-grid">
        <StatCard label="Events in view" value={events.length.toString()} />
        <StatCard label="Drafts arriving live" value={metrics.draftEvents.toString()} />
        <StatCard label="Unique locations" value={metrics.uniqueCities.toString()} />
        <StatCard
          label="Combined capacity"
          value={metrics.totalCapacity.toLocaleString()}
        />
      </section>

      <section className="content-grid">
        <article className="featured-panel">
          <div className="section-heading">
            <div>
              <p className="eyebrow">Next up</p>
              <h2>Featured event</h2>
            </div>
          </div>

          {featuredEvent ? (
            <div className="featured-event">
              <div className="featured-event__meta">
                <span className={`status-badge status-badge--${statusKey(featuredEvent.status)}`}>
                  {statusLabel(featuredEvent.status)}
                </span>
                <span>{formatDateRange(featuredEvent.startDate, featuredEvent.endDate)}</span>
              </div>
              <h3>{featuredEvent.title}</h3>
              <p>{featuredEvent.description || 'A fresh event pushed from the producer stream.'}</p>
              <dl>
                <div>
                  <dt>Location</dt>
                  <dd>{featuredEvent.location}</dd>
                </div>
                <div>
                  <dt>Capacity</dt>
                  <dd>{featuredEvent.maxAttendees.toLocaleString()} attendees</dd>
                </div>
              </dl>
            </div>
          ) : (
            <EmptyState message="No events match the current filter yet." />
          )}
        </article>

        <section className="feed-panel">
          <div className="section-heading">
            <div>
              <p className="eyebrow">Realtime feed</p>
              <h2>Incoming events</h2>
            </div>
            <span className="feed-count">{events.length}</span>
          </div>

          {events.length === 0 ? (
            <EmptyState
              message={
                connectionState === 'error'
                  ? 'The stream is unavailable right now.'
                  : 'Waiting for the producer to send matching events.'
              }
            />
          ) : (
            <div className="event-list">
              {events.map((event) => (
                <article className="event-card" key={event.id}>
                  <div className="event-card__top">
                    <span className={`status-badge status-badge--${statusKey(event.status)}`}>
                      {statusLabel(event.status)}
                    </span>
                    <span>{formatDateRange(event.startDate, event.endDate)}</span>
                  </div>
                  <h3>{event.title}</h3>
                  <p>{event.description || 'Live event without an additional description.'}</p>
                  <div className="event-card__footer">
                    <span>{event.location}</span>
                    <span>{event.maxAttendees.toLocaleString()} seats</span>
                  </div>
                </article>
              ))}
            </div>
          )}
        </section>
      </section>
    </main>
  )
}

type StatCardProps = {
  label: string
  value: string
}

function StatCard({ label, value }: StatCardProps) {
  return (
    <article className="stat-card">
      <span>{label}</span>
      <strong>{value}</strong>
    </article>
  )
}

type EmptyStateProps = {
  message: string
}

function EmptyState({ message }: EmptyStateProps) {
  return (
    <div className="empty-state">
      <p>{message}</p>
    </div>
  )
}

function upsertEvent(currentEvents: LiveEvent[], incomingEvent: LiveEvent) {
  const nextEvents = new Map(currentEvents.map((event) => [event.id, event]))
  nextEvents.set(incomingEvent.id, incomingEvent)

  return [...nextEvents.values()].sort(
    (left, right) =>
      right.startDate.getTime() - left.startDate.getTime() ||
      left.title.localeCompare(right.title),
  )
}

function mapEvent(message: EventMessage): LiveEvent {
  return {
    id: message.id,
    title: message.title,
    description: message.description,
    startDate: message.startDate ?? new Date(),
    endDate: message.endDate ?? new Date(),
    location: message.location,
    maxAttendees: message.maxAttendees,
    status: message.status,
  }
}

function connectionLabel(connectionState: ConnectionState) {
  switch (connectionState) {
    case 'live':
      return 'Live'
    case 'ended':
      return 'Stream ended'
    case 'error':
      return 'Attention needed'
    default:
      return 'Connecting'
  }
}

function statusDescription(connectionState: ConnectionState, errorMessage: string | null) {
  switch (connectionState) {
    case 'live':
      return 'Connected and listening for new events from the gRPC stream.'
    case 'ended':
      return 'The server completed the stream. Refresh or restart the producer to continue.'
    case 'error':
      return errorMessage ?? 'The event stream failed.'
    default:
      return 'Opening the browser stream and replaying the latest events.'
  }
}

function statusLabel(status: EventStatusEnum) {
  switch (status) {
    case EventStatusEnum.EVENT_STATUS_PUBLISHED:
      return 'Published'
    case EventStatusEnum.EVENT_STATUS_CANCELLED:
      return 'Cancelled'
    case EventStatusEnum.EVENT_STATUS_DRAFT:
    default:
      return 'Draft'
  }
}

function statusKey(status: EventStatusEnum) {
  switch (status) {
    case EventStatusEnum.EVENT_STATUS_PUBLISHED:
      return 'published'
    case EventStatusEnum.EVENT_STATUS_CANCELLED:
      return 'cancelled'
    case EventStatusEnum.EVENT_STATUS_DRAFT:
    default:
      return 'draft'
  }
}

function formatDateRange(startDate: Date, endDate: Date) {
  return `${dateFormatter.format(startDate)} - ${dateFormatter.format(endDate)}`
}

function formatTime(date: Date) {
  return new Intl.DateTimeFormat('en-US', {
    timeStyle: 'medium',
  }).format(date)
}

function getErrorMessage(error: unknown) {
  if (error && typeof error === 'object' && 'message' in error) {
    return String(error.message)
  }

  return 'The event stream disconnected unexpectedly.'
}

export default App
