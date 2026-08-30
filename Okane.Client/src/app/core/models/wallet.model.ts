export interface Wallet {
  id: string;
  name: string;
  kind: 'Standard' | 'External';
  status: 'Active' | 'Archived';
  createdAt: string;
}

export interface WalletsPage {
  items: Wallet[];
  page: number;
  pageSize: number;
  totalCount: number;
}

export interface CreateWalletRequest {
  name: string;
}
