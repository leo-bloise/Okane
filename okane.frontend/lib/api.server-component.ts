import { cookies, headers } from "next/headers";

const formatUrl = (input: string | URL | Request): string | Request => {
    switch (typeof input) {
        case 'string':
            if (!input) {
                throw new Error(`Invalid URL ${input}`);
            }
            if (input.startsWith('http://')) {
                throw new Error(`Do not specify host and protocol, start it with / only ${input.toString()}`);
            }

            if (!input.startsWith('/')) {
                throw new Error(`Start it with / only followed by the path of the URL ${input.toString()}`);
            }

            return input;
        case 'object':
            if (input instanceof URL) {
                const { pathname, search } = input;

                let url = pathname;

                if (search) {
                    url += search;
                }

                return url;
            }
            if (input instanceof Request) {
                const url = formatUrl(input.url) as string;

                return new Request({
                    ...input,
                    url
                });
            }
        default:
            throw new Error('Type not mapped');
    }
}

const rebuildRequest = (request: Request | string, h: Headers) => {
    const proto = h.get('x-forwarded-proto') ?? 'http';
    const host = h.get('host');

    if (request instanceof Request) {
        const baseUrl = `${proto}://${host}${request.url}`;

        return {
            ...request,
            url: baseUrl
        };
    }

    return `${proto}://${host}${request}`;
}

/**
 * Fetch adapter to be used in a server component
 * @param input It must have the URL or string or request with only the path, not the host and protocol
 */
export async function fetchFromServer(input: string | URL | Request, init?: RequestInit) {
    const h = await headers();

    const cookieStore = await cookies();

    const request = formatUrl(input);

    return fetch(rebuildRequest(request, h),
        {
            ...init,
            headers: {
                Cookie: cookieStore.toString()
            }
        }
    );
}