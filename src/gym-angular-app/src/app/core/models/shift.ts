import { Employee } from "./employee";

export interface Shift {
    id: number;
    startDate: string;
    endDate: string;
    employee: Employee;
    status: string;
}
