import { type NextRequest, NextResponse } from 'next/server';
import { CLIENTE_COOKIE, type ClienteSessaoCookie } from '@/shared/infrastructure/cookies';
import { cookieSecureFromRequest } from '@/shared/infrastructure/cookies-options';

export async function GET(request: NextRequest) {
  const raw = request.cookies.get(CLIENTE_COOKIE)?.value;
  if (!raw) {
    return NextResponse.json(null, { status: 404 });
  }

  try {
    const sessao = JSON.parse(raw) as ClienteSessaoCookie;
    return NextResponse.json(sessao);
  } catch {
    return NextResponse.json(null, { status: 400 });
  }
}

export async function POST(request: Request) {
  const body = (await request.json()) as ClienteSessaoCookie;
  const response = NextResponse.json(body);
  response.cookies.set(CLIENTE_COOKIE, JSON.stringify(body), {
    httpOnly: true,
    sameSite: 'lax',
    path: '/',
    secure: cookieSecureFromRequest(request),
    maxAge: 60 * 60 * 24 * 30,
  });
  return response;
}

export async function DELETE(request: Request) {
  const response = NextResponse.json({ ok: true });
  response.cookies.set(CLIENTE_COOKIE, '', {
    httpOnly: true,
    sameSite: 'lax',
    path: '/',
    secure: cookieSecureFromRequest(request),
    maxAge: 0,
  });
  return response;
}
