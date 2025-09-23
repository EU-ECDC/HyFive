import { Component, Input, OnInit, Output, EventEmitter } from '@angular/core';
import { Activity } from '../../models/api/Activity';
import { ObservationEventService } from '../../services/events/observation-event.service';
import { faClock } from '@fortawesome/free-solid-svg-icons';
import { ActivityType } from '../../models/api/ActivityType';
import { ActivityTypeConstants } from '../../models/api/ActivityTypeConstants';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';


@Component({
  selector: 'app-register-activity',
  templateUrl: './register-activity.component.html'
})

export class RegisterActivityComponent implements OnInit {

  timerIsStarted: boolean = false;
  interval;
  timingExecuted: boolean = false;
  faClock = faClock;
  showText: boolean = true;

  usedGloves?: boolean = null;

  @Input("parentId") parentId: string;
  @Input("time") time: boolean;
  @Input("disabled") disabled: boolean;
  @Input("activityType") activityType: ActivityType;
  @Input("seconds") seconds: number;
  @Input("isRegistered") isRegistered: boolean;
  @Input("confirmationModalToShow") confirmationModalToShow: boolean;
  @Input("icon") icon: string;

  @Output() activityRegisteredEvent = new EventEmitter<Activity>();
  activityTypeConstants = ActivityTypeConstants;

  constructor(private observationEventService: ObservationEventService, private modalService: NgbModal) {
  }

  ngOnInit(): void {
    if (this.seconds && this.seconds > 0) {
      this.showText = false;
    }
    this.observationEventService.observationResetEvent.subscribe((parentId) => {
      if (parentId === this.parentId) {
        this.resetComponent();
      }
    })
  }

  getActivityText() {
    if (this.activityType?.code === ActivityTypeConstants.Disinfection)
      return 'Spirit';
    if (this.activityType?.code === ActivityTypeConstants.Handwash)
      return 'Wash';
    if(this.activityType?.code === ActivityTypeConstants.NotPerformed)
      return 'Not Done';
  }

  registerActivity(modalName) {

    if (this.confirmationModalToShow && !this.timingExecuted) {
      this.modalService.open(modalName, { windowClass: 'hh-modal' });
    }
    else {
      if (this.time) {
        this.startOrStopTimer();
        this.showText = false;
      }
      else {
        this.activityRegisteredEvent.emit({ activityType: this.activityType, TimingWasPerformed: false });
      }
    }
  }

  confirmRegistration(wasConfirmed: boolean) {

    this.usedGloves = wasConfirmed;

    if (this.time) {
      this.startOrStopTimer();
      this.showText = false;
    }
    else {
      this.activityRegisteredEvent.emit({ activityType: this.activityType, TimingWasPerformed: false, glovesUsed: wasConfirmed});
    }
    
  }

  startOrStopTimer() {
    this.observationEventService.registrationActivityHasBegun.emit({ parentId: this.parentId, activityType: this.activityType })
    if (this.timerIsStarted === false) {
      this.startTimer();
      this.timingExecuted = true;
    }
    else if (this.seconds > 0) {
      this.stoppTimer();
      this.timingExecuted = false;
      this.activityRegisteredEvent.emit({ activityType: this.activityType, secondsUsed: this.seconds, TimingWasPerformed: true, glovesUsed: this.usedGloves})
    }
  }

  private resetComponent() {
    this.stoppTimer();
    this.seconds = 0;
    this.showText = true;
    this.timingExecuted = false;
  }

  private stoppTimer() {
    this.timerIsStarted = false;
    clearInterval(this.interval);
    this.interval = 0;
  }

  private startTimer() {
    this.timerIsStarted = true;
    this.seconds = 0;
    this.interval = setInterval(() => {
      this.seconds++;
    }, 1000);
  }
}
