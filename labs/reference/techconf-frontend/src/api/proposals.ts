import { apiFetchJson } from "./http";
import type { Pagination } from "./speakers";

export type ProposalStatus = "Draft" | "Submitted" | "Accepted" | "Rejected";

export type ProposalListItem = {
  id: number;
  title: string;
  track: string;
  durationMinutes: number;
  status: ProposalStatus;
  speakerName: string;
  speakerProfileId: number;
  eventName: string;
  createdAtUtc: string;
  updatedAtUtc: string;
};

export type ProposalListResponse = {
  data: ProposalListItem[];
  pagination: Pagination;
};

export type ProposalDetail = {
  id: number;
  title: string;
  abstract: string;
  track: string;
  durationMinutes: number;
  status: ProposalStatus;
  speakerProfileId: number;
  speakerName: string;
  eventId: number;
  eventName: string;
  createdAtUtc: string;
  updatedAtUtc: string;
  submittedAtUtc: string | null;
  reviewedAtUtc: string | null;
  decisionNote: string | null;
};

export type CreateProposalPayload = {
  eventId: number;
  title: string;
  abstract: string;
  durationMinutes: number;
  track: string;
};

export async function fetchProposals(params: {
  q?: string;
  status?: string;
  sort?: string;
  page?: number;
}): Promise<ProposalListResponse> {
  const query = new URLSearchParams();
  if (params.q) query.set("q", params.q);
  if (params.status) query.set("status", params.status);
  if (params.sort) query.set("sort", params.sort);
  if (params.page) query.set("page", params.page.toString());
  query.set("pageSize", "12");

  return apiFetchJson<ProposalListResponse>(`/api/proposals?${query.toString()}`, { method: "GET" }, true);
}

export function createProposal(payload: CreateProposalPayload): Promise<ProposalDetail> {
  return apiFetchJson<ProposalDetail>("/api/proposals", {
    method: "POST",
    body: JSON.stringify(payload),
  }, true);
}

export function submitProposal(id: number): Promise<ProposalDetail> {
  return apiFetchJson<ProposalDetail>(`/api/proposals/${id}/submit`, {
    method: "POST",
  }, true);
}

export function reviewProposal(id: number, targetStatus: ProposalStatus, note: string | null): Promise<ProposalDetail> {
  const action = targetStatus === "Accepted" ? "accept" : "reject";
  return apiFetchJson<ProposalDetail>(`/api/proposals/${id}/${action}`, {
    method: "POST",
    body: JSON.stringify({ note }),
  }, true);
}
