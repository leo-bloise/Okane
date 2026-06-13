import { NextResponse } from "next/server";
import { NotAuthorizedError } from "../errors/not-authorized.errors";
import { BaseResponse, createResponse } from "../types/base.response";

const getPageAndPageSize = (url: URL) => {
    const { searchParams } = url;

    let page = searchParams.get("page");
    let pageSize = searchParams.get("pageSize");

    if (!page || Number.isNaN(page)) {
        page = '0';
    }

    if (!pageSize || Number.isNaN(pageSize)) {
        pageSize = '20';
    }

    return {
        page: Number(page),
        pageSize: Number(pageSize)
    }
}

export type PaginatedResponse<T> = BaseResponse<{
    items: T[];
    totalPages: number;
    pageSize: number;
    pageIndex: number;
}>

export const preparePagedGet = async <T>(sourceFunction: (page: number, pageSize: number) => Promise<PaginatedResponse<T>>, request: Request) => {
    const url = new URL(request.url);

    const { page, pageSize } = getPageAndPageSize(url);

    try {
        const response = await sourceFunction(page, pageSize);

        return NextResponse.json(response, {
            status: 200
        });
    } catch(err: unknown) {
        if(err instanceof NotAuthorizedError) {
            return NextResponse.json(createResponse('Not authorized', 401), {
                status: 401
            });
        }
    }
}