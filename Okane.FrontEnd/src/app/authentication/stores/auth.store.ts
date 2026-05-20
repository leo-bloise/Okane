import { Injectable } from "@angular/core";
import { BehaviorSubject, catchError, finalize, tap, throwError } from "rxjs";
import { AuthApi } from "../services/auth.api";
import { HttpErrorResponse } from "@angular/common/http";
import { LoginRequest } from "../services/dtos/login.request";
import { BaseError } from "../../core/errors/base.error";

@Injectable({
  providedIn: 'root'
})
export class AuthStore {
  private readonly loadingSubject = new BehaviorSubject(false);
  public readonly loading$ = this.loadingSubject.asObservable();

  constructor(private apiService: AuthApi) { }

  login(request: LoginRequest) {
    this.loadingSubject.next(true);

    return this.apiService.login(request)
      .pipe(
        tap(response => {
          console.log('Login successful:', response);
        }),
        catchError((err, _) => {
          if(err instanceof HttpErrorResponse) {
            switch(err.status) {
              case 401:
                return throwError(() => new BaseError('Invalid email or password. Please try again.'));
              default:
                return throwError(() => new BaseError('An unexpected error occurred. Please try again later.'));
            }
          }

          return throwError(() => err);
        }),
        finalize(() => this.loadingSubject.next(false))
      )
  }
}
