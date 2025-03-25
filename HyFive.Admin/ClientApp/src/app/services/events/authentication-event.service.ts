import { Injectable, EventEmitter } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class AuthenticationEventService {

  authenticationFailedEvent : EventEmitter<boolean> = new EventEmitter<boolean>();

  constructor() { }
}
