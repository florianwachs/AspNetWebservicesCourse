import { useAuth } from "../auth/AuthProvider";

export function Navbar() {
  const { user, isAuthenticated, logout } = useAuth();

  return (
    <nav className="navbar">
      <span className="navbar-brand">🎤 TechConf</span>
      <div className="navbar-user">
        {isAuthenticated ? (
          <>
            <span className="user-info">
              👤 {user?.email}
              {user?.roles && user.roles.length > 0 && (
                <span className="user-roles"> ({user.roles.join(", ")})</span>
              )}
            </span>
            <button className="btn btn-outline" onClick={logout}>
              Logout
            </button>
          </>
        ) : (
          <span className="user-info">Not signed in</span>
        )}
      </div>
    </nav>
  );
}
