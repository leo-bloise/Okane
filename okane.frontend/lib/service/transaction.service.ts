import { getApi } from "../api";
import { BaseResponse } from "../types/base.response";
import { Paged } from "../types/paged";
import { CreateTransactionFormSchema, Transaction } from "../types/transaction";

export const getPaginatedTransactions = async (page: number, pageSize: number) => {
    const api = await getApi();

    const response = await api.get<Paged<Transaction>>(`/transaction?page=${page}&pageSize=${pageSize}`);

    return response.data;
}

export const createTransaction = async (data: CreateTransactionFormSchema) => {
    const api = await getApi();

    const response = await api.post<BaseResponse<Transaction>>(`/transaction`, data);

    return response.data;
}