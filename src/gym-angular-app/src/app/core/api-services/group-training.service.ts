import { Injectable, inject } from '@angular/core';
import { Observable, map } from 'rxjs';
import { HttpService } from './http.service';
import { GroupTraining } from '../models/group-training';
import { CalendarFilters } from '../models/shared-calendar/calendar-filters';

@Injectable({
  providedIn: 'root'
})
export class GroupTrainingService {
  private httpService = inject(HttpService);

  getAllGroupTrainings(): Observable<GroupTraining[]> {
    return this.httpService.get<GroupTraining[]>('trainings/group');
  }

  cancelGroupTraining(trainingId: number, reason: string): Observable<any> {
    const cancelDto = {
      cancellationReason: reason
    };
    return this.httpService.post<any>(`trainings/group/${trainingId}/cancel`, cancelDto);
  }

  getGroupTrainingsFiltered(filters?: CalendarFilters): Observable<GroupTraining[]> {
    let url = 'trainings/group/filtered';
    const params = this.buildFilterParams(filters);
    
    if (params) {
      url += `?${params}`;
    }
    
    return this.httpService.get<GroupTraining[]>(url);
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
      params.push(`TrainerIds=${filters.employeeIds.join(',')}`);
    }

    if (filters.trainingTypeIds?.length) {
      params.push(`TrainingTypeIds=${filters.trainingTypeIds.join(',')}`);
    }

    return params.join('&');
  }
}