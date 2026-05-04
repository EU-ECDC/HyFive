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
import {KeyEventService} from "../../../services/events/key-event.service";
import { DialogMessageService } from 'src/app/services/data/dialog-message.service';
import { TranslateService } from '@ngx-translate/core';
import { HandHygieneObservation } from 'src/app/models/api/HandHygieneObservation';

@Component({
  selector: 'app-edit-hand-hygiene-observations',
  templateUrl: './edit-hand-hygiene-observations.component.html'
})
export class EditHandHygieneObservationsComponent implements OnInit {

  @Input() observations: HandHygieneObservation[]
  @Input() sessionId: string;
  @Input() departmentId: number;
  @Input() canEdit = false;
  @Output() observationUpdatedEvent = new EventEmitter();
  @Output() observationDeletedEvent = new EventEmitter();

  handHygieneObservationWhichChanged: HandHygieneObservation = null;
  handJewelryObservationAsChanged: HandJewelryObservation = null;

  secondsUsed: number;

  canBeStored = true;
  transferstatusTypeConstants = TransferStatusTypeConstants;
  SessionType = SessionType;

  constructor(
    private readonly observationService: ObservationService,
    private readonly toastrService: ToastrService,
    private readonly keyEventService: KeyEventService,
    private readonly dialogMessageService: DialogMessageService,
    private readonly translate: TranslateService) { }

  ngOnInit(): void {
    this.keyEventService.escapeKeyEvent.subscribe((event: KeyboardEvent) => {
      if (this.handHygieneObservationWhichChanged)
        this.handHygieneObservationWhichChanged= null;
    });
  }

  selectObservation(observation: HandHygieneObservation) {
    if(!this.canEdit){
      return;
    }

    this.handHygieneObservationWhichChanged = structuredClone(observation);
    this.handHygieneObservationWhichChanged.sessionId = this.sessionId;
  }

  selectRole(role: Role) {
    this.handHygieneObservationWhichChanged.role = role;
  }

  indicationOptionChanged(selectedIndications: IndicationType[]) {
    this.handHygieneObservationWhichChanged.indicationTypes = selectedIndications;
  }

  selectActivity(ActivityType: ActivityType) {
    this.handHygieneObservationWhichChanged.activity.activityType = ActivityType;
  }

  changeSecondsUsed(secondsUsed: number) {
    if (secondsUsed == null || Number.isNaN(secondsUsed)) {
    this.handHygieneObservationWhichChanged.activity.secondsUsed = 0;
    return;
    }

    if (secondsUsed < 0) {
      this.handHygieneObservationWhichChanged.activity.secondsUsed = 0;
      return;
    }

    if (secondsUsed > 60) {
      this.handHygieneObservationWhichChanged.activity.secondsUsed = 60;
      return;
    }

    this.handHygieneObservationWhichChanged.activity.secondsUsed = secondsUsed;

    this.handHygieneObservationWhichChanged.activity.secondsUsed = Math.floor(secondsUsed);
  }

  changeComment(comment: string) {
    this.handHygieneObservationWhichChanged.comment = comment;
  }

  updateHandHygieneObservation() {
    if (this.handHygieneObservationWhichChanged.activity.activityType.code === ActivityTypeConstants.NotPerformed
      || this.handHygieneObservationWhichChanged.activity.activityType.code === ActivityTypeConstants.NotRegistered) {
      this.handHygieneObservationWhichChanged.activity.secondsUsed = 0;
      this.handHygieneObservationWhichChanged.activity.timingWasPerformed = false;
    }
    else {
      this.handHygieneObservationWhichChanged.activity.glovesUsed = null;
      if (this.handHygieneObservationWhichChanged.activity.secondsUsed <= 0){
        this.handHygieneObservationWhichChanged.activity.secondsUsed = 0;
        this.handHygieneObservationWhichChanged.activity.timingWasPerformed = false;
      }
      else {
        this.handHygieneObservationWhichChanged.activity.timingWasPerformed = true;
      }
    }

    if (this.canBeStored) {
      this.observationService.updateHandHygieneObservation(this.handHygieneObservationWhichChanged).subscribe(
        (isUpdated) => {
          this.handHygieneObservationWhichChanged = null;
          this.toastrService.success(this.translate.instant("The observation was updated"));
          this.observationUpdatedEvent.emit();
        },
        (error) => {
          this.toastrService.error(error?.error.message ? error.error.message : error, this.translate.instant("Error when updating the observation"), { disableTimeOut: true});
        }
      );
    }
  }

  deleteHandHygieneObservation() {
    this.observationService.deleteHandHygieneObservation(this.handHygieneObservationWhichChanged.id, this.sessionId).subscribe(
      () => {
        this.handHygieneObservationWhichChanged = null;
        this.toastrService.success(this.translate.instant("The observation was deleted"));
        this.observationDeletedEvent.emit();
      },
      (error) => {
        this.toastrService.error(error?.error.message ? error.error.message : error,this.translate.instant("Error when deleting observation"), { disableTimeOut: true});
      });
  }

  cancelEditOfObservation(event) {
    event.stopPropagation();
    this.handHygieneObservationWhichChanged = null;
  }
}
