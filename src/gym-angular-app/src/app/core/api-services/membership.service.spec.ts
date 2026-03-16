import { TestBed } from '@angular/core/testing';
import { MembershipService } from './membership.service';

describe('MembershipService', () => {
  let service: MembershipService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [MembershipService]
    });
    service = TestBed.inject(MembershipService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  describe('getMembershipPlans', () => {
    it('should return membership plans', () => {
      const plans = service.getMembershipPlans();
      
      expect(plans).toBeTruthy();
      expect(plans.length).toBeGreaterThan(0);
      expect(plans[0].name).toBe('Basic');
      expect(plans[1].name).toBe('Premium');
      expect(plans[2].name).toBe('Ultimate');
    });

    it('should include features in each plan', () => {
      const plans = service.getMembershipPlans();
      
      plans.forEach(plan => {
        expect(plan.features).toBeTruthy();
        expect(plan.features.length).toBeGreaterThan(0);
      });
    });
  });

  describe('setSelectedPlan', () => {
    it('should set selected plan', () => {
      const plans = service.getMembershipPlans();
      const plan = plans[0];

      service.setSelectedPlan(plan);

      expect(service.getSelectedPlan()).toEqual(plan);
    });
  });

  describe('clearSelectedPlan', () => {
    it('should clear selected plan', () => {
      const plans = service.getMembershipPlans();
      service.setSelectedPlan(plans[0]);
      service.clearSelectedPlan();

      expect(service.getSelectedPlan()).toBeNull();
    });
  });

  describe('processPayment', () => {
    it('should resolve with success message on successful payment', async () => {
      const paymentData = {
        cardNumber: '4111111111111111',
        expiryDate: '12/25',
        cvv: '123',
        cardholderName: 'John Doe'
      };
      const plan = service.getMembershipPlans()[0];

      const result = await service.processPayment(paymentData, plan);

      expect(result).toBeTruthy();
      expect(result.success).toBeDefined();
      expect(result.message).toBeDefined();
    });

    it('should resolve with message about membership', async () => {
      const paymentData = {
        cardNumber: '4111111111111111',
        expiryDate: '12/25',
        cvv: '123',
        cardholderName: 'John Doe'
      };
      const plan = service.getMembershipPlans()[0];

      const result = await service.processPayment(paymentData, plan);

      expect(result.message).toContain(plan.name);
    }, 10000);
  });
});
