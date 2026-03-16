import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { ClientService } from './client.service';
import { Client } from '../models/client';

describe('ClientService', () => {
  let service: ClientService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [ClientService]
    });
    service = TestBed.inject(ClientService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  describe('getAllClients', () => {
    it('should call clients API and return clients', () => {
      const mockClients: Client[] = [
        { id: 1, firstName: 'John', lastName: 'Doe' },
        { id: 2, firstName: 'Jane', lastName: 'Smith' }
      ];

      service.getAllClients().subscribe(clients => {
        expect(clients).toEqual(mockClients);
      });

      const req = httpMock.expectOne('http://localhost:5000/api/clients');
      expect(req.request.method).toBe('GET');
      req.flush(mockClients);
    });

    it('should return empty array when no clients', () => {
      service.getAllClients().subscribe(clients => {
        expect(clients).toEqual([]);
      });

      const req = httpMock.expectOne('http://localhost:5000/api/clients');
      req.flush([]);
    });
  });
});
