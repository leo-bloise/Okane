import handleServerErrors from "@/lib/service/error-handler"
import { getDashboardReport } from "@/lib/service/report.service";
import { NextResponse } from "next/server";

export async function GET(_: Request) {
    try {
        const response = await getDashboardReport();

        return NextResponse.json(response, {
            status: 200
        });
    } catch(error: unknown) {
        return handleServerErrors(error);
    }
}