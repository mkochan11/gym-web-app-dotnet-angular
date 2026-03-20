import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpService } from './http.service';
import { GymMembership, CancelMembershipRequest } from '../models/gym-membership.model';

@Injectable({
  providedIn: 'root'
})
export class GymMembershipService {
  private httpService = inject(HttpService);

  getMembershipById(id: number): Observable<GymMembership> {
    return this.httpService.get<GymMembership>(`gym-memberships/${id}`);
  }

  getActiveMembership(clientId: number): Observable<GymMembership | null> {
    return this.httpService.get<GymMembership>(`gym-memberships/client/${clientId}/active`);
  }

  getClientMemberships(clientId: number): Observable<GymMembership[]> {
    return this.httpService.get<GymMembership[]>(`gym-memberships/client/${clientId}`);
  }

  cancelMembership(request: CancelMembershipRequest): Observable<GymMembership> {
    return this.httpService.post<GymMembership>('gym-memberships/cancel', request);
  }
}
