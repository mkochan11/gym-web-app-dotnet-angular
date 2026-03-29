import { Component, Inject, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DialogModule } from 'primeng/dialog';
import { ButtonModule } from 'primeng/button';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { CardModule } from 'primeng/card';
import { ToastModule } from 'primeng/toast';
import { MessageService } from 'primeng/api';
import { DynamicDialogRef, DynamicDialogConfig } from 'primeng/dynamicdialog';
import { MembershipService } from '../../../core/api-services/membership.service';
import { ClientListItem } from '../../../core/models/client';
import { take } from 'rxjs';

@Component({
  selector: 'app-client-cancel-dialog',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    DialogModule,
    ButtonModule,
    InputTextareaModule,
    CardModule,
    ToastModule
  ],
  providers: [MessageService],
  templateUrl: './client-cancel-dialog.component.html',
  styleUrls: ['./client-cancel-dialog.component.scss']
})
export class ClientCancelDialogComponent implements OnInit {
  private membershipService = inject(MembershipService);
  private messageService = inject(MessageService);

  client!: ClientListItem;
  cancellationReason = '';
  effectiveEndDate: Date | null = null;

  loading = false;
  cancellationComplete = false;

  constructor(
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig
  ) {
    this.client = config.data.client;
  }

  ngOnInit() {
    this.loadMembershipDetails();
  }

  loadMembershipDetails() {
    this.loading = true;
    this.membershipService.getClientActiveMembership(this.client.id)
      .pipe(take(1))
      .subscribe({
        next: (membership) => {
          if (membership && membership.effectiveEndDate) {
            this.effectiveEndDate = new Date(membership.effectiveEndDate);
          }
          this.loading = false;
        },
        error: () => {
          this.messageService.add({
            severity: 'error',
            summary: 'Error',
            detail: 'Failed to load membership details'
          });
          this.loading = false;
        }
      });
  }

  canCancel(): boolean {
    return this.cancellationReason.trim().length > 0;
  }

  cancelMembership() {
    if (!this.canCancel()) return;

    this.loading = true;

    this.membershipService.getClientActiveMembership(this.client.id)
      .pipe(take(1))
      .subscribe({
        next: (membership) => {
          if (!membership) {
            this.messageService.add({
              severity: 'error',
              summary: 'Error',
              detail: 'No active membership found'
            });
            this.loading = false;
            return;
          }

          this.membershipService.cancelMembership(membership.id, {
            cancellationReason: this.cancellationReason.trim()
          })
            .pipe(take(1))
            .subscribe({
              next: () => {
                this.cancellationComplete = true;
                this.loading = false;
                this.messageService.add({
                  severity: 'success',
                  summary: 'Success',
                  detail: 'Membership cancelled successfully'
                });
              },
              error: (error) => {
                this.loading = false;
                this.messageService.add({
                  severity: 'error',
                  summary: 'Error',
                  detail: error.message || 'Failed to cancel membership'
                });
              }
            });
        },
        error: () => {
          this.loading = false;
          this.messageService.add({
            severity: 'error',
            summary: 'Error',
            detail: 'Failed to retrieve membership'
          });
        }
      });
  }

  close() {
    this.ref.close(this.cancellationComplete);
  }

  formatDate(date: Date | string | undefined): string {
    if (!date) return 'N/A';
    return new Date(date).toLocaleDateString('pl-PL');
  }
}
