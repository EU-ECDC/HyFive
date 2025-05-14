import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { TransferStatusTypeConstants } from '../../../models/api/TransferStatusTypeConstants';
import { SessionType } from '../../../models/api/SessionType';
import { FourIndicationsObservation } from '../../../models/api/FourIndicationsObservation';
import { Role } from '../../../models/api/Role';
import { IndicationType } from '../../../models/api/IndicationType';
import { ActivityType } from '../../../models/api/ActivityType';
import { ActivityTypeConstants } from '../../../models/api/ActivityTypeConstants';
import { ObservationService } from '../../../services/data/observation.service';
import { ToastrService } from 'ngx-toastr';
import {HandJewelryObservation} from "../../../models/api/HandJewelryObservation";
import {Department} from "../../../models/api/Department";
import {KeyEventService} from "../../../services/events/key-event.service";

@Component({
  selector: 'app-edit-four-indications-observations',
  templateUrl: './edit-four-indications-observations.component.html'
})
export class EditFourIndicationsObservationsComponent implements OnInit {

  @Input() observations: FourIndicationsObservation[]
  @Input() sessionId: string;
  @Input() department: Department;
  @Input() canEdit = false;
  @Output() observationUpdatedEvent = new EventEmitter();
  @Output() observationDeletedEvent = new EventEmitter();

  fourIndicationsObservationWhichChanged: FourIndicationsObservation = null;
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
      if (this.fourIndicationsObservationWhichChanged)
        this.fourIndicationsObservationWhichChanged= null;
    });
  }

  selectObservation(observation: FourIndicationsObservation) {
    if(!this.canEdit){
      return;
    }

    this.fourIndicationsObservationWhichChanged = JSON.parse(JSON.stringify(observation));
    this.fourIndicationsObservationWhichChanged.sessionId = this.sessionId;
  }

  selectRole(role: Role) {
    this.fourIndicationsObservationWhichChanged.role = role;
  }

  indicationOptionChanged(selectedIndications: IndicationType[]) {
    this.fourIndicationsObservationWhichChanged.indicationTypes = selectedIndications;
  }

  selectActivity(ActivityType: ActivityType) {
    this.fourIndicationsObservationWhichChanged.activity.activityType = ActivityType;
  }

  changeSecondsUsed(secondsUsed: number) {
    this.fourIndicationsObservationWhichChanged.activity.secondsUsed = secondsUsed;
  }

  changeComment(comment: string) {
    this.fourIndicationsObservationWhichChanged.comment = comment;
  }

  updateFourIndicationsObservation() {
    if (this.fourIndicationsObservationWhichChanged.activity.activityType.code === ActivityTypeConstants.NotExecuted
      || this.fourIndicationsObservationWhichChanged.activity.activityType.code === ActivityTypeConstants.NotRegistered) {
      this.fourIndicationsObservationWhichChanged.activity.secondsUsed = 0;
      this.fourIndicationsObservationWhichChanged.activity.timingWasPerformed = false;
    }
    else {
      this.fourIndicationsObservationWhichChanged.activity.gloveUsed = null;
      if (this.fourIndicationsObservationWhichChanged.activity.secondsUsed <= 0){
        this.fourIndicationsObservationWhichChanged.activity.secondsUsed = 0;
        this.fourIndicationsObservationWhichChanged.activity.timingWasPerformed = false;
      }
      else {
        this.fourIndicationsObservationWhichChanged.activity.timingWasPerformed = true;
      }
    }

    if (this.canBeStored) {
      this.observationService.updateFourIndicationsObservation(this.fourIndicationsObservationWhichChanged).subscribe(
        (isUpdated) => {
          this.fourIndicationsObservationWhichChanged = null;
          this.toastrService.success('The observation was updated');
          this.observationUpdatedEvent.emit();
        },
        (error) => {
          this.toastrService.error(error?.error ? error.error : error, 'Error when updating the observation', { disableTimeOut: true});
        }
      );
    }
  }

  deleteFourIndicationsObservation() {
    this.observationService.deleteFourIndicationsObservation(this.fourIndicationsObservationWhichChanged.id, this.sessionId).subscribe(
      () => {
        this.fourIndicationsObservationWhichChanged = null;
        this.toastrService.success('The observation was deleted');
        this.observationDeletedEvent.emit();
      },
      (error) => {
        this.toastrService.error(error?.error ? error.error : error,'Error when deleting observation', { disableTimeOut: true});
      });
  }

  cancelEditOfObservation(event) {
    event.stopPropagation();
    this.fourIndicationsObservationWhichChanged = null;
  }
}
