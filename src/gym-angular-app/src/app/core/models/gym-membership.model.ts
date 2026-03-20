export interface GymMembership {
  id: number;
  clientId: number;
  clientName: string;
  membershipPlanId: number;
  planName: string;
  planDescription: string;
  planPrice: number;
  planDurationInMonths: number;
  startDate: string;
  endDate: string;
  isActive: boolean;
  isCancelled: boolean;
  cancelledAt: string | null;
  cancellationReason: string | null;
}

export interface CancelMembershipRequest {
  membershipId: number;
  cancellationReason: string | null;
}
