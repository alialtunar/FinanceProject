import { NextResponse } from 'next/server';
import type { NextRequest } from 'next/server';
import { verifyJwtToken } from '@/libs/auth';

// Sabit URL'ler ve dinamik URL desenlerini kontrol etmek için fonksiyonlar
const AUTH_PAGES = ['/', '/auth/:path*'];
const ROLE_BASED_PAGES = ['/admin'];

const isAuthPage = (url: string) => {
  return AUTH_PAGES.some(page => url === page) || url.startsWith('/auth');
};

const isRoleBasedPage = (url: string) => ROLE_BASED_PAGES.some(page => url.startsWith(page));

export async function middleware(request: NextRequest) {
  const { nextUrl, cookies } = request;
  const { value: token } = cookies.get('JWTToken') ?? { value: null };

  const hasVerifiedToken = token && (await verifyJwtToken(token));
  const isAuthPageRequested = isAuthPage(nextUrl.pathname);
  const isRoleBasedPageRequested = isRoleBasedPage(nextUrl.pathname);

  // Eğer giriş sayfasına gidilmeye çalışılıyorsa
  if (isAuthPageRequested) {
    if (hasVerifiedToken) {
      // Token varsa, /user sayfasına yönlendir
      return NextResponse.redirect(new URL('/user', request.url));
    }
    // Token yoksa, giriş sayfasına devam et
    return NextResponse.next();
  }

  // Eğer rol tabanlı sayfaya gidilmeye çalışılıyorsa
  if (isRoleBasedPageRequested) {
    if (!hasVerifiedToken || !hasVerifiedToken.role) {
      // Token yoksa veya rol yoksa, giriş sayfasına yönlendir
      return NextResponse.redirect(new URL('/', request.url));
    }

    if (hasVerifiedToken.role !== 'admin') {
      // Token geçerli fakat rol admin değilse, kullanıcı sayfasına yönlendir
      return NextResponse.redirect(new URL('/user', request.url));
    }

    // Token geçerli ve rol admin ise, admin sayfasına devam et
    return NextResponse.next();
  }

  // Diğer sayfalar için
  if (!hasVerifiedToken) {
    // Token yoksa, giriş sayfasına yönlendir
    return NextResponse.redirect(new URL('/', request.url));
  }

  // Token var ve diğer sayfalar için devam et
  return NextResponse.next();
}

export const config = {
  matcher: [
    '/auth/:path*',   // /auth ve altındaki sayfalar
    '/user/:path*',   // /user ve altındaki sayfalar
    '/admin/:path*'   // /admin ve altındaki sayfalar
  ],
};
