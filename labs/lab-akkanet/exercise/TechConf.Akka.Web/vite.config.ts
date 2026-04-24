import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

const apiTarget =
  process.env.services__api__https__0 ??
  process.env.services__api__http__0 ??
  'https://localhost:7157'

export default defineConfig({
  plugins: [react()],
  server: {
    port: parseInt(process.env.PORT ?? '5173', 10),
    proxy: {
      '/api': {
        target: apiTarget,
        changeOrigin: true,
        secure: false,
      },
    },
  },
})
