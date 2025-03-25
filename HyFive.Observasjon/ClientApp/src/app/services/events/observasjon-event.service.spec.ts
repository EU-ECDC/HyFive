import { TestBed } from '@angular/core/testing';

import { ObservasjonEventService } from './observasjon-event.service';

describe('ObservasjonEventService', () => {
  let service: ObservasjonEventService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ObservasjonEventService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
