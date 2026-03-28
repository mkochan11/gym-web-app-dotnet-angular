import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { TooltipModule } from 'primeng/tooltip';
import { CardModule } from 'primeng/card';
import { TagModule } from 'primeng/tag';
import { DropdownModule } from 'primeng/dropdown';
import { InputTextModule } from 'primeng/inputtext';
import { DialogModule } from 'primeng/dialog';
import { ToastService } from '../../core/services/toast.service';
import { PaymentService } from '../../core/api-services/payment.service';
import { MembershipService } from '../../core/api-services/membership.service';
import { AuthService, ClientService } from '../../core/api-services';
import { Payment, PaymentMethod, PaymentResult } from '../../core/models/payment.model';

@Component({
  selector: 'app-payments',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    TableModule,
    ButtonModule,
    TooltipModule,
    CardModule,
    TagModule,
    DropdownModule,
    InputTextModule,
    DialogModule
  ],
  template: `
    <div class="container mx-auto p-4">
      <div class="mb-4">
        <div class="flex align-items-center gap-3 mb-2">
          <p-button 
            icon="pi pi-arrow-left" 
            [text]="true"
            pTooltip="Back to Membership"
            (onClick)="goBackToMembership()">
          </p-button>
          <div>
            <h1 class="text-3xl font-bold text-900 mb-2">My Payments</h1>
            <p class="text-500">View and pay your membership installments</p>
          </div>
        </div>
      </div>

      <p-card *ngIf="!loading()">
        <ng-template pTemplate="header">
          <div class="px-4 py-3 border-bottom-1 surface-border">
            <span class="text-xl font-semibold">{{ membership()?.planName || 'Membership' }}</span>
          </div>
        </ng-template>

        <div class="mb-3 flex justify-content-between align-items-center">
          <div>
            <span class="text-500">Membership Status: </span>
            <p-tag 
              [value]="membership()?.isActive ? 'Active' : 'Inactive'" 
              [severity]="membership()?.isActive ? 'success' : 'danger'">
            </p-tag>
          </div>
          <div class="text-500 text-sm">
            {{ membership()?.startDate | date:'mediumDate' }} - {{ membership()?.endDate | date:'mediumDate' }}
          </div>
        </div>

        <p-table 
          [value]="payments()" 
          [paginator]="payments().length > 10"
          [rows]="10"
          [rowsPerPageOptions]="[5, 10, 20]"
          styleClass="p-datatable-sm"
          responsiveLayout="scroll">
          <ng-template pTemplate="header">
            <tr>
              <th>#</th>
              <th>Due Date</th>
              <th>Amount</th>
              <th>Status</th>
              <th>Paid Date</th>
              <th>Method</th>
              <th>Actions</th>
            </tr>
          </ng-template>
          <ng-template pTemplate="body" let-payment let-i="rowIndex">
            <tr>
              <td>{{ i + 1 }}</td>
              <td>{{ payment.dueDate | date:'mediumDate' }}</td>
              <td>{{ payment.amount | currency:'EUR' }}</td>
              <td>
                <p-tag 
                  [value]="payment.status" 
                  [severity]="getStatusSeverity(payment.status)">
                </p-tag>
              </td>
              <td>{{ payment.paidDate ? (payment.paidDate | date:'mediumDate') : '-' }}</td>
              <td>{{ payment.paymentMethod }}</td>
              <td>
                <p-button 
                  *ngIf="payment.status === 'Pending' || payment.status === 'Overdue'"
                  label="Pay" 
                  icon="pi pi-credit-card" 
                  size="small"
                  (onClick)="openPaymentDialog(payment)">
                </p-button>
                <span *ngIf="payment.status === 'Paid'" class="text-500 text-sm">
                  <i class="pi pi-check-circle text-green-500 mr-1"></i> Paid
                </span>
              </td>
            </tr>
          </ng-template>
          <ng-template pTemplate="emptymessage">
            <tr>
              <td colspan="7" class="text-center text-500 py-4">
                No payments found
              </td>
            </tr>
          </ng-template>
        </p-table>
      </p-card>

      <div *ngIf="loading()" class="surface-card p-5 border-round shadow-2 text-center">
        <i class="pi pi-spin pi-spinner text-4xl"></i>
        <p class="text-500 mt-3">Loading payments...</p>
      </div>

      <p-dialog 
        header="Make Payment" 
        [(visible)]="showPaymentDialog" 
        [modal]="true" 
        [style]="{width: '450px'}"
        [draggable]="false"
        [resizable]="false">
        <div class="flex flex-column gap-3">
          <div class="field">
            <label class="block text-700 text-sm font-bold mb-2">Payment Amount</label>
            <div class="text-xl font-semibold">{{ selectedPayment()?.amount | currency:'EUR' }}</div>
          </div>

          <div class="field">
            <label class="block text-700 text-sm font-bold mb-2">Payment Method</label>
            <p-dropdown 
              [(ngModel)]="paymentMethod" 
              [options]="paymentMethods" 
              optionLabel="label" 
              optionValue="value"
              placeholder="Select payment method"
              styleClass="w-full">
            </p-dropdown>
          </div>

          <div class="field" *ngIf="paymentMethod !== paymentMethodEnum.Cash">
            <label class="block text-700 text-sm font-bold mb-2">Transaction ID</label>
            <input 
              pInputText 
              [(ngModel)]="transactionId" 
              class="w-full"
              placeholder="Enter transaction ID">
            <small class="text-500">Required for non-cash payments</small>
          </div>

          <div *ngIf="processing()" class="text-center py-3">
            <i class="pi pi-spin pi-spinner text-3xl"></i>
            <p class="text-500 mt-2">Processing payment...</p>
          </div>

          <div *ngIf="paymentError()" class="p-3 surface-100 border-round">
            <span class="text-red-500">{{ paymentError() }}</span>
          </div>
        </div>

        <ng-template pTemplate="footer">
          <div class="flex justify-content-end gap-2">
            <p-button 
              label="Cancel" 
              [text]="true" 
              severity="secondary"
              [disabled]="processing()"
              (onClick)="showPaymentDialog = false">
            </p-button>
            <p-button 
              label="Confirm Payment" 
              icon="pi pi-check"
              [disabled]="!canSubmitPayment() || processing()"
              (onClick)="submitPayment()">
            </p-button>
          </div>
        </ng-template>
      </p-dialog>
    </div>
  `
})
export class PaymentsComponent implements OnInit {
  private paymentService = inject(PaymentService);
  private membershipService = inject(MembershipService);
  private authService = inject(AuthService);
  private clientService = inject(ClientService);
  private toastService = inject(ToastService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);

  payments = signal<Payment[]>([]);
  membership = signal<any>(null);
  loading = signal(true);
  showPaymentDialog = false;
  selectedPayment = signal<Payment | null>(null);
  processing = signal(false);
  paymentError = signal<string>('');

  paymentMethod: PaymentMethod = PaymentMethod.Card;
  transactionId = '';

  paymentMethodEnum = PaymentMethod;

  paymentMethods = [
    { label: 'Card', value: PaymentMethod.Card },
    { label: 'Bank Transfer', value: PaymentMethod.BankTransfer },
    { label: 'Other', value: PaymentMethod.Other },
    { label: 'Cash (at reception)', value: PaymentMethod.Cash }
  ];

  ngOnInit() {
    this.loadData();
  }

  private loadData() {
    const state = this.router.getCurrentNavigation()?.extras.state;
    const membershipIdFromState = state?.['membershipId'];
    
    if (membershipIdFromState) {
      console.log('[Payments] Loading membership from state:', membershipIdFromState);
      this.loadPaymentsByMembershipId(membershipIdFromState);
      return;
    }

    const accountId = this.authService.getUserId();
    if (!accountId) {
      this.toastService.show('Please log in to view your payments', 'error');
      this.loading.set(false);
      return;
    }

    this.clientService.getClientByAccountId(accountId).subscribe({
      next: (client) => {
        if (!client) {
          this.toastService.show('Client profile not found', 'error');
          this.loading.set(false);
          return;
        }

        this.membershipService.getClientActiveMembership(client.id).subscribe({
          next: (membership) => {
            if (!membership) {
              this.toastService.show('No active membership found', 'info');
              this.loading.set(false);
              return;
            }

            this.membership.set(membership);
            this.loadPayments(membership.id);
          },
          error: () => {
            this.toastService.show('Failed to load membership', 'error');
            this.loading.set(false);
          }
        });
      },
      error: () => {
        this.toastService.show('Failed to load client', 'error');
        this.loading.set(false);
      }
    });
  }

  private loadPayments(membershipId: number) {
    console.log('[Payments] Loading payments for membership:', membershipId);
    this.paymentService.getPaymentsByMembership(membershipId).subscribe({
      next: (payments) => {
        console.log('[Payments] Payments loaded:', payments);
        this.payments.set(payments);
        this.loading.set(false);
      },
      error: (err) => {
        console.error('[Payments] Error loading payments:', err);
        this.toastService.show('Failed to load payments', 'error');
        this.loading.set(false);
      }
    });
  }

  private loadPaymentsByMembershipId(membershipId: number) {
    this.membershipService.getMembershipById(membershipId).subscribe({
      next: (membership) => {
        if (!membership) {
          this.toastService.show('Membership not found', 'error');
          this.loading.set(false);
          return;
        }
        this.membership.set(membership);
        this.loadPayments(membershipId);
      },
      error: (err) => {
        console.error('[Payments] Error loading membership:', err);
        this.toastService.show('Failed to load membership', 'error');
        this.loading.set(false);
      }
    });
  }

  openPaymentDialog(payment: Payment) {
    this.selectedPayment.set(payment);
    this.paymentMethod = PaymentMethod.Card;
    this.transactionId = '';
    this.paymentError.set('');
    this.showPaymentDialog = true;
  }

  canSubmitPayment(): boolean {
    if (this.paymentMethod === PaymentMethod.Cash) {
      return true;
    }
    return this.transactionId.trim().length > 0;
  }

  submitPayment() {
    const payment = this.selectedPayment();
    const membership = this.membership();
    if (!payment || !membership) return;

    if (this.paymentMethod !== PaymentMethod.Cash && !this.transactionId.trim()) {
      this.paymentError.set('Transaction ID is required for non-cash payments');
      return;
    }

    this.processing.set(true);
    this.paymentError.set('');

    this.paymentService.processPayment({
      membershipId: membership.id,
      paymentId: payment.id,
      paymentMethod: this.paymentMethod,
      transactionId: this.paymentMethod === PaymentMethod.Cash ? null : this.transactionId.trim()
    }).subscribe({
      next: (result: PaymentResult) => {
        this.processing.set(false);
        if (result.success) {
          this.toastService.show('Payment successful!', 'success');
          this.showPaymentDialog = false;
          this.loadData();
        } else {
          this.paymentError.set(result.message);
        }
      },
      error: (err) => {
        this.processing.set(false);
        this.paymentError.set(err.message || 'Payment failed');
      }
    });
  }

  getStatusSeverity(status: string): 'success' | 'danger' | 'warning' | 'info' | 'secondary' | 'contrast' | undefined {
    switch (status) {
      case 'Paid': return 'success';
      case 'Pending': return 'info';
      case 'Overdue': return 'danger';
      case 'Cancelled': return 'secondary';
      default: return 'info';
    }
  }

  goBackToMembership() {
    this.router.navigate(['/client/membership']);
  }
}