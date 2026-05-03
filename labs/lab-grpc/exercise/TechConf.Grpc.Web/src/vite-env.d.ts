/// <reference types="vite/client" />

interface ImportMetaEnv {
  readonly VITE_GRPC_BASE_URL: string
}

interface ImportMeta {
  readonly env: ImportMetaEnv
}
