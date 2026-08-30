import { CurrencyPipe, DatePipe } from '@angular/common';
import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { LedgerEntry } from '../../../core/models/ledger.model';
import { Wallet } from '../../../core/models/wallet.model';
import { Ledger } from '../../../core/services/ledger';
import { CreateTransactionDrawer } from '../create-transaction-drawer/create-transaction-drawer';

const PAGE_SIZE = 20;

@Component({
  imports: [CurrencyPipe, DatePipe, CreateTransactionDrawer],
  selector: 'app-ledger-page',
  styleUrl: './ledger-page.css',
  templateUrl: './ledger-page.html',
})
export class LedgerPage implements OnInit {
  private readonly ledgerService = inject(Ledger);

  protected readonly entries = signal<LedgerEntry[]>([]);
  protected readonly page = signal(1);
  protected readonly pageSize = PAGE_SIZE;
  protected readonly totalCount = signal(0);
  protected readonly loading = signal(false);
  protected readonly wallets = signal<Wallet[]>([]);
  protected readonly showDrawer = signal(false);
  protected readonly showSuccessModal = signal(false);

  protected readonly totalPages = computed(() => Math.max(1, Math.ceil(this.totalCount() / this.pageSize)));

  ngOnInit(): void {
    this.loadWallets();
    this.loadLedgerPage(1);
  }

  protected openDrawer(): void {
    this.showDrawer.set(true);
  }

  protected onDrawerClosed(): void {
    this.showDrawer.set(false);
  }

  protected onTransactionCreated(): void {
    this.showDrawer.set(false);
    this.showSuccessModal.set(true);
    this.loadLedgerPage(this.page());
  }

  protected closeSuccessModal(): void {
    this.showSuccessModal.set(false);
  }

  protected nextPage(): void {
    if (this.page() < this.totalPages()) {
      this.loadLedgerPage(this.page() + 1);
    }
  }

  protected previousPage(): void {
    if (this.page() > 1) {
      this.loadLedgerPage(this.page() - 1);
    }
  }

  private loadWallets(): void {
    this.ledgerService.getWallets().subscribe({
      next: (response) => this.wallets.set(response.details ?? []),
    });
  }

  private loadLedgerPage(page: number): void {
    this.loading.set(true);

    this.ledgerService.getLedgerPage(page, this.pageSize).subscribe({
      next: (response) => {
        this.loading.set(false);
        this.page.set(response.details?.page ?? page);
        this.totalCount.set(response.details?.totalCount ?? 0);
        this.entries.set(response.details?.items ?? []);
      },
      error: () => {
        this.loading.set(false);
      },
    });
  }
}
