import { api } from "../api";
import { Account, CreateAccountFormSchema } from "../types/accounts";
import { BaseResponse } from "../types/base.response";
import { createHeaders } from "./retrieve-token.service";

export type AccountResponse = BaseResponse<{
    items: Account[],
    totalPages: number,
    pageSize: number,
    pageIndex: number
}>

export const getPaginatedAccounts = async (page: number, pageSize: number) => {
    const headers = await createHeaders();

    const response = await api.get<AccountResponse>(`/account?page=${page}&pageSize=${pageSize}`, {
        headers
    });

    if(response.status != 200) {
        throw new Error(response.statusText);
    }

    return response.data;
}

export const createAccount = async (data: CreateAccountFormSchema) => {
    const headers = await createHeaders();

    const response = await api.post(`/account`, data, {
        headers
    });

    if(response.status != 201) {
        throw new Error(response.statusText);
    }

    return response.data;
}

export const searchAccounts = async (data: string) => {
    const headers = await createHeaders();

    const response = await api.get(`/account/search?query=${data}`, {
        headers
    });

    if(response.status != 200) {
        throw new Error(response.statusText);
    }

    return response.data;
}