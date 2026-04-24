import type { SessionDetails, SessionListResponse } from './types'

async function request<T>(path: string, init?: RequestInit): Promise<T> {
  const headers = new Headers(init?.headers)
  headers.set('Accept', 'application/json')

  if (init?.body && !headers.has('Content-Type')) {
    headers.set('Content-Type', 'application/json')
  }

  const response = await fetch(path, {
    ...init,
    headers,
  })

  if (!response.ok) {
    throw new Error(await readErrorMessage(response))
  }

  return (await response.json()) as T
}

async function readErrorMessage(response: Response) {
  const body = await response.text()

  if (!body) {
    return `Request failed with status ${response.status}.`
  }

  try {
    const payload = JSON.parse(body) as {
      detail?: string
      message?: string
      title?: string
    }

    return payload.message ?? payload.detail ?? payload.title ?? body
  } catch {
    return body
  }
}

export function listSessions() {
  return request<SessionListResponse>('/api/sessions')
}

export function getSession(sessionId: string) {
  return request<SessionDetails>(`/api/sessions/${sessionId}`)
}

export function reserveSeat(sessionId: string, attendeeId: string) {
  return request<SessionDetails>(`/api/sessions/${sessionId}/reserve`, {
    method: 'POST',
    body: JSON.stringify({ attendeeId }),
  })
}

export function releaseSeat(sessionId: string, attendeeId: string) {
  return request<SessionDetails>(`/api/sessions/${sessionId}/release`, {
    method: 'POST',
    body: JSON.stringify({ attendeeId }),
  })
}
