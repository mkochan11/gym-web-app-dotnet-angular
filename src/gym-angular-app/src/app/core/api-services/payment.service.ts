import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpService } from './http.service';
import { Payment, PaymentResult, ProcessPaymentRequest, ProcessMultiplePaymentsRequest, AcceptPaymentRequest, ClientPaymentSchedule } from '../models/payment.model';

@Injectable({
  providedIn: 'root'
})
export class PaymentService {
  private httpService = inject(HttpService);

  getPaymentsByMembership(membershipId: number): Observable<Payment[]> {
    return this.httpService.get<Payment[]>(`payments/membership/${membershipId}`);
  }

  processPayment(request: ProcessPaymentRequest): Observable<PaymentResult> {
    return this.httpService.post<PaymentResult>('payments/process', request);
  }

  processMultiplePayments(request: ProcessMultiplePaymentsRequest): Observable<PaymentResult[]> {
    return this.httpService.post<PaymentResult[]>('payments/process-multiple', request);
  }

  getClientPaymentSchedule(clientId: number): Observable<ClientPaymentSchedule> {
    return this.httpService.get<ClientPaymentSchedule>(`payments/client/${clientId}/schedule`);
  }

  acceptPayment(request: AcceptPaymentRequest): Observable<PaymentResult> {
    return this.httpService.post<PaymentResult>('payments/accept', request);
  }
}