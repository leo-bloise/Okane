import { CurrencyPipe } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { DashboardReport } from '../../../core/models/dashboard.model';
import { Dashboard } from '../../../core/services/dashboard';
import { WalletFlowChart } from '../wallet-flow-chart/wallet-flow-chart';

@Component({
  imports: [CurrencyPipe, WalletFlowChart],
  selector: 'app-general-status-page',
  styleUrl: './general-status-page.css',
  templateUrl: './general-status-page.html',
})
export class GeneralStatusPage implements OnInit {
  private readonly dashboardService = inject(Dashboard);

  protected readonly report = signal<DashboardReport | null>(null);
  protected readonly loading = signal(false);

  ngOnInit(): void {
    this.loadReport();
  }

  private loadReport(): void {
    this.loading.set(true);

    this.dashboardService.getReport().subscribe({
      next: (response) => {
        this.loading.set(false);
        this.report.set(response.details ?? null);
      },
      error: () => {
        this.loading.set(false);
      },
    });
  }
}
