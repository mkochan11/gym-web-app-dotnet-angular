import { inject, Injectable } from '@angular/core';
import { HttpService } from './http.service';
import { Observable } from 'rxjs';
import { Shift } from '../models/shift';
import { CalendarFilters } from '../models/shared-calendar/calendar-filters';

@Injectable({
  providedIn: 'root'
})
export class ShiftService {
  private httpService = inject(HttpService);

  getAllShifts(): Observable<Shift[]> {
    return this.httpService.get<Shift[]>('shifts');
  }

  cancelShift(shiftId: number, reason: string): Observable<any> {
    const cancelDto = {
      cancellationReason: reason
    };
    return this.httpService.post<any>(`shifts/${shiftId}/cancel`, cancelDto);
  }

  deleteShift(shiftId: number): Observable<any> {
    return this.httpService.delete<any>(`shifts/${shiftId}`);
  }

  restoreShift(shiftId: number): Observable<any> {
    return this.httpService.post<any>(`shifts/${shiftId}/restore`, {});
  }

  createShift(shiftData: any): Observable<any>{
    const createDto = {
      employeeId: shiftData.formData.employeeId,
      startTime: shiftData.formData.startTime.toISOString(),
      endTime: shiftData.formData.endTime.toISOString(),
    }

    return this.httpService.post<any>(`shifts`, createDto);
  }

  getShiftsFiltered(filters?: CalendarFilters): Observable<Shift[]> {
    let url = 'shifts/filtered';
    const params = this.buildFilterParams(filters);
    
    if (params) {
      url += `?${params}`;
    }
    
    return this.httpService.get<Shift[]>(url);
  }

  private buildFilterParams(filters?: CalendarFilters): string {
    if (!filters) return '';

    const params: string[] = [];

    if (filters.startDate) {
      params.push(`StartDate=${filters.startDate.toISOString()}`);
    }

    if (filters.endDate) {
      params.push(`EndDate=${filters.endDate.toISOString()}`);
    }

    if (filters.employeeIds?.length) {
      params.push(`EmployeeIds=${filters.employeeIds.join(',')}`);
    }

    return params.join('&');
  }
}