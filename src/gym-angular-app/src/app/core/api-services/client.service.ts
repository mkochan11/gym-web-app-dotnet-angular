import { inject, Injectable } from '@angular/core';
import { Client } from '../models/client';
import { Observable } from 'rxjs';
import { HttpService } from './http.service';

@Injectable({
  providedIn: 'root'
})
export class ClientService {
  private httpService = inject(HttpService);

  getAllClients(): Observable<Client[]> {
    return this.httpService.get<Client[]>('clients');
  }
}
