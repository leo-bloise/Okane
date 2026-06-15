import { zodToDetailsAdapter } from "@/lib/adapters/axios-to-details-adpater";
import handleServerErrors from "@/lib/service/error-handler";
import { preparePagedGet } from "@/lib/service/paginated.service";
import { createTransaction, getPaginatedTransactions } from "@/lib/service/transaction.service";
import { createResponse } from "@/lib/types/base.response";
import { CreateTransactionSchema } from "@/lib/types/transaction";
import { NextResponse } from "next/server";

export async function GET(request: Request) {
    try {
        const f = await preparePagedGet(getPaginatedTransactions, request);

        return f;
    } catch (exception: unknown) {
        return handleServerErrors(exception);
    }
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
            return NextResponse.json(createResponse('Invalid data provided', 422, zodToDetailsAdapter(error)), {
                status: 422
            });
        }

        const responseData = await createTransaction(data);

        return NextResponse.json(responseData, {
            status: 201
        });
    } catch (exception: unknown) {
        handleServerErrors(exception);
    }
}