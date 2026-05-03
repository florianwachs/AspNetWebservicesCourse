import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';

const gatewayTarget = process.env.services__gateway__https__0
  || process.env.services__gateway__http__0
  || process.env.GATEWAY_HTTPS
  || process.env.GATEWAY_HTTP
  || 'https://localhost:5001';

export default defineConfig({
  plugins: [react()],
  server: {
    proxy: {
      '/api': {
        target: gatewayTarget,
        changeOrigin: true,
        secure: false
      }
    }
  }
});
