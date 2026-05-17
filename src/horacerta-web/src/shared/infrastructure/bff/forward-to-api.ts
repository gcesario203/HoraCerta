import { TOKEN_COOKIE } from '../cookies';

const apiUrl = () => process.env.API_URL ?? 'http://localhost:5080';

import type { NextRequest } from 'next/server';

export async function forwardToApi(
  request: NextRequest,
  pathSegments: string[],
): Promise<Response> {
  const token = request.cookies.get(TOKEN_COOKIE)?.value;
  if (!token) {
    return Response.json({ mensagem: 'Não autenticado' }, { status: 401 });
  }

  const path = pathSegments.join('/');
  const url = new URL(`/api/${path}`, apiUrl());
  const incoming = new URL(request.url);
  incoming.searchParams.forEach((value, key) => {
    url.searchParams.set(key, value);
  });

  const headers = new Headers();
  headers.set('Authorization', `Bearer ${token}`);
  const contentType = request.headers.get('content-type');
  if (contentType) headers.set('Content-Type', contentType);

  const init: RequestInit = {
    method: request.method,
    headers,
  };

  if (request.method !== 'GET' && request.method !== 'HEAD') {
    init.body = await request.text();
  }

  const upstream = await fetch(url.toString(), init);
  const body = await upstream.text();

  return new Response(body, {
    status: upstream.status,
    headers: {
      'Content-Type': upstream.headers.get('content-type') ?? 'application/json',
    },
  });
}
