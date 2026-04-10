import { createContext, useContext, useState, useEffect, useCallback } from "react";
import type { ReactNode } from "react";
import { getMe } from "../api/auth";
import type { UserInfo } from "../api/auth";

interface AuthContextType {
  user: UserInfo | null;
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

// ── Cookie mode (active) ─────────────────────────────────────────────

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<UserInfo | null>(null);
  const [isLoading, setIsLoading] = useState(true);

  const refreshUser = useCallback(async () => {
    try {
      const me = await getMe();
      setUser(me);
    } catch {
      setUser(null);
    } finally {
      setIsLoading(false);
    }
  }, []);

  const logout = useCallback(async () => {
    await fetch("/api/auth/logout", { method: "POST", credentials: "include" });
    setUser(null);
  }, []);

  useEffect(() => {
    refreshUser();
  }, [refreshUser]);

  return (
    <AuthContext.Provider
      value={{
        user,
        isAuthenticated: user !== null,
        isLoading,
        refreshUser,
        logout,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
}

// ── JWT MODE (uncomment to switch) ───────────────────────────────────
// Replace the AuthProvider above with this version.
// In JWT mode there is no server-side session, so logout just clears the token.
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
