import { inject, Injectable } from '@angular/core';
import { GroupTrainingService, IndividualTrainingService, ShiftService } from '../api-services';
import { Observable, throwError } from 'rxjs';

export type CalendarEventType = 'group' | 'individual' | 'shift';

@Injectable({
  providedIn: 'root'
})
export class CalendarService {
  private groupTrainingService = inject(GroupTrainingService);
  private individualTrainingService = inject(IndividualTrainingService);
  private shiftService = inject(ShiftService);

  extractId(eventId: string | number): number {
    if (typeof eventId === 'number') return eventId;
    const parts = eventId.split('-');
    return parts.length > 1 ? parseInt(parts[1]) : parseInt(parts[0]);
  }

  createEvent(type: CalendarEventType, data: any): Observable<any> {
    switch (type) {
      case 'group': return this.groupTrainingService.createGroupTraining(data);
      case 'individual': return this.individualTrainingService.createIndividualTraining(data);
      case 'shift': return this.shiftService.createShift(data);
      default: return throwError(() => new Error('Unknown event type'));
    }
  }

  cancelEvent(type: CalendarEventType, id: string | number, reason: string): Observable<any> {
    const numericId = this.extractId(id);
    switch (type) {
      case 'group': return this.groupTrainingService.cancelGroupTraining(numericId, reason);
      case 'individual': return this.individualTrainingService.cancelIndividualTraining(numericId, reason);
      case 'shift': return this.shiftService.cancelShift(numericId, reason);
      default: return throwError(() => new Error('Unknown event type'));
    }
  }

  deleteEvent(type: CalendarEventType, id: string | number): Observable<any> {
    const numericId = this.extractId(id);
    switch (type) {
      case 'group': return this.groupTrainingService.deleteGroupTraining(numericId);
      case 'individual': return this.individualTrainingService.deleteIndividualTraining(numericId);
      case 'shift': return this.shiftService.deleteShift(numericId);
      default: return throwError(() => new Error('Unknown event type'));
    }
  }

  restoreEvent(type: CalendarEventType, id: string | number): Observable<any> {
    const numericId = this.extractId(id);
    switch (type) {
      case 'group': 
        return this.groupTrainingService.restoreGroupTraining(numericId);
      case 'individual': 
        return this.individualTrainingService.restoreIndividualTraining(numericId);
      case 'shift': 
        return this.shiftService.restoreShift(numericId);
      default: 
        return throwError(() => new Error('Unknown event type'));
    }
  }
}