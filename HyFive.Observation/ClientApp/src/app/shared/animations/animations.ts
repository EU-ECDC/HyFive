import { animate, keyframes, state, style, transition, trigger } from '@angular/animations';

export const AnimationsConfig = {
  swipeLeftRightThreshold: 0.40, // percent
  initialOkAnimationDuration: 400,
  initialErrorAnimationDuration: 500,
  swipeLeftAnimationDuration: 200,
  swipeRightAnimationDuration: 200,
};

export const AnimationStates = {
  initialOk: 'initialOk',
  initialError: 'initialError',
  swipeLeft: 'swipeLeft',
  swipeRight: 'swipeRight'
};

export const Animations = {
  swipeLeftRight: trigger('swipeLeftRight', [
    state(AnimationStates.initialOk, style({
      transform: 'translateX(0)'
    })),
    state(AnimationStates.initialError, style({
      transform: 'translateX(0)'
    })),
    state(AnimationStates.swipeLeft, style({
      transform: 'translateX(calc(-100vw + 30%))'
    })),
    state(AnimationStates.swipeRight, style({
      transform: 'translateX(calc(100vw - 40%))'
    })),
    transition(`* => ${AnimationStates.initialOk}`, animate(AnimationsConfig.initialOkAnimationDuration + 'ms')),
    transition(`* => ${AnimationStates.initialError}`, animate(AnimationsConfig.initialErrorAnimationDuration + 'ms', keyframes([
      style({ transform: 'translateX(30px)' }),
      style({ transform: 'translateX(-30px)' }),
      style({ transform: 'translateX(20px)' }),
      style({ transform: 'translateX(-20px)' }),
      style({ transform: 'translateX(0)' }),
    ]))),
    transition(`* => ${AnimationStates.swipeLeft}`, animate(AnimationsConfig.swipeLeftAnimationDuration + 'ms')),
    transition(`* => ${AnimationStates.swipeRight}`, animate(AnimationsConfig.swipeRightAnimationDuration + 'ms')),
  ])
};
