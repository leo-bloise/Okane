import { z } from 'zod';

export const PaginationSchema = z.object({
    page: z.number().min(0).default(0),
    pageSize: z.number().min(0).default(20)
})

export type PaginationFormSchema = z.infer<typeof PaginationSchema>