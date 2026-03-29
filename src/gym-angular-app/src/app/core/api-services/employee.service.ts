import { inject, Injectable } from '@angular/core';
import { Employee } from '../models/employee';
import { EmployeeWithEmployments } from '../models/employment.model';
import { Observable } from 'rxjs';
import { HttpService } from './http.service';

@Injectable({
  providedIn: 'root'
})
export class EmployeeService {
  private httpService = inject(HttpService);

  getAllEmployees(): Observable<Employee[]> {
    return this.httpService.get<Employee[]>('employees');
  }

  getEmployeeEmployments(employeeId: number): Observable<EmployeeWithEmployments> {
    return this.httpService.get<EmployeeWithEmployments>(`employees/${employeeId}/employments`);
  }

}
