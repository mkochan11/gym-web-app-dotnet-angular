// membership-selection.component.ts
import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { TagModule } from 'primeng/tag';

import { MembershipService, MembershipPlan } from '../../core/api-services';

@Component({
  selector: 'app-membership-selection',
  standalone: true,
  imports: [CommonModule, ButtonModule, CardModule, TagModule],
  templateUrl: './membership-selection.component.html',
  styleUrls: ['./membership-selection.component.scss']
})
export class MembershipSelectionComponent {
  private membershipService = inject(MembershipService);
  private router = inject(Router);

  membershipPlans = this.membershipService.getMembershipPlans();
  selectedPlan: MembershipPlan | null = null;

  selectPlan(plan: MembershipPlan) {
    this.selectedPlan = plan;
  }

  continueToPayment() {
    if (this.selectedPlan) {
      this.membershipService.setSelectedPlan(this.selectedPlan);
      this.router.navigate(['/membership/payment']);
    }
  }

  skipPurchase() {
    this.membershipService.clearSelectedPlan();
    this.router.navigate(['/client']);
  }
}