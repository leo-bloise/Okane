import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'register' },
  {
    path: 'register',
    loadComponent: () => import('./features/auth/register/register').then((module) => module.Register),
  },
  {
    path: 'login',
    loadComponent: () => import('./features/auth/login/login').then((module) => module.Login),
  },
  {
    path: 'app',
    canActivate: [authGuard],
    loadComponent: () => import('./layout/shell/shell').then((module) => module.Shell),
    children: [
      { path: '', pathMatch: 'full', redirectTo: 'ledger' },
      {
        path: 'ledger',
        loadComponent: () =>
          import('./features/ledger/ledger-page/ledger-page').then((module) => module.LedgerPage),
      },
      {
        path: 'wallets',
        loadComponent: () =>
          import('./features/wallets/wallets-page/wallets-page').then((module) => module.WalletsPage),
      },
    ],
  },
];
