import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FiveIndicationsObservation } from '../../models/api/FiveIndicationsObservation';
import { Department } from '../../models/api/Department';
import { Activity } from '../../models/api/Activity';
import { FiveIndicationsSessionService } from '../../services/data/five-indications-session.service';
import { faHandHoldingWater, faSave, faHandsWash, faTimesCircle, faTrashAlt } from '@fortawesome/free-solid-svg-icons';
import { Colors } from '../../utils/colors';
import { DialogueTexts } from '../../constants/dialogueTexts';
import { IndicationType } from '../../models/api/IndicationType';
import { ActivityTypeConstants } from '../../models/api/ActivityTypeConstants';
import { ActivityType } from '../../models/api/ActivityType';
import { ActivityService } from '../../services/data/activity.service';
import { ActivityTypeNotExecuted } from '../../models/api/ActivityTypeNotExecuted';
import { ActivityTypeNotExecutedMapper } from '../../utils/ActivityTypeNotExecutedMapper';
import { Uuid } from '../../utils/uuid';
import { ActivityTypeNotPerformedId } from '../../models/api/ActivityTypeNotPerformedId';
import { Activities } from '../../constants/Activities';
import { SessionType } from 'src/app/models/api/SessionType';

@Component({
  selector: 'app-edit-five-indications-observation',
  templateUrl: './edit-five-indications-observation.component.html'
})
export class EditFiveIndicationsObservationComponent implements OnInit {

  isEditMode: boolean = false;
  ActivityTypeConstants = ActivityTypeConstants;
  activity: Activity;
  fiveindications: IndicationType[];
  activityTypes: ActivityType[];
  Colors = Colors;
  dialogueTexts = DialogueTexts;
  activityTypeNotExecutedSelection: ActivityTypeNotExecuted[];
  selectedActivityTypeNotExecutedSelectionId: string;
  id: string = Uuid.generateUUID().substr(4);
  showActivityTypeNotExecuted: boolean = false;
  gloveUseText: string;
  notPerformedActivity: Activity;
  alcohol: string = Activities.Alcohol;
  wash: string = Activities.Wash;
  fiveIndicationsSessionType: SessionType = SessionType.FiveIndications; 

  faHandHoldingWater = faHandHoldingWater;
  faHandsWash = faHandsWash;
  faSave = faSave;
  faTrashAlt = faTrashAlt;
  faTimesCircle = faTimesCircle;

  constructor(
    private sessionService: FiveIndicationsSessionService,
    private activityService: ActivityService
  ) {
    this.activityTypeNotExecutedSelection = ActivityTypeNotExecutedMapper.getNameMap();
  }

  @Input("isReadonly") isReadonly: boolean = false;
  @Input("observation") observation: FiveIndicationsObservation;
  @Input("department") department: Department;
  @Input("facilityid") facilityid: number;
  @Input("gloveUseMustBeRegistered") gloveUseMustBeRegistered: boolean;
  @Input("timeShouldBeRegistred") timeShouldBeRegistred: boolean;
  @Output("observationDeletedEvent") observationDeletedEvent: EventEmitter<FiveIndicationsObservation> = new EventEmitter<FiveIndicationsObservation>();

  ngOnInit(): void {
    if (this.observation.activity.activityType.code === ActivityTypeConstants.NotPerformed) {
      this.showActivityTypeNotExecuted = true;
      this.selectedActivityTypeNotExecutedSelectionId = this.getSelectedActivityTypeNotPerformedId(this.observation.activity);
    }

    this.activityService.getActivityTypes().subscribe((activityTypes) => {
      this.activityTypes = activityTypes;
    });

    if (this.isReadonly && this.observation.activity.activityType.code === ActivityTypeConstants.NotPerformed) {
      if (this.observation.activity.glovesUsed === null)
        this.gloveUseText = this.activityTypeNotExecutedSelection[0].name;
      else if (this.observation.activity.glovesUsed === true)
        this.gloveUseText = this.activityTypeNotExecutedSelection[1].name;
      else if (this.observation.activity.glovesUsed === false)
        this.gloveUseText = this.activityTypeNotExecutedSelection[2].name;
    }
  }

  getSelectedActivityTypeNotPerformedId(activity: Activity): string {
    if (activity.glovesUsed === null && activity.activityType.code === ActivityTypeConstants.NotPerformed) {
      return ActivityTypeNotPerformedId.NotPerformed.toString();
    }
    else if (activity.glovesUsed === true && activity.activityType.code === ActivityTypeConstants.NotPerformed) {
      return ActivityTypeNotPerformedId.GloveWasUsed.toString();
    }
    else if (activity.glovesUsed === false && activity.activityType.code === ActivityTypeConstants.NotPerformed) {
      return ActivityTypeNotPerformedId.GloveWasNotUsed.toString();
    }
  }

  registerActivity(activity: Activity) {
    if (this.observation.activity.activityType.code === ActivityTypeConstants.NotPerformed) {
      this.notPerformedActivity = this.observation.activity;
    }
    if (activity.activityType.code === ActivityTypeConstants.NotPerformed) {
      this.showActivityTypeNotExecuted = true;
      if (this.notPerformedActivity) {
        this.selectedActivityTypeNotExecutedSelectionId = this.selectedActivityTypeNotExecuted(this.notPerformedActivity?.glovesUsed);
      }
      else {
        this.selectedActivityTypeNotExecutedSelectionId = ActivityTypeNotPerformedId.NotPerformed.toString();
      }
      activity.secondsUsed = this.observation.activity.secondsUsed;
      this.observation.activity = !this.notPerformedActivity ? activity : this.notPerformedActivity;
      this.notPerformedActivity = null;
    }
    else {
      this.showActivityTypeNotExecuted = false;
      activity.secondsUsed = !this.observation.activity.secondsUsed ? 0 : this.observation.activity.secondsUsed;
      activity.TimingWasPerformed = activity.secondsUsed > 0;
      this.observation.activity = activity;
      if (this.observation.activity.activityType.code === ActivityTypeConstants.Handwash) {
        this.wash = '';
        this.alcohol = Activities.Alcohol;
      }
      else {
        this.alcohol = '';
        this.wash = Activities.Wash;
      }
    }
  }

  selectedActivityTypeNotExecuted(glovesUsed: boolean): string {
    if (glovesUsed === true) {
      return ActivityTypeNotPerformedId.GloveWasUsed.toString();
    }
    else if (glovesUsed === false) {
      return ActivityTypeNotPerformedId.GloveWasNotUsed.toString();
    }
    else if (glovesUsed || glovesUsed === null) {
      return ActivityTypeNotPerformedId.NotPerformed.toString();
    }
  }

  indicationSelectionChanged(selectedIndications: IndicationType[]) {
    this.observation.indicationTypes = selectedIndications;
  }

  saveObservation() {
    if (this.gloveUseMustBeRegistered && this.observation.activity.activityType.code === ActivityTypeConstants.NotPerformed) {
      this.observation = this.registerActivityTypeNotExecuted(this.observation);
    }
    else if (!this.gloveUseMustBeRegistered && this.observation.activity.activityType.code === ActivityTypeConstants.NotPerformed) {
      this.observation.activity.secondsUsed = 0;
    }
    this.sessionService.changeObservation(this.observation);
    this.isEditMode = false;
    this.notPerformedActivity = null;
  }

  deleteObservation() {
    this.sessionService.deleteObservation(this.observation);
    this.observationDeletedEvent.emit();
  }

  isActivitySelected(activityTypeCode: string): boolean {
    return this.observation.activity.activityType.code === activityTypeCode;
  }

  getActivityType(code: string) {
    return this.activityTypes?.find(x => x.code === code);
  }

  selectedActivityTypeNotExecutedChanged(selectedId) {
    this.selectedActivityTypeNotExecutedSelectionId = selectedId;
  }

  private registerActivityTypeNotExecuted(observation: FiveIndicationsObservation): FiveIndicationsObservation {
    if ((!this.selectedActivityTypeNotExecutedSelectionId || this.selectedActivityTypeNotExecutedSelectionId === ActivityTypeNotPerformedId.NotPerformed.toString())) {
      observation.activity.glovesUsed = null;
    }
    else if (this.selectedActivityTypeNotExecutedSelectionId === ActivityTypeNotPerformedId.GloveWasUsed.toString()) {
      observation.activity.glovesUsed = true;
    }
    else if (this.selectedActivityTypeNotExecutedSelectionId === ActivityTypeNotPerformedId.GloveWasNotUsed.toString()) {
      observation.activity.glovesUsed = false;
    }
    observation.activity.TimingWasPerformed = false;
    observation.activity.secondsUsed = 0;
    return observation;
  }

  registerComment(comment: string) {
    this.observation.comment = comment;
  }
}
