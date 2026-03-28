import { Injectable, inject, signal } from '@angular/core';
import { Observable, tap, catchError, map, of, throwError } from 'rxjs';
import { HttpService } from './http.service';
import { AuthService } from './auth.service';
import { MembershipPlanService } from './membership-plan.service';
import { GymMembership, CancelMembershipRequest, ChangePlanRequest, CreditCalculation } from '../models/gym-membership.model';
import { MembershipPlan as MembershipPlanModel } from '../models/membership-plan.model';

export interface PaymentData {
  cardNumber: string;
  expiryDate: string;
  cvv: string;
  cardholderName: string;
}

export interface PaymentResult {
  success: boolean;
  message: string;
  membershipId?: number;
  paymentId?: number;
  amount?: number;
  paymentMethod?: string;
  startDate?: string;
  endDate?: string;
  planName?: string;
}

export interface PurchaseMembershipRequest {
  membershipPlanId: number;
  clientId: number;
}

@Injectable({
  providedIn: 'root'
})
export class MembershipService {
  private httpService = inject(HttpService);
  private authService = inject(AuthService);
  private membershipPlanService = inject(MembershipPlanService);

  private selectedPlanSignal = signal<MembershipPlanModel | null>(null);
  readonly selectedPlan = this.selectedPlanSignal.asReadonly();

  private loadingSignal = signal(false);
  readonly loading = this.loadingSignal.asReadonly();

  private errorSignal = signal<string | null>(null);
  readonly error = this.errorSignal.asReadonly();

  getMembershipPlans(): Observable<MembershipPlanModel[]> {
    return this.membershipPlanService.getMembershipPlans();
  }

  getActivePlans(): Observable<MembershipPlanModel[]> {
    return this.membershipPlanService.getMembershipPlans().pipe(
      map(plans => plans.filter(p => p.isActive))
    );
  }

  setSelectedPlan(plan: MembershipPlanModel) {
    this.selectedPlanSignal.set(plan);
    this.errorSignal.set(null);
  }

  getSelectedPlan(): MembershipPlanModel | null {
    return this.selectedPlanSignal();
  }

  clearSelectedPlan() {
    this.selectedPlanSignal.set(null);
    this.errorSignal.set(null);
  }

  purchaseMembership(planId: number, clientId: number): Observable<GymMembership> {
    this.loadingSignal.set(true);
    this.errorSignal.set(null);

    const request: PurchaseMembershipRequest = {
      membershipPlanId: planId,
      clientId: clientId
    };

    return this.httpService.post<GymMembership>('gym-memberships/purchase', request).pipe(
      tap(membership => {
        this.loadingSignal.set(false);
      }),
      catchError(error => {
        this.loadingSignal.set(false);
        this.errorSignal.set(error.message || 'Failed to purchase membership');
        return throwError(() => error);
      })
    );
  }

  processPayment(membershipId: number, paymentData: PaymentData): Observable<PaymentResult> {
    this.loadingSignal.set(true);
    this.errorSignal.set(null);

    const request = {
      membershipId: membershipId,
      cardNumber: paymentData.cardNumber,
      expiryDate: paymentData.expiryDate,
      cvv: paymentData.cvv,
      cardholderName: paymentData.cardholderName
    };

    return this.httpService.post<PaymentResult>('payments/process', request).pipe(
      tap(result => {
        this.loadingSignal.set(false);
      }),
      catchError(error => {
        this.loadingSignal.set(false);
        this.errorSignal.set(error.message || 'Failed to process payment');
        return of({
          success: false,
          message: error.message || 'An unexpected error occurred'
        });
      })
    );
  }

  getClientActiveMembership(clientId: number): Observable<GymMembership | null> {
    return this.httpService.get<GymMembership>(`gym-memberships/client/${clientId}/active`).pipe(
      catchError(() => of(null))
    );
  }

  getMembershipById(id: number): Observable<GymMembership> {
    return this.httpService.get<GymMembership>(`gym-memberships/${id}`);
  }

  getClientMemberships(clientId: number): Observable<GymMembership[]> {
    return this.httpService.get<GymMembership[]>(`gym-memberships/client/${clientId}`);
  }

  cancelMembership(membershipId: number, request: CancelMembershipRequest): Observable<GymMembership> {
    return this.httpService.post<GymMembership>(`gym-memberships/${membershipId}/cancel`, request);
  }

  revertCancellation(membershipId: number): Observable<GymMembership> {
    return this.httpService.post<GymMembership>(`gym-memberships/${membershipId}/cancel/revert`, {});
  }

  getAvailablePlans(membershipId: number): Observable<MembershipPlanModel[]> {
    return this.httpService.get<MembershipPlanModel[]>(`gym-memberships/${membershipId}/available-plans`);
  }

  calculateCredit(membershipId: number, newPlanId: number): Observable<CreditCalculation> {
    return this.httpService.get<CreditCalculation>(`gym-memberships/${membershipId}/calculate-credit?newPlanId=${newPlanId}`);
  }

  changePlan(membershipId: number, request: ChangePlanRequest): Observable<GymMembership> {
    return this.httpService.post<GymMembership>(`gym-memberships/${membershipId}/change-plan`, request);
  }
}