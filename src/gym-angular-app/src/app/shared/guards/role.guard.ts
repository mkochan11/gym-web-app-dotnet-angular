import { CanActivateFn } from '@angular/router';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

export const roleGuard: CanActivateFn = (route, state) => {
  const auth = inject(AuthService);
  const router = inject(Router);

  const expectedRole = route.data && (route.data as any).role;
  if (!expectedRole) return true;

  const role = auth.getRole();
  if (!role || role !== expectedRole) {
    router.navigate(['/login']);
    return false;
  }
  return true;
};