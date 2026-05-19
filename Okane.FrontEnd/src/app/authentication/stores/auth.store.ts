import { Injectable } from "@angular/core";
import { BehaviorSubject, catchError, finalize, tap, throwError } from "rxjs";
import { AuthApi } from "../services/auth.api";
import { HttpErrorResponse } from "@angular/common/http";
import { LoginRequest } from "../services/dtos/login.request";

@Injectable({
  providedIn: 'root'
})
export class AuthStore {
  private readonly loadingSubject = new BehaviorSubject(false);
  public readonly loading$ = this.loadingSubject.asObservable();
  private readonly errorSubject = new BehaviorSubject<string | null>(null);
  public readonly error$ = this.errorSubject.asObservable();

  constructor(private apiService: AuthApi) { }

  login(request: LoginRequest) {
    this.loadingSubject.next(true);
    this.errorSubject.next(null);

    return this.apiService.login(request)
      .pipe(
        tap(response => {
          console.log('Login successful:', response);
        }),
        catchError((err, caught) => {
          this.errorSubject.next(err.message);

          return throwError(() => err);
        }),
        finalize(() => this.loadingSubject.next(false))
      )
  }
}
