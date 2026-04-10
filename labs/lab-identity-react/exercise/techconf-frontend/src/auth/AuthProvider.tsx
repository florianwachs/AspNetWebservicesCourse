import { createContext, useContext, useState, useEffect, useCallback } from "react";
import type { ReactNode } from "react";

// TODO: Task 5 — Import getMe and UserInfo from ../api/auth
// Hint: import { getMe } from "../api/auth";
// import type { UserInfo } from "../api/auth";

interface AuthContextType {
  user: null; // TODO: Change to UserInfo | null
  isAuthenticated: boolean;
  isLoading: boolean;
  refreshUser: () => Promise<void>;
  logout: () => void;
}

const AuthContext = createContext<AuthContextType>({
  user: null,
  isAuthenticated: false,
  isLoading: true,
  refreshUser: async () => {},
  logout: () => {},
});

export function useAuth() {
  return useContext(AuthContext);
}

// ── Cookie mode (default) ────────────────────────────────────────────

export function AuthProvider({ children }: { children: ReactNode }) {
  // TODO: Task 5 — Implement the AuthProvider
  // 1. Add state for user (UserInfo | null) and isLoading
  // 2. Create refreshUser that calls getMe() and sets the user state
  // 3. Create logout that POSTs to /api/auth/logout and clears user state
  // 4. Call refreshUser on mount via useEffect
  // 5. Provide user, isAuthenticated, isLoading, refreshUser, logout via context

  const [isLoading] = useState(true);

  return (
    <AuthContext.Provider
      value={{
        user: null,
        isAuthenticated: false,
        isLoading,
        refreshUser: async () => {},
        logout: () => {},
      }}
    >
      {children}
    </AuthContext.Provider>
  );
}

// ── JWT MODE (alternative — uncomment to switch) ─────────────────────
// Replace the AuthProvider above with this version.
// In JWT mode there is no server-side session, so logout just clears the in-memory token.
//
// import { clearToken } from "../api/auth";  // add this import at the top
//
// export function AuthProvider({ children }: { children: ReactNode }) {
//   const [user, setUser] = useState<UserInfo | null>(null);
//   const [isLoading, setIsLoading] = useState(true);
//
//   const refreshUser = useCallback(async () => {
//     try {
//       const me = await getMe();
//       setUser(me);
//     } catch {
//       setUser(null);
//     } finally {
//       setIsLoading(false);
//     }
//   }, []);
//
//   const logout = useCallback(() => {
//     clearToken();
//     setUser(null);
//   }, []);
//
//   // In JWT mode we don't call refreshUser on mount because there is no
//   // cookie to validate — the user must log in first to obtain a token.
//   useEffect(() => {
//     setIsLoading(false);
//   }, []);
//
//   return (
//     <AuthContext.Provider
//       value={{
//         user,
//         isAuthenticated: user !== null,
//         isLoading,
//         refreshUser,
//         logout,
//       }}
//     >
//       {children}
//     </AuthContext.Provider>
//   );
// }
