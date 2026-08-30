export interface DashboardWalletReport {
  name: string;
  inFlow: number;
  outFlow: number;
  balance: number;
}

export interface DashboardReport {
  createdAt: string;
  balance: number;
  inFlow: number;
  outFlow: number;
  wallets: DashboardWalletReport[];
}
