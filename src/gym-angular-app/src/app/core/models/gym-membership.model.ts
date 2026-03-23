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
