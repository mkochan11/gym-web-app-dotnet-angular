import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DialogModule } from 'primeng/dialog';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { FormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';

@Component({
  selector: 'app-membership-cancel-dialog',
  standalone: true,
  imports: [
    CommonModule,
    DialogModule,
    InputTextareaModule,
    FormsModule,
    ButtonModule
  ],
  template: `
    <p-dialog 
      header="Cancel Membership" 
      [(visible)]="visible" 
      [style]="{ width: '500px' }"
      [modal]="true"
      [closable]="!loading"
      (onHide)="onHide()">
      
      <div class="cancel-dialog-content">
        <p class="mb-3">Are you sure you want to cancel your <strong>{{ planName }}</strong> membership?</p>
        
        <div class="text-sm text-500 mb-3">
          <i class="pi pi-info-circle mr-2"></i>
          Your membership will remain active until {{ endDate | date:'mediumDate' }}.
        </div>
        
        <div class="field mt-4">
          <label for="cancellationReason" class="block text-sm font-medium mb-2">
            Reason for Cancellation <span class="text-400">(optional)</span>
          </label>
          <textarea 
            id="cancellationReason"
            pInputTextarea
            [(ngModel)]="cancellationReason"
            rows="4"
            placeholder="Please let us know why you're cancelling..."
            class="w-full"
            [disabled]="loading"
            maxlength="500">
          </textarea>
          <small class="text-400">{{ cancellationReason?.length || 0 }}/500</small>
        </div>
      </div>

      <ng-template pTemplate="footer">
        <button 
          pButton 
          icon="pi pi-times" 
          label="Keep Membership" 
          class="p-button-text" 
          (click)="onClose()"
          [disabled]="loading">
        </button>
        <button 
          pButton 
          icon="pi pi-check" 
          label="Confirm Cancellation" 
          class="p-button-danger" 
          (click)="onConfirm()"
          [loading]="loading">
        </button>
      </ng-template>
    </p-dialog>
  `,
  styles: [`
    .cancel-dialog-content {
      line-height: 1.6;
    }
    
    :host ::ng-deep .p-button-danger {
      background-color: #ef4444;
      border-color: #ef4444;
      
      &:hover {
        background-color: #dc2626;
        border-color: #dc2626;
      }
    }
  `]
})
export class MembershipCancelDialogComponent {
  @Input() visible: boolean = false;
  @Input() planName: string = '';
  @Input() endDate: Date | null = null;
  @Input() loading: boolean = false;

  @Output() visibleChange = new EventEmitter<boolean>();
  @Output() confirmed = new EventEmitter<string | null>();
  @Output() cancelled = new EventEmitter<void>();

  cancellationReason: string | null = null;

  onConfirm() {
    this.confirmed.emit(this.cancellationReason);
  }

  onClose() {
    this.cancelled.emit();
    this.closeDialog();
  }

  onHide() {
    this.closeDialog();
  }

  private closeDialog() {
    this.visible = false;
    this.visibleChange.emit(false);
    this.resetForm();
  }

  private resetForm() {
    this.cancellationReason = null;
  }

  reset() {
    this.resetForm();
  }
}
