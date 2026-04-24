export interface WorkshopSummary {
  id: number;
  title: string;
  city: string;
  maxAttendees: number;
  sessionCount: number;
  isPublished: boolean;
}

export interface WorkshopSession {
  id: number;
  title: string;
  speakerName: string;
  durationMinutes: number;
}

export interface WorkshopDetail {
  id: number;
  title: string;
  city: string;
  maxAttendees: number;
  isPublished: boolean;
  sessions: WorkshopSession[];
}

export interface CreateWorkshopRequest {
  title: string;
  city: string;
  maxAttendees: number;
}

export interface WorkshopCreatedResponse {
  id: number;
}

export interface AddSessionRequest {
  title: string;
  speakerName: string;
  durationMinutes: number;
}

export interface SessionCreatedResponse {
  workshopId: number;
  sessionId: number;
}

export interface PublishWorkshopResponse {
  id: number;
  title: string;
  status: string;
  publishedOnUtc: string;
}
