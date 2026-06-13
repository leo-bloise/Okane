import { cookies } from "next/headers";
import { NotAuthorizedError } from "../errors/not-authorized.errors";
import { decrypt } from "../utils.server";
import { AxiosHeaders } from "axios";

export async function getTokenFromCookies() {
    const cookieStore = await cookies();

    const sessionCookie = cookieStore.get('session');

    if (!sessionCookie) throw new NotAuthorizedError();

    const token = decrypt(sessionCookie.value);

    if(!token) {
        cookieStore.delete('session');
        throw new NotAuthorizedError();
    }

    return token;
}

export async function createHeaders() {
    const h = new AxiosHeaders();
    const token = await getTokenFromCookies();

    h.setContentType('application/json');
    h.setAuthorization(`Bearer ${token}`);

    return h;
}