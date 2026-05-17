/** Cookies Secure só em HTTPS; Docker local usa HTTP mesmo com NODE_ENV=production. */
export function cookieSecureFromRequest(request: Request): boolean {
  return new URL(request.url).protocol === 'https:';
}
