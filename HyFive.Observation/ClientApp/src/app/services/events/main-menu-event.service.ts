import { Injectable, EventEmitter } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class MainMenuEventService {

  mainMenuIsOpenEvent : EventEmitter<boolean> = new EventEmitter<boolean>();

  constructor() { }
}
