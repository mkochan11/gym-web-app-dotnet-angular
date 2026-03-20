import { Component, EventEmitter, Input, Output, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';
import { TagModule } from 'primeng/tag';
import { DividerModule } from 'primeng/divider';
import { ToastService } from '../../../../core/services/toast.service';
import { GymMembershipService } from '../../../../core/api-services';
import { GymMembership } from '../../../../core/models/gym-membership.model';
import { MembershipCancelDialogComponent } from '../membership-cancel-dialog/membership-cancel-dialog.component';

@Component({
  selector: 'app-membership-detail',
  standalone: true,
  imports: [
    CommonModule,
    CardModule,
    ButtonModule,
    TagModule,
    DividerModule,
    MembershipCancelDialogComponent
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
          <div class="text-lg font-semibold">
            {{ membership()!.planPrice | currency:'EUR' }} / {{ membership()!.planDurationInMonths }} {{ membership()!.planDurationInMonths === 1 ? 'month' : 'months' }}
          </div>
        </div>

        <div *ngIf="membership()!.isCancelled" class="cancelled-info mt-4 p-3 surface-100 border-round">
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
        <div class="flex justify-content-end gap-2" *ngIf="!membership()!.isCancelled">
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
      <p-button label="Purchase Membership" icon="pi pi-plus" class="mt-3"></p-button>
    </div>

    <app-membership-cancel-dialog 
      [(visible)]="showCancelDialog"
      [planName]="membership()?.planName || ''"
      [endDate]="membership()?.endDate ? toDate(membership()!.endDate) : null"
      [loading]="loading()"
      (confirmed)="onCancelConfirmed($event)"
      (cancelled)="onCancelCancelled()">
    </app-membership-cancel-dialog>
  `
})
export class MembershipDetailComponent implements OnInit {
  private readonly gymMembershipService: GymMembershipService = inject(GymMembershipService);
  private readonly toastService: ToastService = inject(ToastService);

  @Input() clientId!: number;
  @Output() membershipCancelled = new EventEmitter<void>();

  membership = signal<GymMembership | null>(null);
  loading = signal(false);
  showCancelDialog = false;

  ngOnInit() {
    this.loadMembership();
  }

  loadMembership() {
    this.loading.set(true);
    this.gymMembershipService.getActiveMembership(this.clientId).subscribe({
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

  onCancelConfirmed(reason: string | null) {
    if (!this.membership()) return;

    this.loading.set(true);
    this.gymMembershipService.cancelMembership({
      membershipId: this.membership()!.id,
      cancellationReason: reason
    }).subscribe({
      next: (result) => {
        this.membership.set(result);
        this.showCancelDialog = false;
        this.loading.set(false);
        this.toastService.show('Membership cancelled successfully', 'success');
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
    return this.membership()!.isCancelled ? 'Cancelled' : 'Active';
  }

  getStatusSeverity(): 'success' | 'danger' | 'warning' | 'info' | 'secondary' | 'contrast' | undefined {
    if (!this.membership()) return 'info';
    return this.membership()!.isCancelled ? 'danger' : 'success';
  }

  toDate(dateStr: string): Date {
    return new Date(dateStr);
  }
}
