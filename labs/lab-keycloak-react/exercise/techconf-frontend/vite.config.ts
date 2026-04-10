import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";

const apiTarget = process.env.services__api__https__0 || process.env.services__api__http__0 || "https://localhost:5001";
const keycloakBaseUrl = process.env.services__keycloak__http__0 || process.env.services__keycloak__https__0 || "http://localhost:8080";
const keycloakAuthority = process.env.VITE_KEYCLOAK_AUTHORITY || `${keycloakBaseUrl}/realms/techconf`;

export default defineConfig({
  define: {
    "import.meta.env.VITE_KEYCLOAK_AUTHORITY": JSON.stringify(keycloakAuthority),
  },
  plugins: [react()],
  server: {
    port: parseInt(process.env.PORT || "3000"),
    proxy: {
      "/api": {
        target: apiTarget,
        changeOrigin: true,
        secure: false,
      },
    },
  },
});
