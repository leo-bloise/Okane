'use client';

import { useForm } from "react-hook-form";
import RegisterFormView from "./RegisterFormView";
import { zodResolver } from "@hookform/resolvers/zod";
import { RegisterFormSchema, RegisterSchema } from "@/lib/types/register";
import { useRouter } from "next/navigation";
import { useState } from "react";

export default function RegisterForm() {
    const router = useRouter();

    const [loading, setLoading] = useState(false);

    const {
        register,
        handleSubmit,
        formState: {
            errors
        }
    } = useForm<RegisterFormSchema>({
        resolver: zodResolver(RegisterSchema),
        defaultValues: {
            email: '',
            password: '',
            confirmPassword: ''
        }
    });

    const onSubmit = handleSubmit(async (data) => {
        setLoading(true);
        try {
            const headers = new Headers()

            headers.append('Content-Type', 'application/json');

            const response = await fetch('/register/api', {
                method: 'POST',
                headers,
                body: JSON.stringify(data)
            });

            if (response.redirected) {
                router.push(response.url);
                return;
            }
        } catch(error: unknown) {
            console.error(error);
        } finally {
            setLoading(false);
        }
    })

    return <RegisterFormView
        register={register}
        errors={errors}
        onSubmit={onSubmit}
        loading={loading}
    />
}