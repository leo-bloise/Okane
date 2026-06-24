import { number, z } from 'zod';

export type Transaction = {
    id: number;
    amount: number;
    description: string;
    fromAccountId: number;
    toAccountId: number;
    occuredAt: string;
};

export const CreateTransactionSchema = z
    .object({
        amount: z
            .number({
                error: "Amount is required."
            })
            .refine(
                value => value !== 0,
                "Amount must be different from 0."
            ),

        description: z
            .string({
                error: "Description is required."
            })
            .min(
                10,
                "Description must have at least 10 characters."
            ),

        fromAccountId: z
            .number({
                error: "Origin account is required."
            })
            .positive(
                "Origin account is required."
            ),

        toAccountId: z
            .number({
                error: "Destination account is required."
            })
            .positive(
                "Destination account is required."
            ),

        occuredAt: z.coerce.date({
            error:
                "Occurrence date is required."
        }).refine(
                date => !Number.isNaN(date.getTime()),
                "Occurrence date must be a valid date."
            )
    })
    .refine(
        data =>
            data.fromAccountId !==
            data.toAccountId,
        {
            message:
                "Origin and destination accounts must be different.",
            path: ["fromAccountId"]
        }
    )
    .refine(
        data =>
            data.fromAccountId !==
            data.toAccountId,
        {
            message:
                "Origin and destination accounts must be different.",
            path: ["toAccountId"]
        }
    );

export type CreateTransactionFormSchema = z.input<typeof CreateTransactionSchema>;
