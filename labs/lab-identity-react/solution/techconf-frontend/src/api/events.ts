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

// ── Cookie mode (active) ─────────────────────────────────────────────

export async function fetchEvents(): Promise<EventDto[]> {
  const response = await fetch(API_BASE);
  if (!response.ok) throw new Error("Failed to fetch events");
  return response.json();
}

export async function createEvent(event: CreateEventRequest): Promise<EventDto> {
  const response = await fetch(API_BASE, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    credentials: "include",
    body: JSON.stringify(event),
  });
  if (!response.ok) throw new Error("Failed to create event");
  return response.json();
}

export async function deleteEvent(id: number): Promise<void> {
  const response = await fetch(`${API_BASE}/${id}`, {
    method: "DELETE",
    credentials: "include",
  });
  if (!response.ok) throw new Error("Failed to delete event");
}

// ── JWT MODE (uncomment to switch) ───────────────────────────────────
// Import getToken and pass it as Authorization header instead of using cookies.
//
// import { getToken } from "./auth";
//
// export async function fetchEvents(): Promise<EventDto[]> {
//   const response = await fetch(API_BASE);
//   if (!response.ok) throw new Error("Failed to fetch events");
//   return response.json();
// }
//
// export async function createEvent(event: CreateEventRequest): Promise<EventDto> {
//   const token = getToken();
//   const response = await fetch(API_BASE, {
//     method: "POST",
//     headers: {
//       "Content-Type": "application/json",
//       ...(token ? { Authorization: `Bearer ${token}` } : {}),
//     },
//     body: JSON.stringify(event),
//   });
//   if (!response.ok) throw new Error("Failed to create event");
//   return response.json();
// }
//
// export async function deleteEvent(id: number): Promise<void> {
//   const token = getToken();
//   const response = await fetch(`${API_BASE}/${id}`, {
//     method: "DELETE",
//     headers: token ? { Authorization: `Bearer ${token}` } : {},
//   });
//   if (!response.ok) throw new Error("Failed to delete event");
// }
