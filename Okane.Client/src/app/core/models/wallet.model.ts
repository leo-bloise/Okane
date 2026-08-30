export interface Wallet {
  id: string;
  name: string;
  kind: 'Standard' | 'External';
  status: 'Active' | 'Archived';
  createdAt: string;
}
