import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { TransferStatusTypeConstants } from '../../../models/api/TransferStatusTypeConstants';
import { SessionType } from '../../../models/api/SessionType';
import { Role } from '../../../models/api/Role';
import { IndicationType } from '../../../models/api/IndicationType';
import { ActivityType } from '../../../models/api/ActivityType';
import { ActivityTypeConstants } from '../../../models/api/ActivityTypeConstants';
import { ObservationService } from '../../../services/data/observation.service';
import { ToastrService } from 'ngx-toastr';
import {HandJewelryObservation} from "../../../models/api/HandJewelryObservation";
import {Department} from "../../../models/api/Department";
import {KeyEventService} from "../../../services/events/key-event.service";
import { FiveIndicatorsObservation } from 'src/app/models/api/FiveIndicatorsObservation';

@Component({
  selector: 'app-edit-five-indications-observations',
  templateUrl: './edit-five-indications-observations.component.html'
})
export class EditFiveIndicationsObservationsComponent implements OnInit {

  @Input() observations: FiveIndicatorsObservation[]
  @Input() sessionId: string;
  @Input() department: Department;
  @Input() canEdit = false;
  @Output() observationUpdatedEvent = new EventEmitter();
  @Output() observationDeletedEvent = new EventEmitter();

  fiveIndicationsObservationWhichChanged: FiveIndicatorsObservation = null;
  handJewelryObservationAsChanged: HandJewelryObservation = null;

  secondsUsed: number;

  canBeStored = true;
  transferstatusTypeConstants = TransferStatusTypeConstants;
  SessionType = SessionType;

  constructor(
    private observationService: ObservationService,
    private toastrService: ToastrService,
    private keyEventService: KeyEventService) { }

  ngOnInit(): void {
    this.keyEventService.escapeKeyEvent.subscribe((event: KeyboardEvent) => {
      if (this.fiveIndicationsObservationWhichChanged)
        this.fiveIndicationsObservationWhichChanged= null;
    });
  }

  selectObservation(observation: FiveIndicatorsObservation) {
    if(!this.canEdit){
      return;
    }

    this.fiveIndicationsObservationWhichChanged = JSON.parse(JSON.stringify(observation));
    this.fiveIndicationsObservationWhichChanged.sessionId = this.sessionId;
  }

  selectRole(role: Role) {
    this.fiveIndicationsObservationWhichChanged.role = role;
  }

  indicationOptionChanged(selectedIndications: IndicationType[]) {
    this.fiveIndicationsObservationWhichChanged.indicationTypes = selectedIndications;
  }

  selectActivity(ActivityType: ActivityType) {
    this.fiveIndicationsObservationWhichChanged.activity.activityType = ActivityType;
  }

  changeSecondsUsed(secondsUsed: number) {
    this.fiveIndicationsObservationWhichChanged.activity.secondsUsed = secondsUsed;
  }

  changeComment(comment: string) {
    this.fiveIndicationsObservationWhichChanged.comment = comment;
  }

  updateFiveIndicationsObservation() {
    if (this.fiveIndicationsObservationWhichChanged.activity.activityType.code === ActivityTypeConstants.NotExecuted
      || this.fiveIndicationsObservationWhichChanged.activity.activityType.code === ActivityTypeConstants.NotRegistered) {
      this.fiveIndicationsObservationWhichChanged.activity.secondsUsed = 0;
      this.fiveIndicationsObservationWhichChanged.activity.timingWasPerformed = false;
    }
    else {
      this.fiveIndicationsObservationWhichChanged.activity.gloveUsed = null;
      if (this.fiveIndicationsObservationWhichChanged.activity.secondsUsed <= 0){
        this.fiveIndicationsObservationWhichChanged.activity.secondsUsed = 0;
        this.fiveIndicationsObservationWhichChanged.activity.timingWasPerformed = false;
      }
      else {
        this.fiveIndicationsObservationWhichChanged.activity.timingWasPerformed = true;
      }
    }

    if (this.canBeStored) {
      this.observationService.updateFiveIndicationsObservation(this.fiveIndicationsObservationWhichChanged).subscribe(
        (isUpdated) => {
          this.fiveIndicationsObservationWhichChanged = null;
          this.toastrService.success('The observation was updated');
          this.observationUpdatedEvent.emit();
        },
        (error) => {
          this.toastrService.error(error?.error ? error.error : error, 'Error when updating the observation', { disableTimeOut: true});
        }
      );
    }
  }

  deleteFiveIndicationsObservation() {
    this.observationService.deleteFiveIndicationsObservation(this.fiveIndicationsObservationWhichChanged.id, this.sessionId).subscribe(
      () => {
        this.fiveIndicationsObservationWhichChanged = null;
        this.toastrService.success('The observation was deleted');
        this.observationDeletedEvent.emit();
      },
      (error) => {
        this.toastrService.error(error?.error ? error.error : error,'Error when deleting observation', { disableTimeOut: true});
      });
  }

  cancelEditOfObservation(event) {
    event.stopPropagation();
    this.fiveIndicationsObservationWhichChanged = null;
  }
}
