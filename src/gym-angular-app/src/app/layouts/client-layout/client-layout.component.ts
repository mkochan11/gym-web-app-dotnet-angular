import { Component } from '@angular/core';
import { RouterLink, RouterOutlet, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../core/api-services';
import { ToastService } from '../../core/services/toast.service';
import { primeNgModules } from '../../shared/primeng';

@Component({
  selector: 'app-client-layout',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterOutlet, ...primeNgModules],
  templateUrl: './client-layout.component.html',
  styleUrls: ['./client-layout.component.scss']
})
export class ClientLayoutComponent {

  menuItems = [
    { label: 'Products', routerLink: '/client/products' },
    { label: 'Settings', routerLink: '/client/settings' }
  ];

  constructor(
    private auth: AuthService,
    private router: Router,
    private toastService: ToastService
  ) {}

  logout() {
    this.auth.logout();
    this.toastService.show('You\'ve been logged out', 'success');
    this.router.navigate(['/']);
  }
}