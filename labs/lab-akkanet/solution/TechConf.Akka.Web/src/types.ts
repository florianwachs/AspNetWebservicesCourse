export interface SessionSummary {
  id: string
  title: string
  capacity: number
  reservedSeats: number
  availableSeats: number
}

export interface SessionDetails extends SessionSummary {
  attendeeIds: string[]
}

export interface SessionListResponse {
  sessions: SessionSummary[]
}
