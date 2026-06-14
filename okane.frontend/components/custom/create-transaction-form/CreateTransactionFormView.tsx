'use client';

import { Button } from "@/components/ui/button";
import { Field, FieldError, FieldLabel } from "@/components/ui/field";
import InfiniteSelect from "@/components/ui/infinite-select";
import { Input } from "@/components/ui/input";
import { Spinner } from "@/components/ui/spinner";
import { Textarea } from "@/components/ui/textarea";
import { Account } from "@/lib/types/accounts";
import { MouseEventHandler, ReactNode } from "react";
import { Control, Controller, FieldErrors, UseFormHandleSubmit, UseFormRegister } from "react-hook-form";

type FormValues = {
    amount: number;
    description: string;
    fromAccountId: number;
    toAccountId: number;
    occured_at: Date;
};

type CreateTransactionFormViewProps = {
    control: Control<FormValues>;
    register: UseFormRegister<FormValues>;
    handleSubmit: UseFormHandleSubmit<FormValues>;
    errors: FieldErrors<FormValues>;
    loading: boolean;
    onSubmit: (data: FormValues) => void;
    accountSource: (continuationToken: string) => Promise<{ items: Account[], continuationToken: string }>;
    merger: (oldItems: Account[], newItems: Account[]) => Account[];
};

export default function CreateTransactionFormView({
    control,
    register,
    handleSubmit,
    errors,
    loading,
    onSubmit,
    accountSource,
    merger
}: CreateTransactionFormViewProps) {
    const itemAdapter = (data: Account, onClick: MouseEventHandler<HTMLLIElement>, index: number, selectedIndex: number) => (
        <li 
            key={data.id}
            data-index={index}
            onClick={onClick}
            className={selectedIndex === index ? 'active' : ''}
        >
            {data.name}
        </li>
    );

    return (
        <form
            className="flex flex-col gap-y-4"
            onSubmit={handleSubmit(onSubmit)}
        >
            <Field>
                <FieldLabel>Amount</FieldLabel>
                <Input 
                    disabled={loading} 
                    type="number" 
                    placeholder="100.00" 
                    {...register('amount', { valueAsNumber: true })} 
                />
                <FieldError>{errors.amount && errors.amount.message}</FieldError>
            </Field>

            <Field>
                <FieldLabel>Description</FieldLabel>
                <Textarea 
                    disabled={loading} 
                    placeholder="Transaction related to savings account." 
                    {...register('description')} 
                />
                <FieldError>{errors.description && errors.description.message}</FieldError>
            </Field>

            <Field>
                <FieldLabel>From Account</FieldLabel>
                <Controller
                    control={control}
                    name="fromAccountId"
                    render={({ field, fieldState }) => (
                        <InfiniteSelect
                            source={accountSource}
                            merger={merger}
                            itemToLiAdapter={itemAdapter}
                            getDisplay={(acc) => acc.name}
                            placeholderText="Select origin account"
                        />
                    )}
                />
                <FieldError>{errors.fromAccountId && errors.fromAccountId.message}</FieldError>
            </Field>

            <Field>
                <FieldLabel>To Account</FieldLabel>
                <Controller
                    control={control}
                    name="toAccountId"
                    render={({ field }) => (
                        <InfiniteSelect
                            {...field}
                            source={accountSource}
                            merger={merger}
                            itemToLiAdapter={itemAdapter}
                            getDisplay={(acc) => acc.name}
                            placeholderText="Select destination account"
                        />
                    )}
                />
                <FieldError>{errors.toAccountId && errors.toAccountId.message}</FieldError>
            </Field>

            <Field>
                <FieldLabel>Occured At</FieldLabel>
                <Input 
                    type="datetime-local" 
                    disabled={loading} 
                    {...register('occured_at', { valueAsDate: true })} 
                />
                <FieldError>{errors.occured_at && errors.occured_at.message}</FieldError>
            </Field>

            <Field>
                <Button className="hover:cursor-pointer" type="submit" disabled={loading}>
                    {loading && <Spinner data-icon="inline-start" />}
                    {loading ? "Loading" : "Create"}
                </Button>
            </Field>
        </form>
    );
}
