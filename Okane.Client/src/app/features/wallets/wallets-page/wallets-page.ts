import { DatePipe } from '@angular/common';
import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { Wallet } from '../../../core/models/wallet.model';
import { Wallets } from '../../../core/services/wallets';
import { CreateWalletDrawer } from '../create-wallet-drawer/create-wallet-drawer';

const PAGE_SIZE = 20;

@Component({
  imports: [DatePipe, CreateWalletDrawer],
  selector: 'app-wallets-page',
  styleUrl: './wallets-page.css',
  templateUrl: './wallets-page.html',
})
export class WalletsPage implements OnInit {
  private readonly walletsService = inject(Wallets);

  protected readonly wallets = signal<Wallet[]>([]);
  protected readonly page = signal(1);
  protected readonly pageSize = PAGE_SIZE;
  protected readonly totalCount = signal(0);
  protected readonly loading = signal(false);
  protected readonly showDrawer = signal(false);
  protected readonly showSuccessModal = signal(false);

  protected readonly totalPages = computed(() => Math.max(1, Math.ceil(this.totalCount() / this.pageSize)));

  ngOnInit(): void {
    this.loadWalletsPage(1);
  }

  protected openDrawer(): void {
    this.showDrawer.set(true);
  }

  protected onDrawerClosed(): void {
    this.showDrawer.set(false);
  }

  protected onWalletCreated(): void {
    this.showDrawer.set(false);
    this.showSuccessModal.set(true);
    this.loadWalletsPage(this.page());
  }

  protected closeSuccessModal(): void {
    this.showSuccessModal.set(false);
  }

  protected nextPage(): void {
    if (this.page() < this.totalPages()) {
      this.loadWalletsPage(this.page() + 1);
    }
  }

  protected previousPage(): void {
    if (this.page() > 1) {
      this.loadWalletsPage(this.page() - 1);
    }
  }

  private loadWalletsPage(page: number): void {
    this.loading.set(true);

    this.walletsService.getWalletsPage(page, this.pageSize).subscribe({
      next: (response) => {
        this.loading.set(false);
        this.page.set(response.details?.page ?? page);
        this.totalCount.set(response.details?.totalCount ?? 0);
        this.wallets.set(response.details?.items ?? []);
      },
      error: () => {
        this.loading.set(false);
      },
    });
  }
}
