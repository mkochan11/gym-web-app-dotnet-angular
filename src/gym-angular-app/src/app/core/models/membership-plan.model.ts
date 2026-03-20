export interface MembershipPlan {
  id: number;
  type: string;
  description: string;
  price: number;
  durationTime: string;
  durationInMonths: number;
  canReserveTrainings: boolean;
  canAccessGroupTraining: boolean;
  canAccessPersonalTraining: boolean;
  canReceiveTrainingPlans: boolean;
  maxTrainingsPerMonth: number | null;
  isActive: boolean;
  createdAt: string;
  updatedAt: string | null;
}

export interface CreateMembershipPlanRequest {
  type: string;
  description: string;
  price: number;
  durationTime: string;
  durationInMonths: number;
  canReserveTrainings: boolean;
  canAccessGroupTraining: boolean;
  canAccessPersonalTraining: boolean;
  canReceiveTrainingPlans: boolean;
  maxTrainingsPerMonth: number | null;
  isActive: boolean;
}

export interface UpdateMembershipPlanRequest {
  id: number;
  type: string;
  description: string;
  price: number;
  durationTime: string;
  durationInMonths: number;
  canReserveTrainings: boolean;
  canAccessGroupTraining: boolean;
  canAccessPersonalTraining: boolean;
  canReceiveTrainingPlans: boolean;
  maxTrainingsPerMonth: number | null;
  isActive: boolean;
}
