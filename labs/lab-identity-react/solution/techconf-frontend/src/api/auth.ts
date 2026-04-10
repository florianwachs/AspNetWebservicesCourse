export interface UserInfo {
  id: string;
  email: string;
  roles: string[];
}

// ── Cookie mode (active) ─────────────────────────────────────────────
// Cookies are sent automatically by the browser. No token management needed.

export async function login(email: string, password: string, rememberMe: boolean = false): Promise<void> {
  const response = await fetch("/api/auth/login", {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    credentials: "include",
    body: JSON.stringify({ email, password, rememberMe }),
  });
  if (!response.ok) {
    const error = await response.json().catch(() => null);
    throw new Error(error?.detail ?? "Invalid email or password");
  }
}

export async function register(email: string, password: string): Promise<void> {
  const response = await fetch("/api/auth/register", {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    credentials: "include",
    body: JSON.stringify({ email, password }),
  });
  if (!response.ok) {
    const error = await response.json().catch(() => null);
    const messages = error?.errors
      ? Object.values(error.errors as Record<string, string[]>).flat().join(", ")
      : "Registration failed";
    throw new Error(messages);
  }
}

export async function getMe(): Promise<UserInfo> {
  const response = await fetch("/api/auth/me", { credentials: "include" });
  if (!response.ok) throw new Error("Not authenticated");
  return response.json();
}

// ── JWT MODE (uncomment to switch) ───────────────────────────────────
// To use JWT instead of cookies:
//   1. Uncomment the functions below and comment out the cookie versions above.
//   2. Switch the backend to JWT mode (see Program.cs comments).
//   3. Update AuthProvider.tsx and events.ts to use getToken() (see JWT MODE comments there).
//
// let _token: string | null = null;
//
// export function getToken(): string | null {
//   return _token;
// }
//
// export function clearToken(): void {
//   _token = null;
// }
//
// export async function login(email: string, password: string): Promise<void> {
//   const response = await fetch("/api/auth/token", {
//     method: "POST",
//     headers: { "Content-Type": "application/json" },
//     body: JSON.stringify({ email, password }),
//   });
//   if (!response.ok) {
//     const error = await response.json().catch(() => null);
//     throw new Error(error?.detail ?? "Invalid email or password");
//   }
//   const data = await response.json();
//   _token = data.accessToken;
// }
//
// export async function register(email: string, password: string): Promise<void> {
//   const response = await fetch("/api/auth/register", {
//     method: "POST",
//     headers: { "Content-Type": "application/json" },
//     body: JSON.stringify({ email, password }),
//   });
//   if (!response.ok) {
//     const error = await response.json().catch(() => null);
//     const messages = error?.errors
//       ? Object.values(error.errors as Record<string, string[]>).flat().join(", ")
//       : "Registration failed";
//     throw new Error(messages);
//   }
// }
//
// export async function getMe(): Promise<UserInfo> {
//   if (!_token) throw new Error("Not authenticated");
//   const response = await fetch("/api/auth/me", {
//     headers: { Authorization: `Bearer ${_token}` },
//   });
//   if (!response.ok) throw new Error("Not authenticated");
//   return response.json();
// }
