export interface GymMembership {
  id: number;
  clientId: number;
  clientName: string;
  membershipPlanId: number;
  planName: string;
  planDescription: string;
  planPrice: number;
  planDurationInMonths: number;
  canReserveTrainings: boolean;
  canAccessGroupTraining: boolean;
  canAccessPersonalTraining: boolean;
  canReceiveTrainingPlans: boolean;
  maxTrainingsPerMonth: number | null;
  startDate: string;
  endDate: string;
  status: number;
  isActive: boolean;
  isCancelled: boolean;
  cancelledAt: string | null;
  cancellationRequestedDate: string | null;
  effectiveEndDate: string | null;
  cancellationReason: string | null;
}

export interface CancelMembershipRequest {
  cancellationReason: string | null;
}

export interface ChangePlanRequest {
  newPlanId: number;
}

export interface CreditCalculation {
  unusedDays: number;
  creditAmount: number;
  newMonthlyAmount: number;
  totalDifference: number;
  firstPaymentAmount: number;
  currentPlanId: number;
  newPlanId: number;
  currentPlanName: string;
  newPlanName: string;
  currentPlanPrice: number;
  newPlanPrice: number;
  isUpgrade: boolean;
  currentPlanCanReserveTrainings: boolean;
  newPlanCanReserveTrainings: boolean;
  currentPlanCanAccessGroupTraining: boolean;
  newPlanCanAccessGroupTraining: boolean;
  currentPlanCanAccessPersonalTraining: boolean;
  newPlanCanAccessPersonalTraining: boolean;
  currentPlanCanReceiveTrainingPlans: boolean;
  newPlanCanReceiveTrainingPlans: boolean;
  currentPlanMaxTrainingsPerMonth: number | null;
  newPlanMaxTrainingsPerMonth: number | null;
}
