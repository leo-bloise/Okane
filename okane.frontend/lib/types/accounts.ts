import { z } from 'zod';

export type Account = {
    id: number;
    name: string;
    description: string;
}

export const CreateAccountSchema = z.object({
    name: z.string().min(3),
    description: z.string().min(10)
});

export type CreateAccountFormSchema = z.infer<typeof CreateAccountSchema>;