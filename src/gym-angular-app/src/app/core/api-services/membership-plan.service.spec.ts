import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { MembershipPlanService } from './membership-plan.service';
import { MembershipPlan, CreateMembershipPlanRequest, UpdateMembershipPlanRequest } from '../models/membership-plan.model';
import { environment } from '../../../environments/environment';

describe('MembershipPlanService', () => {
  let service: MembershipPlanService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [MembershipPlanService]
    });
    service = TestBed.inject(MembershipPlanService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  describe('getMembershipPlans', () => {
    it('should call GET /api/membership-plans and return plans', () => {
      const mockPlans: MembershipPlan[] = [
        {
          id: 1,
          type: 'Premium',
          description: 'Premium plan',
          price: 99.99,
          durationTime: '30 days',
          durationInMonths: 1,
          canReserveTrainings: true,
          canAccessGroupTraining: true,
          canAccessPersonalTraining: false,
          canReceiveTrainingPlans: true,
          maxTrainingsPerMonth: 10,
          isActive: true,
          createdAt: '2026-01-01',
          updatedAt: null
        },
        {
          id: 2,
          type: 'Basic',
          description: 'Basic plan',
          price: 29.99,
          durationTime: '30 days',
          durationInMonths: 1,
          canReserveTrainings: false,
          canAccessGroupTraining: false,
          canAccessPersonalTraining: false,
          canReceiveTrainingPlans: false,
          maxTrainingsPerMonth: null,
          isActive: true,
          createdAt: '2026-01-01',
          updatedAt: null
        }
      ];

      service.getMembershipPlans().subscribe(plans => {
        expect(plans).toEqual(mockPlans);
      });

      const req = httpMock.expectOne(`${environment.apiUrl}/membership-plans`);
      expect(req.request.method).toBe('GET');
      req.flush(mockPlans);
    });

    it('should return error on API failure', () => {
      service.getMembershipPlans().subscribe({
        next: () => fail('should have failed'),
        error: (error) => {
          expect(error).toBeTruthy();
        }
      });

      const req = httpMock.expectOne(`${environment.apiUrl}/membership-plans`);
      req.error(new ProgressEvent('error'));
    });
  });

  describe('getMembershipPlanById', () => {
    it('should call GET /api/membership-plans/{id} and return plan', () => {
      const mockPlan: MembershipPlan = {
        id: 1,
        type: 'Premium',
        description: 'Premium plan',
        price: 99.99,
        durationTime: '30 days',
        durationInMonths: 1,
        canReserveTrainings: true,
        canAccessGroupTraining: true,
        canAccessPersonalTraining: false,
        canReceiveTrainingPlans: true,
        maxTrainingsPerMonth: 10,
        isActive: true,
        createdAt: '2026-01-01',
        updatedAt: null
      };

      service.getMembershipPlanById(1).subscribe(plan => {
        expect(plan).toEqual(mockPlan);
      });

      const req = httpMock.expectOne(`${environment.apiUrl}/membership-plans/1`);
      expect(req.request.method).toBe('GET');
      req.flush(mockPlan);
    });
  });

  describe('createMembershipPlan', () => {
    it('should call POST /api/membership-plans and return created plan', () => {
      const createRequest: CreateMembershipPlanRequest = {
        type: 'Premium',
        description: 'Premium plan',
        price: 99.99,
        durationTime: '30 days',
        durationInMonths: 1,
        canReserveTrainings: true,
        canAccessGroupTraining: true,
        canAccessPersonalTraining: false,
        canReceiveTrainingPlans: true,
        maxTrainingsPerMonth: 10,
        isActive: true
      };

      const createdPlan: MembershipPlan = {
        id: 1,
        ...createRequest,
        createdAt: '2026-01-01',
        updatedAt: null
      };

      service.createMembershipPlan(createRequest).subscribe(plan => {
        expect(plan).toEqual(createdPlan);
      });

      const req = httpMock.expectOne(`${environment.apiUrl}/membership-plans`);
      expect(req.request.method).toBe('POST');
      expect(req.request.body).toEqual(createRequest);
      req.flush(createdPlan);
    });
  });

  describe('updateMembershipPlan', () => {
    it('should call PUT /api/membership-plans/{id} and return updated plan', () => {
      const updateRequest: UpdateMembershipPlanRequest = {
        id: 1,
        type: 'Premium Plus',
        description: 'Updated premium plan',
        price: 119.99,
        durationTime: '30 days',
        durationInMonths: 1,
        canReserveTrainings: true,
        canAccessGroupTraining: true,
        canAccessPersonalTraining: true,
        canReceiveTrainingPlans: true,
        maxTrainingsPerMonth: 15,
        isActive: true
      };

      const updatedPlan: MembershipPlan = {
        ...updateRequest,
        createdAt: '2026-01-01',
        updatedAt: '2026-01-15'
      };

      service.updateMembershipPlan(1, updateRequest).subscribe(plan => {
        expect(plan).toEqual(updatedPlan);
      });

      const req = httpMock.expectOne(`${environment.apiUrl}/membership-plans/1`);
      expect(req.request.method).toBe('PUT');
      expect(req.request.body).toEqual(updateRequest);
      req.flush(updatedPlan);
    });
  });

  describe('deleteMembershipPlan', () => {
    it('should call DELETE /api/membership-plans/{id}', () => {
      service.deleteMembershipPlan(1).subscribe(result => {
        expect(result).toBeUndefined();
      });

      const req = httpMock.expectOne(`${environment.apiUrl}/membership-plans/1`);
      expect(req.request.method).toBe('DELETE');
      req.flush(null);
    });
  });
});
