import { Component } from '@angular/core';
import { primeNgModules } from '../../../shared/primeng';
import { MenuItem } from 'primeng/api';
import { CommonModule } from '@angular/common';
import { AuthService, ToastService } from '../../../shared/services';
import { Router } from '@angular/router';

@Component({
  selector: 'app-user-menu',
  standalone: true,
  imports: [...primeNgModules, CommonModule],
  templateUrl: './user-menu.component.html',
  styleUrl: './user-menu.component.scss'
})
export class UserMenuComponent {
  badgeCount: number = 5;

    constructor(
      private auth: AuthService,
      private router: Router,
      private toastService: ToastService
    ) {}

  items: MenuItem[] = [
      { label: 'Messages', icon: 'pi pi-comments', routerLink: ['/management/messages'], badge: this.badgeCount.toString() },
      { label: 'Settings', icon: 'pi pi-cog', routerLink: ['/management/settings'] },
      { label: 'Sign out', icon: 'pi pi-sign-out', command: () => this.logout() }
    ];

    logout() {
    this.auth.logout();
    this.toastService.show('Logged out successfully', 'success');
    this.router.navigate(['/']);
  }
}
