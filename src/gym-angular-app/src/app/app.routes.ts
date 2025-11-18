import { Routes } from '@angular/router';
import { HomeLayoutComponent } from './layouts/home-layout/home-layout.component';
import { AuthLayoutComponent } from './layouts/auth-layout/auth-layout.component';
import { ClientLayoutComponent } from './layouts/client-layout/client-layout.component';
import { ManagementLayoutComponent } from './layouts/management-layout/management-layout.component';
import { authGuard } from './shared/guards/auth.guard';
import { roleGuard } from './shared/guards/role.guard';

export const routes: Routes = [

  {
    path: '',
    component: HomeLayoutComponent,
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
        path: 'admin',
        canActivate: [authGuard, roleGuard],
        data: { roles: ['Admin'] },
        loadComponent: () => import('./management/admin/dashboard/dashboard.component').then(m => m.DashboardComponent)
      },
      {
        path: 'manager',
        canActivate: [authGuard, roleGuard],
        data: { roles: ['Manager'] },
        loadComponent: () => import('./management/manager/dashboard/dashboard.component').then(m => m.DashboardComponent),
        children: [
          {
            path: 'trainers',
            canActivate: [authGuard, roleGuard],
            data: { roles: ['Admin','Manager','Owner'] },
            loadComponent: () => import('./shared/components/manage-users/trainers/trainers.component').then(m => m.TrainersComponent)
          },
          {
            path: 'receptionists',
            canActivate: [authGuard, roleGuard],
            data: { roles: ['Admin','Manager','Owner'] },
            loadComponent: () => import('./shared/components/manage-users/receptionists/receptionists.component').then(m => m.ReceptionistsComponent)
          },
          {
            path: 'clients',
            canActivate: [authGuard, roleGuard],
            data: { roles: ['Admin','Manager','Owner'] },
            loadComponent: () => import('./shared/components/manage-users/clients/clients.component').then(m => m.ClientsComponent)
          }
        ]
      },
    ]
  },

  { path: '**', redirectTo: '' }
];
