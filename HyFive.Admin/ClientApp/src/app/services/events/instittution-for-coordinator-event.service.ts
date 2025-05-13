import { Injectable, EventEmitter } from '@angular/core';

@Injectable({
  providedIn: 'root'
})

export class InstitutionForCoordinatorEventService {

  updateInstitutionList = new EventEmitter();

  constructor() { }
}
