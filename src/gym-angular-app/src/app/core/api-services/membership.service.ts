import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

export interface MembershipPlan {
  id: string;
  name: string;
  description: string;
  price: number;
  duration: number;
  features: string[];
  popular?: boolean;
}

export interface PaymentData {
  cardNumber: string;
  expiryDate: string;
  cvv: string;
  cardholderName: string;
}

@Injectable({
  providedIn: 'root'
})
export class MembershipService {
  private selectedPlanSubject = new BehaviorSubject<MembershipPlan | null>(null);
  public selectedPlan$ = this.selectedPlanSubject.asObservable();

  private membershipPlans: MembershipPlan[] = [
    {
      id: 'basic',
      name: 'Basic',
      description: 'Perfect for beginners',
      price: 29.99,
      duration: 30,
      features: [
        'Access to gym equipment',
        'Locker room access',
        'Free WiFi'
      ]
    },
    {
      id: 'premium',
      name: 'Premium',
      description: 'Our most popular plan',
      price: 49.99,
      duration: 30,
      features: [
        'All Basic features',
        'Group classes access',
        'Personal trainer consultation',
        'Nutrition plan'
      ],
      popular: true
    },
    {
      id: 'ultimate',
      name: 'Ultimate',
      description: 'For serious fitness enthusiasts',
      price: 79.99,
      duration: 30,
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

  processPayment(paymentData: PaymentData, plan: MembershipPlan): Promise<{ success: boolean; message: string }> {
    return new Promise((resolve) => {
      setTimeout(() => {
        const success = Math.random() > 0.2;
        if (success) {
          resolve({
            success: true,
            message: `Payment processed successfully! You now have ${plan.name} membership.`
          });
        } else {
          resolve({
            success: false,
            message: 'Payment failed. Please check your card details and try again.'
          });
        }
      }, 2000);
    });
  }
}