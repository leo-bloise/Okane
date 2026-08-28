export interface ApiResponse<TDetails = undefined> {
  message: string;
  status: number;
  timestamp: number;
  details?: TDetails;
}

export type ValidationErrorDetails = Record<string, string[]>;

export type ApiErrorResponse = ApiResponse<ValidationErrorDetails>;
