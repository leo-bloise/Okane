import { z } from "zod";

export const LoginSchema = z.object({
    email: z.email("Email is not valid"),
    password: z.string().min(6, "Password must be at least 6 characters long"),
});

export type LoginFormSchema = z.infer<typeof LoginSchema>;