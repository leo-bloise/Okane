import { DetailedResponse } from "../../../core/dtos/detailed.response";

export type LoginResponse = DetailedResponse<{
  token: string;
}>