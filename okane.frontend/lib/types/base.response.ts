export type BaseResponse<T> = {
    message: string;
    details?: T;
    status: number;
    timestamp: number;
}

export function createResponse<T>(message: string, status: number, details?: T): BaseResponse<T> {
    return {
        message,
        status,
        details,
        timestamp: Date.now()
    }
}