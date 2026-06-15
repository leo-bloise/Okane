'use client';

import { useForm } from "react-hook-form";
import CreateAccountFormView from "./CreateAccountFormView";
import { CreateAccountFormSchema, CreateAccountSchema } from "@/lib/types/accounts";
import { zodResolver } from "@hookform/resolvers/zod";
import { useState } from "react";
import { useRouter } from "next/navigation";
import { BaseResponse } from "@/lib/types/base.response";

export default function CreateAccountForm() {
    const [loading, setLoading] = useState<boolean>(false);
    const router = useRouter()

    const {
        register,
        handleSubmit,
        formState: {
            errors
        },
        setError
    } = useForm<CreateAccountFormSchema>({
        resolver: zodResolver(CreateAccountSchema),
        defaultValues: {
            name: '',
            description: ''
        }
    });

    const onSubmit = handleSubmit(async (data) => {
        setLoading(true);

        const headers = new Headers()

        headers.append('Content-Type', 'application/json');

        try {
            const response = await fetch('/accounts/api', {
                method: 'POST',
                headers,
                body: JSON.stringify(data)
            });

            if (!response.ok) {
                const data = await response.json() as BaseResponse<{
                    [key: string]: string
                }>;

                Object.keys(data.details!).forEach(k => {
                    setError(k as "name" | "description", {
                        message: data.details![k],
                    });
                })
                return;
            }

            return router.replace('/accounts');
        } catch (error: unknown) {
            console.error(error);
        } finally {
            setLoading(false);
        }
    });

    return <CreateAccountFormView
        errors={errors}
        loading={loading}
        onSubmit={onSubmit}
        register={register}
    />
}