import { AxiosError } from "axios";
import { NextResponse } from "next/server";
import { createResponse } from "../types/base.response";
import { AxiosErrorHandler } from "./errors/axios-error-handler";

export interface IErrorHandler {
    handle(exception: unknown, previousResponse?: NextResponse): NextResponse;

    hasLinkTo(exception: unknown): boolean;
}

class ErrorHandlerChain {
    private _handlers: IErrorHandler[] = [];

    public register(handler: IErrorHandler) {
        this._handlers.push(handler);
    }

    public handle(exception: unknown): NextResponse {
        const handlers = this._handlers.filter(
            handler => handler.hasLinkTo(exception)
        );

        let previous: NextResponse | undefined;

        for(let i = 0; i < handlers.length; i++) {
            previous = handlers[i].handle(exception, previous);
        }

        if(previous === undefined) {
            previous = NextResponse.json(createResponse('Unknown error occured. Please, contact the administrator', 500), {
                status: 500
            })
        }

        return previous;
    }
}

function getErrorHandler(handlers: IErrorHandler[] = []): ErrorHandlerChain {
    const defaults = [
        new AxiosErrorHandler()
    ]

    const userAndDefaultsErrorHandlers = defaults.concat(handlers);

    const chain = new ErrorHandlerChain();

    userAndDefaultsErrorHandlers.forEach(h => chain.register(h));

    return chain;
}

export default function handleServerErrors(exception: unknown, handlers: IErrorHandler[] = []) {
    const chain = getErrorHandler(handlers);
    
    return chain.handle(exception);
}