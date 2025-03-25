import { Injectable, EventEmitter } from '@angular/core';
import { AuthorizedRole } from 'src/app/_felles/authorization/authorized-role';

@Injectable({
  providedIn: 'root'
})

export class RolleEventService {

  byttRolleEvent : EventEmitter<AuthorizedRole> = new EventEmitter<AuthorizedRole>();

  constructor() { }
}
