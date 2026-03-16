import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { GroupTrainingService } from './group-training.service';

describe('GroupTrainingService', () => {
  let service: GroupTrainingService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [GroupTrainingService]
    });
    service = TestBed.inject(GroupTrainingService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  describe('getAllGroupTrainings', () => {
    it('should call trainings/group API', () => {
      service.getAllGroupTrainings().subscribe();

      const req = httpMock.expectOne('http://localhost:5000/api/trainings/group');
      expect(req.request.method).toBe('GET');
      req.flush([]);
    });
  });

  describe('createGroupTraining', () => {
    it('should call POST trainings/group API', () => {
      const trainingData = {
        formData: {
          trainerId: 1,
          trainingTypeId: 1,
          difficultyLevel: 2,
          startTime: new Date('2024-01-01T09:00:00'),
          endTime: new Date('2024-01-01T10:00:00'),
          maxParticipants: 10,
          description: 'Test training'
        }
      };

      service.createGroupTraining(trainingData).subscribe();

      const req = httpMock.expectOne('http://localhost:5000/api/trainings/group');
      expect(req.request.method).toBe('POST');
      req.flush({});
    });
  });

  describe('cancelGroupTraining', () => {
    it('should call POST trainings/group/{id}/cancel API', () => {
      service.cancelGroupTraining(1, 'Sick leave').subscribe();

      const req = httpMock.expectOne('http://localhost:5000/api/trainings/group/1/cancel');
      expect(req.request.method).toBe('POST');
      req.flush({});
    });
  });

  describe('deleteGroupTraining', () => {
    it('should call DELETE trainings/group/{id} API', () => {
      service.deleteGroupTraining(1).subscribe();

      const req = httpMock.expectOne('http://localhost:5000/api/trainings/group/1');
      expect(req.request.method).toBe('DELETE');
      req.flush({});
    });
  });

  describe('restoreGroupTraining', () => {
    it('should call POST trainings/group/{id}/restore API', () => {
      service.restoreGroupTraining(1).subscribe();

      const req = httpMock.expectOne('http://localhost:5000/api/trainings/group/1/restore');
      expect(req.request.method).toBe('POST');
      req.flush({});
    });
  });

  describe('getGroupTrainingsFiltered', () => {
    it('should call trainings/group/filtered API', () => {
      service.getGroupTrainingsFiltered().subscribe();

      const req = httpMock.expectOne('http://localhost:5000/api/trainings/group/filtered');
      expect(req.request.method).toBe('GET');
      req.flush([]);
    });

    it('should call trainings/group/filtered API with filters', () => {
      const filters = {
        startDate: new Date('2024-01-01'),
        endDate: new Date('2024-01-31'),
        employeeIds: [1],
        trainingTypeIds: [1]
      };

      service.getGroupTrainingsFiltered(filters).subscribe();

      const req = httpMock.expectOne(req => req.url.includes('http://localhost:5000/api/trainings/group/filtered'));
      expect(req.request.method).toBe('GET');
      req.flush([]);
    });
  });
});
