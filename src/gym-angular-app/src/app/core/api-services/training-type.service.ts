import { inject, Injectable } from '@angular/core';
import { TrainingType } from '../models/training-type';
import { Observable } from 'rxjs';
import { HttpService } from './http.service';

@Injectable({
  providedIn: 'root'
})
export class TrainingTypeService {
  private httpService = inject(HttpService);

  getAllTrainingTypes(): Observable<TrainingType[]> {
    return this.httpService.get<TrainingType[]>('training-types');
  }
}
