import { Injectable, EventEmitter } from '@angular/core';
import { ActivityType } from '../../models/api/ActivityType';

@Injectable({
  providedIn: 'root'
})
export class ObservasjonEventService {

  observasjonNullstiltEvent : EventEmitter<string> = new EventEmitter<string>();
  registreringAvAktivitetHarBegynt: EventEmitter<AktivitetUnderRegistrering> = new EventEmitter<AktivitetUnderRegistrering>();

  constructor() { }
}

export interface AktivitetUnderRegistrering{
  parentId: string;
  activityType: ActivityType;
}
