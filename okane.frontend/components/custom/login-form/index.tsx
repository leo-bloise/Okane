"use client";

import { useForm } from "react-hook-form";
import LoginFormView from "./LoginFormView";
import { zodResolver } from "@hookform/resolvers/zod";
import { LoginFormSchema, LoginSchema } from "@/lib/types/login";
import { useRouter } from 'next/navigation';
import { useState } from "react";

export default function LoginForm() {
    const router = useRouter();
    const [loading, setLoading] = useState(false);
    const {
        register,
        handleSubmit,
        formState: {
            errors
        }
    } = useForm<LoginFormSchema>({
        resolver: zodResolver(LoginSchema),
        defaultValues: {
            email: "",
            password: "",
        }
    })

    const onSubmit = handleSubmit(async (data) => {
        setLoading(true);
        const headers = new Headers()

        headers.append('Content-Type', 'application/json');
        
        try {
            const response = await fetch('/login/api', {
                method: 'POST',
                headers,
                body: JSON.stringify(data)
            });

            if(response.redirected) {
               router.push(response.url) ;
               return;
            }
        } catch(error: unknown) {
            console.error(error);
        } finally {
            setLoading(false);
        }
    });

    return <LoginFormView 
        onSubmit={onSubmit}
        register={register}
        errors={errors}
        loading={loading}
    />
}