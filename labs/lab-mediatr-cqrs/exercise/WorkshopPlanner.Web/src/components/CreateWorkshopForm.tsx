import { useState, type FormEvent } from 'react';
import type { CreateWorkshopRequest } from '../types';

type CreateWorkshopFormProps = {
  isBusy: boolean;
  onCreate: (request: CreateWorkshopRequest) => Promise<boolean>;
};

const initialForm: CreateWorkshopRequest = {
  title: '',
  city: '',
  maxAttendees: 24,
};

export function CreateWorkshopForm({ isBusy, onCreate }: CreateWorkshopFormProps) {
  const [form, setForm] = useState(initialForm);

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();

    const wasSuccessful = await onCreate(form);
    if (wasSuccessful) {
      setForm(initialForm);
    }
  }

  return (
    <form className="form-card form-stack" onSubmit={(event) => void handleSubmit(event)}>
      <div className="field-group">
        <label className="field-label" htmlFor="workshop-title">
          Title
        </label>
        <input
          className="field-input"
          data-testid="workshop-title"
          id="workshop-title"
          onChange={(event) => setForm({ ...form, title: event.target.value })}
          placeholder="CQRS for Real Projects"
          required
          type="text"
          value={form.title}
        />
      </div>

      <div className="field-group">
        <label className="field-label" htmlFor="workshop-city">
          City
        </label>
        <input
          className="field-input"
          data-testid="workshop-city"
          id="workshop-city"
          onChange={(event) => setForm({ ...form, city: event.target.value })}
          placeholder="Rosenheim"
          required
          type="text"
          value={form.city}
        />
      </div>

      <div className="field-group">
        <label className="field-label" htmlFor="workshop-attendees">
          Max attendees
        </label>
        <input
          className="field-input"
          data-testid="workshop-attendees"
          id="workshop-attendees"
          min={5}
          onChange={(event) =>
            setForm({ ...form, maxAttendees: Number.parseInt(event.target.value, 10) || 0 })
          }
          required
          type="number"
          value={form.maxAttendees}
        />
        <p className="field-help">The validator and behavior pipeline will enforce the minimum size.</p>
      </div>

      <button
        className="primary-button"
        data-testid="create-workshop"
        disabled={isBusy}
        type="submit"
      >
        {isBusy ? 'Creating...' : 'Create workshop'}
      </button>
    </form>
  );
}
