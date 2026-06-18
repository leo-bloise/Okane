import { getApi } from "../api"
import { BaseResponse } from "../types/base.response";
import { DashboardReport } from "../types/dashboard-report";

export type GetDashboardReportResponse = BaseResponse<DashboardReport>;

export const getDashboardReport = async () => {
    const api = await getApi();
    
    const response = await api.get<GetDashboardReportResponse>('/report/dashboardReport');

    return response.data;
}