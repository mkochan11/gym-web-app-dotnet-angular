import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { MembershipService, PurchaseMembershipRequest } from './membership.service';
import { GymMembership } from '../models/gym-membership.model';

const mockMembership: GymMembership = {
  id: 1,
  clientId: 2,
  clientName: 'John Doe',
  membershipPlanId: 1,
  planName: 'Premium',
  planDescription: 'Premium plan',
  planPrice: 100,
  planDurationInMonths: 3,
  canReserveTrainings: true,
  canAccessGroupTraining: true,
  canAccessPersonalTraining: false,
  canReceiveTrainingPlans: false,
  maxTrainingsPerMonth: null,
  startDate: new Date().toISOString(),
  endDate: new Date(Date.now() + 30 * 24 * 60 * 60 * 1000).toISOString(),
  status: 0,
  isActive: true,
  isCancelled: false,
  cancelledAt: null,
  cancellationRequestedDate: null,
  effectiveEndDate: null,
  cancellationReason: null
};

describe('MembershipService', () => {
  let service: MembershipService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [MembershipService]
    });
    service = TestBed.inject(MembershipService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  describe('purchaseMembership', () => {
    it('should call POST /gym-memberships/purchase with correct data', (done) => {
      const mockRequest: PurchaseMembershipRequest = {
        membershipPlanId: 1,
        clientId: 2
      };

      service.purchaseMembership(1, 2).subscribe(response => {
        expect(response).toEqual(mockMembership);
        done();
      });

      const req = httpMock.expectOne('http://localhost:5000/api/gym-memberships/purchase');
      expect(req.request.method).toBe('POST');
      expect(req.request.body).toEqual(mockRequest);
      req.flush(mockMembership);
    });

    it('should set loading state during request', (done) => {
      service.purchaseMembership(1, 2).subscribe(() => {
        done();
      });

      expect(service.loading()).toBeTrue();

      const req = httpMock.expectOne('http://localhost:5000/api/gym-memberships/purchase');
      req.flush({ id: 1 });

      expect(service.loading()).toBeFalse();
    });

    it('should set error on failure', (done) => {
      service.purchaseMembership(1, 2).subscribe({
        error: () => {
          expect(service.error()).toBeTruthy();
          done();
        }
      });

      const req = httpMock.expectOne('http://localhost:5000/api/gym-memberships/purchase');
      req.error(new ProgressEvent('Error'), { status: 500, statusText: 'Server Error' });
    });
  });

  describe('processPayment', () => {
    it('should call POST /payments/process with correct data', (done) => {
      const mockRequest = {
        membershipId: 1,
        cardNumber: '4111111111111111',
        expiryDate: '12/25',
        cvv: '123',
        cardholderName: 'John Doe'
      };
      const mockResponse = {
        success: true,
        message: 'Payment successful'
      };

      service.processPayment(1, mockRequest).subscribe(response => {
        expect(response).toEqual(mockResponse);
        done();
      });

      const req = httpMock.expectOne('http://localhost:5000/api/payments/process');
      expect(req.request.method).toBe('POST');
      expect(req.request.body).toEqual(mockRequest);
      req.flush(mockResponse);
    });

    it('should return error response on failure instead of throwing', (done) => {
      service.processPayment(1, {
        cardNumber: '4111111111111111',
        expiryDate: '12/25',
        cvv: '123',
        cardholderName: 'John Doe'
      }).subscribe(response => {
        expect(response.success).toBeFalse();
        expect(response.message).toBeTruthy();
        done();
      });

      const req = httpMock.expectOne('http://localhost:5000/api/payments/process');
      req.error(new ProgressEvent('Error'), { status: 400, statusText: 'Bad Request' });
    });
  });

  describe('getClientActiveMembership', () => {
    it('should return membership when found', (done) => {
      service.getClientActiveMembership(2).subscribe(membership => {
        expect(membership).toEqual(mockMembership);
        done();
      });

      const req = httpMock.expectOne('http://localhost:5000/api/gym-memberships/client/2/active');
      req.flush(mockMembership);
    });

    it('should return null when not found', (done) => {
      service.getClientActiveMembership(999).subscribe(membership => {
        expect(membership).toBeNull();
        done();
      });

      const req = httpMock.expectOne('http://localhost:5000/api/gym-memberships/client/999/active');
      req.flush('Not found', { status: 404, statusText: 'Not Found' });
    });
  });

  describe('getMembershipById', () => {
    it('should call GET /gym-memberships/{id}', (done) => {
      service.getMembershipById(1).subscribe(membership => {
        expect(membership).toEqual(mockMembership);
        done();
      });

      const req = httpMock.expectOne('http://localhost:5000/api/gym-memberships/1');
      expect(req.request.method).toBe('GET');
      req.flush(mockMembership);
    });
  });

  describe('getClientMemberships', () => {
    it('should call GET /gym-memberships/client/{clientId}', (done) => {
      const memberships = [mockMembership];
      service.getClientMemberships(2).subscribe(result => {
        expect(result).toEqual(memberships);
        done();
      });

      const req = httpMock.expectOne('http://localhost:5000/api/gym-memberships/client/2');
      expect(req.request.method).toBe('GET');
      req.flush(memberships);
    });
  });

  describe('cancelMembership', () => {
    it('should call POST /gym-memberships/{id}/cancel with cancellation reason', (done) => {
      const pendingMembership: GymMembership = {
        ...mockMembership,
        status: 1,
        cancellationRequestedDate: new Date().toISOString(),
        effectiveEndDate: new Date(Date.now() + 15 * 24 * 60 * 60 * 1000).toISOString(),
        cancellationReason: 'Moving away'
      };

      service.cancelMembership(1, { cancellationReason: 'Moving away' }).subscribe(membership => {
        expect(membership.status).toBe(1);
        expect(membership.cancellationReason).toBe('Moving away');
        done();
      });

      const req = httpMock.expectOne('http://localhost:5000/api/gym-memberships/1/cancel');
      expect(req.request.method).toBe('POST');
      expect(req.request.body).toEqual({ cancellationReason: 'Moving away' });
      req.flush(pendingMembership);
    });

    it('should call POST /gym-memberships/{id}/cancel with null reason', (done) => {
      const pendingMembership: GymMembership = {
        ...mockMembership,
        status: 1,
        cancellationReason: null
      };

      service.cancelMembership(1, { cancellationReason: null }).subscribe(membership => {
        expect(membership.status).toBe(1);
        done();
      });

      const req = httpMock.expectOne('http://localhost:5000/api/gym-memberships/1/cancel');
      req.flush(pendingMembership);
    });
  });

  describe('revertCancellation', () => {
    it('should call POST /gym-memberships/{id}/cancel/revert', (done) => {
      const activeMembership: GymMembership = {
        ...mockMembership,
        status: 0,
        cancellationRequestedDate: null,
        effectiveEndDate: null
      };

      service.revertCancellation(1).subscribe(membership => {
        expect(membership.status).toBe(0);
        expect(membership.cancellationRequestedDate).toBeNull();
        expect(membership.effectiveEndDate).toBeNull();
        done();
      });

      const req = httpMock.expectOne('http://localhost:5000/api/gym-memberships/1/cancel/revert');
      expect(req.request.method).toBe('POST');
      expect(req.request.body).toEqual({});
      req.flush(activeMembership);
    });
  });

  describe('selectedPlan signal', () => {
    it('should set and get selected plan', () => {
      const mockPlan = {
        id: 1,
        name: 'Premium',
        description: 'Premium plan',
        price: 100,
        durationInMonths: 3,
        isActive: true,
        features: ['Gym', 'Pool']
      } as any;

      service.setSelectedPlan(mockPlan);
      expect(service.getSelectedPlan()).toEqual(mockPlan);
    });

    it('should clear selected plan', () => {
      const mockPlan = {
        id: 1,
        name: 'Premium',
        description: 'Premium plan',
        price: 100,
        durationInMonths: 3,
        isActive: true,
        features: ['Gym', 'Pool']
      } as any;

      service.setSelectedPlan(mockPlan);
      service.clearSelectedPlan();
      expect(service.getSelectedPlan()).toBeNull();
    });
  });
});
