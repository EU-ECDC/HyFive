import { Injectable, EventEmitter } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class KeyEventService {

  escapeKeyEvent: EventEmitter<KeyboardEvent> = new EventEmitter<KeyboardEvent>();

  constructor() { }
}

export enum KEY_CODE {
  ENTER = 13,
  ESC = 27,
  LEFT_ARROW = 37,
  UP_ARROW = 38,
  RIGHT_ARROW = 39,
  DOWN_ARROW = 40,
}
