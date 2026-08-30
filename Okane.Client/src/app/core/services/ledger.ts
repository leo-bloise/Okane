import { HttpClient } from '@angular/common/http';
import { Service, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiResponse } from '../models/api-response.model';
import { CreateTransactionRequest, LedgerPage, TransactionResult } from '../models/ledger.model';
import { WalletsPage } from '../models/wallet.model';
import { environment } from '../../../environments/environment';

@Service()
export class Ledger {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = environment.apiUrl;

  getWallets(): Observable<ApiResponse<WalletsPage>> {
    return this.http.get<ApiResponse<WalletsPage>>(`${this.apiUrl}/wallets`, {
      params: { page: 1, pageSize: 100 },
    });
  }

  getLedgerPage(page: number, pageSize: number): Observable<ApiResponse<LedgerPage>> {
    return this.http.get<ApiResponse<LedgerPage>>(`${this.apiUrl}/ledger`, {
      params: { page, pageSize },
    });
  }

  createTransaction(request: CreateTransactionRequest): Observable<ApiResponse<TransactionResult>> {
    return this.http.post<ApiResponse<TransactionResult>>(`${this.apiUrl}/transactions`, request);
  }
}
