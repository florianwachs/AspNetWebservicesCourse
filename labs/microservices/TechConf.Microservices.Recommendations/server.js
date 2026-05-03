import http from 'node:http';

const port = Number(process.env.PORT || 3105);

const recommendations = [
  {
    id: 'rec-cloud-1',
    eventId: '11111111-1111-1111-1111-111111111111',
    title: 'Book the tracing workshop next',
    reason: 'Attendees who register for Cloud Native Summit often need hands-on observability practice.',
    confidence: 92,
  },
  {
    id: 'rec-cloud-2',
    eventId: '11111111-1111-1111-1111-111111111111',
    title: 'Invite platform engineers',
    reason: 'This event has sessions about runtime ownership and production diagnostics.',
    confidence: 86,
  },
  {
    id: 'rec-api-1',
    eventId: '22222222-2222-2222-2222-222222222222',
    title: 'Promote contract review slots',
    reason: 'API Craft Day has strong overlap with versioning and consumer-driven contract topics.',
    confidence: 89,
  },
  {
    id: 'rec-api-2',
    eventId: '22222222-2222-2222-2222-222222222222',
    title: 'Surface gateway examples',
    reason: 'The selected event is a good fit for BFF and API composition discussions.',
    confidence: 82,
  },
  {
    id: 'rec-arch-1',
    eventId: '33333333-3333-3333-3333-333333333333',
    title: 'Start with the modular monolith comparison',
    reason: 'Architecture Clinic attendees usually benefit from seeing the cheaper option first.',
    confidence: 94,
  },
  {
    id: 'rec-arch-2',
    eventId: '33333333-3333-3333-3333-333333333333',
    title: 'Schedule a boundary-mapping exercise',
    reason: 'This event is about clarifying service boundaries before splitting deployables.',
    confidence: 88,
  },
];

function sendJson(response, statusCode, body) {
  const json = JSON.stringify(body);

  response.writeHead(statusCode, {
    'content-type': 'application/json; charset=utf-8',
    'content-length': Buffer.byteLength(json),
  });
  response.end(json);
}

function handleRecommendations(request, response, url) {
  const eventId = url.searchParams.get('eventId');
  const result = eventId
    ? recommendations.filter((recommendation) => recommendation.eventId === eventId)
    : recommendations;

  console.log('Recommendation query served', { eventId, count: result.length });
  sendJson(response, 200, result);
}

const server = http.createServer((request, response) => {
  const url = new URL(request.url ?? '/', `http://${request.headers.host ?? 'localhost'}`);

  if (request.method === 'GET' && (url.pathname === '/health' || url.pathname === '/alive')) {
    sendJson(response, 200, { status: 'Healthy', runtime: 'node' });
    return;
  }

  if (request.method === 'GET' && url.pathname === '/recommendations') {
    handleRecommendations(request, response, url);
    return;
  }

  if (request.method === 'GET' && url.pathname === '/recommendations/count') {
    sendJson(response, 200, { count: recommendations.length });
    return;
  }

  sendJson(response, 404, { error: 'Not found' });
});

server.listen(port, () => {
  console.log(`TechConf recommendations service listening on port ${port}`);
});
