const TOKEN_STORAGE_KEY = "techconf.reference.token";

export function getAccessToken(): string | null {
  return localStorage.getItem(TOKEN_STORAGE_KEY);
}

export function setAccessToken(token: string): void {
  localStorage.setItem(TOKEN_STORAGE_KEY, token);
}

export function clearAccessToken(): void {
  localStorage.removeItem(TOKEN_STORAGE_KEY);
}

export async function apiFetchJson<T>(input: string, init: RequestInit = {}, authorized = false): Promise<T> {
  const response = await fetch(input, withHeaders(init, authorized));

  if (!response.ok) {
    throw new Error(await readApiError(response));
  }

  if (response.status === 204) {
    return undefined as T;
  }

  return response.json() as Promise<T>;
}

function withHeaders(init: RequestInit, authorized: boolean): RequestInit {
  const headers = new Headers(init.headers);
  const hasJsonBody = init.body !== undefined && !(init.body instanceof FormData);

  if (hasJsonBody && !headers.has("Content-Type")) {
    headers.set("Content-Type", "application/json");
  }

  if (authorized) {
    const token = getAccessToken();
    if (token) {
      headers.set("Authorization", `Bearer ${token}`);
    }
  }

  return {
    ...init,
    headers,
  };
}

async function readApiError(response: Response): Promise<string> {
  const payload = await response.json().catch(() => null) as {
    title?: string;
    detail?: string;
    errors?: Record<string, string[]>;
  } | null;

  if (payload?.errors) {
    return Object.values(payload.errors).flat().join(" ");
  }

  return payload?.detail ?? payload?.title ?? `Request failed with status ${response.status}`;
}
