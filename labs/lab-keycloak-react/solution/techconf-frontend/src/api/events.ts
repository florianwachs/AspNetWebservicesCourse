const API_BASE = "/api/events";

export interface EventDto {
  id: number;
  name: string;
  date: string;
  city: string;
  description: string | null;
}

export interface CreateEventRequest {
  name: string;
  date: string;
  city: string;
  description?: string;
}

export async function fetchEvents(): Promise<EventDto[]> {
  const response = await fetch(API_BASE);
  if (!response.ok) throw new Error("Failed to fetch events");
  return response.json();
}

export async function createEvent(event: CreateEventRequest, token: string): Promise<EventDto> {
  const response = await fetch(API_BASE, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`,
    },
    body: JSON.stringify(event),
  });
  if (!response.ok) throw new Error("Failed to create event");
  return response.json();
}

export async function deleteEvent(id: number, token: string): Promise<void> {
  const response = await fetch(`${API_BASE}/${id}`, {
    method: "DELETE",
    headers: { Authorization: `Bearer ${token}` },
  });
  if (!response.ok) throw new Error("Failed to delete event");
}
