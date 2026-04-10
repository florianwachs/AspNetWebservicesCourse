import { AuthProvider as OidcAuthProvider } from "react-oidc-context";

const keycloakAuthority = import.meta.env.VITE_KEYCLOAK_AUTHORITY || "http://localhost:8080/realms/techconf";
const clientId = import.meta.env.VITE_KEYCLOAK_CLIENT_ID || "techconf-frontend";

function clearOidcCallbackUrl() {
  window.history.replaceState({}, document.title, window.location.pathname);
}

const oidcConfig = {
  authority: keycloakAuthority,
  client_id: clientId,
  redirect_uri: window.location.origin,
  post_logout_redirect_uri: window.location.origin,
  scope: "openid profile email",
  onSigninCallback: clearOidcCallbackUrl,
};

export function AuthProvider({ children }: { children: React.ReactNode }) {
  return <OidcAuthProvider {...oidcConfig}>{children}</OidcAuthProvider>;
}
