import login from '@/lib/service/login.service';
import { createResponse } from '@/lib/types/base.response';
import { LoginSchema } from '@/lib/types/login';
import { AxiosError } from 'axios';
import { NextResponse } from 'next/server';

export async function POST(request: Request) {
    const body = await request.json();

    const { success, data, error } = LoginSchema.safeParse(body);

    if (!success) return NextResponse.json(createResponse('Invalid data provided', 422, error.flatten()), {
        status: 422
    });

    try {
        await login(data);
        return NextResponse.redirect(
            new URL('/dashboard', request.url)
        );
    } catch (error: unknown) {
        if (error instanceof AxiosError) {
            switch (error.status) {
                case 401:
                    return NextResponse.json(
                        createResponse('Not Authorized', 401, undefined),
                        {
                            status: 401
                        }
                    );
            }
        }

        return NextResponse.json(
            createResponse(
                'Unknown error happened', 500, undefined
            ),
            {
                status: 500
            }
        );
    }
}