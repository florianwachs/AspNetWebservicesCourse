import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

const grpcBaseUrl =
  process.env.services__api__https__0 ||
  process.env.services__api__http__0 ||
  'https://localhost:5001'

export default defineConfig({
  define: {
    'import.meta.env.VITE_GRPC_BASE_URL': JSON.stringify(grpcBaseUrl),
  },
  plugins: [react()],
  server: {
    port: parseInt(process.env.PORT || '5173', 10),
  },
})
