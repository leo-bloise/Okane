import { zodToDetailsAdapter } from "@/lib/adapters/axios-to-details-adpater";
import { createAccount, getPaginatedAccounts } from "@/lib/service/accounts.service";
import handleServerErrors from "@/lib/service/error-handler";
import { preparePagedGet } from "@/lib/service/paginated.service";
import { CreateAccountSchema } from "@/lib/types/accounts";
import { createResponse } from "@/lib/types/base.response";
import { parseSafe } from "@/lib/utils.server";
import { NextResponse } from "next/server";

export async function GET(request: Request) {
    try {
        const f = await preparePagedGet(getPaginatedAccounts, request);

        return f;
    } catch(exception: unknown) {
        return handleServerErrors(exception);
    }
}

export async function POST(request: Request) {
    try {
        const { 
            success,
            data,
            error
        } = await parseSafe(request, CreateAccountSchema);

        if (!success) {
            return NextResponse.json(createResponse('Invalid data provided', 422, zodToDetailsAdapter(error)), {
                status: 422
            });
        }

        const responseData = await createAccount(data);

        return NextResponse.json(responseData, {
            status: 201
        });
    } catch (exception: unknown) {
        return handleServerErrors(exception);
    }
}