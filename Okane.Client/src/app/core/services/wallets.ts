import { HttpClient } from '@angular/common/http';
import { Service, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiResponse } from '../models/api-response.model';
import { CreateWalletRequest, Wallet, WalletsPage } from '../models/wallet.model';
import { environment } from '../../../environments/environment';

@Service()
export class Wallets {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = environment.apiUrl;

  getWalletsPage(page: number, pageSize: number): Observable<ApiResponse<WalletsPage>> {
    return this.http.get<ApiResponse<WalletsPage>>(`${this.apiUrl}/wallets`, {
      params: { page, pageSize },
    });
  }

  createWallet(request: CreateWalletRequest): Observable<ApiResponse<Wallet>> {
    return this.http.post<ApiResponse<Wallet>>(`${this.apiUrl}/wallets`, request);
  }
}
