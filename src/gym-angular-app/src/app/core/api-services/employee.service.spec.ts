import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { EmployeeService } from './employee.service';

describe('EmployeeService', () => {
  let service: EmployeeService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [EmployeeService]
    });
    service = TestBed.inject(EmployeeService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  describe('getAllEmployees', () => {
    it('should call employees API', () => {
      service.getAllEmployees().subscribe();

      const req = httpMock.expectOne('http://localhost:5000/api/employees');
      expect(req.request.method).toBe('GET');
      req.flush([]);
    });

    it('should return employees when API returns data', () => {
      const mockEmployees = [
        { id: 1, email: 'trainer1@test.com', firstName: 'John', lastName: 'Doe', role: 'Trainer' },
        { id: 2, email: 'trainer2@test.com', firstName: 'Jane', lastName: 'Smith', role: 'Trainer' }
      ];

      service.getAllEmployees().subscribe(employees => {
        expect(employees).toEqual(mockEmployees);
      });

      const req = httpMock.expectOne('http://localhost:5000/api/employees');
      req.flush(mockEmployees);
    });
  });
});
