import { apiFetchJson } from "./http";

export type ConferenceEventDto = {
  id: number;
  slug: string;
  name: string;
  city: string;
  location: string;
  startDate: string;
  endDate: string;
  proposalDeadlineUtc: string;
};

export function getEvents(): Promise<ConferenceEventDto[]> {
  return apiFetchJson<ConferenceEventDto[]>("/api/events", { method: "GET" });
}
