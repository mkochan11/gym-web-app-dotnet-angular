import { Injectable, inject } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { HttpService } from './http.service';
import { AuthService } from './auth.service';
import { catchError, map, throwError } from 'rxjs';

export interface MembershipPlan {
  id: number;
  type: string;
  description: string;
  price: number;
  durationInMonths: number;
  features: string[];
  popular?: boolean;
}

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

@Injectable({
  providedIn: 'root'
})
export class MembershipService {
  private httpService = inject(HttpService);
  private authService = inject(AuthService);

  private selectedPlanSubject = new BehaviorSubject<MembershipPlan | null>(null);
  public selectedPlan$ = this.selectedPlanSubject.asObservable();

  private membershipPlans: MembershipPlan[] = [
    {
      id: 1,
      type: 'Basic',
      description: 'Perfect for beginners',
      price: 29.99,
      durationInMonths: 1,
      features: [
        'Access to gym equipment',
        'Locker room access',
        'Free WiFi'
      ]
    },
    {
      id: 2,
      type: 'Premium',
      description: 'Our most popular plan',
      price: 49.99,
      durationInMonths: 1,
      features: [
        'All Basic features',
        'Group classes access',
        'Personal trainer consultation',
        'Nutrition plan'
      ],
      popular: true
    },
    {
      id: 3,
      type: 'Ultimate',
      description: 'For serious fitness enthusiasts',
      price: 79.99,
      durationInMonths: 1,
      features: [
        'All Premium features',
        'Unlimited personal training',
        'Massage therapy sessions',
        'Priority booking',
        'Supplement discounts'
      ]
    }
  ];

  getMembershipPlans(): MembershipPlan[] {
    return this.membershipPlans;
  }

  setSelectedPlan(plan: MembershipPlan) {
    this.selectedPlanSubject.next(plan);
  }

  getSelectedPlan(): MembershipPlan | null {
    return this.selectedPlanSubject.value;
  }

  clearSelectedPlan() {
    this.selectedPlanSubject.next(null);
  }

  processPayment(paymentData: PaymentData, plan: MembershipPlan): Promise<PaymentResult> {
    const request = {
      membershipPlanId: plan.id,
      cardNumber: paymentData.cardNumber,
      expiryDate: paymentData.expiryDate,
      cvv: paymentData.cvv,
      cardholderName: paymentData.cardholderName
    };

    return new Promise((resolve) => {
      this.httpService.post<PaymentResult>('payments/process', request)
        .pipe(
          map(response => {
            if (response.success) {
              resolve({
                success: true,
                message: response.message,
                membershipId: response.membershipId,
                paymentId: response.paymentId,
                amount: response.amount,
                paymentMethod: response.paymentMethod,
                startDate: response.startDate,
                endDate: response.endDate,
                planName: response.planName
              });
            } else {
              resolve({
                success: false,
                message: response.message
              });
            }
          }),
          catchError(error => {
            resolve({
              success: false,
              message: error.message || 'An unexpected error occurred'
            });
            return throwError(() => error);
          })
        )
        .subscribe();
    });
  }
}