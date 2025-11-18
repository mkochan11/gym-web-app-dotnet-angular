import { Component, inject } from '@angular/core';
import { primeNgModules } from '../../shared/primeng';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService, ToastService } from '../../shared/services';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [...primeNgModules, CommonModule, ReactiveFormsModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss'
})
export class RegisterComponent {
  private fb = inject(FormBuilder);
  private router = inject(Router);
  private authService = inject(AuthService);
  private toastService = inject(ToastService);

  loading = false;
  
  form: FormGroup = this.fb.group({
    firstName: ['', [Validators.required]],
    lastName: ['', [Validators.required]],
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]],
    confirmPassword: ['', [Validators.required]],
  }, { 
    validators: this.passwordMatchValidator 
  });

  private passwordMatchValidator(g: FormGroup) {
    const password = g.get('password')?.value;
    const confirmPassword = g.get('confirmPassword')?.value;
    
    if (!password || !confirmPassword) {
      return null;
    }
    
    return password === confirmPassword ? null : { mismatch: true };
  }

  submit() {
    if (this.form.invalid) {
      this.markAllAsTouched();
      return;
    }

    this.loading = true;
    
    const { confirmPassword, ...registrationData } = this.form.value;

    this.authService.register(registrationData).subscribe({
      next: (response) => {
        this.loading = false;
        this.toastService.show('Account created successfully!', 'success');
        
        this.router.navigate(['/membership']);
      },
      error: (error) => {
        this.loading = false;
        console.error('Registration error:', error);
        
        const errorMessage = error?.error?.message || 'Registration failed. Please try again.';
        this.toastService.show(errorMessage, 'error');
      }
    });
  }

  private markAllAsTouched() {
    Object.keys(this.form.controls).forEach(key => {
      this.form.get(key)?.markAsTouched();
    });
  }
}
