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
        const role = this.auth.getRole()?.toLowerCase();
        const managementRoles = ['admin', 'manager', 'owner', 'trainer', ];

        if (role && role === 'client') 
            this.router.navigate(['/client']);
        else if (role && managementRoles.includes(role)) {
            this.router.navigate([`/management/${role}`]);
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
