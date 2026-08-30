export interface WalletSummary {
  id: string;
  name: string;
}

export interface LedgerEntry {
  id: string;
  fromWallet: WalletSummary;
  toWallet: WalletSummary;
  amount: number;
  description: string | null;
  recordedAt: string;
}

export interface LedgerPage {
  items: LedgerEntry[];
  page: number;
  pageSize: number;
  totalCount: number;
}

export interface CreateTransactionRequest {
  fromWalletId: string;
  toWalletId: string;
  amount: number;
  description?: string;
}

export interface TransactionResult {
  id: string;
  fromWalletId: string;
  toWalletId: string;
  amount: number;
  description: string | null;
  recordedAt: string;
}
