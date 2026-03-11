import { inject, Injectable } from '@angular/core';
import { HttpService } from './http.service';
import { IndividualTraining } from '../models/individual-training';
import { Observable } from 'rxjs';
import { CalendarFilters } from '../models/shared-calendar/calendar-filters';

@Injectable({
  providedIn: 'root'
})
export class IndividualTrainingService {
  private httpService = inject(HttpService);

  getAllIndividualTrainings(): Observable<IndividualTraining[]> {
    return this.httpService.get<IndividualTraining[]>('trainings/individual');
  }

  cancelIndividualTraining(trainingId: number, reason: string): Observable<any> {
    const cancelDto = {
      cancellationReason: reason
    };
    return this.httpService.post<any>(`trainings/individual/${trainingId}/cancel`, cancelDto);
  }

  deleteIndividualTraining(trainingId: number): Observable<any> {
    return this.httpService.delete<any>(`trainings/individual/${trainingId}`);
  }

  restoreIndividualTraining(trainingId: number): Observable<any> {
    return this.httpService.post<any>(`trainings/individual/${trainingId}/restore`, {});
  }

  createIndividualTraining(trainingData: any): Observable<any>{
    const createDto = {
      trainerId: trainingData.formData.trainerId,
      startDate: trainingData.formData.startTime.toISOString(),
      endDate: trainingData.formData.endTime.toISOString(),
      description: trainingData.formData.description
    }

    return this.httpService.post<any>(`trainings/individual`, createDto);
  }

  getIndividualTrainingsFiltered(filters?: CalendarFilters): Observable<IndividualTraining[]> {
    let url = 'trainings/individual/filtered';
    const params = this.buildFilterParams(filters);
    
    if (params) {
      url += `?${params}`;
    }
    
    return this.httpService.get<IndividualTraining[]>(url);
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

    if (filters.clientIds?.length) {
      params.push(`ClientIds=${filters.clientIds.join(',')}`);
    }

    return params.join('&');
  }
}