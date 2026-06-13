import { preparePagedGet } from "@/lib/service/paginated.service";
import { getPaginatedTransactions } from "@/lib/service/transaction.service";

export async function GET(request: Request) {
    const f = await preparePagedGet(getPaginatedTransactions, request);

    return f;
}