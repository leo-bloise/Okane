import { number, z } from 'zod';

export type Transaction = {
    id: number;
    amount: number;
    description: string;
    fromAccountId: number;
    toAccountId: number;
    occured_at: string;
};

export const CreateTransactionSchema = z.object({
    amount: z.number(),
    description: z.string(),
    fromAccountId: z.number(),
    toAccountId: z.number(),
    occured_at: z.date()
});
