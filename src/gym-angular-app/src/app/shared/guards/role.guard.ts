import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';

export const roleGuard: CanActivateFn = (route, state) => {
  const auth = inject(AuthService);
  const router = inject(Router);

  const expectedRoles = (route.data?.['roles'] || []) as string[];
  if (expectedRoles.length === 0) return true;

  const userRole = auth.getRole();

  if (!userRole) {
    router.navigate(['/login']);
    return false;
  }

  const userRoles = Array.isArray(userRole)
    ? userRole.map(r => r.toLowerCase())
    : [userRole.toLowerCase()];

  const expectedLower = expectedRoles.map(r => r.toLowerCase());

  const isAllowed = userRoles.some(r => expectedLower.includes(r));

  if (!isAllowed) {
    router.navigate(['/login']);
    return false;
  }

  return true;
};
