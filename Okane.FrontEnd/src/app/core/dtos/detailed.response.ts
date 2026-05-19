import { BaseResponse } from "./base.response"

export type DetailedResponse<T> = BaseResponse & {
  details: T
};
