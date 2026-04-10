import { useState } from "react";

// TODO: Task 5 — Import login, register from ../api/auth and useAuth from ../auth/AuthProvider

export function LoginForm() {
  const [isRegister, setIsRegister] = useState(false);
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);
    setLoading(true);
    try {
      // TODO: Task 5 — Call register/login and then refreshUser
      // Hint:
      // if (isRegister) {
      //   await register(email, password);
      //   await login(email, password);
      // } else {
      //   await login(email, password);
      // }
      // await refreshUser();
    } catch (err) {
      setError(err instanceof Error ? err.message : "An error occurred");
    } finally {
      setLoading(false);
    }
  };

  return (
    <form className="login-form" onSubmit={handleSubmit}>
      <h2>{isRegister ? "Register" : "Login"}</h2>
      {error && <div className="error-message">{error}</div>}
      <div className="form-group">
        <label htmlFor="email">Email</label>
        <input
          id="email"
          type="email"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
          required
        />
      </div>
      <div className="form-group">
        <label htmlFor="password">Password</label>
        <input
          id="password"
          type="password"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          required
        />
      </div>
      <button type="submit" className="btn btn-primary" disabled={loading}>
        {loading ? "Please wait..." : isRegister ? "Register" : "Login"}
      </button>
      <p className="auth-toggle">
        {isRegister ? "Already have an account?" : "Don't have an account?"}{" "}
        <button type="button" className="btn-link" onClick={() => setIsRegister(!isRegister)}>
          {isRegister ? "Login" : "Register"}
        </button>
      </p>
    </form>
  );
}
