import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpService } from './http.service';
import { MembershipPlan, CreateMembershipPlanRequest, UpdateMembershipPlanRequest } from '../models/membership-plan.model';

@Injectable({
  providedIn: 'root'
})
export class MembershipPlanService {
  private httpService = inject(HttpService);

  getMembershipPlans(): Observable<MembershipPlan[]> {
    return this.httpService.get<MembershipPlan[]>('membership-plans');
  }

  getMembershipPlanById(id: number): Observable<MembershipPlan> {
    return this.httpService.get<MembershipPlan>(`membership-plans/${id}`);
  }

  createMembershipPlan(plan: CreateMembershipPlanRequest): Observable<MembershipPlan> {
    return this.httpService.post<MembershipPlan>('membership-plans', plan);
  }

  updateMembershipPlan(id: number, plan: UpdateMembershipPlanRequest): Observable<MembershipPlan> {
    return this.httpService.put<MembershipPlan>(`membership-plans/${id}`, plan);
  }

  deleteMembershipPlan(id: number): Observable<void> {
    return this.httpService.delete<void>(`membership-plans/${id}`);
  }
}
