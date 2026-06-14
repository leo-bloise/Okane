import { BaseResponse } from "./base.response";

export type Paged<T> = BaseResponse<{
    items: T[],
    totalPages: number,
    pageSize: number,
    pageIndex: number
}>