import { Component, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormGroup, FormControl, Validators } from '@angular/forms';
import { Router, ActivatedRoute, RouterLink } from '@angular/router';
import { AuthService } from '../../core/api-services';
import { ToastService } from '../../core/services';
import { primeNgModules } from '../../shared/primeng';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterLink,
    ...primeNgModules
  ],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnDestroy {

  form = new FormGroup({
    email: new FormControl('', [Validators.required, Validators.email]),
    password: new FormControl('', Validators.required)
  });

  loading = false;
  error = '';
  returnUrl: string | null = null;

  constructor(
    private auth: AuthService,
    private router: Router,
    private route: ActivatedRoute,
    private toastService: ToastService
  ) {
    this.returnUrl = this.route.snapshot.queryParamMap.get('returnUrl');
  }

  ngOnDestroy(): void {
    this.returnUrl = null;
  }

  submit() {
    if (this.form.invalid) return;

    this.loading = true;
    this.error = '';

    this.auth.login(this.form.value as { email: string; password: string }).subscribe({
      next: () => {
        this.loading = false;
        const role = this.auth.getRole();
        console.log('Login success, role:', role);

        if (role) {
          const roles = role.split(',').map(r => r.trim().toLowerCase());
          
          if (roles.includes('admin')) {
            this.router.navigate(['/management/admin']);
          } else if (roles.includes('manager')) {
            this.router.navigate(['/management/manager']);
          } else if (roles.includes('owner')) {
            this.router.navigate(['/management/admin']);
          } else if (roles.includes('trainer')) {
            this.router.navigate(['/management/trainer']);
          } else if (roles.includes('receptionist')) {
            this.router.navigate(['/management/receptionist']);
          } else if (roles.includes('client')) {
            this.router.navigate(['/client']);
          } else {
            this.router.navigate(['/']);
          }
        } else {
          this.router.navigate(['/']);
        }

        this.toastService.show('Logged in successfully', 'success');
      },
      error: (err) => {
        this.loading = false;
        this.error = err.error?.message || 'Invalid email or password';
      }
    });
  }
}
