import { Component, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormGroup, FormControl, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { AuthService, ToastService } from '../../shared/services';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule
  ],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnDestroy{

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
        const managementRoles = ['manager', 'admin', 'trainer', 'receptionist', 'owner'];

        if (this.returnUrl) {
          this.router.navigateByUrl(this.returnUrl);
        }

        if (role) {
          const rolesArray = Array.isArray(role) ? role.map(r => r.toLowerCase()) : [role.toLowerCase()];

          if (rolesArray.includes('client')) {
            this.router.navigate(['/client']);
          } else if (rolesArray.some(r => managementRoles.includes(r))) {
            this.router.navigate(['/management']);
          }
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
