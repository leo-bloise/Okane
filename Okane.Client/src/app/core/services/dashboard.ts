import { HttpClient } from '@angular/common/http';
import { Service, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiResponse } from '../models/api-response.model';
import { DashboardReport } from '../models/dashboard.model';
import { environment } from '../../../environments/environment';

@Service()
export class Dashboard {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = environment.apiUrl;

  getReport(): Observable<ApiResponse<DashboardReport>> {
    return this.http.get<ApiResponse<DashboardReport>>(`${this.apiUrl}/dashboard`);
  }
}
