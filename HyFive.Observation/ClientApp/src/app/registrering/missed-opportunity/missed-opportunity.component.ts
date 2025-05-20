import { Component, OnInit, ChangeDetectionStrategy, Input, Output, EventEmitter } from '@angular/core';
import { Activity } from '../../models/api/Activity';
import { faHandsWash, faTimesCircle } from '@fortawesome/free-solid-svg-icons';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ActivityService } from '../../services/data/activity.service';
import { ActivityTypeConstants } from '../../models/api/ActivityTypeConstants';

@Component({
  selector: 'app-missed-opportunity',
  templateUrl: './missed-opportunity.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class MissedOpportunityComponent implements OnInit {

  faHandsWash = faHandsWash;
  faTimesCircle = faTimesCircle;
  ikonklasse = '';

  activity: Activity = null;

  @Input("gloveUseMustBeRegistered") gloveUseMustBeRegistered: boolean;
  @Input("disabled") disabled: boolean;
  @Input("isRegistered") isRegistered: boolean;

  @Output() activityRegisteredEvent = new EventEmitter<Activity>();

  constructor(private modalService: NgbModal, private activityService: ActivityService) {
    this.activityService.getActivityTypes().subscribe((activityTypes) => {
      this.activity = {
        activityType: activityTypes.find(x => x.code === ActivityTypeConstants.NotExecuted),
        timeRecordingWasDone: false,
        gloveUsed: null
      };
    });
  }

  ngOnInit(): void {
  }

  registerNotExecutedActivity(modalName): void {
    if (this.gloveUseMustBeRegistered) {
      this.modalService.open(modalName, { windowClass: 'hh-modal' });
    } else {
      this.activityRegisteredEvent.emit(this.activity);
    }
  }

  registerActivity(bleHanskerBrukt: boolean) {
    this.activity.gloveUsed = bleHanskerBrukt;
    this.activityRegisteredEvent.emit(this.activity);
  }
}
