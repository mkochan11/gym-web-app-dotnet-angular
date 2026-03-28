import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { CheckboxModule } from 'primeng/checkbox';
import { CalendarModule } from 'primeng/calendar';
import { PasswordModule } from 'primeng/password';
import { MessageService } from 'primeng/api';
import { ToastModule } from 'primeng/toast';
import { DynamicDialogRef } from 'primeng/dynamicdialog';
import { ClientService } from '../../../core/api-services/client.service';
import { ClientUser, CreateClientRequest } from '../../../core/models/client';
import { take } from 'rxjs';

@Component({
  selector: 'app-client-registration-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    ButtonModule,
    InputTextModule,
    CheckboxModule,
    CalendarModule,
    PasswordModule,
    ToastModule
  ],
  providers: [MessageService],
  templateUrl: './client-registration-dialog.component.html',
  styleUrl: './client-registration-dialog.component.scss'
})
export class ClientRegistrationDialogComponent {
  private clientService = inject(ClientService);
  private messageService = inject(MessageService);
  private fb = inject(FormBuilder);
  
  ref = inject(DynamicDialogRef);
  
  today = new Date();

  clientForm = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    firstName: ['', [Validators.required, Validators.maxLength(100)]],
    lastName: ['', [Validators.required, Validators.maxLength(100)]],
    phoneNumber: ['', Validators.maxLength(20)],
    dateOfBirth: [null as Date | null],
    autoGeneratePassword: [true],
    password: [{ value: '', disabled: true }]
  });

  loading = false;

  constructor() {
    this.clientForm.get('autoGeneratePassword')?.valueChanges.subscribe((autoGenerate) => {
      const passwordControl = this.clientForm.get('password');
      if (autoGenerate) {
        passwordControl?.disable();
        passwordControl?.setValue('');
      } else {
        passwordControl?.enable();
      }
    });
  }

  onSubmit() {
    if (this.clientForm.invalid) {
      this.markFormGroupTouched();
      return;
    }

    this.loading = true;

    const formValue = this.clientForm.value;
    const request: CreateClientRequest = {
      email: formValue.email!,
      firstName: formValue.firstName!,
      lastName: formValue.lastName!,
      phoneNumber: formValue.phoneNumber || undefined,
      dateOfBirth: formValue.dateOfBirth || undefined,
      password: formValue.autoGeneratePassword ? undefined : formValue.password || undefined
    };

    this.clientService.createClient(request)
      .pipe(take(1))
      .subscribe({
        next: (client: ClientUser) => {
          this.loading = false;
          this.messageService.add({
            severity: 'success',
            summary: 'Success',
            detail: 'Client registered successfully'
          });
          this.ref.close(client);
        },
        error: (error) => {
          this.loading = false;
          const errorMessage = error.error?.message || error.message || 'Failed to register client';
          this.messageService.add({
            severity: 'error',
            summary: 'Error',
            detail: errorMessage
          });
        }
      });
  }

  onCancel() {
    this.ref.close(undefined);
  }

  private markFormGroupTouched() {
    Object.values(this.clientForm.controls).forEach(control => {
      control.markAsTouched();
    });
  }

  get email() { return this.clientForm.get('email'); }
  get firstName() { return this.clientForm.get('firstName'); }
  get lastName() { return this.clientForm.get('lastName'); }
  get phoneNumber() { return this.clientForm.get('phoneNumber'); }
  get dateOfBirth() { return this.clientForm.get('dateOfBirth'); }
  get password() { return this.clientForm.get('password'); }
  get autoGeneratePassword() { return this.clientForm.get('autoGeneratePassword'); }
}
