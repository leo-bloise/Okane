import { api } from "../api";
import { BaseResponse } from "../types/base.response";
import { CreateTransactionFormSchema, Transaction } from "../types/transaction";
import { createHeaders } from "./retrieve-token.service";

export type TransactionResponse = BaseResponse<{
    items: Transaction[];
    totalPages: number;
    pageSize: number;
    pageIndex: number;
}>

export type TransactionCreateed = BaseResponse<Transaction>;

export const getPaginatedTransactions = async (page: number, pageSize: number) => {
    const headers = await createHeaders();

    const response = await api.get<TransactionResponse>(`/transaction?page=${page}&pageSize=${pageSize}`, {
        headers
    });

    if (response.status != 200) {
        throw new Error(response.statusText);
    }

    return response.data;
}

export const createTransaction = async (data: CreateTransactionFormSchema) => {
    const headers = await createHeaders();

    const response = await api.post<TransactionCreateed>(`/transaction`, data, {
        headers
    });

    if(response.status != 200) {
        throw new Error(response.statusText);
    }

    return response.data;
}