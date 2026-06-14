import { getApi } from "../api";
import { Account, CreateAccountFormSchema } from "../types/accounts";
import { BaseResponse } from "../types/base.response";
import { Paged } from "../types/paged";

export const getPaginatedAccounts = async (page: number, pageSize: number) => {
    const api = await getApi();

    const response = await api.get<Paged<Account>>(`/account?page=${page}&pageSize=${pageSize}`);

    return response.data;
}

export const createAccount = async (data: CreateAccountFormSchema) => {
    const api = await getApi();

    const response = await api.post<BaseResponse<Account>>(`/account`, data);

    return response.data;
}

export const searchAccounts = async (data: string) => {
    const api = await getApi();

    const response = await api.get(`/account/search?query=${data}`);

    return response.data;
}