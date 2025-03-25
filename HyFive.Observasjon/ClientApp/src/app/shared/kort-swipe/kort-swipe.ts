import { Directive } from "@angular/core";
import { NgbModal } from "@ng-bootstrap/ng-bootstrap";
import { AnimationsConfig, AnimationStates } from '../../shared/animasjoner/animasjoner';
import { DialogModalComponent, DialogModalComponentConfig } from "../dialog-modal/dialog-modal.component";

@Directive()
export abstract class BaseKortSwipe {

  transform: string = "";
  animation: string = "";
  showDeleteShadow: boolean = false;
  showSaveShadow: boolean = false;
  directions = {
    left: 'left',
    right: 'right'
  };
  cardLockedInPlace = false;
  MAX_SWIPE_PERCENTAGE: number = 0.7;

  constructor(protected modalService: NgbModal) {

  }

  onPanMove(event): void {
    if(this.cardLockedInPlace)
      return;

    this.animation = ""; /* reset animation state */

    if (Math.abs(event.deltaX / event.target.parentNode.clientWidth) < this.MAX_SWIPE_PERCENTAGE)
      this.transform = 'translateX(' + event.deltaX + 'px)';

    if (event.deltaX === 0) {
      this.showDeleteShadow = false;
      this.showSaveShadow = false;
    }
    else if (event.deltaX > 0) { // right
      this.showDeleteShadow = false;
      this.showSaveShadow = true;
    }
    else { // left
      this.showDeleteShadow = true;
      this.showSaveShadow = false;
    }
  }

  onPanEnd(event): void {
    if(this.cardLockedInPlace)
      return;
    const direction = this.getSwipeDirection(event);

    if (direction === this.directions.left) {
      this.swipeLeftAction();
    }
    else if (direction === this.directions.right) {
      this.swipeRightAction();
    }
    else {
      this.transform = 'translateX(0px)';
      this.showDeleteShadow = false;
      this.showSaveShadow = false;
    }
  }

  getSwipeDirection(event) {
    return Math.abs(event.deltaX / event.target.parentNode.clientWidth) > AnimationsConfig.swipeLeftRightThreshold
      ? (event.deltaX > 0 ? this.directions.right : this.directions.left)
      : '';
  }

  async swipeLeftAction() {

    if(this.cardLockedInPlace)
      return;

    this.animation = AnimationStates.swipeLeft;

    setTimeout(async () => {
      if (await this.skalSletteKort()) {
        this.nullstillKort();
        setTimeout(() => {
          this.slettKort();
          this.showDeleteShadow = false;
        }, AnimationsConfig.swipeLeftAnimationDuration);
      }
      else {
        this.animation = AnimationStates.initialOk;
      }
    }, AnimationsConfig.swipeLeftAnimationDuration);
  }

  swipeRightAction() {
    if(this.cardLockedInPlace)
      return;

    this.animation = AnimationStates.swipeRight;

    setTimeout(() => {
      if (this.kanIkkeLagre()) {
        this.animation = AnimationStates.initialError;
        setTimeout(() => {
          this.showSaveShadow = false;
        }, AnimationsConfig.initialErrorAnimationDuration);
      }
      else {
        this.registrerObservasjon();

        this.animation = AnimationStates.initialOk;
        setTimeout(() => {
          this.showSaveShadow = false;
        }, AnimationsConfig.initialErrorAnimationDuration);
      }
    }, AnimationsConfig.swipeRightAnimationDuration);
  }

  async skalSletteKort(): Promise<boolean> {
    const modalRef = this.modalService.open(DialogModalComponent, {
      keyboard: false,
      ariaLabelledBy: 'modal-basic-title',
      windowClass: DialogModalComponentConfig.windowClass
    });

    modalRef.componentInstance.melding = "Ønsker du å slette dette kortet?";

    return await modalRef.result.then((result: boolean) => {
      return result;
    }, (reason: boolean) => {
      return reason;
    });
  }

  abstract kanIkkeLagre();
  abstract registrerObservasjon();
  abstract nullstillKort();
  abstract slettKort();
}
