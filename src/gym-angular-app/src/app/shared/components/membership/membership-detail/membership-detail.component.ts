import { Component, EventEmitter, Input, Output, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';
import { TagModule } from 'primeng/tag';
import { DividerModule } from 'primeng/divider';
import { ToastService } from '../../../../core/services/toast.service';
import { MembershipService } from '../../../../core/api-services/membership.service';
import { GymMembership } from '../../../../core/models/gym-membership.model';
import { MembershipCancelDialogComponent } from '../membership-cancel-dialog/membership-cancel-dialog.component';
import { PlanChangeDialogComponent } from '../plan-change-dialog/plan-change-dialog.component';

@Component({
  selector: 'app-membership-detail',
  standalone: true,
  imports: [
    CommonModule,
    CardModule,
    ButtonModule,
    TagModule,
    DividerModule,
    MembershipCancelDialogComponent,
    PlanChangeDialogComponent
  ],
  template: `
    <p-card *ngIf="membership()">
      <ng-template pTemplate="header">
        <div class="flex justify-content-between align-items-center px-4 py-3">
          <span class="text-xl font-semibold">{{ membership()!.planName }}</span>
          <p-tag 
            [value]="getStatusLabel()" 
            [severity]="getStatusSeverity()">
          </p-tag>
        </div>
      </ng-template>

      <div class="membership-details">
        <div class="grid">
          <div class="col-12 md:col-6">
            <div class="field">
              <label class="text-500 text-sm">Start Date</label>
              <div class="font-medium">{{ membership()!.startDate | date:'mediumDate' }}</div>
            </div>
          </div>
          <div class="col-12 md:col-6">
            <div class="field">
              <label class="text-500 text-sm">End Date</label>
              <div class="font-medium">{{ membership()!.endDate | date:'mediumDate' }}</div>
            </div>
          </div>
        </div>

        <p-divider></p-divider>

        <div class="plan-info">
          <h4 class="text-sm text-500 mb-2">Plan Details</h4>
          <p class="mb-2">{{ membership()!.planDescription }}</p>
          <div class="text-lg font-semibold mb-3">
            {{ membership()!.planPrice | currency:'EUR' }} / {{ membership()!.planDurationInMonths }} {{ membership()!.planDurationInMonths === 1 ? 'month' : 'months' }}
          </div>
          
          <div class="permissions-info">
            <h4 class="text-sm text-500 mb-2">Your Permissions</h4>
            <div class="flex flex-wrap gap-3">
              <div *ngIf="membership()!.canReserveTrainings" class="flex align-items-center">
                <i class="pi pi-calendar text-green-500 mr-2"></i>
                <span class="text-sm">Reserve Trainings</span>
              </div>
              <div *ngIf="membership()!.canAccessGroupTraining" class="flex align-items-center">
                <i class="pi pi-users text-green-500 mr-2"></i>
                <span class="text-sm">Group Training</span>
              </div>
              <div *ngIf="membership()!.canAccessPersonalTraining" class="flex align-items-center">
                <i class="pi pi-user text-green-500 mr-2"></i>
                <span class="text-sm">Personal Training</span>
              </div>
              <div *ngIf="membership()!.canReceiveTrainingPlans" class="flex align-items-center">
                <i class="pi pi-file text-green-500 mr-2"></i>
                <span class="text-sm">Training Plans</span>
              </div>
              <div *ngIf="membership()!.maxTrainingsPerMonth" class="flex align-items-center">
                <i class="pi pi-chart-bar text-green-500 mr-2"></i>
                <span class="text-sm">{{ membership()!.maxTrainingsPerMonth }} trainings/month</span>
              </div>
            </div>
          </div>
        </div>

        <div *ngIf="membership()!.status === 1" class="pending-cancellation-info mt-4 p-3 surface-100 border-round">
          <div class="flex align-items-center mb-2">
            <i class="pi pi-info-circle mr-2 text-500"></i>
            <span class="text-500 text-sm">Cancellation requested on {{ membership()!.cancellationRequestedDate | date:'medium' }}</span>
          </div>
          <div *ngIf="membership()!.effectiveEndDate" class="text-sm mb-2">
            <strong>Access until:</strong> {{ membership()!.effectiveEndDate | date:'mediumDate' }}
          </div>
          <div *ngIf="membership()!.cancellationReason" class="text-sm mb-2">
            <strong>Reason:</strong> {{ membership()!.cancellationReason }}
          </div>
          <p-button 
            label="Revert Cancellation" 
            icon="pi pi-undo" 
            severity="success" 
            [outlined]="true"
            (onClick)="onRevertCancellation()"
            [disabled]="loading()">
          </p-button>
        </div>

        <div *ngIf="membership()!.status === 2" class="cancelled-info mt-4 p-3 surface-100 border-round">
          <div class="flex align-items-center mb-2">
            <i class="pi pi-info-circle mr-2 text-500"></i>
            <span class="text-500 text-sm">Cancelled on {{ membership()!.cancelledAt | date:'medium' }}</span>
          </div>
          <div *ngIf="membership()!.cancellationReason" class="text-sm">
            <strong>Reason:</strong> {{ membership()!.cancellationReason }}
          </div>
        </div>
      </div>

      <ng-template pTemplate="footer">
        <div class="flex justify-content-between align-items-center" *ngIf="membership()!.status === 0">
          <div class="flex gap-2">
            <p-button 
              label="View Payments" 
              icon="pi pi-credit-card" 
              severity="info" 
              [outlined]="true"
              (onClick)="goToPayments()"
              [disabled]="loading()">
            </p-button>
            <p-button 
              label="Change Plan" 
              icon="pi pi-sync" 
              severity="warning" 
              [outlined]="true"
              (onClick)="showPlanChangeDialog = true"
              [disabled]="loading()">
            </p-button>
          </div>
          <p-button 
            label="Cancel Membership" 
            icon="pi pi-times" 
            severity="danger" 
            [outlined]="true"
            (onClick)="showCancelDialog = true"
            [disabled]="loading()">
          </p-button>
        </div>
      </ng-template>
    </p-card>

    <div *ngIf="!membership() && !loading()" class="text-center p-4">
      <i class="pi pi-info-circle text-4xl text-500 mb-3"></i>
      <p class="text-500">No active membership found</p>
      <p-button label="Purchase Membership" icon="pi pi-plus" class="mt-3" (onClick)="goToMembership()"></p-button>
    </div>

    <app-membership-cancel-dialog 
      [(visible)]="showCancelDialog"
      [planName]="membership()?.planName || ''"
      [endDate]="membership()?.endDate ? toDate(membership()!.endDate) : null"
      [loading]="loading()"
      (confirmed)="onCancelConfirmed($event)"
      (cancelled)="onCancelCancelled()">
    </app-membership-cancel-dialog>

    <app-plan-change-dialog 
      [(visible)]="showPlanChangeDialog"
      [membership]="membership()"
      (visibleChange)="showPlanChangeDialog = $event"
      (planChanged)="onPlanChanged()">
    </app-plan-change-dialog>
  `
})
export class MembershipDetailComponent implements OnInit {
  private readonly membershipService: MembershipService = inject(MembershipService);
  private readonly toastService = inject(ToastService);
  private readonly router = inject(Router);

  @Input() clientId!: number;
  @Output() membershipCancelled = new EventEmitter<void>();
  @Output() planChanged = new EventEmitter<void>();

  membership = signal<GymMembership | null>(null);
  loading = signal(false);
  showCancelDialog = false;
  showPlanChangeDialog = false;

  ngOnInit() {
    this.loadMembership();
  }

  loadMembership() {
    this.loading.set(true);
    this.membershipService.getClientActiveMembership(this.clientId).subscribe({
      next: (data) => {
        this.membership.set(data);
        this.loading.set(false);
      },
      error: () => {
        this.membership.set(null);
        this.loading.set(false);
      }
    });
  }

  goToMembership() {
    this.router.navigate(['/membership']);
  }

  goToPayments() {
    if (this.membership()) {
      this.router.navigate(['/client/payments'], { 
        state: { membershipId: this.membership()!.id }
      });
    }
  }

  onCancelConfirmed(reason: string | null) {
    if (!this.membership()) return;

    this.loading.set(true);
    this.membershipService.cancelMembership(this.membership()!.id, {
      cancellationReason: reason
    }).subscribe({
      next: (result) => {
        this.membership.set(result);
        this.showCancelDialog = false;
        this.loading.set(false);
        this.toastService.show('Your cancellation request has been submitted. Your membership will remain active until the end of your current billing period.', 'success');
        this.membershipCancelled.emit();
      },
      error: (err) => {
        this.loading.set(false);
        this.toastService.show(err.error?.message || 'Failed to cancel membership', 'error');
      }
    });
  }

  onCancelCancelled() {
    this.showCancelDialog = false;
  }

  getStatusLabel(): string {
    if (!this.membership()) return '';
    const status = this.membership()!.status;
    switch (status) {
      case 0: return 'Active';
      case 1: return 'Pending Cancellation';
      case 2: return 'Cancelled';
      case 3: return 'Expired';
      default: return 'Active';
    }
  }

  getStatusSeverity(): 'success' | 'danger' | 'warning' | 'info' | 'secondary' | 'contrast' | undefined {
    if (!this.membership()) return 'info';
    const status = this.membership()!.status;
    switch (status) {
      case 0: return 'success';
      case 1: return 'warning';
      case 2: return 'danger';
      case 3: return 'secondary';
      default: return 'info';
    }
  }

  isPendingCancellation(): boolean {
    return this.membership()?.status === 1;
  }

  isActive(): boolean {
    return this.membership()?.status === 0;
  }

  isCancelled(): boolean {
    return this.membership()?.status === 2;
  }

  onRevertCancellation() {
    if (!this.membership()) return;

    this.loading.set(true);
    this.membershipService.revertCancellation(this.membership()!.id).subscribe({
      next: (result) => {
        this.membership.set(result);
        this.loading.set(false);
        this.toastService.show('Cancellation reverted successfully', 'success');
      },
      error: (err) => {
        this.loading.set(false);
        this.toastService.show(err.error?.message || 'Failed to revert cancellation', 'error');
      }
    });
  }

  toDate(dateStr: string): Date {
    return new Date(dateStr);
  }

  onPlanChanged() {
    this.showPlanChangeDialog = false;
    this.loadMembership();
    this.planChanged.emit();
  }
}
