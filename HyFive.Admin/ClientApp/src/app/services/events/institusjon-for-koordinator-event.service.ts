import { Injectable, EventEmitter } from '@angular/core';

@Injectable({
  providedIn: 'root'
})

export class InstitusjonForKoordinatorEventService {

  oppdaterInstitusjonsListe = new EventEmitter();

  constructor() { }
}
