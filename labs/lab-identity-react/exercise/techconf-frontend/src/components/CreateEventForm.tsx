import { useState } from "react";

interface CreateEventFormProps {
  onCreated: () => void;
}

export function CreateEventForm({ onCreated }: CreateEventFormProps) {
  const [name, setName] = useState("");
  const [date, setDate] = useState("");
  const [city, setCity] = useState("");
  const [description, setDescription] = useState("");

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    // TODO: Task 6 — Call createEvent() from ../api/events
    // Then reset the form and call onCreated() to refresh the list
  };

  return (
    <form className="create-form" onSubmit={handleSubmit}>
      <h2>Create Event</h2>
      <div className="form-group">
        <label htmlFor="name">Name</label>
        <input id="name" value={name} onChange={(e) => setName(e.target.value)} required />
      </div>
      <div className="form-group">
        <label htmlFor="date">Date</label>
        <input id="date" type="date" value={date} onChange={(e) => setDate(e.target.value)} required />
      </div>
      <div className="form-group">
        <label htmlFor="city">City</label>
        <input id="city" value={city} onChange={(e) => setCity(e.target.value)} required />
      </div>
      <div className="form-group">
        <label htmlFor="description">Description</label>
        <textarea id="description" value={description} onChange={(e) => setDescription(e.target.value)} />
      </div>
      <button type="submit" className="btn btn-primary">Create Event</button>
    </form>
  );
}
