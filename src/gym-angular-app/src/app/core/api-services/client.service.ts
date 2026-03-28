import { inject, Injectable } from '@angular/core';
import { Client, ClientListItem, ClientDetails } from '../models/client';
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

  getClients(search?: string, page: number = 1, pageSize: number = 50): Observable<ClientListItem[]> {
    return this.httpService.get<ClientListItem[]>('clients', { search, page, pageSize });
  }

  getClientById(id: number): Observable<ClientDetails> {
    return this.httpService.get<ClientDetails>(`clients/${id}`);
  }

  getClientByAccountId(accountId: string): Observable<Client | null> {
    return this.httpService.get<Client>(`clients/account/${accountId}`);
  }
}
