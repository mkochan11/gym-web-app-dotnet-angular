import { Component, Inject, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DialogModule } from 'primeng/dialog';
import { ButtonModule } from 'primeng/button';
import { DropdownModule } from 'primeng/dropdown';
import { InputTextModule } from 'primeng/inputtext';
import { InputNumberModule } from 'primeng/inputnumber';
import { RadioButtonModule } from 'primeng/radiobutton';
import { CardModule } from 'primeng/card';
import { StepsModule } from 'primeng/steps';
import { ToastModule } from 'primeng/toast';
import { TagModule } from 'primeng/tag';
import { MessageService } from 'primeng/api';
import { DynamicDialogRef, DynamicDialogConfig } from 'primeng/dynamicdialog';
import { MembershipPlanService } from '../../../core/api-services/membership-plan.service';
import { MembershipService, ActivateMembershipRequest } from '../../../core/api-services/membership.service';
import { ClientListItem } from '../../../core/models/client';
import { MembershipPlan } from '../../../core/models/membership-plan.model';
import { Observable, take } from 'rxjs';

@Component({
  selector: 'app-client-activation-dialog',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    DialogModule,
    ButtonModule,
    DropdownModule,
    InputTextModule,
    InputNumberModule,
    RadioButtonModule,
    CardModule,
    StepsModule,
    ToastModule,
    TagModule
  ],
  providers: [MessageService],
  templateUrl: './client-activation-dialog.component.html',
  styleUrls: ['./client-activation-dialog.component.scss']
})
export class ClientActivationDialogComponent implements OnInit {
  private membershipPlanService = inject(MembershipPlanService);
  private membershipService = inject(MembershipService);
  private messageService = inject(MessageService);
  
  client!: ClientListItem;
  
  // Step 1: Plan Selection
  availablePlans: MembershipPlan[] = [];
  selectedPlan: MembershipPlan | null = null;
  loadingPlans = false;
  
  // Step 2: Payment
  paymentMethod: string = 'Cash';
  transactionId: string = '';
  paymentAmount: number = 0;
  
  paymentMethods = [
    { label: 'Cash', value: 'Cash' },
    { label: 'Card', value: 'Card' },
    { label: 'Bank Transfer', value: 'BankTransfer' },
    { label: 'Other', value: 'Other' }
  ];
  
  // Steps
  currentStep = 0;
  steps = [
    { label: 'Select Plan' },
    { label: 'Payment' },
    { label: 'Confirm' }
  ];
  
  loading = false;
  activationComplete = false;
  activatedMembership: any = null;

  constructor(
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig
  ) {
    this.client = config.data.client;
  }

  ngOnInit() {
    this.loadAvailablePlans();
  }

  loadAvailablePlans() {
    this.loadingPlans = true;
    this.membershipPlanService.getMembershipPlans()
      .pipe(take(1))
      .subscribe({
        next: (plans) => {
          this.availablePlans = plans.filter(p => p.isActive);
          this.loadingPlans = false;
        },
        error: () => {
          this.messageService.add({
            severity: 'error',
            summary: 'Error',
            detail: 'Failed to load membership plans'
          });
          this.loadingPlans = false;
        }
      });
  }

  onPlanSelect(plan: MembershipPlan) {
    this.selectedPlan = plan;
    this.paymentAmount = plan.price;
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
        return this.paymentAmount > 0 && this.paymentMethod !== '';
      default:
        return true;
    }
  }

  activateMembership() {
    if (!this.selectedPlan) return;

    this.loading = true;
    
    const request: ActivateMembershipRequest = {
      clientId: this.client.id,
      membershipPlanId: this.selectedPlan.id,
      paymentMethod: this.paymentMethod,
      transactionId: this.transactionId || undefined,
      amount: this.paymentAmount
    };

    this.membershipService.activateMembershipForClient(request)
      .pipe(take(1))
      .subscribe({
        next: (membership) => {
          this.activatedMembership = membership;
          this.activationComplete = true;
          this.loading = false;
          this.messageService.add({
            severity: 'success',
            summary: 'Success',
            detail: 'Membership activated successfully'
          });
        },
        error: (error) => {
          this.loading = false;
          this.messageService.add({
            severity: 'error',
            summary: 'Error',
            detail: error.message || 'Failed to activate membership'
          });
        }
      });
  }

  close() {
    this.ref.close(this.activationComplete);
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

  getStatusSeverity(status: string): 'success' | 'info' | 'warning' | 'danger' | 'secondary' | 'contrast' | undefined {
    switch (status) {
      case 'Active':
        return 'success';
      case 'Expired':
        return 'warning';
      case 'Cancelled':
        return 'danger';
      case 'None':
      default:
        return 'secondary';
    }
  }
}
