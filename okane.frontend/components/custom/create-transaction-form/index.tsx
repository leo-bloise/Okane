'use client';

import { Account } from "@/lib/types/accounts";
import { CreateTransactionFormSchema, CreateTransactionSchema } from "@/lib/types/transaction";
import { zodResolver } from "@hookform/resolvers/zod";
import { useState, useCallback } from "react";
import { useForm } from "react-hook-form";
import CreateTransactionFormView from "./CreateTransactionFormView";

export default function CreateTransactionForm() {
    const [loading, setLoading] = useState<boolean>(false);

    const onSubmit = async (data: CreateTransactionFormSchema) => {
        setLoading(true);

        const headers = new Headers();

        headers.append('Content-Type', 'application/json');

        try {
            const response = await fetch('/transactions/api', {
                method: 'POST',
                headers,
                body: JSON.stringify(data)
            });

            if(!response.ok) {
                console.log(await response.json());
            }
        } catch(error: unknown) {
            console.error(error);
        } finally {
            setLoading(false);
        }
    };

    const merger = useCallback((oldItems: Account[], newItems: Account[]) => {
        const hashMap = new Map<number, Account>();
        oldItems.forEach(oi => hashMap.set(oi.id, oi));
        newItems.forEach(ni => hashMap.set(ni.id, ni));
        return Array.from(hashMap.values());
    }, []);

    const source = useCallback(async (continuationToken: string) => {
        const searchParams = new URLSearchParams(continuationToken);
        const page = searchParams.get('page') ?? '0';
        const pageSize = searchParams.get('pageSize') ?? '20';

        const response = await fetch(`/accounts/api?page=${page}&pageSize=${pageSize}`, {
            credentials: "include"
        });

        const { details } = await response.json();
        const { items, totalPages } = details;

        return {
            items: items as Account[],
            continuationToken: totalPages <= Number(page) + 1 ? '' : `?page=${Number(page) + 1}&pageSize=${pageSize}`
        };
    }, []);

    const {
        register,
        control,
        handleSubmit,
        formState: {
            errors
        },
        setValue
    } = useForm({
        resolver: zodResolver(CreateTransactionSchema),
        defaultValues: {
            amount: 0,
            description: '',
            fromAccountId: 0,
            occuredAt: new Date(),
            toAccountId: 0
        }
    });    

    return (
        <CreateTransactionFormView
            control={control}
            register={register}
            handleSubmit={handleSubmit}
            errors={errors}
            loading={loading}
            onSubmit={onSubmit}
            accountSource={source}
            merger={merger}
            setValue={setValue}
        />
    );
}
