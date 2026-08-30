import { Component, inject } from '@angular/core';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { Auth } from '../../core/services/auth';

@Component({
  imports: [RouterLink, RouterLinkActive, RouterOutlet],
  selector: 'app-shell',
  styleUrl: './shell.css',
  templateUrl: './shell.html',
})
export class Shell {
  private readonly authService = inject(Auth);
  private readonly router = inject(Router);

  protected readonly menuItems = [
    { label: 'General Status', icon: 'dashboard', path: '/app/general-status' },
    { label: 'Ledger', icon: 'account_balance', path: '/app/ledger' },
    { label: 'Wallets', icon: 'account_balance_wallet', path: '/app/wallets' },
  ];

  protected logout(): void {
    this.authService.logout();
    this.router.navigateByUrl('/login');
  }
}
