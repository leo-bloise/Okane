import { cookies } from "next/headers";
import { api } from "../api";
import { LoginFormSchema } from "../types/login";
import { encrypt } from "../utils.server";

export default async function login(data: LoginFormSchema) {
    const response = await api.post('/auth/login', data);
    const cookieStore = await cookies();

    if (response.status != 200) throw new Error('Response was not OK');

    if (!response.data?.details?.token) throw new Error('Token was not provided');

    const token = encrypt(
        response.data?.details?.token
    );

    cookieStore.set("session", token, {
        httpOnly: true,
        secure: process.env.NODE_ENV === "production",
        sameSite: 'lax',
        path: '/',
        maxAge: 60 * 60 * 24
    });

    return;
}