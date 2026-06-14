import { NextResponse } from "next/server";
import { IErrorHandler } from "../error-handler";
import { AxiosError } from "axios";
import { createResponse } from "@/lib/types/base.response";

export class AxiosErrorHandler implements IErrorHandler {
    handle(exception: unknown, previousResponse?: NextResponse): NextResponse {
        if (previousResponse) return previousResponse;

        const parsedException = exception as AxiosError;

        if (!parsedException.status) {
            return NextResponse.json(createResponse('Unknown error ocurred.', 500), {
                status: 500
            })
        }

        if (parsedException.status === 401) {
            return NextResponse.json(createResponse('Unauthorized', 401), {
                status: 401
            });
        }

        if (parsedException.status! > 401 && parsedException.status! < 500) {
            return NextResponse.json(parsedException.response?.data, {
                status: parsedException.status
            });
        }

        return NextResponse.json(createResponse('Unknown error ocurred.', 500), {
            status: 500
        })
    }

    hasLinkTo(exception: unknown): boolean {
        return exception instanceof AxiosError;
    }
}