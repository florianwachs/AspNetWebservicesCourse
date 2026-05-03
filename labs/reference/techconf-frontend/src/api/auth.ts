import { apiFetchJson, clearAccessToken, setAccessToken } from "./http";

export type UserClaim = {
  type: string;
  value: string;
};

export type CurrentUser = {
  id: string;
  email: string;
  roles: string[];
  claims: UserClaim[];
  speakerProfileId: number | null;
};

export type AuthResponse = {
  accessToken: string;
  tokenType: string;
  expiresIn: number;
  user: CurrentUser;
};

export type RegisterPayload = {
  email: string;
  password: string;
  displayName: string;
  tagline: string;
  bio: string;
  company: string;
  city: string;
  websiteUrl: string | null;
  photoUrl: string | null;
};

export async function login(email: string, password: string): Promise<AuthResponse> {
  const response = await apiFetchJson<AuthResponse>("/api/auth/token", {
    method: "POST",
    body: JSON.stringify({ email, password }),
  });

  setAccessToken(response.accessToken);
  return response;
}

export async function register(payload: RegisterPayload): Promise<AuthResponse> {
  const response = await apiFetchJson<AuthResponse>("/api/auth/register", {
    method: "POST",
    body: JSON.stringify(payload),
  });

  setAccessToken(response.accessToken);
  return response;
}

export async function getMe(): Promise<CurrentUser> {
  return apiFetchJson<CurrentUser>("/api/auth/me", { method: "GET" }, true);
}

export function logout(): void {
  clearAccessToken();
}
