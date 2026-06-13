import { NotAuthorizedError } from "@/lib/errors/not-authorized.errors";
import { createAccount, getPaginatedAccounts } from "@/lib/service/accounts.service";
import { CreateAccountSchema } from "@/lib/types/accounts";
import { createResponse } from "@/lib/types/base.response";
import { AxiosError } from "axios";
import { NextResponse } from "next/server";

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

export async function GET(request: Request) {
    const url = new URL(request.url);

    const { page, pageSize } = getPageAndPageSize(url);

    try {
        const response = await getPaginatedAccounts({
            page,
            pageSize
        });

        return NextResponse.json(response, {
            status: 200
        });
    } catch (err: unknown) {
        if (err instanceof NotAuthorizedError) {
            return NextResponse.json(createResponse('Not authorized', 401), {
                status: 401
            })
        }
    }
}

export async function POST(request: Request) {
    try {
        const body = await request.json();

        const {
            success,
            data,
            error
        } = CreateAccountSchema.safeParse(body);

        if (!success) {
            return NextResponse.json(createResponse('Invalid data provided', 422, error.flatten()), {
                status: 422
            });
        }

        const responseData = await createAccount(data);
        
        return NextResponse.json(responseData, {
            status: 201
        });
    } catch (exception: unknown) {
        if(exception instanceof AxiosError) {
            switch(exception.status) {
                case 422:
                    return NextResponse.json(exception.response?.data, {
                        status: exception.response?.data.status
                    });
            }
        }
        console.error(exception);
    }
}