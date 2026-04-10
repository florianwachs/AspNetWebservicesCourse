export function Navbar() {
  // TODO: Task 5 — Use the useAuth() hook from react-oidc-context to get auth state
  // Hint: const auth = useAuth();

  return (
    <nav className="navbar">
      <span className="navbar-brand">🎤 TechConf</span>
      <div className="navbar-user">
        {/* TODO: Task 5 — Show login/logout button and user name based on auth state */}
        {/* Hint: auth.isAuthenticated ? show user + logout : show login button */}
        <span className="user-info">Not connected to Keycloak</span>
      </div>
    </nav>
  );
}
