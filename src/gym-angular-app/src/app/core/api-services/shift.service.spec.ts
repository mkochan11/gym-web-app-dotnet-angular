import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { ShiftService } from './shift.service';

describe('ShiftService', () => {
  let service: ShiftService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [ShiftService]
    });
    service = TestBed.inject(ShiftService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  describe('getAllShifts', () => {
    it('should call shifts API', () => {
      service.getAllShifts().subscribe();

      const req = httpMock.expectOne('http://localhost:5000/api/shifts');
      expect(req.request.method).toBe('GET');
      req.flush([]);
    });
  });

  describe('createShift', () => {
    it('should call POST shifts API', () => {
      const shiftData = {
        formData: {
          employeeId: 1,
          startTime: new Date('2024-01-01T09:00:00'),
          endTime: new Date('2024-01-01T17:00:00')
        }
      };

      service.createShift(shiftData).subscribe();

      const req = httpMock.expectOne('http://localhost:5000/api/shifts');
      expect(req.request.method).toBe('POST');
      req.flush({});
    });
  });

  describe('updateShift', () => {
    it('should call PUT shifts/{id} API', () => {
      const shiftData = {
        formData: {
          employeeId: 1,
          startTime: new Date('2024-01-01T09:00:00'),
          endTime: new Date('2024-01-01T17:00:00')
        }
      };

      service.updateShift(1, shiftData).subscribe();

      const req = httpMock.expectOne('http://localhost:5000/api/shifts/1');
      expect(req.request.method).toBe('PUT');
      req.flush({});
    });
  });

  describe('cancelShift', () => {
    it('should call POST shifts/{id}/cancel API', () => {
      service.cancelShift(1, 'Sick leave').subscribe();

      const req = httpMock.expectOne('http://localhost:5000/api/shifts/1/cancel');
      expect(req.request.method).toBe('POST');
      expect(req.request.body).toEqual({ cancellationReason: 'Sick leave' });
      req.flush({});
    });
  });

  describe('deleteShift', () => {
    it('should call DELETE shifts/{id} API', () => {
      service.deleteShift(1).subscribe();

      const req = httpMock.expectOne('http://localhost:5000/api/shifts/1');
      expect(req.request.method).toBe('DELETE');
      req.flush({});
    });
  });

  describe('restoreShift', () => {
    it('should call POST shifts/{id}/restore API', () => {
      service.restoreShift(1).subscribe();

      const req = httpMock.expectOne('http://localhost:5000/api/shifts/1/restore');
      expect(req.request.method).toBe('POST');
      req.flush({});
    });
  });

  describe('getShiftsFiltered', () => {
    it('should call shifts/filtered API without filters', () => {
      service.getShiftsFiltered().subscribe();

      const req = httpMock.expectOne('http://localhost:5000/api/shifts/filtered');
      expect(req.request.method).toBe('GET');
      req.flush([]);
    });

    it('should call shifts/filtered API with filters', () => {
      const filters = {
        startDate: new Date('2024-01-01'),
        endDate: new Date('2024-01-31'),
        employeeIds: [1, 2]
      };

      service.getShiftsFiltered(filters).subscribe();

      const req = httpMock.expectOne(req => req.url.startsWith('http://localhost:5000/api/shifts/filtered'));
      expect(req.request.method).toBe('GET');
      req.flush([]);
    });
  });
});
