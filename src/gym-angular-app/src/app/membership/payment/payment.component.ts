import { Component, inject, OnInit, signal } from '@angular/core';
import { Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Observable, of, switchMap, tap, map } from 'rxjs';

import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { MessageModule } from 'primeng/message';
import { ProgressSpinnerModule } from 'primeng/progressspinner';

import { MembershipService, PaymentData } from '../../core/api-services';
import { ToastService } from '../../core/services';
import { AuthService } from '../../core/api-services/auth.service';
import { ClientService } from '../../core/api-services/client.service';
import { Client } from '../../core/models/client';
import { MembershipPlan } from '../../core/models/membership-plan.model';

@Component({
  selector: 'app-payment',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    InputTextModule,
    ButtonModule,
    CardModule,
    MessageModule,
    ProgressSpinnerModule
  ],
  templateUrl: './payment.component.html',
  styleUrls: ['./payment.component.scss']
})
export class PaymentComponent implements OnInit {
  private fb = inject(FormBuilder);
  router = inject(Router);
  private membershipService = inject(MembershipService);
  private toastService = inject(ToastService);
  private authService = inject(AuthService);
  private clientService = inject(ClientService);

  selectedPlan: MembershipPlan | null = null;
  processing = signal(false);
  step = signal<'form' | 'processing' | 'success' | 'error'>('form');
  errorMessage = signal<string>('');

  paymentForm: FormGroup = this.fb.group({
    cardNumber: ['', [Validators.required, Validators.pattern(/^\d{16}$/)]],
    expiryDate: ['', [Validators.required, Validators.pattern(/^(0[1-9]|1[0-2])\/\d{2}$/)]],
    cvv: ['', [Validators.required, Validators.pattern(/^\d{3,4}$/)]],
    cardholderName: ['', [Validators.required]]
  });

  ngOnInit() {
    this.selectedPlan = this.membershipService.getSelectedPlan();
    
    if (!this.selectedPlan) {
      this.toastService.show('Please select a membership plan first', 'error');
      this.router.navigate(['/membership']);
    }
  }

  processPayment() {
    if (this.paymentForm.invalid || !this.selectedPlan) {
      this.markAllAsTouched();
      return;
    }

    this.processing.set(true);
    this.step.set('processing');
    this.errorMessage.set('');

    const accountId = this.authService.getUserId();
    if (!accountId) {
      this.handleError('User not authenticated');
      return;
    }

    this.clientService.getClientByAccountId(accountId).pipe(
      switchMap(client => {
        if (!client) {
          throw new Error('Client profile not found');
        }
        return this.membershipService.purchaseMembership(this.selectedPlan!.id, client.id);
      }),
      switchMap(membership => {
        const paymentData: PaymentData = this.paymentForm.value;
        return this.membershipService.processPayment(membership.id, paymentData).pipe(
          map(result => ({ membership, result }))
        );
      })
    ).subscribe({
      next: ({ membership, result }) => {
        this.processing.set(false);
        
        if (result.success) {
          this.step.set('success');
          this.toastService.show('Membership purchased successfully!', 'success');
          this.membershipService.clearSelectedPlan();
          setTimeout(() => {
            this.router.navigate(['/client']);
          }, 2000);
        } else {
          this.step.set('error');
          this.errorMessage.set(result.message || 'Payment failed');
          this.toastService.show(result.message || 'Payment failed', 'error');
        }
      },
      error: (error) => {
        this.handleError(error.message || 'An unexpected error occurred');
      }
    });
  }

  private handleError(message: string) {
    this.processing.set(false);
    this.step.set('error');
    this.errorMessage.set(message);
    this.toastService.show(message, 'error');
    console.error('Payment error:', message);
  }

  goBack() {
    this.router.navigate(['/membership']);
  }

  tryAgain() {
    this.step.set('form');
    this.errorMessage.set('');
  }

  private markAllAsTouched() {
    Object.keys(this.paymentForm.controls).forEach(key => {
      this.paymentForm.get(key)?.markAsTouched();
    });
  }
}