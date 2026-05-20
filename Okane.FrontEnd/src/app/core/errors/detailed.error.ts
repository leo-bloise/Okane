import { BaseError } from "./base.error";

export class DetailedError extends BaseError {
    constructor(message: string, public readonly details: {
        [key: string]: string
    }) {
        super(message);
    }
}