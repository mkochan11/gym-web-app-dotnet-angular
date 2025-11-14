import { Routes } from '@angular/router';
import { MainLayoutComponent } from './layouts/main-layout/main-layout.component';
import { AuthLayoutComponent } from './layouts/auth-layout/auth-layout.component';
import { ClientLayoutComponent } from './layouts/client-layout/client-layout.component';
import { ManagementLayoutComponent } from './layouts/management-layout/management-layout.component';
import { authGuard } from './shared/guards/auth.guard';
import { roleGuard } from './shared/guards/role.guard';

export const routes: Routes = [

  {
    path: '',
    component: MainLayoutComponent,
    children: [
      {
        path: '',
        loadComponent: () => import('./home/home.component').then(m => m.HomeComponent)
      }
    ]
  },

  {
    path: '',
    component: AuthLayoutComponent,
    children: [
      {
        path: 'login',
        loadComponent: () => import('./auth/login/login.component').then(m => m.LoginComponent)
      },
      {
        path: 'register',
        loadComponent: () => import('./auth/register/register.component').then(m => m.RegisterComponent)
      }
    ]
  },

  {
    path: 'client',
    component: ClientLayoutComponent,
    canActivate: [authGuard, roleGuard],
    data: { roles: ['Client'] },
    children: [
      {
        path: '',
        loadComponent: () => import('./client/dashboard/dashboard.component').then(m => m.DashboardComponent)
      }
    ]
  },

  {
    path: 'management',
    component: ManagementLayoutComponent,
    canActivate: [authGuard, roleGuard],
    data: { roles: ['Admin','Manager','Trainer','Receptionist','Owner'] },
    children: [
      {
        path: '',
        loadComponent: () => import('./management/dashboard/dashboard.component').then(m => m.DashboardComponent)
      }
    ]
  },

  { path: '**', redirectTo: '' }
];
