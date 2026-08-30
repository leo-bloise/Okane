import { HttpClient } from '@angular/common/http';
import { Service, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiResponse } from '../models/api-response.model';
import { LoginRequest, LoginResult, RegisterRequest, RegisteredUser } from '../models/auth.model';
import { SESSION_COOKIE_NAME } from '../constants/auth.constants';
import { environment } from '../../../environments/environment';

@Service()
export class Auth {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/auth`;

  register(request: RegisterRequest): Observable<ApiResponse<RegisteredUser>> {
    return this.http.post<ApiResponse<RegisteredUser>>(`${this.baseUrl}/register`, request);
  }

  login(request: LoginRequest): Observable<ApiResponse<LoginResult>> {
    return this.http.post<ApiResponse<LoginResult>>(`${this.baseUrl}/login`, request);
  }

  /**
   * Logs out by clearing only the client-readable session marker cookie.
   * The HttpOnly access token cookie can't be touched from JavaScript and is
   * left as-is - this only removes the browser-side "am I logged in" signal
   * the route guard checks, matching how a 401 already logs the app out.
   */
  logout(): void {
    document.cookie = `${SESSION_COOKIE_NAME}=; expires=Thu, 01 Jan 1970 00:00:00 GMT; path=/`;
  }
}
