export interface Client {
    id: number;
    firstName: string;
    lastName: string;
}

export interface ClientListItem {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  membershipStatus: 'Active' | 'Expired' | 'Cancelled' | 'None';
  currentPlanName?: string;
}

export interface ClientMembership {
  id: number;
  planName: string;
  planDescription: string;
  status: 'Active' | 'Expired' | 'Cancelled';
  startDate: Date;
  endDate: Date;
  price: number;
  canAccessGroupTraining: boolean;
  canAccessPersonalTraining: boolean;
}

export interface ClientDetails {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber?: string;
  dateOfBirth?: Date;
  registrationDate: Date;
  address?: string;
  currentMembership?: ClientMembership;
}
