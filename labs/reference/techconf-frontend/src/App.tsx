import { useEffect, useMemo, useState } from "react";
import { HubConnectionBuilder } from "@microsoft/signalr";
import "./App.css";
import { useAuth } from "./auth/AuthProvider";
import { getEvents, type ConferenceEventDto } from "./api/events";
import { getAccessToken } from "./api/http";
import {
  createProposal,
  fetchProposals,
  reviewProposal,
  submitProposal,
  type ProposalDetail,
  type ProposalListResponse,
  type ProposalStatus,
} from "./api/proposals";
import {
  fetchSpeakers,
  getMySpeakerProfile,
  getSpeakerDetail,
  upsertMySpeakerProfile,
  type MySpeakerProfile,
  type SpeakerDetail,
  type SpeakerListResponse,
} from "./api/speakers";

type BannerTone = "success" | "error" | "info";
type Tab = "directory" | "profile" | "proposals" | "review";

type Banner = {
  tone: BannerTone;
  message: string;
};

const initialRegisterForm = {
  email: "",
  password: "",
  displayName: "",
  tagline: "",
  bio: "",
  company: "",
  city: "",
  websiteUrl: "",
  photoUrl: "",
};

const initialProposalForm = {
  eventId: "",
  title: "",
  abstract: "",
  durationMinutes: "45",
  track: "API Design",
};

const initialProfileForm = {
  displayName: "",
  tagline: "",
  bio: "",
  company: "",
  city: "",
  websiteUrl: "",
  photoUrl: "",
};

export default function App() {
  const { user, isAuthenticated, isLoading, login, logout, refreshUser, register } = useAuth();
  const isOrganizer = user?.roles.includes("Organizer") ?? false;

  const [banner, setBanner] = useState<Banner | null>(null);
  const [activeTab, setActiveTab] = useState<Tab>("directory");
  const [directoryVersion, setDirectoryVersion] = useState<1 | 2>(2);
  const [directoryParams, setDirectoryParams] = useState({
    q: "",
    company: "",
    sort: "talks",
    page: 1,
  });
  const [speakerDirectory, setSpeakerDirectory] = useState<SpeakerListResponse | null>(null);
  const [selectedSpeakerId, setSelectedSpeakerId] = useState<number | null>(null);
  const [selectedSpeaker, setSelectedSpeaker] = useState<SpeakerDetail | null>(null);
  const [events, setEvents] = useState<ConferenceEventDto[]>([]);
  const [myProfile, setMyProfile] = useState<MySpeakerProfile | null>(null);
  const [profileForm, setProfileForm] = useState(initialProfileForm);
  const [proposalForm, setProposalForm] = useState(initialProposalForm);
  const [proposalQuery, setProposalQuery] = useState({
    q: "",
    status: "",
    sort: "latest",
    page: 1,
  });
  const [proposals, setProposals] = useState<ProposalListResponse | null>(null);
  const [authMode, setAuthMode] = useState<"login" | "register">("login");
  const [authForm, setAuthForm] = useState({
    email: "",
    password: "",
  });
  const [registerForm, setRegisterForm] = useState(initialRegisterForm);
  const [busyAction, setBusyAction] = useState<string | null>(null);
  const [loadingDirectory, setLoadingDirectory] = useState(true);
  const [loadingDetail, setLoadingDetail] = useState(false);
  const [loadingProfile, setLoadingProfile] = useState(false);
  const [loadingProposals, setLoadingProposals] = useState(false);

  const metrics = useMemo(() => {
    const publicSpeakers = speakerDirectory?.data.length ?? 0;
    const proposalCount = proposals?.pagination.totalCount ?? 0;
    return {
      publicSpeakers,
      proposalCount,
      events: events.length,
    };
  }, [events.length, proposals?.pagination.totalCount, speakerDirectory?.data.length]);

  useEffect(() => {
    void loadDirectory();
  }, [directoryVersion, directoryParams]);

  useEffect(() => {
    if (selectedSpeakerId === null) {
      setSelectedSpeaker(null);
      return;
    }

    void loadSpeakerDetail(selectedSpeakerId);
  }, [selectedSpeakerId]);

  useEffect(() => {
    void loadEvents();
  }, []);

  useEffect(() => {
    if (!isAuthenticated) {
      setMyProfile(null);
      setProposals(null);
      return;
    }

    void loadMyProfile();
    void loadProposals();
  }, [isAuthenticated, proposalQuery, user?.id]);

  useEffect(() => {
    if (!isAuthenticated) {
      return;
    }

    const token = getAccessToken();
    if (!token) {
      return;
    }

    const connection = new HubConnectionBuilder()
      .withUrl("/hubs/proposals", {
        accessTokenFactory: () => getAccessToken() ?? "",
      })
      .withAutomaticReconnect()
      .build();

    connection.on("ProposalReviewed", (notification: { title: string; status: string; message: string }) => {
      setBanner({
        tone: notification.status === "Accepted" ? "success" : "info",
        message: `${notification.title}: ${notification.message}`,
      });
      void loadProposals();
      void loadDirectory();
      if (selectedSpeakerId !== null) {
        void loadSpeakerDetail(selectedSpeakerId);
      }
    });

    void connection.start().catch((error: unknown) => {
      console.error("Failed to connect to proposal notifications", error);
    });

    return () => {
      void connection.stop();
    };
  }, [isAuthenticated, selectedSpeakerId]);

  async function loadDirectory() {
    try {
      setLoadingDirectory(true);
      const response = await fetchSpeakers(directoryVersion, directoryParams);
      setSpeakerDirectory(response);
      setSelectedSpeakerId((current) => current ?? response.data[0]?.id ?? null);
    } catch (error) {
      setBanner({
        tone: "error",
        message: error instanceof Error ? error.message : "Failed to load the public speaker directory.",
      });
    } finally {
      setLoadingDirectory(false);
    }
  }

  async function loadSpeakerDetail(id: number) {
    try {
      setLoadingDetail(true);
      const detail = await getSpeakerDetail(id);
      setSelectedSpeaker(detail);
    } catch (error) {
      setBanner({
        tone: "error",
        message: error instanceof Error ? error.message : "Failed to load speaker details.",
      });
    } finally {
      setLoadingDetail(false);
    }
  }

  async function loadEvents() {
    try {
      const nextEvents = await getEvents();
      setEvents(nextEvents);
      setProposalForm((current) => ({
        ...current,
        eventId: current.eventId || nextEvents[0]?.id.toString() || "",
      }));
    } catch (error) {
      setBanner({
        tone: "error",
        message: error instanceof Error ? error.message : "Failed to load conference events.",
      });
    }
  }

  async function loadMyProfile() {
    try {
      setLoadingProfile(true);
      const profile = await getMySpeakerProfile();
      setMyProfile(profile);
      setProfileForm({
        displayName: profile.displayName,
        tagline: profile.tagline,
        bio: profile.bio,
        company: profile.company,
        city: profile.city,
        websiteUrl: profile.websiteUrl ?? "",
        photoUrl: profile.photoUrl ?? "",
      });
    } catch (error) {
      setMyProfile(null);
      if (error instanceof Error && error.message.includes("404")) {
        return;
      }
      console.error("Failed to load profile", error);
    } finally {
      setLoadingProfile(false);
    }
  }

  async function loadProposals() {
    if (!isAuthenticated) {
      return;
    }

    try {
      setLoadingProposals(true);
      setProposals(await fetchProposals(proposalQuery));
    } catch (error) {
      setBanner({
        tone: "error",
        message: error instanceof Error ? error.message : "Failed to load proposals.",
      });
    } finally {
      setLoadingProposals(false);
    }
  }

  async function handleAuthSubmit(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setBusyAction("auth");

    try {
      if (authMode === "login") {
        await login(authForm.email, authForm.password);
        setBanner({ tone: "success", message: "Welcome back to the speaker portal." });
      } else {
        await register({
          email: registerForm.email,
          password: registerForm.password,
          displayName: registerForm.displayName,
          tagline: registerForm.tagline,
          bio: registerForm.bio,
          company: registerForm.company,
          city: registerForm.city,
          websiteUrl: registerForm.websiteUrl || null,
          photoUrl: registerForm.photoUrl || null,
        });
        setBanner({ tone: "success", message: "Account created and signed in." });
        setRegisterForm(initialRegisterForm);
      }

      await refreshUser();
      setAuthForm({ email: "", password: "" });
      setActiveTab("profile");
    } catch (error) {
      setBanner({
        tone: "error",
        message: error instanceof Error ? error.message : "Authentication failed.",
      });
    } finally {
      setBusyAction(null);
    }
  }

  async function handleSaveProfile(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setBusyAction("profile");

    try {
      const saved = await upsertMySpeakerProfile({
        displayName: profileForm.displayName,
        tagline: profileForm.tagline,
        bio: profileForm.bio,
        company: profileForm.company,
        city: profileForm.city,
        websiteUrl: profileForm.websiteUrl || null,
        photoUrl: profileForm.photoUrl || null,
      });
      setMyProfile(saved);
      setBanner({ tone: "success", message: "Speaker profile updated." });
      await loadDirectory();
      if (saved.id === selectedSpeakerId) {
        await loadSpeakerDetail(saved.id);
      }
    } catch (error) {
      setBanner({
        tone: "error",
        message: error instanceof Error ? error.message : "Failed to save the speaker profile.",
      });
    } finally {
      setBusyAction(null);
    }
  }

  async function handleCreateProposal(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setBusyAction("proposal");

    try {
      const created = await createProposal({
        eventId: Number(proposalForm.eventId),
        title: proposalForm.title,
        abstract: proposalForm.abstract,
        durationMinutes: Number(proposalForm.durationMinutes),
        track: proposalForm.track,
      });
      setBanner({ tone: "success", message: `Created draft "${created.title}".` });
      setProposalForm((current) => ({
        ...current,
        title: "",
        abstract: "",
      }));
      await loadProposals();
    } catch (error) {
      setBanner({
        tone: "error",
        message: error instanceof Error ? error.message : "Failed to create the proposal.",
      });
    } finally {
      setBusyAction(null);
    }
  }

  async function handleSubmitProposal(proposal: ProposalDetail | ProposalListResponse["data"][number]) {
    setBusyAction(`submit-${proposal.id}`);

    try {
      const updated = await submitProposal(proposal.id);
      setBanner({ tone: "success", message: `Submitted "${updated.title}" for review.` });
      await loadProposals();
    } catch (error) {
      setBanner({
        tone: "error",
        message: error instanceof Error ? error.message : "Failed to submit the proposal.",
      });
    } finally {
      setBusyAction(null);
    }
  }

  async function handleReviewProposal(proposalId: number, targetStatus: ProposalStatus) {
    const note = window.prompt(`Optional note for the ${targetStatus.toLowerCase()} decision:`, "");

    setBusyAction(`${targetStatus}-${proposalId}`);
    try {
      const updated = await reviewProposal(proposalId, targetStatus, note || null);
      setBanner({
        tone: targetStatus === "Accepted" ? "success" : "info",
        message: `${updated.title} marked as ${targetStatus.toLowerCase()}.`,
      });
      await loadProposals();
      await loadDirectory();
      if (selectedSpeakerId !== null) {
        await loadSpeakerDetail(selectedSpeakerId);
      }
    } catch (error) {
      setBanner({
        tone: "error",
        message: error instanceof Error ? error.message : "Failed to review the proposal.",
      });
    } finally {
      setBusyAction(null);
    }
  }

  return (
    <div className="app-shell">
      <header className="hero">
        <div>
          <p className="eyebrow">Capstone reference implementation</p>
          <h1>Speaker Submission Portal</h1>
          <p className="hero-copy">
            A final sample that combines ASP.NET Identity, JWT auth, Vertical Slice API design,
            versioned REST endpoints, PostgreSQL, HybridCache, SignalR, and a polished React SPA.
          </p>
        </div>
        <div className="hero-metrics">
          <article className="metric-card">
            <span className="metric-value">{metrics.publicSpeakers}</span>
            <span className="metric-label">Public speakers</span>
          </article>
          <article className="metric-card">
            <span className="metric-value">{metrics.proposalCount}</span>
            <span className="metric-label">{isOrganizer ? "Proposals in view" : "My proposals"}</span>
          </article>
          <article className="metric-card">
            <span className="metric-value">{metrics.events}</span>
            <span className="metric-label">Event editions</span>
          </article>
        </div>
      </header>

      {banner && (
        <div className={`banner banner-${banner.tone}`} role="status">
          {banner.message}
        </div>
      )}

      <section className="topbar">
        <div className="chip-row">
          <span className="status-pill live">{directoryVersion === 2 ? "Public API v2" : "Public API v1"}</span>
          {user?.roles.map((role) => (
            <span key={role} className="detail-badge">
              {role}
            </span>
          ))}
        </div>
        <div className="topbar-actions">
          {isAuthenticated ? (
            <>
              <div className="signed-in-copy">
                <strong>{user?.email}</strong>
                <span>{user?.claims.map((claim) => claim.value === "true" ? claim.type : `${claim.type}=${claim.value}`).join(" · ")}</span>
              </div>
              <button className="ghost-button" onClick={logout} type="button">
                Sign out
              </button>
            </>
          ) : (
            <p className="field-help">Sign in with the seeded demo users or create a new speaker account.</p>
          )}
        </div>
      </section>

      <nav className="tab-strip" aria-label="Portal navigation">
        {[
          ["directory", "Public directory"],
          ["profile", "My profile"],
          ["proposals", "Proposals"],
          ["review", "Review queue"],
        ].map(([key, label]) => (
          <button
            key={key}
            className={`tab-button ${activeTab === key ? "active" : ""}`}
            disabled={key !== "directory" && !isAuthenticated}
            onClick={() => setActiveTab(key as Tab)}
            type="button"
          >
            {label}
          </button>
        ))}
      </nav>

      <main className="workspace">
        <section className="panel panel-list">
          <div className="panel-header">
            <div>
              <p className="panel-kicker">Public REST surface</p>
              <h2>Speaker directory</h2>
            </div>
            <div className="inline-controls">
              <button
                className={`ghost-button ${directoryVersion === 1 ? "selected" : ""}`}
                onClick={() => setDirectoryVersion(1)}
                type="button"
              >
                v1
              </button>
              <button
                className={`ghost-button ${directoryVersion === 2 ? "selected" : ""}`}
                onClick={() => setDirectoryVersion(2)}
                type="button"
              >
                v2
              </button>
            </div>
          </div>

          <div className="search-bar">
            <input
              className="field-input"
              placeholder="Search name, bio, tagline..."
              value={directoryParams.q}
              onChange={(event) => setDirectoryParams((current) => ({ ...current, q: event.target.value, page: 1 }))}
            />
            <input
              className="field-input"
              placeholder="Company"
              value={directoryParams.company}
              onChange={(event) => setDirectoryParams((current) => ({ ...current, company: event.target.value, page: 1 }))}
            />
            <select
              className="field-input"
              value={directoryParams.sort}
              onChange={(event) => setDirectoryParams((current) => ({ ...current, sort: event.target.value, page: 1 }))}
            >
              <option value="talks">Most accepted talks</option>
              <option value="company">Company</option>
              <option value="name">Name</option>
            </select>
          </div>

          {loadingDirectory ? (
            <div className="empty-state"><p>Loading the public directory…</p></div>
          ) : (
            <div className="speaker-list">
              {speakerDirectory?.data.map((speaker) => (
                <button
                  key={speaker.id}
                  className={`workshop-card ${selectedSpeakerId === speaker.id ? "selected" : ""}`}
                  onClick={() => setSelectedSpeakerId(speaker.id)}
                  type="button"
                >
                  <header>
                    <div>
                      <h3>{speaker.name}</h3>
                      {"company" in speaker && <p className="workshop-meta">{speaker.company} · {speaker.city}</p>}
                    </div>
                    {"acceptedTalkCount" in speaker && <span className="status-pill live">{speaker.acceptedTalkCount} talks</span>}
                  </header>
                  {"tagline" in speaker && <p className="card-copy">{speaker.tagline}</p>}
                  {"recentTalks" in speaker && (
                    <div className="token-row">
                      {(speaker.recentTalks ?? []).slice(0, 2).map((talk) => (
                        <span key={talk} className="detail-badge">{talk}</span>
                      ))}
                    </div>
                  )}
                </button>
              ))}
            </div>
          )}

          {speakerDirectory && (
            <div className="pagination-row">
              <button
                className="ghost-button"
                disabled={speakerDirectory.pagination.page <= 1}
                onClick={() => setDirectoryParams((current) => ({ ...current, page: current.page - 1 }))}
                type="button"
              >
                Previous
              </button>
              <span className="field-help">
                Page {speakerDirectory.pagination.page} of {Math.max(speakerDirectory.pagination.totalPages, 1)}
              </span>
              <button
                className="ghost-button"
                disabled={speakerDirectory.pagination.page >= speakerDirectory.pagination.totalPages}
                onClick={() => setDirectoryParams((current) => ({ ...current, page: current.page + 1 }))}
                type="button"
              >
                Next
              </button>
            </div>
          )}
        </section>

        <section className="panel panel-detail">
          <div className="panel-header">
            <div>
              <p className="panel-kicker">Versioned detail view</p>
              <h2>Speaker details</h2>
            </div>
          </div>

          {loadingDetail ? (
            <div className="empty-state"><p>Loading speaker details…</p></div>
          ) : selectedSpeaker ? (
            <div className="detail-shell">
              <div className="profile-head">
                {selectedSpeaker.photoUrl ? (
                  <img alt={selectedSpeaker.name} className="speaker-avatar" src={selectedSpeaker.photoUrl} />
                ) : (
                  <div className="speaker-avatar avatar-fallback">{selectedSpeaker.name.slice(0, 2).toUpperCase()}</div>
                )}
                <div>
                  <h3>{selectedSpeaker.name}</h3>
                  <p className="detail-meta">{selectedSpeaker.tagline}</p>
                  <div className="token-row">
                    <span className="detail-badge">{selectedSpeaker.company}</span>
                    <span className="detail-badge">{selectedSpeaker.city}</span>
                    <span className="detail-badge">{selectedSpeaker.acceptedTalkCount} accepted</span>
                  </div>
                </div>
              </div>
              <p className="card-copy">{selectedSpeaker.bio}</p>
              {selectedSpeaker.websiteUrl && (
                <a className="link-inline" href={selectedSpeaker.websiteUrl} rel="noreferrer" target="_blank">
                  Visit speaker website
                </a>
              )}
              <div className="detail-grid">
                {selectedSpeaker.recentTalks.map((talk) => (
                  <article key={talk} className="detail-stat">
                    <strong>{talk}</strong>
                    <span>Accepted session</span>
                  </article>
                ))}
              </div>
            </div>
          ) : (
            <div className="empty-state">
              <h3>Select a speaker</h3>
              <p>Use the public directory to inspect the versioned read model and the enriched v2 details.</p>
            </div>
          )}
        </section>

        <aside className="panel panel-form">
          {isLoading ? (
            <div className="empty-state"><p>Checking authentication…</p></div>
          ) : !isAuthenticated ? (
            <form className="form-card auth-card" onSubmit={handleAuthSubmit}>
              <p className="panel-kicker">Identity + JWT</p>
              <h3>{authMode === "login" ? "Sign in" : "Register a speaker"}</h3>
              {authMode === "login" ? (
                <>
                  <label className="field-label" htmlFor="auth-email">Email</label>
                  <input
                    id="auth-email"
                    className="field-input"
                    type="email"
                    autoComplete="email"
                    value={authForm.email}
                    onChange={(event) => setAuthForm((current) => ({ ...current, email: event.target.value }))}
                    required
                  />
                  <label className="field-label" htmlFor="auth-password">Password</label>
                  <input
                    id="auth-password"
                    className="field-input"
                    type="password"
                    autoComplete="current-password"
                    value={authForm.password}
                    onChange={(event) => setAuthForm((current) => ({ ...current, password: event.target.value }))}
                    required
                  />
                </>
              ) : (
                <>
                  <label className="field-label" htmlFor="register-email">Email</label>
                  <input
                    id="register-email"
                    className="field-input"
                    type="email"
                    autoComplete="email"
                    value={registerForm.email}
                    onChange={(event) => setRegisterForm((current) => ({ ...current, email: event.target.value }))}
                    required
                  />
                  <label className="field-label" htmlFor="register-password">Password</label>
                  <input
                    id="register-password"
                    className="field-input"
                    type="password"
                    autoComplete="new-password"
                    value={registerForm.password}
                    onChange={(event) => setRegisterForm((current) => ({ ...current, password: event.target.value }))}
                    required
                  />
                  <label className="field-label" htmlFor="register-name">Display name</label>
                  <input
                    id="register-name"
                    className="field-input"
                    autoComplete="name"
                    value={registerForm.displayName}
                    onChange={(event) => setRegisterForm((current) => ({ ...current, displayName: event.target.value }))}
                    required
                  />
                  <label className="field-label" htmlFor="register-tagline">Tagline</label>
                  <input
                    id="register-tagline"
                    className="field-input"
                    value={registerForm.tagline}
                    onChange={(event) => setRegisterForm((current) => ({ ...current, tagline: event.target.value }))}
                    required
                  />
                  <label className="field-label" htmlFor="register-company">Company</label>
                  <input
                    id="register-company"
                    className="field-input"
                    value={registerForm.company}
                    onChange={(event) => setRegisterForm((current) => ({ ...current, company: event.target.value }))}
                    required
                  />
                  <label className="field-label" htmlFor="register-city">City</label>
                  <input
                    id="register-city"
                    className="field-input"
                    value={registerForm.city}
                    onChange={(event) => setRegisterForm((current) => ({ ...current, city: event.target.value }))}
                    required
                  />
                  <label className="field-label" htmlFor="register-bio">Bio</label>
                  <textarea
                    id="register-bio"
                    className="field-input field-textarea"
                    value={registerForm.bio}
                    onChange={(event) => setRegisterForm((current) => ({ ...current, bio: event.target.value }))}
                    required
                  />
                  <label className="field-label" htmlFor="register-website">Website</label>
                  <input
                    id="register-website"
                    className="field-input"
                    value={registerForm.websiteUrl}
                    onChange={(event) => setRegisterForm((current) => ({ ...current, websiteUrl: event.target.value }))}
                  />
                  <label className="field-label" htmlFor="register-photo">Photo URL</label>
                  <input
                    id="register-photo"
                    className="field-input"
                    value={registerForm.photoUrl}
                    onChange={(event) => setRegisterForm((current) => ({ ...current, photoUrl: event.target.value }))}
                  />
                </>
              )}
              <button className="primary-button" disabled={busyAction === "auth"} type="submit">
                {busyAction === "auth" ? "Working…" : authMode === "login" ? "Sign in" : "Create account"}
              </button>
              <button
                className="ghost-button"
                onClick={() => setAuthMode((current) => current === "login" ? "register" : "login")}
                type="button"
              >
                {authMode === "login" ? "Need a new speaker account?" : "Already have an account?"}
              </button>
              <div className="hint-card">
                <h3>Seeded demo users</h3>
                <ul>
                  <li><strong>organizer@techconf.dev</strong> / Organizer123!</li>
                  <li><strong>sarah.speaker@techconf.dev</strong> / Speaker123!</li>
                  <li><strong>maya.api@techconf.dev</strong> / Speaker123!</li>
                </ul>
              </div>
            </form>
          ) : activeTab === "profile" ? (
            <form className="form-card" onSubmit={handleSaveProfile}>
              <p className="panel-kicker">Protected speaker flow</p>
              <h3>{loadingProfile ? "Loading profile…" : myProfile ? "Edit my profile" : "Create my profile"}</h3>
              <label className="field-label" htmlFor="profile-display-name">Display name</label>
              <input
                id="profile-display-name"
                className="field-input"
                value={profileForm.displayName}
                onChange={(event) => setProfileForm((current) => ({ ...current, displayName: event.target.value }))}
                required
              />
              <label className="field-label" htmlFor="profile-tagline">Tagline</label>
              <input
                id="profile-tagline"
                className="field-input"
                value={profileForm.tagline}
                onChange={(event) => setProfileForm((current) => ({ ...current, tagline: event.target.value }))}
                required
              />
              <label className="field-label" htmlFor="profile-company">Company</label>
              <input
                id="profile-company"
                className="field-input"
                value={profileForm.company}
                onChange={(event) => setProfileForm((current) => ({ ...current, company: event.target.value }))}
                required
              />
              <label className="field-label" htmlFor="profile-city">City</label>
              <input
                id="profile-city"
                className="field-input"
                value={profileForm.city}
                onChange={(event) => setProfileForm((current) => ({ ...current, city: event.target.value }))}
                required
              />
              <label className="field-label" htmlFor="profile-bio">Bio</label>
              <textarea
                id="profile-bio"
                className="field-input field-textarea"
                value={profileForm.bio}
                onChange={(event) => setProfileForm((current) => ({ ...current, bio: event.target.value }))}
                required
              />
              <label className="field-label" htmlFor="profile-website">Website</label>
              <input
                id="profile-website"
                className="field-input"
                value={profileForm.websiteUrl}
                onChange={(event) => setProfileForm((current) => ({ ...current, websiteUrl: event.target.value }))}
              />
              <label className="field-label" htmlFor="profile-photo">Photo URL</label>
              <input
                id="profile-photo"
                className="field-input"
                value={profileForm.photoUrl}
                onChange={(event) => setProfileForm((current) => ({ ...current, photoUrl: event.target.value }))}
              />
              <button className="primary-button" disabled={busyAction === "profile"} type="submit">
                {busyAction === "profile" ? "Saving…" : "Save speaker profile"}
              </button>
            </form>
          ) : activeTab === "proposals" ? (
            <div className="form-card">
              <p className="panel-kicker">Speaker workflow</p>
              <h3>Create proposal draft</h3>
              <form className="stack-form" onSubmit={handleCreateProposal}>
                <label className="field-label" htmlFor="proposal-event">Event</label>
                <select
                  id="proposal-event"
                  className="field-input"
                  value={proposalForm.eventId}
                  onChange={(event) => setProposalForm((current) => ({ ...current, eventId: event.target.value }))}
                  required
                >
                  {events.map((conferenceEvent) => (
                    <option key={conferenceEvent.id} value={conferenceEvent.id}>
                      {conferenceEvent.name} · {conferenceEvent.city}
                    </option>
                  ))}
                </select>
                <label className="field-label" htmlFor="proposal-title">Title</label>
                <input
                  id="proposal-title"
                  className="field-input"
                  value={proposalForm.title}
                  onChange={(event) => setProposalForm((current) => ({ ...current, title: event.target.value }))}
                  required
                />
                <label className="field-label" htmlFor="proposal-track">Track</label>
                <input
                  id="proposal-track"
                  className="field-input"
                  value={proposalForm.track}
                  onChange={(event) => setProposalForm((current) => ({ ...current, track: event.target.value }))}
                  required
                />
                <label className="field-label" htmlFor="proposal-duration">Duration (minutes)</label>
                <input
                  id="proposal-duration"
                  className="field-input"
                  type="number"
                  min={15}
                  max={90}
                  value={proposalForm.durationMinutes}
                  onChange={(event) => setProposalForm((current) => ({ ...current, durationMinutes: event.target.value }))}
                  required
                />
                <label className="field-label" htmlFor="proposal-abstract">Abstract</label>
                <textarea
                  id="proposal-abstract"
                  className="field-input field-textarea"
                  value={proposalForm.abstract}
                  onChange={(event) => setProposalForm((current) => ({ ...current, abstract: event.target.value }))}
                  required
                />
                <button className="primary-button" disabled={busyAction === "proposal"} type="submit">
                  {busyAction === "proposal" ? "Creating…" : "Create proposal draft"}
                </button>
              </form>
            </div>
          ) : (
            <div className="hint-card">
              <h3>{isOrganizer ? "Organizer review queue" : "Review permissions"}</h3>
              <p>
                {isOrganizer
                  ? "Use the middle panel to review submitted proposals. Accepting a proposal pushes a SignalR notification and a worker message."
                  : "Only organizers with the review claim can accept or reject submitted proposals."}
              </p>
            </div>
          )}
        </aside>
      </main>

      {isAuthenticated && (
        <section className="proposal-board panel">
          <div className="panel-header">
            <div>
              <p className="panel-kicker">Authenticated API surface</p>
              <h2>{isOrganizer ? "Proposal review queue" : "My proposals"}</h2>
            </div>
            <div className="search-bar compact">
              <input
                className="field-input"
                placeholder="Search title or speaker"
                value={proposalQuery.q}
                onChange={(event) => setProposalQuery((current) => ({ ...current, q: event.target.value, page: 1 }))}
              />
              <select
                className="field-input"
                value={proposalQuery.status}
                onChange={(event) => setProposalQuery((current) => ({ ...current, status: event.target.value, page: 1 }))}
              >
                <option value="">All statuses</option>
                <option value="Draft">Draft</option>
                <option value="Submitted">Submitted</option>
                <option value="Accepted">Accepted</option>
                <option value="Rejected">Rejected</option>
              </select>
            </div>
          </div>

          {loadingProposals ? (
            <div className="empty-state"><p>Loading proposals…</p></div>
          ) : proposals && proposals.data.length > 0 ? (
            <div className="proposal-list">
              {proposals.data.map((proposal) => (
                <article className="session-card" key={proposal.id}>
                  <div className="detail-top">
                    <div>
                      <h3>{proposal.title}</h3>
                      <p className="detail-meta">
                        {proposal.track} · {proposal.durationMinutes} min · {proposal.eventName}
                      </p>
                    </div>
                    <span className={`status-pill ${proposal.status.toLowerCase()}`}>{proposal.status}</span>
                  </div>
                  <div className="detail-actions">
                    <div className="token-row">
                      <span className="detail-badge">{proposal.speakerName}</span>
                      <span className="detail-badge">Updated {new Date(proposal.updatedAtUtc).toLocaleDateString()}</span>
                    </div>
                    <div className="inline-controls">
                      {proposal.status === "Draft" && (
                        <button
                          className="secondary-button"
                          disabled={busyAction === `submit-${proposal.id}`}
                          onClick={() => void handleSubmitProposal(proposal)}
                          type="button"
                        >
                          Submit
                        </button>
                      )}
                      {isOrganizer && proposal.status === "Submitted" && (
                        <>
                          <button
                            className="primary-button"
                            disabled={busyAction === `Accepted-${proposal.id}`}
                            onClick={() => void handleReviewProposal(proposal.id, "Accepted")}
                            type="button"
                          >
                            Accept
                          </button>
                          <button
                            className="ghost-button"
                            disabled={busyAction === `Rejected-${proposal.id}`}
                            onClick={() => void handleReviewProposal(proposal.id, "Rejected")}
                            type="button"
                          >
                            Reject
                          </button>
                        </>
                      )}
                    </div>
                  </div>
                </article>
              ))}
            </div>
          ) : (
            <div className="empty-state">
              <h3>No proposals yet</h3>
              <p>Create a draft, submit it for review, and watch the organizer flow update in real time.</p>
            </div>
          )}
        </section>
      )}
    </div>
  );
}
