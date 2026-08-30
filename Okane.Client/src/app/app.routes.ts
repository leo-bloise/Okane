import { Routes } from '@angular/router';

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
];
