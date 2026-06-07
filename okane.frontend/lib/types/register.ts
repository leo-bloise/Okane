import { z } from 'zod';

export const RegisterSchema = z.object({
    email: z.email("Email is not valid"),
    password: z.string().min(6, "Password must be at least 6 characters long"),
    confirmPassword: z.string()
}).refine(
    (data) => data.password === data.confirmPassword,
    {
        message: 'Passwords do not match',
        path: ['confirmPassword']
    }
);

export type RegisterFormSchema = z.infer<typeof RegisterSchema>;