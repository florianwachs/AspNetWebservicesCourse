import type {
  AddSessionRequest,
  PublishWorkshopResponse,
  SessionCreatedResponse,
  WorkshopCreatedResponse,
  WorkshopDetail,
  WorkshopSummary,
  CreateWorkshopRequest,
} from '../types';

type ApiError = {
  error?: string;
};

async function readResponse<T>(response: Response): Promise<T> {
  if (!response.ok) {
    let message = `Request failed with status ${response.status}.`;

    try {
      const payload = (await response.json()) as ApiError;
      if (payload.error) {
        message = payload.error;
      }
    } catch {
      // Ignore invalid JSON and keep the generic message.
    }

    throw new Error(message);
  }

  return (await response.json()) as T;
}

export async function getWorkshops() {
  return readResponse<WorkshopSummary[]>(await fetch('/api/workshops'));
}

export async function getWorkshop(id: number) {
  return readResponse<WorkshopDetail>(await fetch(`/api/workshops/${id}`));
}

export async function createWorkshop(request: CreateWorkshopRequest) {
  return readResponse<WorkshopCreatedResponse>(
    await fetch('/api/workshops', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(request),
    }),
  );
}

export async function addSession(workshopId: number, request: AddSessionRequest) {
  return readResponse<SessionCreatedResponse>(
    await fetch(`/api/workshops/${workshopId}/sessions`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(request),
    }),
  );
}

export async function publishWorkshop(workshopId: number) {
  return readResponse<PublishWorkshopResponse>(
    await fetch(`/api/workshops/${workshopId}/publish`, {
      method: 'POST',
    }),
  );
}
