import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { TrainingTypeService } from './training-type.service';

describe('TrainingTypeService', () => {
  let service: TrainingTypeService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [TrainingTypeService]
    });
    service = TestBed.inject(TrainingTypeService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  describe('getAllTrainingTypes', () => {
    it('should call training-types API', () => {
      service.getAllTrainingTypes().subscribe();

      const req = httpMock.expectOne('http://localhost:5000/api/training-types');
      expect(req.request.method).toBe('GET');
      req.flush([]);
    });

    it('should return training types when API returns data', () => {
      const mockTypes = [
        { id: 1, name: 'Yoga', description: 'Relaxing yoga session', difficultyLevel: 1 },
        { id: 2, name: 'HIIT', description: 'High intensity interval training', difficultyLevel: 3 },
        { id: 3, name: 'Spinning', description: 'Indoor cycling class', difficultyLevel: 2 }
      ];

      service.getAllTrainingTypes().subscribe(types => {
        expect(types).toEqual(mockTypes);
      });

      const req = httpMock.expectOne('http://localhost:5000/api/training-types');
      req.flush(mockTypes);
    });

    it('should return empty array when no training types', () => {
      service.getAllTrainingTypes().subscribe(types => {
        expect(types).toEqual([]);
      });

      const req = httpMock.expectOne('http://localhost:5000/api/training-types');
      req.flush([]);
    });
  });
});
