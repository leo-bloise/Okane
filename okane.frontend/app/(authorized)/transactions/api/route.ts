import { preparePagedGet } from "@/lib/service/paginated.service";
import { createTransaction, getPaginatedTransactions } from "@/lib/service/transaction.service";
import { createResponse } from "@/lib/types/base.response";
import { CreateTransactionSchema } from "@/lib/types/transaction";
import { AxiosError } from "axios";
import { NextResponse } from "next/server";

export async function GET(request: Request) {
    const f = await preparePagedGet(getPaginatedTransactions, request);

    return f;
}

export async function POST(request: Request) {
    try {
        const body = await request.json();

        const {
            success,
            data,
            error
        } = CreateTransactionSchema.safeParse(body);

        if (!success) {
            return NextResponse.json(createResponse('Invalid data provided', 422, error.flatten()), {
                status: 422
            });
        }

        const responseData = await createTransaction(data);

        return NextResponse.json(responseData, {
            status: 201
        });
    } catch (exception: unknown) {
        if (exception instanceof AxiosError) {
            switch (exception.status) {
                case 422:
                    return NextResponse.json(exception.response?.data, {
                        status: exception.response?.data.status
                    });
            }
        }
        console.error(exception);
        return NextResponse.json(exception, {
            status: 500
        })
    }
}