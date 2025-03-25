import { HammerGestureConfig, HAMMER_GESTURE_CONFIG } from "@angular/platform-browser";
import * as Hammer from 'hammerjs';
import {Injectable} from '@angular/core';

@Injectable()
export class HammerConfig extends HammerGestureConfig {
  overrides = <any>{
    swipe: { direction: Hammer.DIRECTION_HORIZONTAL },
    pan: { direction: Hammer.DIRECTION_HORIZONTAL },
  };
}

export const HandHygieneHammerJS = {
  provide: HAMMER_GESTURE_CONFIG,
  useClass: HammerConfig,
}
