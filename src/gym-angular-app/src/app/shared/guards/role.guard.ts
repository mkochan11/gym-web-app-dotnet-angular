import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { AuthService } from '../../core/api-services';

export const roleGuard: CanActivateFn = (route, state) => {
  const auth = inject(AuthService);
  const router = inject(Router);

  const expectedRoles = (route.data?.['roles'] || []) as string[];
  console.log('[RoleGuard] Checking roles for:', state.url, 'Expected:', expectedRoles);
  
  if (!expectedRoles.length) {
    console.log('[RoleGuard] No roles required, allowing access');
    return true;
  }

  const userRole = auth.getRole();
  console.log('[RoleGuard] User role:', userRole);
  
  if (!userRole) {
    console.log('[RoleGuard] No user role, redirecting to login');
    router.navigate(['/login'], { queryParams: { returnUrl: state.url } });
    return false;
  }

  const rolesArray = userRole.split(',').map(r => r.trim().toLowerCase());
  console.log('[RoleGuard] User roles array:', rolesArray);

  const isAllowed = expectedRoles.some(r => rolesArray.includes(r.toLowerCase()));
  console.log('[RoleGuard] Is allowed:', isAllowed);

  if (!isAllowed) {
    console.log('[RoleGuard] Access denied, redirecting to login');
    router.navigate(['/login'], { queryParams: { returnUrl: state.url } });
    return false;
  }

  return true;
};