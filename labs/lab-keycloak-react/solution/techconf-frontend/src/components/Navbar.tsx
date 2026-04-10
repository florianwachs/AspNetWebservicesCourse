import { useAuth } from "react-oidc-context";

export function Navbar() {
  const auth = useAuth();

  return (
    <nav className="navbar">
      <span className="navbar-brand">🎤 TechConf</span>
      <div className="navbar-user">
        {auth.isAuthenticated ? (
          <>
            <span className="user-info">
              👤 {auth.user?.profile.preferred_username ?? auth.user?.profile.name}
            </span>
            <button className="btn btn-outline" onClick={() => auth.signoutRedirect()}>
              Logout
            </button>
          </>
        ) : (
          <button className="btn btn-primary" onClick={() => auth.signinRedirect()}>
            Login
          </button>
        )}
      </div>
    </nav>
  );
}
