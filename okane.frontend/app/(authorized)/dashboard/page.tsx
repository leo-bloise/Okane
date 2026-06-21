import CardIndicator from "@/components/custom/dashboard-view/CardIndicator";
import DashboardGraph from "@/components/custom/dashboard-view/DashboardGraph";
import { fetchFromServer } from "@/lib/api.server-component";
import { BaseResponse } from "@/lib/types/base.response";
import { DashboardReport } from "@/lib/types/dashboard-report";

async function getReportData() {
    const response = await fetchFromServer(
        `/dashboard/api`
    );

    if (!response.ok) {
        return {
            data: undefined,
            error: true
        }
    }

    return {
        data: response.json() as Promise<BaseResponse<DashboardReport>>,
        error: false
    }
}


export default async function Page() {
    const data = await getReportData();

    const response = await data.data

    const details = response!.details;

    if(!details) throw new Error('no details');

    return <div className="p-2 flex flex-col h-full">
        <section className="flex gap-x-10">
            <CardIndicator
                className="flex-1" 
                title="Balance"
                description="Difference between incoming and liabilities"
                value={details.balance}
            />
            <CardIndicator
                className="flex-1" 
                title="Incoming"
                description="How much flows in"
                value={details.incoming}
                green
            />
            <CardIndicator
                className="flex-1" 
                title="Liabilities"
                description="How much flows out"
                value={details.liabilities}
                red
            />
        </section>
        <section className="flex items-center justify-center flex-1">
            <DashboardGraph 
                report={details}
            />
        </section>
    </div>
}