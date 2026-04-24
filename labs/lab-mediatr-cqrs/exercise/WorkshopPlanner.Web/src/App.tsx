import { useEffect, useMemo, useState } from 'react';
import { getWorkshop, getWorkshops, createWorkshop, addSession, publishWorkshop } from './api/workshops';
import { CreateWorkshopForm } from './components/CreateWorkshopForm';
import { WorkshopDetail } from './components/WorkshopDetail';
import { WorkshopList } from './components/WorkshopList';
import type { AddSessionRequest, WorkshopDetail as WorkshopDetailModel, WorkshopSummary, CreateWorkshopRequest } from './types';
import './App.css';

type Banner = {
  tone: 'success' | 'error';
  message: string;
};

function App() {
  const [workshops, setWorkshops] = useState<WorkshopSummary[]>([]);
  const [selectedWorkshopId, setSelectedWorkshopId] = useState<number | null>(null);
  const [selectedWorkshop, setSelectedWorkshop] = useState<WorkshopDetailModel | null>(null);
  const [loadingList, setLoadingList] = useState(true);
  const [loadingDetail, setLoadingDetail] = useState(false);
  const [busyAction, setBusyAction] = useState<'create' | 'session' | 'publish' | null>(null);
  const [banner, setBanner] = useState<Banner | null>(null);

  useEffect(() => {
    void loadWorkshops();
  }, []);

  useEffect(() => {
    if (selectedWorkshopId === null) {
      setSelectedWorkshop(null);
      return;
    }

    void loadWorkshopDetail(selectedWorkshopId);
  }, [selectedWorkshopId]);

  const publishedCount = useMemo(
    () => workshops.filter((workshop) => workshop.isPublished).length,
    [workshops],
  );

  async function loadWorkshops(preferredWorkshopId?: number) {
    setLoadingList(true);

    try {
      const nextWorkshops = await getWorkshops();
      setWorkshops(nextWorkshops);
      setSelectedWorkshopId((current) => {
        const desiredId = preferredWorkshopId ?? current;

        if (desiredId !== null && nextWorkshops.some((workshop) => workshop.id === desiredId)) {
          return desiredId;
        }

        return nextWorkshops[0]?.id ?? null;
      });
    } catch (error) {
      setBanner({
        tone: 'error',
        message: error instanceof Error ? error.message : 'Failed to load workshops.',
      });
    } finally {
      setLoadingList(false);
    }
  }

  async function loadWorkshopDetail(workshopId: number) {
    setLoadingDetail(true);

    try {
      const workshop = await getWorkshop(workshopId);
      setSelectedWorkshop(workshop);
    } catch (error) {
      setBanner({
        tone: 'error',
        message: error instanceof Error ? error.message : 'Failed to load the selected workshop.',
      });
      setSelectedWorkshop(null);
    } finally {
      setLoadingDetail(false);
    }
  }

  async function handleCreateWorkshop(request: CreateWorkshopRequest) {
    setBusyAction('create');

    try {
      const created = await createWorkshop(request);
      await loadWorkshops(created.id);
      setBanner({
        tone: 'success',
        message: `Created workshop "${request.title}".`,
      });

      return true;
    } catch (error) {
      setBanner({
        tone: 'error',
        message: error instanceof Error ? error.message : 'Failed to create workshop.',
      });

      return false;
    } finally {
      setBusyAction(null);
    }
  }

  async function handleAddSession(request: AddSessionRequest) {
    if (selectedWorkshopId === null) {
      return false;
    }

    setBusyAction('session');

    try {
      await addSession(selectedWorkshopId, request);
      await Promise.all([
        loadWorkshops(selectedWorkshopId),
        loadWorkshopDetail(selectedWorkshopId),
      ]);
      setBanner({
        tone: 'success',
        message: `Added session "${request.title}".`,
      });

      return true;
    } catch (error) {
      setBanner({
        tone: 'error',
        message: error instanceof Error ? error.message : 'Failed to add the session.',
      });

      return false;
    } finally {
      setBusyAction(null);
    }
  }

  async function handlePublishWorkshop() {
    if (selectedWorkshopId === null) {
      return false;
    }

    setBusyAction('publish');

    try {
      const result = await publishWorkshop(selectedWorkshopId);
      await Promise.all([
        loadWorkshops(selectedWorkshopId),
        loadWorkshopDetail(selectedWorkshopId),
      ]);
      setBanner({
        tone: 'success',
        message: `Published "${result.title}" successfully.`,
      });

      return true;
    } catch (error) {
      setBanner({
        tone: 'error',
        message: error instanceof Error ? error.message : 'Failed to publish the workshop.',
      });

      return false;
    } finally {
      setBusyAction(null);
    }
  }

  return (
    <div className="app-shell">
      <header className="hero">
        <div>
          <p className="eyebrow">Optional Day 3 follow-up lab</p>
          <h1>MediatR, CQRS, and Pipeline Behaviors</h1>
          <p className="hero-copy">
            Explore a full-stack WorkshopPlanner where thin endpoints delegate to
            requests, handlers, validators, and behaviors.
          </p>
        </div>
        <div className="hero-metrics">
          <article className="metric-card">
            <span className="metric-value">{workshops.length}</span>
            <span className="metric-label">Workshops</span>
          </article>
          <article className="metric-card">
            <span className="metric-value">{publishedCount}</span>
            <span className="metric-label">Published</span>
          </article>
          <article className="metric-card">
            <span className="metric-value">
              {workshops.reduce((total, workshop) => total + workshop.sessionCount, 0)}
            </span>
            <span className="metric-label">Sessions</span>
          </article>
        </div>
      </header>

      {banner && (
        <div className={`banner banner-${banner.tone}`} role="status">
          {banner.message}
        </div>
      )}

      <main className="workspace">
        <section className="panel panel-list">
          <div className="panel-header">
            <div>
              <p className="panel-kicker">Query side</p>
              <h2>Workshop board</h2>
            </div>
            <button
              className="ghost-button"
              onClick={() => void loadWorkshops(selectedWorkshopId ?? undefined)}
              type="button"
            >
              Refresh
            </button>
          </div>
          <WorkshopList
            isLoading={loadingList}
            selectedWorkshopId={selectedWorkshopId}
            workshops={workshops}
            onSelect={setSelectedWorkshopId}
          />
        </section>

        <section className="panel panel-detail">
          <div className="panel-header">
            <div>
              <p className="panel-kicker">Command and query flow</p>
              <h2>Workshop details</h2>
            </div>
          </div>
          <WorkshopDetail
            busyAction={busyAction}
            isLoading={loadingDetail}
            workshop={selectedWorkshop}
            onAddSession={handleAddSession}
            onPublish={handlePublishWorkshop}
          />
        </section>

        <aside className="panel panel-form">
          <div className="panel-header">
            <div>
              <p className="panel-kicker">Command side</p>
              <h2>Create workshop</h2>
            </div>
          </div>
          <CreateWorkshopForm
            isBusy={busyAction === 'create'}
            onCreate={handleCreateWorkshop}
          />
          <div className="hint-card">
            <h3>Trace the starter flow</h3>
            <p>
              `CreateWorkshop` is the finished example. `AddSession`, `PublishWorkshop`,
              and the logging registration are the main TODOs in the exercise.
            </p>
            <ol>
              <li>Endpoint receives the request.</li>
              <li>MediatR sends it to a handler.</li>
              <li>Behaviors add logging and validation.</li>
              <li>The UI refreshes from query endpoints.</li>
            </ol>
          </div>
        </aside>
      </main>
    </div>
  );
}

export default App;
