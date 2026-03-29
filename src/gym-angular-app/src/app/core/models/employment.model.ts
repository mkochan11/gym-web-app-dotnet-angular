export interface Employment {
  id: number;
  startDate: Date;
  endDate?: Date;
  hourlyRate: number;
  createdAt: Date;
  createdBy: string;
  status: 'Active' | 'Ended';
}

export interface EmployeeWithEmployments {
  id: number;
  firstName: string;
  lastName: string;
  role: string;
  employments: Employment[];
}
