import { HttpClient } from '@angular/common/http';
import { Service, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiResponse } from '../models/api-response.model';
import { RegisterRequest, RegisteredUser } from '../models/auth.model';
import { environment } from '../../../environments/environment';

@Service()
export class Auth {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/auth`;

  register(request: RegisterRequest): Observable<ApiResponse<RegisteredUser>> {
    return this.http.post<ApiResponse<RegisteredUser>>(`${this.baseUrl}/register`, request);
  }
}
