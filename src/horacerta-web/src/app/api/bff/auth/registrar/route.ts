const apiUrl = () => process.env.API_URL ?? 'http://localhost:5080';

export async function POST(request: Request) {
  const body = await request.json();

  const upstream = await fetch(`${apiUrl()}/api/auth/registrar`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(body),
  });

  const text = await upstream.text();
  return new Response(text, {
    status: upstream.status,
    headers: { 'Content-Type': 'application/json' },
  });
}
