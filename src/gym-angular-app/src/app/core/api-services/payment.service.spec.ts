import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { PaymentService } from './payment.service';
import { Payment, PaymentResult, ProcessPaymentRequest, ProcessMultiplePaymentsRequest, PaymentMethod } from '../models/payment.model';

describe('PaymentService', () => {
  let service: PaymentService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [PaymentService]
    });
    service = TestBed.inject(PaymentService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  describe('getPaymentsByMembership', () => {
    it('should call GET /payments/membership/{membershipId} and return payments', (done) => {
      const mockPayments: Payment[] = [
        {
          id: 1,
          membershipId: 1,
          dueDate: '2026-01-15',
          amount: 100,
          status: 'Paid',
          paidDate: '2026-01-10',
          paymentMethod: 'Card',
          transactionId: 'TXN-001'
        },
        {
          id: 2,
          membershipId: 1,
          dueDate: '2026-02-15',
          amount: 100,
          status: 'Pending',
          paidDate: null,
          paymentMethod: '',
          transactionId: null
        }
      ];

      service.getPaymentsByMembership(1).subscribe(payments => {
        expect(payments).toEqual(mockPayments);
        done();
      });

      const req = httpMock.expectOne('http://localhost:5000/api/payments/membership/1');
      expect(req.request.method).toBe('GET');
      req.flush(mockPayments);
    });

    it('should return empty array when no payments exist', (done) => {
      service.getPaymentsByMembership(999).subscribe(payments => {
        expect(payments).toEqual([]);
        done();
      });

      const req = httpMock.expectOne('http://localhost:5000/api/payments/membership/999');
      req.flush([]);
    });
  });

  describe('processPayment', () => {
    it('should call POST /payments/process with correct data', (done) => {
      const mockRequest: ProcessPaymentRequest = {
        membershipId: 1,
        paymentId: 2,
        paymentMethod: PaymentMethod.Card,
        transactionId: 'TXN-123'
      };
      const mockResponse: PaymentResult = {
        success: true,
        message: 'Payment successful',
        membershipId: 1,
        paymentId: 2,
        amount: 100,
        paymentMethod: 'Card',
        startDate: '2026-01-01',
        endDate: '2026-02-01',
        planName: 'Premium'
      };

      service.processPayment(mockRequest).subscribe(response => {
        expect(response).toEqual(mockResponse);
        done();
      });

      const req = httpMock.expectOne('http://localhost:5000/api/payments/process');
      expect(req.request.method).toBe('POST');
      expect(req.request.body).toEqual(mockRequest);
      req.flush(mockResponse);
    });

    it('should handle cash payment without transaction ID', (done) => {
      const mockRequest: ProcessPaymentRequest = {
        membershipId: 1,
        paymentId: 2,
        paymentMethod: PaymentMethod.Cash,
        transactionId: null
      };
      const mockResponse: PaymentResult = {
        success: true,
        message: 'Payment successful',
        membershipId: 1,
        paymentId: 2,
        amount: 100,
        paymentMethod: 'Cash',
        startDate: null,
        endDate: null,
        planName: null
      };

      service.processPayment(mockRequest).subscribe(response => {
        expect(response).toEqual(mockResponse);
        done();
      });

      const req = httpMock.expectOne('http://localhost:5000/api/payments/process');
      expect(req.request.method).toBe('POST');
      expect(req.request.body).toEqual(mockRequest);
      req.flush(mockResponse);
    });

    it('should handle payment failure', (done) => {
      const mockRequest: ProcessPaymentRequest = {
        membershipId: 1,
        paymentId: 2,
        paymentMethod: PaymentMethod.Card,
        transactionId: 'TXN-123'
      };

      service.processPayment(mockRequest).subscribe({
        error: () => {
          done();
        }
      });

      const req = httpMock.expectOne('http://localhost:5000/api/payments/process');
      req.error(new ProgressEvent('Error'), { status: 400, statusText: 'Bad Request' });
    });
  });

  describe('processMultiplePayments', () => {
    it('should call POST /payments/process-multiple with correct data', (done) => {
      const mockRequest: ProcessMultiplePaymentsRequest = {
        membershipId: 1,
        paymentIds: [1, 2, 3],
        paymentMethod: PaymentMethod.BankTransfer,
        transactionId: 'TXN-BANK-001'
      };
      const mockResponse: PaymentResult[] = [
        {
          success: true,
          message: 'Payment 1 successful',
          membershipId: 1,
          paymentId: 1,
          amount: 100,
          paymentMethod: 'BankTransfer',
          startDate: null,
          endDate: null,
          planName: null
        },
        {
          success: true,
          message: 'Payment 2 successful',
          membershipId: 1,
          paymentId: 2,
          amount: 100,
          paymentMethod: 'BankTransfer',
          startDate: null,
          endDate: null,
          planName: null
        }
      ];

      service.processMultiplePayments(mockRequest).subscribe(response => {
        expect(response).toEqual(mockResponse);
        done();
      });

      const req = httpMock.expectOne('http://localhost:5000/api/payments/process-multiple');
      expect(req.request.method).toBe('POST');
      expect(req.request.body).toEqual(mockRequest);
      req.flush(mockResponse);
    });

    it('should handle partial failure in multiple payments', (done) => {
      const mockRequest: ProcessMultiplePaymentsRequest = {
        membershipId: 1,
        paymentIds: [1, 2],
        paymentMethod: PaymentMethod.Other,
        transactionId: 'TXN-OTHER-001'
      };
      const mockResponse: PaymentResult[] = [
        {
          success: true,
          message: 'Payment 1 successful',
          membershipId: 1,
          paymentId: 1,
          amount: 100,
          paymentMethod: 'Other',
          startDate: null,
          endDate: null,
          planName: null
        },
        {
          success: false,
          message: 'Payment 2 failed - already paid',
          membershipId: 1,
          paymentId: 2,
          amount: 100,
          paymentMethod: 'Other',
          startDate: null,
          endDate: null,
          planName: null
        }
      ];

      service.processMultiplePayments(mockRequest).subscribe(response => {
        expect(response).toEqual(mockResponse);
        done();
      });

      const req = httpMock.expectOne('http://localhost:5000/api/payments/process-multiple');
      expect(req.request.method).toBe('POST');
      req.flush(mockResponse);
    });
  });
});