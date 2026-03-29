import { Component, Inject, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DialogModule } from 'primeng/dialog';
import { ButtonModule } from 'primeng/button';
import { DropdownModule } from 'primeng/dropdown';
import { StepsModule } from 'primeng/steps';
import { CardModule } from 'primeng/card';
import { ToastModule } from 'primeng/toast';
import { RadioButtonModule } from 'primeng/radiobutton';
import { InputTextModule } from 'primeng/inputtext';
import { InputNumberModule } from 'primeng/inputnumber';
import { MessageService } from 'primeng/api';
import { DynamicDialogRef, DynamicDialogConfig } from 'primeng/dynamicdialog';
import { MembershipService } from '../../../core/api-services/membership.service';
import { ClientListItem } from '../../../core/models/client';
import { GymMembership, CreditCalculation } from '../../../core/models/gym-membership.model';
import { MembershipPlan } from '../../../core/models/membership-plan.model';
import { Observable, take, of } from 'rxjs';

@Component({
  selector: 'app-client-change-plan-dialog',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    DialogModule,
    ButtonModule,
    DropdownModule,
    StepsModule,
    CardModule,
    ToastModule,
    RadioButtonModule,
    InputTextModule,
    InputNumberModule
  ],
  providers: [MessageService],
  templateUrl: './client-change-plan-dialog.component.html',
  styleUrls: ['./client-change-plan-dialog.component.scss']
})
export class ClientChangePlanDialogComponent implements OnInit {
  private membershipService = inject(MembershipService);
  private messageService = inject(MessageService);

  client!: ClientListItem;
  currentMembership: GymMembership | null = null;

  steps = [
    { label: 'Select Plan' },
    { label: 'Review' },
    { label: 'Payment' },
    { label: 'Confirm' }
  ];
  currentStep = 0;

  availablePlans: MembershipPlan[] = [];
  selectedPlan: MembershipPlan | null = null;
  loadingPlans = false;

  creditCalculation: CreditCalculation | null = null;
  loadingCalculation = false;

  paymentMethod = 'Cash';
  transactionId = '';
  additionalPaymentAmount = 0;

  paymentMethods = [
    { label: 'Cash', value: 'Cash' },
    { label: 'Card', value: 'Card' },
    { label: 'Bank Transfer', value: 'BankTransfer' },
    { label: 'Other', value: 'Other' }
  ];

  loading = false;
  changeComplete = false;

  constructor(
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig
  ) {
    this.client = config.data.client;
  }

  ngOnInit() {
    this.loadData();
  }

  loadData() {
    this.loading = true;

    this.membershipService.getClientActiveMembership(this.client.id)
      .pipe(take(1))
      .subscribe({
        next: (membership) => {
          this.currentMembership = membership;
          if (membership) {
            this.loadAvailablePlans(membership.id);
          } else {
            this.loading = false;
            this.messageService.add({
              severity: 'error',
              summary: 'Error',
              detail: 'No active membership found'
            });
          }
        },
        error: () => {
          this.loading = false;
          this.messageService.add({
            severity: 'error',
            summary: 'Error',
            detail: 'Failed to load membership details'
          });
        }
      });
  }

  loadAvailablePlans(membershipId: number) {
    this.loadingPlans = true;

    this.membershipService.getAvailablePlans(membershipId)
      .pipe(take(1))
      .subscribe({
        next: (plans) => {
          this.availablePlans = plans;
          this.loadingPlans = false;
          this.loading = false;
        },
        error: () => {
          this.loadingPlans = false;
          this.loading = false;
          this.messageService.add({
            severity: 'error',
            summary: 'Error',
            detail: 'Failed to load available plans'
          });
        }
      });
  }

  onPlanSelect(plan: MembershipPlan) {
    this.selectedPlan = plan;
    this.loadCreditCalculation();
  }

  loadCreditCalculation() {
    if (!this.currentMembership || !this.selectedPlan) return;

    this.loadingCalculation = true;
    this.membershipService.calculateCredit(this.currentMembership.id, this.selectedPlan.id)
      .pipe(take(1))
      .subscribe({
        next: (calculation) => {
          this.creditCalculation = calculation;
          this.loadingCalculation = false;

          if (calculation.isUpgrade && calculation.firstPaymentAmount > 0) {
            this.additionalPaymentAmount = calculation.firstPaymentAmount;
          }
        },
        error: () => {
          this.loadingCalculation = false;
          this.messageService.add({
            severity: 'error',
            summary: 'Error',
            detail: 'Failed to calculate credit'
          });
        }
      });
  }

  nextStep() {
    if (this.currentStep < this.steps.length - 1) {
      this.currentStep++;
    }
  }

  prevStep() {
    if (this.currentStep > 0) {
      this.currentStep--;
    }
  }

  canProceedToNextStep(): boolean {
    switch (this.currentStep) {
      case 0:
        return this.selectedPlan !== null;
      case 1:
        return this.creditCalculation !== null;
      case 2:
        if (this.creditCalculation?.isUpgrade && this.creditCalculation.firstPaymentAmount > 0) {
          return this.paymentMethod !== '' && this.additionalPaymentAmount > 0;
        }
        return true;
      default:
        return true;
    }
  }

  isUpgrade(): boolean {
    return this.creditCalculation?.isUpgrade ?? false;
  }

  requiresPayment(): boolean {
    return this.isUpgrade() && (this.creditCalculation?.firstPaymentAmount ?? 0) > 0;
  }

  changePlan() {
    if (!this.currentMembership || !this.selectedPlan) return;

    this.loading = true;

    this.membershipService.changePlan(this.currentMembership.id, {
      newPlanId: this.selectedPlan.id
    })
      .pipe(take(1))
      .subscribe({
        next: () => {
          this.changeComplete = true;
          this.loading = false;
          this.messageService.add({
            severity: 'success',
            summary: 'Success',
            detail: 'Membership plan changed successfully'
          });
        },
        error: (error) => {
          this.loading = false;
          this.messageService.add({
            severity: 'error',
            summary: 'Error',
            detail: error.message || 'Failed to change plan'
          });
        }
      });
  }

  close() {
    this.ref.close(this.changeComplete);
  }

  formatDate(date: Date | string | undefined): string {
    if (!date) return 'N/A';
    return new Date(date).toLocaleDateString('pl-PL');
  }

  formatCurrency(amount: number): string {
    return new Intl.NumberFormat('pl-PL', {
      style: 'currency',
      currency: 'PLN'
    }).format(amount);
  }

  abs(value: number): number {
    return Math.abs(value);
  }
}
