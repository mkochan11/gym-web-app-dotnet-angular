import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { AuthService } from '../../core/api-services';

export const roleGuard: CanActivateFn = (route, state) => {
  const auth = inject(AuthService);
  const router = inject(Router);

  const expectedRoles = (route.data?.['roles'] || []) as string[];
  if (!expectedRoles.length) return true;

  const userRole = auth.getRole();
  if (!userRole) {
    router.navigate(['/login'], { queryParams: { returnUrl: state.url } });
    return false;
  }

  const rolesArray = Array.isArray(userRole)
    ? userRole.map(r => r.toLowerCase())
    : [userRole.toLowerCase()];

  const isAllowed = expectedRoles.some(r => rolesArray.includes(r.toLowerCase()));

  if (!isAllowed) {
    router.navigate(['/login'], { queryParams: { returnUrl: state.url } });
    return false;
  }

  return true;
};