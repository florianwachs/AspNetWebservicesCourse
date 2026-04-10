import { useState } from "react";

interface CreateEventFormProps {
  token?: string;
  onCreated: () => void;
}

export function CreateEventForm({ token, onCreated }: CreateEventFormProps) {
  const [name, setName] = useState("");
  const [date, setDate] = useState("");
  const [city, setCity] = useState("");
  const [description, setDescription] = useState("");

  void token;
  void onCreated;

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    // TODO: Task 6 — Call the API to create an event, passing the JWT token
    // Hint: Use createEvent() from ../api/events with the token
    // Then call onCreated() to refresh the list and reset the form
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
