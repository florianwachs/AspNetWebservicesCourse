import { apiFetchJson } from "./http";

export type Pagination = {
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
};

export type SpeakerSummary = {
  id: number;
  name: string;
  tagline?: string;
  company?: string;
  city?: string;
  acceptedTalkCount?: number;
  recentTalks?: string[];
};

export type SpeakerListResponse = {
  data: SpeakerSummary[];
  pagination: Pagination;
};

export type SpeakerDetail = {
  id: number;
  name: string;
  tagline: string;
  bio: string;
  company: string;
  city: string;
  websiteUrl: string | null;
  photoUrl: string | null;
  acceptedTalkCount: number;
  recentTalks: string[];
  updatedAtUtc: string;
};

export type MySpeakerProfile = {
  id: number;
  displayName: string;
  tagline: string;
  bio: string;
  company: string;
  city: string;
  email: string;
  websiteUrl: string | null;
  photoUrl: string | null;
  proposalCount: number;
  updatedAtUtc: string;
};

export type SaveSpeakerProfileRequest = {
  displayName: string;
  tagline: string;
  bio: string;
  company: string;
  city: string;
  websiteUrl: string | null;
  photoUrl: string | null;
};

export async function fetchSpeakers(
  version: 1 | 2,
  params: { q?: string; company?: string; sort?: string; page?: number },
): Promise<SpeakerListResponse> {
  const query = new URLSearchParams();
  if (params.q) query.set("q", params.q);
  if (params.company) query.set("company", params.company);
  if (params.sort) query.set("sort", params.sort);
  if (params.page) query.set("page", params.page.toString());
  query.set("pageSize", "6");

  return apiFetchJson<SpeakerListResponse>(`/api/v${version}/speakers?${query.toString()}`, { method: "GET" });
}

export function getSpeakerDetail(id: number): Promise<SpeakerDetail> {
  return apiFetchJson<SpeakerDetail>(`/api/v2/speakers/${id}`, { method: "GET" });
}

export function getMySpeakerProfile(): Promise<MySpeakerProfile> {
  return apiFetchJson<MySpeakerProfile>("/api/speaker-profile/me", { method: "GET" }, true);
}

export function upsertMySpeakerProfile(payload: SaveSpeakerProfileRequest): Promise<MySpeakerProfile> {
  return apiFetchJson<MySpeakerProfile>("/api/speaker-profile/me", {
    method: "PUT",
    body: JSON.stringify(payload),
  }, true);
}
