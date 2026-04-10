import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import App from "./App";

// TODO: Task 5 — Wrap <App /> with the AuthProvider from ./auth/AuthProvider
// Hint: Import AuthProvider and wrap: <AuthProvider><App /></AuthProvider>

createRoot(document.getElementById("root")!).render(
  <StrictMode>
    <App />
  </StrictMode>
);
