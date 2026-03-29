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
import { ToastService } from '../../../core/services/toast.service';
import { PaymentService } from '../../../core/api-services/payment.service';
import { AuthService } from '../../../core/api-services';
import { Payment, PaymentMethod, PaymentResult, ClientPaymentSchedule } from '../../../core/models/payment.model';

@Component({
  selector: 'app-client-payments',
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
            pTooltip="Back to Clients"
            (onClick)="goBackToClients()">
          </p-button>
          <div>
            <h1 class="text-3xl font-bold text-900 mb-2">Client Payments</h1>
            <p class="text-500" *ngIf="clientSchedule()">
              {{ clientSchedule()?.clientName }} {{ clientSchedule()?.clientSurname }}
            </p>
          </div>
        </div>
      </div>

      <p-card *ngIf="!loading() && clientSchedule()">
        <ng-template pTemplate="header">
          <div class="px-4 py-3 border-bottom-1 surface-border flex justify-content-between align-items-center">
            <span class="text-xl font-semibold">{{ clientSchedule()?.planName || 'No Membership' }}</span>
            <p-tag 
              *ngIf="clientSchedule()?.membershipId"
              [value]="clientSchedule()?.isActive ? 'Active' : 'Inactive'" 
              [severity]="clientSchedule()?.isActive ? 'success' : 'danger'">
            </p-tag>
          </div>
        </ng-template>

        <div *ngIf="clientSchedule()?.membershipId" class="mb-3 flex justify-content-between align-items-center">
          <div class="text-500 text-sm">
            {{ clientSchedule()?.startDate | date:'mediumDate' }} - {{ clientSchedule()?.endDate | date:'mediumDate' }}
          </div>
          <div class="flex gap-2">
            <p-tag value="Paid: {{ clientSchedule()?.paidPayments }}" severity="success"></p-tag>
            <p-tag value="Pending: {{ clientSchedule()?.pendingPayments }}" severity="info"></p-tag>
            <p-tag value="Overdue: {{ clientSchedule()?.overduePayments }}" severity="danger"></p-tag>
          </div>
        </div>

        <p-table 
          *ngIf="clientSchedule()?.membershipId"
          [value]="clientSchedule()?.payments || []" 
          [paginator]="(clientSchedule()?.payments?.length || 0) > 10"
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

        <div *ngIf="!clientSchedule()?.membershipId" class="text-center py-5">
          <i class="pi pi-info-circle text-4xl text-500 mb-3"></i>
          <p class="text-500">No active membership found for this client.</p>
        </div>
      </p-card>

      <div *ngIf="loading()" class="surface-card p-5 border-round shadow-2 text-center">
        <i class="pi pi-spin pi-spinner text-4xl"></i>
        <p class="text-500 mt-3">Loading payments...</p>
      </div>

      <p-dialog 
        header="Accept Payment" 
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
export class ClientPaymentsComponent implements OnInit {
  private paymentService = inject(PaymentService);
  private authService = inject(AuthService);
  private toastService = inject(ToastService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);

  clientSchedule = signal<ClientPaymentSchedule | null>(null);
  loading = signal(true);
  showPaymentDialog = false;
  selectedPayment = signal<Payment | null>(null);
  processing = signal(false);
  paymentError = signal<string>('');
  clientId = signal<number>(0);

  paymentMethod: PaymentMethod = PaymentMethod.Cash;
  transactionId = '';

  paymentMethodEnum = PaymentMethod;

  paymentMethods = [
    { label: 'Cash', value: PaymentMethod.Cash },
    { label: 'Card', value: PaymentMethod.Card },
    { label: 'Bank Transfer', value: PaymentMethod.BankTransfer },
    { label: 'Other', value: PaymentMethod.Other }
  ];

  ngOnInit() {
    const clientIdParam = this.route.snapshot.paramMap.get('clientId');
    if (clientIdParam) {
      this.clientId.set(parseInt(clientIdParam, 10));
      this.loadData();
    } else {
      this.toastService.show('Client ID not provided', 'error');
      this.loading.set(false);
    }
  }

  private loadData() {
    const clientId = this.clientId();
    if (!clientId) {
      this.loading.set(false);
      return;
    }

    this.loading.set(true);
    this.paymentService.getClientPaymentSchedule(clientId).subscribe({
      next: (schedule) => {
        this.clientSchedule.set(schedule);
        this.loading.set(false);
      },
      error: (err) => {
        console.error('[ClientPayments] Error loading schedule:', err);
        this.toastService.show('Failed to load client payment schedule', 'error');
        this.loading.set(false);
      }
    });
  }

  openPaymentDialog(payment: Payment) {
    this.selectedPayment.set(payment);
    this.paymentMethod = PaymentMethod.Cash;
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
    const clientId = this.clientId();
    if (!payment || !clientId) return;

    if (this.paymentMethod !== PaymentMethod.Cash && !this.transactionId.trim()) {
      this.paymentError.set('Transaction ID is required for non-cash payments');
      return;
    }

    this.processing.set(true);
    this.paymentError.set('');

    this.paymentService.acceptPayment({
      clientId: clientId,
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

  goBackToClients() {
    const userRole = this.authService.getRole();
    if (userRole === 'Manager' || userRole === 'Admin') {
      this.router.navigate(['/management/manager/clients']);
    } else {
      this.router.navigate(['/management/receptionist/clients']);
    }
  }
}
