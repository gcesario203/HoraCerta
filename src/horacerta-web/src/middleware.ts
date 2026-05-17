import { NextResponse } from 'next/server';
import type { NextRequest } from 'next/server';

const TOKEN_COOKIE = 'horacerta_token';

export function middleware(request: NextRequest) {
  if (!request.nextUrl.pathname.startsWith('/proprietario')) {
    return NextResponse.next();
  }

  const token = request.cookies.get(TOKEN_COOKIE)?.value;
  if (!token) {
    const login = new URL('/login', request.url);
    login.searchParams.set('redirect', request.nextUrl.pathname);
    return NextResponse.redirect(login);
  }

  return NextResponse.next();
}

export const config = {
  matcher: ['/proprietario/:path*'],
};
