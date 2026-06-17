'use client';

import { Button } from "@/components/ui/button";
import { Field, FieldError, FieldLabel } from "@/components/ui/field";
import InfiniteSelect from "@/components/ui/infinite-select";
import { Input } from "@/components/ui/input";
import { Spinner } from "@/components/ui/spinner";
import { Textarea } from "@/components/ui/textarea";
import { Account } from "@/lib/types/accounts";
import { CreateTransactionFormSchema } from "@/lib/types/transaction";
import { MouseEventHandler } from "react";
import {
    Control,
    Controller,
    FieldErrors,
    UseFormHandleSubmit,
    UseFormRegister
} from "react-hook-form";

type CreateTransactionFormViewProps = {
    control: Control<CreateTransactionFormSchema>;
    register: UseFormRegister<CreateTransactionFormSchema>;
    handleSubmit: UseFormHandleSubmit<CreateTransactionFormSchema>;
    errors: FieldErrors<CreateTransactionFormSchema>;
    loading: boolean;
    onSubmit: (data: CreateTransactionFormSchema) => void;

    accountSource: (
        continuationToken: string
    ) => Promise<{
        items: Account[];
        continuationToken: string;
    }>;

    merger: (
        oldItems: Account[],
        newItems: Account[]
    ) => Account[];
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

    const itemAdapter = (
        data: Account,
        onClick: MouseEventHandler<HTMLLIElement>,
        index: number,
        selected: boolean
    ) => (
        <li
            key={data.id}
            data-index={index}
            onClick={onClick}
            className={selected ? "active" : ""}
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
                    {...register("amount", {
                        valueAsNumber: true
                    })}
                />

                <FieldError>
                    {errors.amount?.message}
                </FieldError>
            </Field>

            <Field>
                <FieldLabel>Description</FieldLabel>

                <Textarea
                    disabled={loading}
                    placeholder="Transaction related to savings account."
                    {...register("description")}
                />

                <FieldError>
                    {errors.description?.message}
                </FieldError>
            </Field>

            <Field>
                <FieldLabel>From Account</FieldLabel>

                <Controller
                    control={control}
                    name="fromAccountId"
                    render={({ field }) => (
                        <InfiniteSelect<Account, number>
                            value={field.value}
                            onChange={field.onChange}
                            source={accountSource}
                            merger={merger}
                            getValue={acc => acc.id}
                            getDisplay={acc => acc.name}
                            itemToLiAdapter={itemAdapter}
                            placeholderText="Select origin account"
                        />
                    )}
                />

                <FieldError>
                    {errors.fromAccountId?.message}
                </FieldError>
            </Field>

            <Field>
                <FieldLabel>To Account</FieldLabel>

                <Controller
                    control={control}
                    name="toAccountId"
                    render={({ field }) => (
                        <InfiniteSelect<Account, number>
                            value={field.value}
                            onChange={field.onChange}
                            source={accountSource}
                            merger={merger}
                            getValue={acc => acc.id}
                            getDisplay={acc => acc.name}
                            itemToLiAdapter={itemAdapter}
                            placeholderText="Select destination account"
                        />
                    )}
                />

                <FieldError>
                    {errors.toAccountId?.message}
                </FieldError>
            </Field>

            <Field>
                <FieldLabel>Occured At</FieldLabel>
                <Input
                    type="date"
                    disabled={loading}
                    {...register("occuredAt", {
                        valueAsDate: true
                    })}
                />

                <FieldError>
                    {errors.toAccountId?.message}
                </FieldError>
            </Field>

            <Field>
                <Button
                    className="hover:cursor-pointer"
                    type="submit"
                    disabled={loading}
                >
                    {loading && (
                        <Spinner data-icon="inline-start" />
                    )}

                    {loading
                        ? "Loading"
                        : "Create"}
                </Button>
            </Field>
        </form>
    );
}