import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";

export default defineConfig({
  plugins: [react()],
  server: {
    port: parseInt(process.env.PORT || "3000"),
    proxy: {
      "/api": {
        target: process.env.services__api__http__0 || process.env.services__api__https__0 || "http://localhost:5000",
        changeOrigin: true,
        secure: false,
      },
    },
  },
});
