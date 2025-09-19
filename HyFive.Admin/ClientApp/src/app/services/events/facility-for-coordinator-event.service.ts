import { Injectable, EventEmitter } from '@angular/core';

@Injectable({
  providedIn: 'root'
})

export class FacilityForCoordinatorEventService {

  updateFacilityList = new EventEmitter();

  constructor() { }
}
