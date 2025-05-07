import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FourIndicationsObservation } from '../../models/api/FourIndicationsObservation';
import { Department } from '../../models/api/Department';
import { Activity } from '../../models/api/Activity';
import { FourIndicationsSessionService } from '../../services/data/four-indications-session.service';
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
import { ActivityTypeNotExecutedId } from '../../models/api/ActivityTypeNotExecutedId';
import { Activities } from '../../constants/Activities';

@Component({
  selector: 'app-edit-four-indications-observation',
  templateUrl: './edit-four-indications-observation.component.html'
})
export class EditFourIndicationsObservationComponent implements OnInit {

  isEditMode: boolean = false;
  ActivityTypeConstants = ActivityTypeConstants;
  activity: Activity;
  fourIndications: IndicationType[];
  activityTypes: ActivityType[];
  Colors = Colors;
  dialogueTexts = DialogueTexts;
  activityTypeNotExecutedSelection: ActivityTypeNotExecuted[];
  selectedActivityTypeNotExecutedSelectionId: string;
  id: string = Uuid.generateUUID().substr(4);
  showActivityTypeNotExecuted: boolean = false;
  gloveUseText: string;
  notExecutedActivity: Activity;
  alcohol: string = Activities.Alcohol;
  wash: string = Activities.Wash;

  faHandHoldingWater = faHandHoldingWater;
  faHandsWash = faHandsWash;
  faSave = faSave;
  faTrashAlt = faTrashAlt;
  faTimesCircle = faTimesCircle;

  constructor(
    private sessionService: FourIndicationsSessionService,
    private activityService: ActivityService
  ) {
    this.activityTypeNotExecutedSelection = ActivityTypeNotExecutedMapper.getNameMap();
  }

  @Input("isReadonly") isReadonly: boolean = false;
  @Input("observation") observation: FourIndicationsObservation;
  @Input("department") department: Department;
  @Input("gloveUseMustBeRegistered") gloveUseMustBeRegistered: boolean;
  @Input("timeShouldBeRegistred") timeShouldBeRegistred: boolean;
  @Output("observationDeletedEvent") observationDeletedEvent: EventEmitter<FourIndicationsObservation> = new EventEmitter<FourIndicationsObservation>();

  ngOnInit(): void {
    if (this.observation.activity.activityType.code === ActivityTypeConstants.NotExecuted) {
      this.showActivityTypeNotExecuted = true;
      this.selectedActivityTypeNotExecutedSelectionId = this.getSelectedActivityTypeNotExecutedId(this.observation.activity);
    }

    this.activityService.getActivityTypes().subscribe((activityTypes) => {
      this.activityTypes = activityTypes;
    });

    if (this.isReadonly && this.observation.activity.activityType.code === ActivityTypeConstants.NotExecuted) {
      if (this.observation.activity.gloveUsed === null)
        this.gloveUseText = this.activityTypeNotExecutedSelection[0].name;
      else if (this.observation.activity.gloveUsed === true)
        this.gloveUseText = this.activityTypeNotExecutedSelection[1].name;
      else if (this.observation.activity.gloveUsed === false)
        this.gloveUseText = this.activityTypeNotExecutedSelection[2].name;
    }
  }

  getSelectedActivityTypeNotExecutedId(activity: Activity): string {
    if (activity.gloveUsed === null && activity.activityType.code === ActivityTypeConstants.NotExecuted) {
      return ActivityTypeNotExecutedId.NotExecuted.toString();
    }
    else if (activity.gloveUsed === true && activity.activityType.code === ActivityTypeConstants.NotExecuted) {
      return ActivityTypeNotExecutedId.GloveWasUsed.toString();
    }
    else if (activity.gloveUsed === false && activity.activityType.code === ActivityTypeConstants.NotExecuted) {
      return ActivityTypeNotExecutedId.GloveWasNotUsed.toString();
    }
  }

  registerActivity(activity: Activity) {
    if (this.observation.activity.activityType.code === ActivityTypeConstants.NotExecuted) {
      this.notExecutedActivity = this.observation.activity;
    }
    if (activity.activityType.code === ActivityTypeConstants.NotExecuted) {
      this.showActivityTypeNotExecuted = true;
      if (this.notExecutedActivity) {
        this.selectedActivityTypeNotExecutedSelectionId = this.selectedActivityTypeNotExecuted(this.notExecutedActivity?.gloveUsed);
      }
      else {
        this.selectedActivityTypeNotExecutedSelectionId = ActivityTypeNotExecutedId.NotExecuted.toString();
      }
      activity.timeSpent = this.observation.activity.timeSpent;
      this.observation.activity = !this.notExecutedActivity ? activity : this.notExecutedActivity;
      this.notExecutedActivity = null;
    }
    else {
      this.showActivityTypeNotExecuted = false;
      activity.timeSpent = !this.observation.activity.timeSpent ? 0 : this.observation.activity.timeSpent;
      activity.timeRecordingWasDone = activity.timeSpent > 0;
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

  selectedActivityTypeNotExecuted(gloveUsed: boolean): string {
    if (gloveUsed === true) {
      return ActivityTypeNotExecutedId.GloveWasUsed.toString();
    }
    else if (gloveUsed === false) {
      return ActivityTypeNotExecutedId.GloveWasNotUsed.toString();
    }
    else if (gloveUsed || gloveUsed === null) {
      return ActivityTypeNotExecutedId.NotExecuted.toString();
    }
  }

  indicationSelectionChanged(selectedIndications: IndicationType[]) {
    this.observation.indicationTypes = selectedIndications;
  }

  saveObservation() {
    if (this.gloveUseMustBeRegistered && this.observation.activity.activityType.code === ActivityTypeConstants.NotExecuted) {
      this.observation = this.registerActivityTypeNotExecuted(this.observation);
    }
    else if (!this.gloveUseMustBeRegistered && this.observation.activity.activityType.code === ActivityTypeConstants.NotExecuted) {
      this.observation.activity.timeSpent = 0;
    }
    this.sessionService.changeObservation(this.observation);
    this.isEditMode = false;
    this.notExecutedActivity = null;
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

  private registerActivityTypeNotExecuted(observation: FourIndicationsObservation): FourIndicationsObservation {
    if ((!this.selectedActivityTypeNotExecutedSelectionId || this.selectedActivityTypeNotExecutedSelectionId === ActivityTypeNotExecutedId.NotExecuted.toString())) {
      observation.activity.gloveUsed = null;
    }
    else if (this.selectedActivityTypeNotExecutedSelectionId === ActivityTypeNotExecutedId.GloveWasUsed.toString()) {
      observation.activity.gloveUsed = true;
    }
    else if (this.selectedActivityTypeNotExecutedSelectionId === ActivityTypeNotExecutedId.GloveWasNotUsed.toString()) {
      observation.activity.gloveUsed = false;
    }
    observation.activity.timeRecordingWasDone = false;
    observation.activity.timeSpent = 0;
    return observation;
  }

  registerComment(comment: string) {
    this.observation.comment = comment;
  }
}
