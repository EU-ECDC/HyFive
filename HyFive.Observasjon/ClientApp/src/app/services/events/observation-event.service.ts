import { Injectable, EventEmitter } from '@angular/core';
import { ActivityType } from '../../models/api/ActivityType';

@Injectable({
  providedIn: 'root'
})
export class ObservationEventService {

  observationResetEvent : EventEmitter<string> = new EventEmitter<string>();
  registrationActivityHasBegun: EventEmitter<ActivityUnderRegistration> = new EventEmitter<ActivityUnderRegistration>();

  constructor() { }
}

export interface ActivityUnderRegistration{
  parentId: string;
  activityType: ActivityType;
}
