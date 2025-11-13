import { Component } from '@angular/core';
import { FormBuilder, Validators, FormGroup } from '@angular/forms';
import { AuthService } from '../../shared/services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent {
  loading = false;
  error: string | null = null;
  form: FormGroup;

  constructor(private fb: FormBuilder, private auth: AuthService, private router: Router) {
    this.form = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', Validators.required]
    });
  }

  submit() {
  if (this.form.invalid) {
    this.form.markAllAsTouched();
    return;
  }

  const { email, password } = this.form.value;
  if (!email || !password) return;

  this.loading = true;
  this.error = null;

  this.auth.login({ email, password }).subscribe({
    next: () => {
      const role = this.auth.getRole();
      const managementRoles = ['manager', 'admin', 'trainer', 'receptionist', 'owner'];

      if (role) {
        const rolesArray = Array.isArray(role) ? role.map(r => r.toLowerCase()) : [role.toLowerCase()];

        if (rolesArray.includes('client')) {
          this.router.navigate(['/client']);
        } else if (rolesArray.some(r => managementRoles.includes(r))) {
          this.router.navigate(['/management']);
        } else {
          this.router.navigate(['/']);
        }
      } else {
        this.router.navigate(['/']);
      }

      this.loading = false;
    },
    error: (err) => {
      this.error = err?.error?.message || 'Login failed';
      this.loading = false;
    }
  });
}

}