import { Component, Input, OnInit, Output, EventEmitter } from '@angular/core';
import { Activity } from '../../models/api/Activity';
import { ObservationEventService } from '../../services/events/observation-event.service';
import { faClock } from '@fortawesome/free-solid-svg-icons';
import { ActivityType } from '../../models/api/ActivityType';
import { ActivityTypeConstants } from '../../models/api/ActivityTypeConstants';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { TranslateService } from '@ngx-translate/core';


@Component({
  selector: 'app-register-activity',
  templateUrl: './register-activity.component.html'
})

export class RegisterActivityComponent implements OnInit {

  private readonly maxSeconds = 60;

  timerIsStarted: boolean = false;
  interval;
  timingExecuted: boolean = false;
  faClock = faClock;
  showText: boolean = true;

  usedGloves?: boolean = null;

  private clampSeconds(value: number): number {
    if (value == null || Number.isNaN(value)) return 0;
    return Math.max(0, Math.min(this.maxSeconds, Math.floor(value)));
  }

  @Input() parentId: string;
  @Input() time: boolean;
  @Input() disabled: boolean;
  @Input() activityType: ActivityType;
  @Input() seconds: number;
  @Input() isRegistered: boolean;
  @Input() confirmationModalToShow: boolean;
  @Input() icon: string;

  @Output() activityRegisteredEvent = new EventEmitter<Activity>();
  activityTypeConstants = ActivityTypeConstants;

  constructor(private readonly observationEventService: ObservationEventService, 
              private readonly modalService: NgbModal,
              private readonly translate: TranslateService) {
  }

  ngOnInit(): void {
    this.seconds = this.clampSeconds(this.seconds);

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
      return this.translate.instant('ABHR');
    if (this.activityType?.code === ActivityTypeConstants.Handwash)
      return this.translate.instant('Wash');
    if(this.activityType?.code === ActivityTypeConstants.NotPerformed)
      return this.translate.instant('Not Done');
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

  private startTimer(): void {
    this.timerIsStarted = true;
    this.seconds = 0;

    this.interval = setInterval(() => {
      if (this.seconds >= this.maxSeconds) {
        this.seconds = this.maxSeconds;
        this.stoppTimer();
        this.timingExecuted = false;

        // ✅ Auto-save when reaching 60 seconds
        this.activityRegisteredEvent.emit({
          activityType: this.activityType,
          secondsUsed: this.seconds,
          TimingWasPerformed: true,
          glovesUsed: this.usedGloves
        });

        return;
      }

      this.seconds++;
    }, 1000);
  }
}
