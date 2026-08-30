import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { SESSION_COOKIE_NAME } from '../constants/auth.constants';

export const authGuard: CanActivateFn = () => {
  const router = inject(Router);
  const hasSession = document.cookie
    .split('; ')
    .some((entry) => entry.startsWith(`${SESSION_COOKIE_NAME}=`));

  return hasSession ? true : router.createUrlTree(['/login']);
};
