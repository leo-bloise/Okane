export type Transaction = {
    id: number;
    amount: number;
    description: string;
    fromAccountId: number;
    toAccountId: number;
    occured_at: string;
};