import { Routes } from '@angular/router';
import { MainLayoutComponent } from './layouts/main-layout/main-layout.component';
import { AuthLayoutComponent } from './layouts/auth-layout/auth-layout.component';

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
        loadChildren: () => import('./auth/auth.module').then(m => m.AuthModule)
      }
    ]
  },

  {
    path: 'client',
    loadComponent: () =>
      import('./layouts/client-layout/client-layout.component').then(m => m.ClientLayoutComponent),
    children: [
      {
        path: '',
        loadChildren: () => import('./client/client.module').then(m => m.ClientModule)
      }
    ]
  },

  {
    path: 'management',
    loadComponent: () =>
      import('./layouts/management-layout/management-layout.component').then(m => m.ManagementLayoutComponent),
    children: [
      {
        path: '',
        loadChildren: () => import('./management/management.module').then(m => m.ManagementModule)
      }
    ]
  },

  { path: '**', redirectTo: '' }
];
