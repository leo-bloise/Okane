import { CurrencyPipe } from '@angular/common';
import { Component, computed, input, signal } from '@angular/core';
import { ChartConfiguration, ChartData } from 'chart.js';
import { BaseChartDirective } from 'ng2-charts';
import { DashboardWalletReport } from '../../../core/models/dashboard.model';

const SERIES_IN = '#2a78d6';
const SERIES_OUT = '#eb6834';

function formatCompactCurrency(value: number): string {
  return new Intl.NumberFormat('pt-BR', {
    style: 'currency',
    currency: 'BRL',
    maximumFractionDigits: 0,
  }).format(value);
}

@Component({
  imports: [CurrencyPipe, BaseChartDirective],
  selector: 'app-wallet-flow-chart',
  styleUrl: './wallet-flow-chart.css',
  templateUrl: './wallet-flow-chart.html',
})
export class WalletFlowChart {
  readonly wallets = input.required<DashboardWalletReport[]>();

  protected readonly showTable = signal(false);

  protected readonly hasActivity = computed(() =>
    this.wallets().some((wallet) => wallet.inFlow > 0 || wallet.outFlow > 0),
  );

  protected readonly chartData = computed<ChartData<'bar'>>(() => ({
    labels: this.wallets().map((wallet) => wallet.name),
    datasets: [
      {
        label: 'In',
        data: this.wallets().map((wallet) => wallet.inFlow),
        backgroundColor: SERIES_IN,
        borderRadius: 4,
      },
      {
        label: 'Out',
        data: this.wallets().map((wallet) => wallet.outFlow),
        backgroundColor: SERIES_OUT,
        borderRadius: 4,
      },
    ],
  }));

  protected readonly chartOptions: ChartConfiguration<'bar'>['options'] = {
    responsive: true,
    maintainAspectRatio: false,
    interaction: { mode: 'index', intersect: false },
    plugins: {
      legend: { display: false },
      tooltip: {
        callbacks: {
          label: (context) => `${context.dataset.label}: ${formatCompactCurrency(context.parsed.y ?? 0)}`,
        },
      },
    },
    scales: {
      x: {
        grid: { display: false },
        ticks: { color: '#52514e', font: { family: 'Inter, system-ui, sans-serif' } },
      },
      y: {
        beginAtZero: true,
        grid: { color: '#e1e0d9' },
        ticks: {
          color: '#898781',
          font: { family: 'Inter, system-ui, sans-serif' },
          callback: (value) => formatCompactCurrency(Number(value)),
        },
      },
    },
  };

  protected toggleTable(): void {
    this.showTable.update((value) => !value);
  }
}
