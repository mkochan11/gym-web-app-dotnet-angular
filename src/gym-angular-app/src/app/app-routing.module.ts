import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { authGuard } from './shared/guards/auth.guard';
import { roleGuard } from './shared/guards/role.guard';
import { HomeComponent } from './home/home.component';

const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'login', loadChildren: () => import('./auth/auth.module').then(m => m.AuthModule) },
  {
    path: 'client',
    canActivate: [authGuard, roleGuard],
    data: { role: 'Client' },
    loadChildren: () => import('./client/client.module').then(m => m.ClientModule)
  },
  {
    path: 'management',
    canActivate: [authGuard, roleGuard],
    data: { role: 'Manager' },
    loadChildren: () => import('./management/management.module').then(m => m.ManagementModule)
  },
  { path: '**', redirectTo: '' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule {}
