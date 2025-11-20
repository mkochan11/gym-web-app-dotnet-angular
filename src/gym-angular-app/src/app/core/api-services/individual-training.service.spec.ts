import { TestBed } from '@angular/core/testing';

import { IndividualTrainingService } from './individual-training.service';

describe('IndividualTrainingService', () => {
  let service: IndividualTrainingService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(IndividualTrainingService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
