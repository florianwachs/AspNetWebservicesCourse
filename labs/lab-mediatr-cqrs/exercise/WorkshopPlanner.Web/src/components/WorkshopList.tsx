import type { WorkshopSummary } from '../types';

type WorkshopListProps = {
  isLoading: boolean;
  selectedWorkshopId: number | null;
  workshops: WorkshopSummary[];
  onSelect: (workshopId: number) => void;
};

export function WorkshopList({
  isLoading,
  selectedWorkshopId,
  workshops,
  onSelect,
}: WorkshopListProps) {
  if (isLoading) {
    return <div className="loading-shell">Loading workshops...</div>;
  }

  if (workshops.length === 0) {
    return (
      <div className="empty-state">
        <h3>No workshops yet</h3>
        <p>Create the first workshop from the command panel to see the query side update.</p>
      </div>
    );
  }

  return (
    <div className="workshop-list">
      {workshops.map((workshop) => (
        <button
          className={`workshop-card ${selectedWorkshopId === workshop.id ? 'selected' : ''}`}
          key={workshop.id}
          onClick={() => onSelect(workshop.id)}
          type="button"
        >
          <header>
            <div>
              <h3>{workshop.title}</h3>
              <p className="workshop-meta">{workshop.city}</p>
            </div>
            <span className={`status-pill ${workshop.isPublished ? 'published' : 'draft'}`}>
              {workshop.isPublished ? 'Published' : 'Draft'}
            </span>
          </header>

          <div className="workshop-stats">
            <span>{workshop.sessionCount} sessions</span>
            <span>{workshop.maxAttendees} seats</span>
          </div>
        </button>
      ))}
    </div>
  );
}
