export type BalanceByDates = {
    timepoint: number;
    value: number;
    accumulatedBalance: number;
}

export type DashboardReport = {
    balance: number;
    incoming: number;
    liabilities: number;
    balanceByDates: BalanceByDates[]
};