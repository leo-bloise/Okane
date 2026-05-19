import { HttpErrorResponse, HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { catchError, Observable, throwError } from "rxjs";

@Injectable({
    providedIn: 'root'
})
export class ErrorInterceptor implements HttpInterceptor {
    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        return next.handle(req)
            .pipe(
                catchError((error: HttpErrorResponse, _) => {
                    switch(error.status) {
                        case 500:
                            return throwError(() => new Error('An unexpected error occurred. Please try again later.', {
                                cause: `Status: ${error.status}, Message: ${error.message}, URL: ${error.url}`
                            }));
                    }
                    return throwError(() => error);
                })
            );
    } 
}