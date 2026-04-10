import { AuthProvider as OidcAuthProvider } from "react-oidc-context";

const keycloakAuthority = import.meta.env.VITE_KEYCLOAK_AUTHORITY || "http://localhost:8080/realms/techconf";
const clientId = import.meta.env.VITE_KEYCLOAK_CLIENT_ID || "techconf-frontend";

// TODO: Task 5 — Configure the OIDC settings for Keycloak
// Hint: Create an oidcConfig object with:
//   authority: keycloakAuthority,
//   client_id: clientId,
//   redirect_uri: window.location.origin,
//   post_logout_redirect_uri: window.location.origin,
//   scope: "openid profile email",
//   onSigninCallback: () => window.history.replaceState({}, document.title, window.location.pathname),

void OidcAuthProvider;
void keycloakAuthority;
void clientId;

export function AuthProvider({ children }: { children: React.ReactNode }) {
  // TODO: Task 5 — Wrap children with OidcAuthProvider and pass oidcConfig
  // Hint: return <OidcAuthProvider {...oidcConfig}>{children}</OidcAuthProvider>;
  return <>{children}</>;
}
