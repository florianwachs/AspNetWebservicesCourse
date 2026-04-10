export function Navbar() {
  // TODO: Task 5 — Use the useAuth() hook from ../auth/AuthProvider
  // Hint: const { user, isAuthenticated, logout } = useAuth();

  return (
    <nav className="navbar">
      <span className="navbar-brand">🎤 TechConf</span>
      <div className="navbar-user">
        {/* TODO: Task 5 — Show user email + roles and logout button when authenticated */}
        {/* Hint: isAuthenticated ? show user + logout : show "Not signed in" */}
        <span className="user-info">Not signed in</span>
      </div>
    </nav>
  );
}
