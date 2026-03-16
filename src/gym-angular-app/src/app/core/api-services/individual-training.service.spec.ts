import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { IndividualTrainingService } from './individual-training.service';

describe('IndividualTrainingService', () => {
  let service: IndividualTrainingService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [IndividualTrainingService]
    });
    service = TestBed.inject(IndividualTrainingService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  describe('getAllIndividualTrainings', () => {
    it('should call trainings/individual API', () => {
      service.getAllIndividualTrainings().subscribe();

      const req = httpMock.expectOne('http://localhost:5000/api/trainings/individual');
      expect(req.request.method).toBe('GET');
      req.flush([]);
    });
  });

  describe('createIndividualTraining', () => {
    it('should call POST trainings/individual API', () => {
      const trainingData = {
        formData: {
          trainerId: 1,
          startTime: new Date('2024-01-01T09:00:00'),
          endTime: new Date('2024-01-01T10:00:00'),
          description: 'Test training'
        }
      };

      service.createIndividualTraining(trainingData).subscribe();

      const req = httpMock.expectOne('http://localhost:5000/api/trainings/individual');
      expect(req.request.method).toBe('POST');
      req.flush({});
    });
  });

  describe('cancelIndividualTraining', () => {
    it('should call POST trainings/individual/{id}/cancel API', () => {
      service.cancelIndividualTraining(1, 'Sick leave').subscribe();

      const req = httpMock.expectOne('http://localhost:5000/api/trainings/individual/1/cancel');
      expect(req.request.method).toBe('POST');
      req.flush({});
    });
  });

  describe('deleteIndividualTraining', () => {
    it('should call DELETE trainings/individual/{id} API', () => {
      service.deleteIndividualTraining(1).subscribe();

      const req = httpMock.expectOne('http://localhost:5000/api/trainings/individual/1');
      expect(req.request.method).toBe('DELETE');
      req.flush({});
    });
  });

  describe('restoreIndividualTraining', () => {
    it('should call POST trainings/individual/{id}/restore API', () => {
      service.restoreIndividualTraining(1).subscribe();

      const req = httpMock.expectOne('http://localhost:5000/api/trainings/individual/1/restore');
      expect(req.request.method).toBe('POST');
      req.flush({});
    });
  });

  describe('getIndividualTrainingsFiltered', () => {
    it('should call trainings/individual/filtered API', () => {
      service.getIndividualTrainingsFiltered().subscribe();

      const req = httpMock.expectOne('http://localhost:5000/api/trainings/individual/filtered');
      expect(req.request.method).toBe('GET');
      req.flush([]);
    });

    it('should call trainings/individual/filtered API with filters', () => {
      const filters = {
        startDate: new Date('2024-01-01'),
        endDate: new Date('2024-01-31'),
        employeeIds: [1],
        clientIds: [2]
      };

      service.getIndividualTrainingsFiltered(filters).subscribe();

      const req = httpMock.expectOne(req => req.url.includes('http://localhost:5000/api/trainings/individual/filtered'));
      expect(req.request.method).toBe('GET');
      req.flush([]);
    });
  });
});
