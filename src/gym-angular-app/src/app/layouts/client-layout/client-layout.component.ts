import { Component } from '@angular/core';
import { RouterLink, RouterOutlet, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../shared/services/auth.service';
import { ToastService } from '../../shared/services/toast.service';

@Component({
  selector: 'app-client-layout',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterOutlet],
  templateUrl: './client-layout.component.html',
  styleUrls: ['./client-layout.component.scss']
})
export class ClientLayoutComponent {

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