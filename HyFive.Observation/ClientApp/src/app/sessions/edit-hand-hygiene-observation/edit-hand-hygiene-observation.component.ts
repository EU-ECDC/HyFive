import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { HandHygieneObservation } from '../../models/api/HandHygieneObservation';
import { Activity } from '../../models/api/Activity';
import { faHandHoldingWater, faSave, faHandsWash, faTimesCircle, faTrashAlt } from '@fortawesome/free-solid-svg-icons';
import { Colors } from '../../utils/colors';
import { DialogueTexts } from '../../constants/dialogueTexts';
import { IndicationType } from '../../models/api/IndicationType';
import { ActivityTypeConstants } from '../../models/api/ActivityTypeConstants';
import { ActivityType } from '../../models/api/ActivityType';
import { ActivityService } from '../../services/data/activity.service';
import { ActivityTypeNotPerformed } from '../../models/api/ActivityTypeNotPerformed';
import { ActivityTypeNotPerformedMapper } from '../../utils/ActivityTypeNotPerformedMapper';
import { Uuid } from '../../utils/uuid';
import { ActivityTypeNotPerformedId } from '../../models/api/ActivityTypeNotPerformedId';
import { Activities } from '../../constants/Activities';
import { SessionType } from 'src/app/models/api/SessionType';
import { OrganisationUnit } from 'src/app/models/api/OrganisationUnit';
import { HandHygieneSessionService } from 'src/app/services/data/hand-hygiene-session.service';

@Component({
  selector: 'app-edit-hand-hygiene-observation',
  templateUrl: './edit-hand-hygiene-observation.component.html'
})
export class EditHandHygieneObservationComponent implements OnInit {

  isEditMode: boolean = false;
  ActivityTypeConstants = ActivityTypeConstants;
  activity: Activity;
  handHygiene: IndicationType[];
  activityTypes: ActivityType[];
  Colors = Colors;
  dialogueTexts = DialogueTexts;
  activityTypeNotPerformedSelection: ActivityTypeNotPerformed[];
  selectedActivityTypeNotPerformedSelectionId: string;
  id: string = Uuid.generateUUID().substr(4);
  showActivityTypeNotPerformed: boolean = false;
  gloveUseText: string;
  notPerformedActivity: Activity;
  alcohol: string = Activities.Alcohol;
  wash: string = Activities.Wash;
  handHygieneSessionType: SessionType = SessionType.HandHygiene; 

  faHandHoldingWater = faHandHoldingWater;
  faHandsWash = faHandsWash;
  faSave = faSave;
  faTrashAlt = faTrashAlt;
  faTimesCircle = faTimesCircle;

  constructor(
    private readonly sessionService: HandHygieneSessionService,
    private readonly activityService: ActivityService
  ) {
    this.activityTypeNotPerformedSelection = ActivityTypeNotPerformedMapper.getNameMap();
  }

  @Input() isReadonly: boolean = false;
  @Input() observation: HandHygieneObservation;
  @Input() department: OrganisationUnit;
  @Input() facilityid: number;
  @Input() gloveUseMustBeRegistered: boolean;
  @Input() timeShouldBeRegistred: boolean;
  @Output() observationDeletedEvent: EventEmitter<HandHygieneObservation> = new EventEmitter<HandHygieneObservation>();

  ngOnInit(): void {
    if (this.observation.activity.activityType.code === ActivityTypeConstants.NotPerformed) {
      this.showActivityTypeNotPerformed = true;
      this.selectedActivityTypeNotPerformedSelectionId = this.getSelectedActivityTypeNotPerformedId(this.observation.activity);
    }

    this.activityService.getActivityTypes().subscribe((activityTypes) => {
      this.activityTypes = activityTypes;
    });

    if (this.isReadonly && this.observation.activity.activityType.code === ActivityTypeConstants.NotPerformed) {
      if (this.observation.activity.glovesUsed === null)
        this.gloveUseText = this.activityTypeNotPerformedSelection[0].name;
      else if (this.observation.activity.glovesUsed === true)
        this.gloveUseText = this.activityTypeNotPerformedSelection[1].name;
      else if (this.observation.activity.glovesUsed === false)
        this.gloveUseText = this.activityTypeNotPerformedSelection[2].name;
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
      this.showActivityTypeNotPerformed = true;
      if (this.notPerformedActivity) {
        this.selectedActivityTypeNotPerformedSelectionId = this.selectedActivityTypeNotPerformed(this.notPerformedActivity?.glovesUsed);
      }
      else {
        this.selectedActivityTypeNotPerformedSelectionId = ActivityTypeNotPerformedId.NotPerformed.toString();
      }
      activity.secondsUsed = this.observation.activity.secondsUsed;
      this.observation.activity = !this.notPerformedActivity ? activity : this.notPerformedActivity;
      this.notPerformedActivity = null;
    }
    else {
      this.showActivityTypeNotPerformed = false;
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

  selectedActivityTypeNotPerformed(glovesUsed: boolean): string {
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
      this.observation = this.registerActivityTypeNotPerformed(this.observation);
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

  selectedActivityTypeNotPerformedChanged(selectedId) {
    this.selectedActivityTypeNotPerformedSelectionId = selectedId;
  }

  private registerActivityTypeNotPerformed(observation: HandHygieneObservation): HandHygieneObservation {
    if ((!this.selectedActivityTypeNotPerformedSelectionId || this.selectedActivityTypeNotPerformedSelectionId === ActivityTypeNotPerformedId.NotPerformed.toString())) {
      observation.activity.glovesUsed = null;
    }
    else if (this.selectedActivityTypeNotPerformedSelectionId === ActivityTypeNotPerformedId.GloveWasUsed.toString()) {
      observation.activity.glovesUsed = true;
    }
    else if (this.selectedActivityTypeNotPerformedSelectionId === ActivityTypeNotPerformedId.GloveWasNotUsed.toString()) {
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
