import { Component } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
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

  form = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', Validators.required]
  });

  constructor(private fb: FormBuilder, private auth: AuthService, private router: Router) {}

  submit() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.loading = true;
    this.error = null;

    this.auth.login(this.form.value).subscribe({
      next: () => {
        const role = this.auth.getRole();
        // redirect based on role
        if (role && role.toLowerCase().includes('client')) {
          this.router.navigate(['/client']);
        } else if (role && role.toLowerCase().includes('manager')) {
          this.router.navigate(['/management']);
        } else {
          // fallback
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