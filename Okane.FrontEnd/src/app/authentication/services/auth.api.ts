import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { LoginResponse } from "./dtos/login.response";
import { LoginRequest } from "./dtos/login.request";

@Injectable({
  providedIn: 'root'
})
export class AuthApi {
  constructor(private httpClient: HttpClient) { }

  login(request: LoginRequest): Observable<LoginResponse> {
    return this.httpClient.post<LoginResponse>(
      '/api/auth/login', request
    );
  }
}
