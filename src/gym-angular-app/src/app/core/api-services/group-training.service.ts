import { Injectable, inject } from '@angular/core';
import { Observable, map } from 'rxjs';
import { HttpService } from './http.service';
import { GroupTraining } from '../models/group-training';

@Injectable({
  providedIn: 'root'
})
export class GroupTrainingService {
  private httpService = inject(HttpService);

  getAllGroupTrainings(): Observable<GroupTraining[]> {
    return this.httpService.get<GroupTraining[]>('group-trainings');
  }
}
