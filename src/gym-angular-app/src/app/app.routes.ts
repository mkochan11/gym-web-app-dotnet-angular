import { Routes } from '@angular/router';
import { HomeLayoutComponent } from './layouts/home-layout/home-layout.component';
import { AuthLayoutComponent } from './layouts/auth-layout/auth-layout.component';
import { ClientLayoutComponent } from './layouts/client-layout/client-layout.component';
import { ManagementLayoutComponent } from './layouts/management-layout/management-layout.component';
import { authGuard } from './shared/guards/auth.guard';
import { roleGuard } from './shared/guards/role.guard';
import { ManagerLayoutComponent } from './layouts/management-layout/manager-layout/manager-layout.component';

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
    path: 'membership',
    children: [
      { 
        path: '', 
        canActivate: [authGuard, roleGuard],
        data: { roles: ['Client'] },
        loadComponent: () => import('./membership/membership-selection/membership-selection.component').then(m => m.MembershipSelectionComponent) 
      },
      { 
        path: 'payment', 
        canActivate: [authGuard, roleGuard],
        data: { roles: ['Client'] },
        loadComponent: () => import('./membership/payment/payment.component').then(m => m.PaymentComponent) 
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
      },
      {
        path: 'membership',
        loadComponent: () => import('./client/my-membership/my-membership.component').then(m => m.MyMembershipComponent)
      },
      {
        path: 'payments',
        loadComponent: () => import('./client/payments/payments.component').then(m => m.PaymentsComponent)
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
        children: [
          {
            path: '',
            loadComponent: () => import('./management/admin/dashboard/dashboard.component').then(m => m.DashboardComponent)
          },
          {
            path: 'users',
            canActivate: [authGuard, roleGuard],
            data: { roles: ['Admin'] },
            loadComponent: () => import('./management/admin/users/users.component').then(m => m.UsersComponent)
          },
          {
            path: 'membership-plans',
            canActivate: [authGuard, roleGuard],
            data: { roles: ['Admin', 'Manager', 'Receptionist'] },
            loadComponent: () => import('./features/membership-plans/membership-plans-list/membership-plans-list.component').then(m => m.MembershipPlansListComponent)
          }
        ]
      },
      {
        path: 'manager',
        component: ManagerLayoutComponent,
        children: [
          {
            path: '',
            canActivate: [authGuard, roleGuard],
            data: { roles: ['Manager', 'Admin'] },
            loadComponent: () => import('./management/manager/dashboard/dashboard.component').then(m => m.DashboardComponent)
          },
          {
            path: 'calendar',
            canActivate: [authGuard, roleGuard],
            data: { roles: ['Manager', 'Admin', 'Receptionist'] },
            loadComponent: () => import('./management/manager/manager-calendar/manager-calendar.component').then(m => m.ManagerCalendarComponent)
          },
          {
            path: 'users',
            canActivate: [authGuard, roleGuard],
            data: { roles: ['Manager', 'Admin'] },
            loadComponent: () => import('./management/manager/users/manager-users.component').then(m => m.ManagerUsersComponent)
          },
          {
            path: 'membership-plans',
            canActivate: [authGuard, roleGuard],
            data: { roles: ['Admin', 'Manager', 'Receptionist'] },
            loadComponent: () => import('./features/membership-plans/membership-plans-list/membership-plans-list.component').then(m => m.MembershipPlansListComponent)
          },
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
            path: 'employees/:id/employments',
            canActivate: [authGuard, roleGuard],
            data: { roles: ['Manager'] },
            loadComponent: () => import('./management/manager/employee-employments/employee-employments.component').then(m => m.EmployeeEmploymentsComponent)
          },
          {
            path: 'clients',
            canActivate: [authGuard, roleGuard],
            data: { roles: ['Admin','Manager','Receptionist','Owner'] },
            loadComponent: () => import('./features/clients/clients.component').then(m => m.ClientsComponent)
          },
          {
            path: 'client-payments/:clientId',
            canActivate: [authGuard, roleGuard],
            data: { roles: ['Admin','Manager','Receptionist'] },
            loadComponent: () => import('./management/receptionist/client-payments/client-payments.component').then(m => m.ClientPaymentsComponent)
          }
        ]
      },
      {
        path: 'receptionist',
        component: ManagerLayoutComponent,
        children: [
          {
            path: '',
            canActivate: [authGuard, roleGuard],
            data: { roles: ['Receptionist'] },
            loadComponent: () => import('./management/receptionist/dashboard/dashboard.component').then(m => m.ReceptionistDashboardComponent)
          },
          {
            path: 'calendar',
            canActivate: [authGuard, roleGuard],
            data: { roles: ['Manager', 'Admin', 'Receptionist'] },
            loadComponent: () => import('./management/manager/manager-calendar/manager-calendar.component').then(m => m.ManagerCalendarComponent)
          },
          {
            path: 'membership-plans',
            canActivate: [authGuard, roleGuard],
            data: { roles: ['Admin', 'Manager', 'Receptionist'] },
            loadComponent: () => import('./features/membership-plans/membership-plans-list/membership-plans-list.component').then(m => m.MembershipPlansListComponent)
          },
          {
            path: 'clients',
            canActivate: [authGuard, roleGuard],
            data: { roles: ['Admin','Manager','Receptionist','Owner'] },
            loadComponent: () => import('./features/clients/clients.component').then(m => m.ClientsComponent)
          },
          {
            path: 'client-payments/:clientId',
            canActivate: [authGuard, roleGuard],
            data: { roles: ['Admin','Manager','Receptionist'] },
            loadComponent: () => import('./management/receptionist/client-payments/client-payments.component').then(m => m.ClientPaymentsComponent)
          }
        ]
      },
    ]
  },

  { path: '**', redirectTo: '' }
];
