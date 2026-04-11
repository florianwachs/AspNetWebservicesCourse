import { useCallback, useEffect, useMemo, useState } from 'react';
import aspireLogo from '/Aspire.png';
import './App.css';

type FeedTone = 'info' | 'warning' | 'error';
type BusyAction = 'initial' | 'generate' | 'audit' | 'dispute' | `inspect:${string}` | null;

interface TariffCard {
  countryCode: string;
  countryName: string;
  exportSpecialty: string;
  tariffPercent: number;
  tariffBand: string;
  absurdityScore: number;
  riskLevel: string;
  volatility: string;
  sillyReason: string;
  ministerNote: string;
}

interface TariffBatchResponse {
  batchId: string;
  scenario: string;
  headline: string;
  generatedAt: string;
  absurdityIndex: number;
  averageTariff: number;
  latestAudit: TariffAuditSummary | null;
  latestDispute: TradeDisputeSummary | null;
  tariffs: TariffCard[];
}

interface TariffAuditSummary {
  auditId: string;
  createdAt: string;
  focusCountryCode: string | null;
  focusCountryName: string | null;
  flaggedCountries: number;
  verdict: string;
  dashboardHint: string;
}

interface TradeDisputeSummary {
  disputeId: string;
  createdAt: string;
  countryCode: string;
  countryName: string;
  severity: string;
  outcome: string;
  highlightAsError: boolean;
}

interface TariffInspectionResponse {
  batchId: string;
  tariff: TariffCard;
  story: string;
  traceTip: string;
  logHint: string;
  metricHint: string;
}

interface TariffAuditResponse {
  auditId: string;
  batchId: string;
  createdAt: string;
  scenario: string;
  focusCountryCode: string | null;
  focusCountryName: string | null;
  flaggedCountries: number;
  averageTariff: number;
  highestTariff: TariffCard;
  verdict: string;
  findings: string[];
  dashboardHint: string;
}

interface TradeDisputeResponse {
  disputeId: string;
  batchId: string;
  createdAt: string;
  countryCode: string;
  countryName: string;
  tariffPercent: number;
  complaint: string;
  outcome: string;
  severity: string;
  ruling: string;
  highlightAsError: boolean;
  dashboardHint: string;
}

interface FeedItem {
  id: string;
  title: string;
  detail: string;
  tone: FeedTone;
  timestamp: string;
}

const GENERATION_SCENARIOS = [
  'Midnight accordion shortage',
  'Executive whimsy summit',
  'Moonbeam retaliation week',
  'Emergency glitter stabilization plan',
  'Unexpected spreadsheet thunderstorm',
];

const DISPUTE_COMPLAINTS = [
  'Our ceremonial biscuits now cost more than a lighthouse.',
  'This tariff appears to have been rounded with interpretive dance.',
  'A customs goose keeps demanding bigger percentages.',
  'Three forklifts resigned after seeing the latest decree.',
];

const dateTimeFormatter = new Intl.DateTimeFormat(undefined, {
  month: 'short',
  day: 'numeric',
  hour: '2-digit',
  minute: '2-digit',
});

const timeFormatter = new Intl.DateTimeFormat(undefined, {
  hour: '2-digit',
  minute: '2-digit',
});

function formatPercent(value: number) {
  return `${Math.round(value)}%`;
}

function formatDateTime(value: string) {
  return dateTimeFormatter.format(new Date(value));
}

function formatTime(value: string) {
  return timeFormatter.format(new Date(value));
}

function getToneFromTariff(value: number): FeedTone {
  if (value >= 210) {
    return 'error';
  }

  if (value >= 160) {
    return 'warning';
  }

  return 'info';
}

function summarizeAudit(audit: TariffAuditResponse): TariffAuditSummary {
  return {
    auditId: audit.auditId,
    createdAt: audit.createdAt,
    focusCountryCode: audit.focusCountryCode,
    focusCountryName: audit.focusCountryName,
    flaggedCountries: audit.flaggedCountries,
    verdict: audit.verdict,
    dashboardHint: audit.dashboardHint,
  };
}

function summarizeDispute(dispute: TradeDisputeResponse): TradeDisputeSummary {
  return {
    disputeId: dispute.disputeId,
    createdAt: dispute.createdAt,
    countryCode: dispute.countryCode,
    countryName: dispute.countryName,
    severity: dispute.severity,
    outcome: dispute.outcome,
    highlightAsError: dispute.highlightAsError,
  };
}

async function requestJson<T>(url: string, options?: RequestInit): Promise<T> {
  const response = await fetch(url, {
    ...options,
    headers: {
      Accept: 'application/json',
      ...(options?.body ? { 'Content-Type': 'application/json' } : {}),
      ...(options?.headers ?? {}),
    },
  });

  if (!response.ok) {
    const body = await response.text();
    throw new Error(body || `Request failed with status ${response.status}`);
  }

  return (await response.json()) as T;
}

function App() {
  const [batch, setBatch] = useState<TariffBatchResponse | null>(null);
  const [inspection, setInspection] = useState<TariffInspectionResponse | null>(null);
  const [audit, setAudit] = useState<TariffAuditResponse | null>(null);
  const [dispute, setDispute] = useState<TradeDisputeResponse | null>(null);
  const [feed, setFeed] = useState<FeedItem[]>([]);
  const [error, setError] = useState<string | null>(null);
  const [busyAction, setBusyAction] = useState<BusyAction>('initial');
  const [scenarioIndex, setScenarioIndex] = useState(0);
  const [complaintIndex, setComplaintIndex] = useState(0);

  const highestTariff = batch?.tariffs[0] ?? null;
  const lowestTariff = batch ? batch.tariffs[batch.tariffs.length - 1] : null;
  const latestAudit = batch?.latestAudit ?? null;
  const latestDispute = batch?.latestDispute ?? null;
  const selectedCountryCode = inspection?.tariff.countryCode ?? null;
  const auditedCountryCode = latestAudit?.focusCountryCode ?? null;
  const disputedCountryCode = latestDispute?.countryCode ?? null;
  const inspectingCountryCode =
    busyAction && busyAction.startsWith('inspect:') ? busyAction.slice('inspect:'.length) : null;
  const isWorking = busyAction !== null;

  const feedSummary = useMemo(() => {
    if (feed.length === 0) {
      return 'No local mission log yet.';
    }

    return `${feed.length} recent control room updates`;
  }, [feed]);

  const appendFeedItem = useCallback((title: string, detail: string, tone: FeedTone = 'info') => {
    setFeed((current) => [
      {
        id: `${Date.now()}-${Math.random().toString(16).slice(2)}`,
        title,
        detail,
        tone,
        timestamp: new Date().toISOString(),
      },
      ...current,
    ].slice(0, 6));
  }, []);

  const handleRequestError = useCallback((message: string) => {
    setError(message);
    appendFeedItem('Action failed', message, 'error');
  }, [appendFeedItem]);

  const loadCurrentBoard = useCallback(async () => {
    setBusyAction('initial');
    setError(null);

    try {
      const nextBatch = await requestJson<TariffBatchResponse>('/api/tariffs');
      setBatch(nextBatch);
      setInspection(null);
      setAudit(null);
      setDispute(null);
      appendFeedItem(
        'Tariff board loaded',
        `${nextBatch.scenario} set the absurdity index to ${nextBatch.absurdityIndex}${nextBatch.latestAudit || nextBatch.latestDispute ? ' and restored persisted activity.' : '.'}`,
        getToneFromTariff(nextBatch.averageTariff),
      );
    } catch (requestError) {
      handleRequestError(requestError instanceof Error ? requestError.message : 'Failed to load tariffs.');
    } finally {
      setBusyAction(null);
    }
  }, [appendFeedItem, handleRequestError]);

  useEffect(() => {
    void loadCurrentBoard();
  }, [loadCurrentBoard]);

  const generateTariffs = useCallback(async () => {
    const scenario = GENERATION_SCENARIOS[scenarioIndex % GENERATION_SCENARIOS.length];
    setBusyAction('generate');
    setError(null);

    try {
      const nextBatch = await requestJson<TariffBatchResponse>('/api/tariffs/generate', {
        method: 'POST',
        body: JSON.stringify({ count: 8, scenario }),
      });

      setBatch(nextBatch);
      setInspection(null);
      setAudit(null);
      setDispute(null);
      setScenarioIndex((current) => current + 1);
      appendFeedItem(
        'Fresh decree issued',
        `${nextBatch.tariffs.length} countries were repriced for ${scenario} and stored in Postgres.`,
        getToneFromTariff(nextBatch.averageTariff),
      );
    } catch (requestError) {
      handleRequestError(requestError instanceof Error ? requestError.message : 'Failed to generate tariffs.');
    } finally {
      setBusyAction(null);
    }
  }, [appendFeedItem, handleRequestError, scenarioIndex]);

  const inspectCountry = useCallback(async (countryCode: string) => {
    setBusyAction(`inspect:${countryCode}`);
    setError(null);

    try {
      const nextInspection = await requestJson<TariffInspectionResponse>(`/api/tariffs/${countryCode}`);
      setInspection(nextInspection);
      appendFeedItem(
        `Inspected ${nextInspection.tariff.countryName}`,
        `${nextInspection.tariff.countryName} is sitting at ${formatPercent(nextInspection.tariff.tariffPercent)} with ${nextInspection.tariff.volatility} volatility.`,
        getToneFromTariff(nextInspection.tariff.tariffPercent),
      );
    } catch (requestError) {
      handleRequestError(requestError instanceof Error ? requestError.message : 'Failed to inspect tariff.');
    } finally {
      setBusyAction(null);
    }
  }, [appendFeedItem, handleRequestError]);

  const runAudit = useCallback(async () => {
    const focusCountryCode = inspection?.tariff.countryCode ?? batch?.tariffs[0]?.countryCode;

    setBusyAction('audit');
    setError(null);

    try {
      const nextAudit = await requestJson<TariffAuditResponse>('/api/tariffs/audit', {
        method: 'POST',
        body: JSON.stringify({ focusCountryCode }),
      });

      setAudit(nextAudit);
      setBatch((current) =>
        current
          ? {
              ...current,
              latestAudit: summarizeAudit(nextAudit),
            }
          : current,
      );
      appendFeedItem(
        'Emergency audit finished',
        `${nextAudit.flaggedCountries} countries were flagged during ${nextAudit.scenario}; audit ${nextAudit.auditId.slice(0, 8)} was saved to Postgres.`,
        nextAudit.flaggedCountries > 0 ? 'warning' : 'info',
      );
    } catch (requestError) {
      handleRequestError(requestError instanceof Error ? requestError.message : 'Failed to run audit.');
    } finally {
      setBusyAction(null);
    }
  }, [appendFeedItem, batch, handleRequestError, inspection?.tariff.countryCode]);

  const stageDispute = useCallback(async () => {
    const countryCode = inspection?.tariff.countryCode ?? batch?.tariffs[0]?.countryCode;
    const complaint = DISPUTE_COMPLAINTS[complaintIndex % DISPUTE_COMPLAINTS.length];

    setBusyAction('dispute');
    setError(null);

    try {
      const nextDispute = await requestJson<TradeDisputeResponse>('/api/tariffs/disputes', {
        method: 'POST',
        body: JSON.stringify({ countryCode, complaint }),
      });

      setDispute(nextDispute);
      setBatch((current) =>
        current
          ? {
              ...current,
              latestDispute: summarizeDispute(nextDispute),
            }
          : current,
      );
      setComplaintIndex((current) => current + 1);
      appendFeedItem(
        `Dispute filed against ${nextDispute.countryName}`,
        `${nextDispute.outcome} Incident ${nextDispute.disputeId.slice(0, 8)} was saved to Postgres.`,
        nextDispute.highlightAsError ? 'error' : 'warning',
      );
    } catch (requestError) {
      handleRequestError(requestError instanceof Error ? requestError.message : 'Failed to stage dispute.');
    } finally {
      setBusyAction(null);
    }
  }, [appendFeedItem, batch, complaintIndex, handleRequestError, inspection?.tariff.countryCode]);

  return (
    <div className="app-shell">
      <div className="ambient-orb orb-a" aria-hidden="true" />
      <div className="ambient-orb orb-b" aria-hidden="true" />
      <div className="ambient-orb orb-c" aria-hidden="true" />

      <div className="app-frame">
        <header className="hero">
          <section className="card hero-copy">
            <div className="hero-badge">
              <img src={aspireLogo} alt="" className="hero-badge-logo" aria-hidden="true" />
              <span>Aspire + OpenTelemetry playground</span>
            </div>

            <h1 className="hero-title">Ministry of Random Tariffs</h1>
            <p className="hero-subtitle">
              Set beautifully absurd tariffs for countries of the world, click a few funny buttons,
              and watch server-side traces, logs, metrics, and span events light up in the Aspire dashboard.
            </p>

            <div className="hero-actions">
              <button
                type="button"
                className="button button-primary"
                onClick={generateTariffs}
                disabled={isWorking}
              >
                {busyAction === 'generate' ? 'Spinning...' : 'Spin the nonsense wheel'}
              </button>
              <button
                type="button"
                className="button button-secondary"
                onClick={runAudit}
                disabled={isWorking || !batch}
              >
                {busyAction === 'audit' ? 'Auditing...' : 'Run emergency audit'}
              </button>
              <button
                type="button"
                className="button button-ghost"
                onClick={stageDispute}
                disabled={isWorking || !batch}
              >
                {busyAction === 'dispute' ? 'Disputing...' : 'Stage diplomatic incident'}
              </button>
            </div>

            <div className="action-status-strip" aria-live="polite">
              <article className="action-status-card">
                <span className="stat-label">Latest audit</span>
                {latestAudit ? (
                  <>
                    <strong>{latestAudit.flaggedCountries} countries flagged</strong>
                    <p className="action-status-copy">{latestAudit.verdict}</p>
                    <span className="status-caption">
                      {latestAudit.focusCountryName ? `Focus: ${latestAudit.focusCountryName}` : 'Whole board'} ·{' '}
                      {formatDateTime(latestAudit.createdAt)}
                    </span>
                  </>
                ) : (
                  <>
                    <strong>No audit persisted yet</strong>
                    <p className="action-status-copy">
                      Run an audit and the result will appear here immediately.
                    </p>
                  </>
                )}
              </article>

              <article
                className={`action-status-card ${latestDispute?.highlightAsError ? 'is-error' : ''}`}
              >
                <span className="stat-label">Latest incident</span>
                {latestDispute ? (
                  <>
                    <strong>{latestDispute.countryName}</strong>
                    <p className="action-status-copy">{latestDispute.outcome}</p>
                    <span className="status-caption">
                      {latestDispute.severity} · {formatDateTime(latestDispute.createdAt)}
                    </span>
                  </>
                ) : (
                  <>
                    <strong>No incident persisted yet</strong>
                    <p className="action-status-copy">
                      Stage a dispute and the affected country will light up here and on the board.
                    </p>
                  </>
                )}
              </article>
            </div>

            <p className="hero-caption">
              Best demo flow: generate a batch, inspect one country, run an audit, then file a dispute.
            </p>
          </section>

          <aside className="card hero-panel">
            <span className="eyebrow">Current decree</span>
            {batch ? (
              <>
                <h2 className="panel-title">{batch.headline}</h2>
                <div className="hero-stats">
                  <article className="stat-card">
                    <span className="stat-label">Batch</span>
                    <strong>{batch.batchId.slice(0, 8)}</strong>
                  </article>
                  <article className="stat-card">
                    <span className="stat-label">Scenario</span>
                    <strong>{batch.scenario}</strong>
                  </article>
                  <article className="stat-card">
                    <span className="stat-label">Countries</span>
                    <strong>{batch.tariffs.length}</strong>
                  </article>
                  <article className="stat-card">
                    <span className="stat-label">Absurdity index</span>
                    <strong>{batch.absurdityIndex}</strong>
                  </article>
                  <article className="stat-card">
                    <span className="stat-label">Average tariff</span>
                    <strong>{formatPercent(batch.averageTariff)}</strong>
                  </article>
                  {highestTariff && (
                    <article className="stat-card">
                      <span className="stat-label">Highest target</span>
                      <strong>{highestTariff.countryName}</strong>
                    </article>
                  )}
                  {lowestTariff && (
                    <article className="stat-card">
                      <span className="stat-label">Least doomed</span>
                      <strong>{lowestTariff.countryName}</strong>
                    </article>
                  )}
                </div>
                <p className="hero-note">Last updated {formatDateTime(batch.generatedAt)}</p>
              </>
            ) : (
              <div className="hero-loading">
                <div className="skeleton-line skeleton-line-large" />
                <div className="skeleton-grid">
                  <div className="skeleton-card" />
                  <div className="skeleton-card" />
                  <div className="skeleton-card" />
                  <div className="skeleton-card" />
                </div>
              </div>
            )}
          </aside>
        </header>

        {error && (
          <div className="error-banner" role="alert">
            <strong>Control room alert:</strong>
            <span>{error}</span>
          </div>
        )}

        <main className="dashboard">
          <section className="card board-panel">
            <div className="section-heading">
              <div>
                <span className="eyebrow">Country board</span>
                <h2 className="section-title">Freshly invented tariffs</h2>
                <p className="section-copy">
                  Each card comes from the latest persisted tariff batch in Postgres. Inspecting a country
                  triggers a dedicated server-side span, a correlated log, database work, and country-tagged metrics.
                </p>
              </div>
              {batch && (
                <div className="section-meta">
                  <span>{batch.tariffs.length} countries in play</span>
                  <span>{formatDateTime(batch.generatedAt)}</span>
                </div>
              )}
            </div>

            {busyAction === 'initial' && !batch ? (
              <div className="tariff-grid">
                {Array.from({ length: 8 }).map((_, index) => (
                  <div key={index} className="tariff-card skeleton-card" aria-hidden="true" />
                ))}
              </div>
            ) : batch ? (
              <>
                {(latestAudit || latestDispute) && (
                  <div className="board-alerts" aria-live="polite">
                    {latestAudit && (
                      <article className="board-alert">
                        <strong>Latest audit:</strong>
                        <span>
                          {latestAudit.flaggedCountries} flagged in {latestAudit.focusCountryName ?? 'the full board'}.
                        </span>
                      </article>
                    )}
                    {latestDispute && (
                      <article
                        className={`board-alert ${latestDispute.highlightAsError ? 'is-error' : 'is-warning'}`}
                      >
                        <strong>Latest incident:</strong>
                        <span>
                          {latestDispute.countryName} is now carrying the most recent diplomatic headache.
                        </span>
                      </article>
                    )}
                  </div>
                )}

                <div className="tariff-grid">
                  {batch.tariffs.map((tariff) => {
                  const tone = getToneFromTariff(tariff.tariffPercent);
                  const isSelected = selectedCountryCode === tariff.countryCode;
                  const isInspecting = inspectingCountryCode === tariff.countryCode;
                  const isAuditFocus = auditedCountryCode === tariff.countryCode;
                  const hasOpenIncident = disputedCountryCode === tariff.countryCode;

                  return (
                    <article
                      key={tariff.countryCode}
                      className={`tariff-card ${isSelected ? 'is-selected' : ''} ${isAuditFocus ? 'has-audit-focus' : ''} ${hasOpenIncident ? 'has-open-incident' : ''}`}
                      data-tone={tone}
                    >
                      <div className="tariff-card-top">
                        <span className="country-chip">{tariff.countryCode}</span>
                        <span className="risk-chip">{tariff.riskLevel}</span>
                      </div>

                      <div>
                        <h3 className="country-name">{tariff.countryName}</h3>
                        <p className="country-export">{tariff.exportSpecialty}</p>
                      </div>

                      <div className="tariff-number-group">
                        <span className="tariff-number">{formatPercent(tariff.tariffPercent)}</span>
                        <span className="tariff-band">{tariff.tariffBand}</span>
                      </div>

                      <div className="tariff-tags">
                        {isAuditFocus && <span className="status-pill status-audit">Audit focus</span>}
                        {hasOpenIncident && (
                          <span
                            className={`status-pill ${latestDispute?.highlightAsError ? 'status-incident-error' : 'status-incident'}`}
                          >
                            Incident open
                          </span>
                        )}
                        <span className="mini-tag">Absurdity {tariff.absurdityScore}</span>
                        <span className="mini-tag">{tariff.volatility}</span>
                      </div>

                      <p className="tariff-reason">{tariff.sillyReason}</p>
                      <p className="tariff-note">{tariff.ministerNote}</p>

                      <button
                        type="button"
                        className="button button-card"
                        onClick={() => void inspectCountry(tariff.countryCode)}
                        disabled={isWorking}
                      >
                        {isInspecting ? 'Inspecting...' : 'Inspect tariff trace'}
                      </button>
                    </article>
                  );
                  })}
                </div>
              </>
            ) : (
              <div className="empty-state">The ministry is waiting for its first decree.</div>
            )}
          </section>

          <div className="sidebar">
            <section className="card detail-panel">
              <span className="eyebrow">Country inspector</span>
              <h2 className="panel-title">
                {inspection ? inspection.tariff.countryName : 'Pick a country card'}
              </h2>
              {inspection ? (
                <>
                  <div className="detail-grid">
                    <div className="detail-item">
                      <span className="detail-label">Batch</span>
                      <strong>{inspection.batchId.slice(0, 8)}</strong>
                    </div>
                    <div className="detail-item">
                      <span className="detail-label">Tariff</span>
                      <strong>{formatPercent(inspection.tariff.tariffPercent)}</strong>
                    </div>
                    <div className="detail-item">
                      <span className="detail-label">Band</span>
                      <strong>{inspection.tariff.tariffBand}</strong>
                    </div>
                    <div className="detail-item">
                      <span className="detail-label">Volatility</span>
                      <strong>{inspection.tariff.volatility}</strong>
                    </div>
                    <div className="detail-item">
                      <span className="detail-label">Absurdity</span>
                      <strong>{inspection.tariff.absurdityScore}</strong>
                    </div>
                  </div>

                  <p className="panel-copy">{inspection.story}</p>
                  <ul className="hint-list">
                    <li>{inspection.traceTip}</li>
                    <li>{inspection.logHint}</li>
                    <li>{inspection.metricHint}</li>
                  </ul>
                </>
              ) : (
                <p className="placeholder-copy">
                  Inspecting a country is the easiest way to create a focused custom span and a clean, correlated log entry.
                </p>
              )}
            </section>

            <section className="card observability-panel">
              <span className="eyebrow">What to watch in Aspire</span>
              <h2 className="panel-title">Telemetry checklist</h2>
              <ul className="guide-list">
                <li>
                  <strong>Traces</strong>
                  <p>
                    Generate a batch and inspect the custom span <code>tariff.generate.batch</code> with events like
                    <code>tariff.country.calculated</code>.
                  </p>
                </li>
                <li>
                  <strong>Logs</strong>
                  <p>
                    Audits and disputes emit structured server logs with country, scenario, severity, and tariff percentage.
                  </p>
                </li>
                <li>
                  <strong>Metrics</strong>
                  <p>
                    Watch <code>tariff.batch.generated</code>, <code>tariff.audit.runs</code>, <code>tariff.percent</code>,
                    and <code>tariff.absurdity.index</code>.
                  </p>
                </li>
                <li>
                  <strong>Database activity</strong>
                  <p>
                    This version persists batches and incidents in Postgres via EF Core, so the dashboard should also
                    show Npgsql and EF metrics plus SQL trace activity.
                  </p>
                </li>
                <li>
                  <strong>Error signal</strong>
                  <p>
                    Repeated disputes against very high tariffs can produce an error-status custom span for a spicy dashboard moment.
                  </p>
                </li>
              </ul>
            </section>

            <section className="card audit-panel">
              <span className="eyebrow">Emergency audit</span>
              <h2 className="panel-title">
                {audit ? audit.verdict : 'Audit results will appear here'}
              </h2>
              {audit ? (
                <>
                  <div className="detail-grid">
                    <div className="detail-item">
                      <span className="detail-label">Audit</span>
                      <strong>{audit.auditId.slice(0, 8)}</strong>
                    </div>
                    <div className="detail-item">
                      <span className="detail-label">Focus</span>
                      <strong>{audit.focusCountryName ?? 'Whole board'}</strong>
                    </div>
                    <div className="detail-item">
                      <span className="detail-label">Scenario</span>
                      <strong>{audit.scenario}</strong>
                    </div>
                    <div className="detail-item">
                      <span className="detail-label">Flagged</span>
                      <strong>{audit.flaggedCountries}</strong>
                    </div>
                    <div className="detail-item">
                      <span className="detail-label">Average</span>
                      <strong>{formatPercent(audit.averageTariff)}</strong>
                    </div>
                    <div className="detail-item">
                      <span className="detail-label">Highest</span>
                      <strong>{audit.highestTariff.countryName}</strong>
                    </div>
                    <div className="detail-item">
                      <span className="detail-label">Created</span>
                      <strong>{formatDateTime(audit.createdAt)}</strong>
                    </div>
                  </div>

                  <ul className="hint-list">
                    {audit.findings.map((finding) => (
                      <li key={finding}>{finding}</li>
                    ))}
                  </ul>
                  <p className="panel-copy">{audit.dashboardHint}</p>
                </>
              ) : (
                <p className="placeholder-copy">
                  Run an audit to generate warning-friendly logs and extra custom span events for the current board.
                </p>
              )}
            </section>

            <section className={`card dispute-panel ${dispute?.highlightAsError ? 'is-error' : ''}`}>
              <span className="eyebrow">Diplomatic incident</span>
              <h2 className="panel-title">
                {dispute ? dispute.outcome : 'No incident staged yet'}
              </h2>
              {dispute ? (
                <>
                  <div className="dispute-meta">
                    <span className="country-chip">{dispute.countryCode}</span>
                    <span className={`severity-pill severity-${dispute.severity}`}>
                      {dispute.severity}
                    </span>
                    <span className="mini-tag">Incident {dispute.disputeId.slice(0, 8)}</span>
                  </div>
                  <p className="panel-copy">
                    <strong>{dispute.countryName}</strong> was challenged over a tariff of{' '}
                    <strong>{formatPercent(dispute.tariffPercent)}</strong>.
                  </p>
                  <p className="panel-copy">Created {formatDateTime(dispute.createdAt)}</p>
                  <p className="panel-copy">{dispute.complaint}</p>
                  <p className="panel-copy">{dispute.ruling}</p>
                  <p className="panel-copy">{dispute.dashboardHint}</p>
                </>
              ) : (
                <p className="placeholder-copy">
                  Filing a dispute is the fun button for correlated logs plus an occasional error-status span.
                </p>
              )}
            </section>

            <section className="card feed-panel">
              <div className="feed-heading">
                <div>
                  <span className="eyebrow">Control room log</span>
                  <h2 className="panel-title">Local mission timeline</h2>
                </div>
                <span className="feed-summary">{feedSummary}</span>
              </div>

              {feed.length > 0 ? (
                <ul className="feed-list">
                  {feed.map((entry) => (
                    <li key={entry.id} className="feed-item" data-tone={entry.tone}>
                      <div className="feed-item-top">
                        <strong>{entry.title}</strong>
                        <span>{formatTime(entry.timestamp)}</span>
                      </div>
                      <p>{entry.detail}</p>
                    </li>
                  ))}
                </ul>
              ) : (
                <p className="placeholder-copy">Your clicks will leave a readable local timeline here.</p>
              )}
            </section>
          </div>
        </main>

        <footer className="footer">
          <span>Funny tariffs, serious telemetry.</span>
          <a href="https://aspire.dev" target="_blank" rel="noreferrer">
            Learn more about Aspire
          </a>
        </footer>
      </div>
    </div>
  );
}

export default App;
