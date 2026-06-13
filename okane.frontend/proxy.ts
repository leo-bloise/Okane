import { NextRequest, NextResponse } from "next/server";
import { createResponse } from "./lib/types/base.response";

const PUBLIC_ROUTES = [
    "/login",
    "/register",
    "/login/api",
    "/register/api",
    "/"
];

export default function proxy(request: NextRequest) {
    const { pathname } = request.nextUrl;

    const isPublicRoute = PUBLIC_ROUTES.some(
        (route) => pathname === route || pathname.startsWith(`${route}/`)
    );

    const sessionCookie = request.cookies.get("session");

    console.log('session cookie', sessionCookie, 'pathname', pathname);

    if (isPublicRoute && !sessionCookie) {
        return NextResponse.next();
    }

    if (isPublicRoute && sessionCookie) {
        return NextResponse.redirect(request.nextUrl.origin + "/dashboard");
    }

    if (!sessionCookie) {
        return redirectToLogin(request);
    }

    return NextResponse.next();
}

function redirectToLogin(request: NextRequest) {
    const { pathname } = request.nextUrl;

    if (pathname.includes('api')) {
        return NextResponse.json(createResponse('Not authorized', 401), {
            status: 401
        });
    }

    const loginUrl = new URL("/login", request.url);

    loginUrl.searchParams.set(
        "redirect",
        request.nextUrl.pathname
    );

    return NextResponse.redirect(loginUrl);
}

export const config = {
    matcher: [
        "/((?!_next/static|_next/image|favicon.ico|.*\\..*).*)",
    ],
};