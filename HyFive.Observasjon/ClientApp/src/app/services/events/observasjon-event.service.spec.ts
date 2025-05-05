import { TestBed } from '@angular/core/testing';

import { ObservationEventService } from './observation-event.service';

describe('ObservationEventService', () => {
  let service: ObservationEventService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ObservationEventService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
