import { Component, inject, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { MessageModule } from 'primeng/message';
import { ProgressSpinnerModule } from 'primeng/progressspinner';

import { MembershipService, MembershipPlan, PaymentData, ToastService } from '../../shared/services';

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
  private router = inject(Router);
  private membershipService = inject(MembershipService);
  private toastService = inject(ToastService);

  selectedPlan: MembershipPlan | null = null;
  processing = false;

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

    this.processing = true;
    const paymentData: PaymentData = this.paymentForm.value;

    this.membershipService.processPayment(paymentData, this.selectedPlan)
      .then(result => {
        this.processing = false;
        
        if (result.success) {
          this.toastService.show(result.message, 'success');
          this.membershipService.clearSelectedPlan();
          this.router.navigate(['/dashboard']);
        } else {
          this.toastService.show(result.message, 'error');
        }
      })
      .catch(error => {
        this.processing = false;
        this.toastService.show('An unexpected error occurred', 'error');
        console.error('Payment error:', error);
      });
  }

  goBack() {
    this.router.navigate(['/membership']);
  }

  private markAllAsTouched() {
    Object.keys(this.paymentForm.controls).forEach(key => {
      this.paymentForm.get(key)?.markAsTouched();
    });
  }
}