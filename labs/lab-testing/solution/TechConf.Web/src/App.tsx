import { useEffect, useState } from 'react'

interface Event {
  id: string
  title: string
  description: string
  date: string
  location: string
}

export default function App() {
  const [events, setEvents] = useState<Event[]>([])
  const [title, setTitle] = useState('')
  const [description, setDescription] = useState('')
  const [date, setDate] = useState('')
  const [location, setLocation] = useState('')

  const loadEvents = async () => {
    const res = await fetch('/api/events')
    if (res.ok) setEvents(await res.json())
  }

  useEffect(() => { loadEvents() }, [])

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    const res = await fetch('/api/events', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ title, description, date, location }),
    })
    if (res.ok) {
      setTitle('')
      setDescription('')
      setDate('')
      setLocation('')
      await loadEvents()
    }
  }

  return (
    <div style={{ maxWidth: 800, margin: '0 auto', padding: 20 }}>
      <h1>TechConf Events</h1>

      <form onSubmit={handleSubmit} data-testid="event-form">
        <div style={{ marginBottom: 8 }}>
          <input
            data-testid="event-title"
            placeholder="Title"
            value={title}
            onChange={e => setTitle(e.target.value)}
            required
          />
        </div>
        <div style={{ marginBottom: 8 }}>
          <input
            data-testid="event-description"
            placeholder="Description"
            value={description}
            onChange={e => setDescription(e.target.value)}
            required
          />
        </div>
        <div style={{ marginBottom: 8 }}>
          <input
            data-testid="event-date"
            type="date"
            value={date}
            onChange={e => setDate(e.target.value)}
            required
          />
        </div>
        <div style={{ marginBottom: 8 }}>
          <input
            data-testid="event-location"
            placeholder="Location"
            value={location}
            onChange={e => setLocation(e.target.value)}
            required
          />
        </div>
        <button type="submit" data-testid="submit-event">Add Event</button>
      </form>

      <h2>Events</h2>
      <ul data-testid="events-list">
        {events.map(ev => (
          <li key={ev.id} data-testid={`event-${ev.id}`}>
            <strong>{ev.title}</strong> — {ev.location} ({new Date(ev.date).toLocaleDateString()})
            <p>{ev.description}</p>
          </li>
        ))}
        {events.length === 0 && <li data-testid="no-events">No events yet.</li>}
      </ul>
    </div>
  )
}
