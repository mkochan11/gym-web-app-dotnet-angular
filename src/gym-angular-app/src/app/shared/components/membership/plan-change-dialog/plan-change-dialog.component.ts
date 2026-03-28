import { Component, EventEmitter, Input, Output, inject, signal, OnInit, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DialogModule } from 'primeng/dialog';
import { ButtonModule } from 'primeng/button';
import { DropdownModule } from 'primeng/dropdown';
import { DividerModule } from 'primeng/divider';
import { FormsModule } from '@angular/forms';
import { ToastService } from '../../../../core/services/toast.service';
import { MembershipService } from '../../../../core/api-services/membership.service';
import { MembershipPlanService } from '../../../../core/api-services/membership-plan.service';
import { GymMembership, ChangePlanRequest, CreditCalculation } from '../../../../core/models/gym-membership.model';
import { MembershipPlan } from '../../../../core/models/membership-plan.model';

interface PlanOption {
  id: number;
  name: string;
  price: number;
  durationInMonths: number;
  description: string;
}

@Component({
  selector: 'app-plan-change-dialog',
  standalone: true,
  imports: [
    CommonModule,
    DialogModule,
    ButtonModule,
    DropdownModule,
    FormsModule,
    DividerModule
  ],
  template: `
    <p-dialog 
      [(visible)]="visible" 
      [header]="'Change Membership Plan'" 
      [modal]="true" 
      [style]="{ width: '500px' }"
      [appendTo]="'body'"
      [blockScroll]="true"
      (onHide)="onCancel()">
      
      <div class="flex flex-column gap-4">
        <div class="field">
          <label for="plan" class="block text-sm font-medium mb-2">Select New Plan</label>
          <p-dropdown 
            id="plan"
            [options]="plans()" 
            [(ngModel)]="selectedPlanId"
            optionLabel="name" 
            optionValue="id"
            placeholder="Select a plan"
            [style]="{ width: '100%' }"
            [appendTo]="'body'"
            (onChange)="onPlanSelect($event)">
            <ng-template let-plan pTemplate="item">
              <div class="flex flex-column">
                <span class="font-medium">{{ plan.name }}</span>
                <span class="text-sm text-500">{{ plan.price | currency:'EUR' }} / {{ plan.durationInMonths }} {{ plan.durationInMonths === 1 ? 'month' : 'months' }}</span>
              </div>
            </ng-template>
            <ng-template let-plan pTemplate="selectedItem">
              <div class="flex flex-column">
                <span class="font-medium">{{ plan.name }}</span>
                <span class="text-sm text-500">{{ plan.price | currency:'EUR' }} / {{ plan.durationInMonths }} {{ plan.durationInMonths === 1 ? 'month' : 'months' }}</span>
              </div>
            </ng-template>
          </p-dropdown>
        </div>

        <div *ngIf="selectedPlanId && creditCalculation()" class="credit-info p-3 surface-100 border-round">
          <h4 class="text-sm text-500 mb-3">Plan Change Summary</h4>
          
          <div class="grid">
            <div class="col-6">
              <div class="text-sm text-500">Current Plan</div>
              <div class="font-medium">{{ creditCalculation()!.currentPlanName }}</div>
              <div class="text-sm">{{ creditCalculation()!.currentPlanPrice | currency:'EUR' }}/month</div>
            </div>
            <div class="col-6">
              <div class="text-sm text-500">New Plan</div>
              <div class="font-medium">{{ creditCalculation()!.newPlanName }}</div>
              <div class="text-sm">{{ creditCalculation()!.newPlanPrice | currency:'EUR' }}/month</div>
            </div>
          </div>

          <p-divider></p-divider>

          <div class="mb-3">
            <div class="text-sm text-500 mb-2">New Plan Features:</div>
            <div class="flex flex-wrap gap-2">
              <span *ngIf="creditCalculation()!.newPlanCanReserveTrainings" class="p-1 surface-300 border-round text-xs">Reserve Trainings</span>
              <span *ngIf="creditCalculation()!.newPlanCanAccessGroupTraining" class="p-1 surface-300 border-round text-xs">Group Training</span>
              <span *ngIf="creditCalculation()!.newPlanCanAccessPersonalTraining" class="p-1 surface-300 border-round text-xs">Personal Training</span>
              <span *ngIf="creditCalculation()!.newPlanCanReceiveTrainingPlans" class="p-1 surface-300 border-round text-xs">Training Plans</span>
              <span *ngIf="creditCalculation()!.newPlanMaxTrainingsPerMonth" class="p-1 surface-300 border-round text-xs">{{ creditCalculation()!.newPlanMaxTrainingsPerMonth }} trainings/month</span>
            </div>
          </div>

          <p-divider></p-divider>

          <div class="flex justify-content-between mb-2">
            <span class="text-sm">Credit for unused days (old plan):</span>
            <span class="font-medium text-green-500">-{{ creditCalculation()!.creditAmount | currency:'EUR' }}</span>
          </div>

          <div class="flex justify-content-between mb-2">
            <span class="text-sm">First payment amount:</span>
            <span class="font-medium">{{ creditCalculation()!.firstPaymentAmount | currency:'EUR' }}</span>
          </div>

          <div *ngIf="creditCalculation()!.firstPaymentAmount === 0 && creditCalculation()!.creditAmount > creditCalculation()!.newPlanPrice" class="text-sm text-500 mt-2">
            First month is free! Remaining credit will be applied to next month.
          </div>
          
          <div *ngIf="creditCalculation()!.firstPaymentAmount > 0" class="text-sm text-500 mt-2">
            Following months: {{ creditCalculation()!.newMonthlyAmount | currency:'EUR' }}/month
          </div>
        </div>

        <div *ngIf="loading()" class="flex justify-content-center p-3">
          <i class="pi pi-spin pi-spinner text-4xl"></i>
        </div>
      </div>

      <ng-template pTemplate="footer">
        <div class="flex gap-2 justify-content-end">
          <p-button 
            label="Cancel" 
            icon="pi pi-times" 
            [outlined]="true" 
            (onClick)="onCancel()"
            [disabled]="loading()">
          </p-button>
          <p-button 
            *ngIf="selectedPlanId && !creditCalculation()"
            label="Calculate" 
            icon="pi pi-calculator" 
            (onClick)="onCalculate()"
            [disabled]="loading()">
          </p-button>
          <p-button 
            *ngIf="selectedPlanId && creditCalculation()"
            label="Confirm Change" 
            icon="pi pi-check" 
            severity="success"
            (onClick)="onConfirm()"
            [disabled]="loading() || !creditCalculation()">
          </p-button>
        </div>
      </ng-template>
    </p-dialog>
  `
})
export class PlanChangeDialogComponent implements OnInit, OnChanges {
  private readonly membershipService: MembershipService = inject(MembershipService);
  private readonly membershipPlanService: MembershipPlanService = inject(MembershipPlanService);
  private readonly toastService = inject(ToastService);

  @Input() visible = false;
  @Input() membership: GymMembership | null = null;
  @Output() visibleChange = new EventEmitter<boolean>();
  @Output() planChanged = new EventEmitter<void>();

  plans = signal<PlanOption[]>([]);
  selectedPlanId: number | null = null;
  selectedPlan = signal<PlanOption | null>(null);
  creditCalculation = signal<CreditCalculation | null>(null);
  loading = signal(false);

  Math = Math;

  ngOnInit() {
    if (this.membership) {
      this.loadPlans();
    }
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes['visible'] && this.visible && this.membership) {
      this.loadPlans();
      this.reset();
    }
  }

  loadPlans() {
    if (!this.membership) return;
    
    this.membershipService.getAvailablePlans(this.membership.id).subscribe({
      next: (plans) => {
        this.plans.set(plans.map(p => ({
          id: p.id,
          name: p.type,
          price: p.price,
          durationInMonths: p.durationInMonths,
          description: p.description
        })));
      },
      error: () => {
        this.toastService.show('Failed to load available plans', 'error');
      }
    });
  }

  private reset() {
    this.selectedPlanId = null;
    this.selectedPlan.set(null);
    this.creditCalculation.set(null);
  }

  onPlanSelect(event: any) {
    this.selectedPlan.set(this.plans().find(p => p.id === event.value) || null);
    this.onCalculate();
  }

  onCalculate() {
    if (!this.membership || !this.selectedPlanId) return;

    this.loading.set(true);
    this.membershipService.calculateCredit(this.membership.id, this.selectedPlanId).subscribe({
      next: (result) => {
        this.creditCalculation.set(result);
        this.loading.set(false);
      },
      error: (err) => {
        this.loading.set(false);
        this.toastService.show(err.error?.message || 'Failed to calculate credit', 'error');
      }
    });
  }

  onConfirm() {
    if (!this.membership || !this.selectedPlanId) return;

    this.loading.set(true);
    const request: ChangePlanRequest = { newPlanId: this.selectedPlanId };
    
    this.membershipService.changePlan(this.membership.id, request).subscribe({
      next: () => {
        this.loading.set(false);
        this.toastService.show('Plan changed successfully', 'success');
        this.planChanged.emit();
        this.resetAndClose();
      },
      error: (err) => {
        this.loading.set(false);
        this.toastService.show(err.error?.message || 'Failed to change plan', 'error');
      }
    });
  }

  onCancel() {
    this.resetAndClose();
  }

  private resetAndClose() {
    this.selectedPlanId = null;
    this.selectedPlan.set(null);
    this.creditCalculation.set(null);
    this.visibleChange.emit(false);
  }
}
