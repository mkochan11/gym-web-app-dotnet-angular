import { CanActivateFn } from '@angular/router';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../core/api-services';

export const authGuard: CanActivateFn = (route, state) => {
  const auth = inject(AuthService);
  const router = inject(Router);

  const token = auth.getToken();
  console.log('[AuthGuard] Checking auth for:', state.url, 'Token exists:', !!token);
  if (!token) {
    console.log('[AuthGuard] No token, redirecting to login');
    router.navigate(['/login'], { queryParams: { returnUrl: state.url } });
    return false;
  }
  console.log('[AuthGuard] Token exists, allowing access');
  return true;
};