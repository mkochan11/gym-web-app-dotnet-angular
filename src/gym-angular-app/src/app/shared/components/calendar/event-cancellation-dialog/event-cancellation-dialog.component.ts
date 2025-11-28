import { Component, EventEmitter, Input, Output, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DialogModule } from 'primeng/dialog';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { FormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';

@Component({
  selector: 'app-event-cancellation-dialog',
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
      header="Cancel Event" 
      [(visible)]="visible" 
      [style]="{ width: '500px' }"
      [modal]="true"
      [closable]="!loading"
      (onHide)="onHide()">
      
      <div class="cancel-dialog-content">
        <p>Are you sure you want to cancel <strong>{{ eventTitle }}</strong>?</p>
        
        <div class="field mt-3">
          <label for="cancellationReason" class="block text-sm font-medium mb-2">
            Cancellation Reason <span class="text-red-500">*</span>
          </label>
          <textarea 
            id="cancellationReason"
            pInputTextarea
            [(ngModel)]="cancellationReason"
            rows="4"
            placeholder="Please provide a reason for cancellation..."
            class="w-full"
            [disabled]="loading"
            [class]="{'p-invalid': showValidationError && !cancellationReason.trim()}">
          </textarea>
          <small 
            *ngIf="showValidationError && !cancellationReason.trim()" 
            class="p-error block mt-1">
            Cancellation reason is required
          </small>
        </div>
      </div>

      <ng-template pTemplate="footer">
        <button 
          pButton 
          icon="pi pi-times" 
          label="Cancel" 
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
export class EventCancellationDialogComponent {
  @Input() visible: boolean = false;
  @Input() eventTitle: string = '';
  @Input() loading: boolean = false;

  @Output() visibleChange = new EventEmitter<boolean>();
  @Output() confirmed = new EventEmitter<string>();
  @Output() cancelled = new EventEmitter<void>();

  cancellationReason: string = '';
  showValidationError: boolean = false;

  onConfirm() {
    if (!this.cancellationReason.trim()) {
      this.showValidationError = true;
      return;
    }

    this.confirmed.emit(this.cancellationReason.trim());
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
    this.cancellationReason = '';
    this.showValidationError = false;
  }

  reset() {
    this.resetForm();
  }
}