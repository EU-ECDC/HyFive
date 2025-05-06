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
  selector: 'app-rediger-fire-indikasjoner-observasjon',
  templateUrl: './rediger-fire-indikasjoner-observasjon.component.html'
})
export class RedigerFireIndikasjonerObservasjonComponent implements OnInit {

  erRedigeringsmodus: boolean = false;
  ActivityTypeConstants = ActivityTypeConstants;
  activity: Activity;
  fireIndikasjoner: IndicationType[];
  activityTypes: ActivityType[];
  Colors = Colors;
  dialogueTexts = DialogueTexts;
  ActivityTypeNotExecutedSelection: ActivityTypeNotExecuted[];
  selectedActivityTypeNotExecutedSelectionId: string;
  id: string = Uuid.generateUUID().substr(4);
  showActivityTypeNotExecuted: boolean = false;
  hanskebrukTekst: string;
  ikkeUtfortAktivitet: Activity;
  sprit: string = Activities.Alcohol;
  vask: string = Activities.Wash;

  faHandHoldingWater = faHandHoldingWater;
  faHandsWash = faHandsWash;
  faSave = faSave;
  faTrashAlt = faTrashAlt;
  faTimesCircle = faTimesCircle;

  constructor(
    private sesjonService: FourIndicationsSessionService,
    private activityService: ActivityService
  ) {
    this.ActivityTypeNotExecutedSelection = ActivityTypeNotExecutedMapper.getNameMap();
  }

  @Input("isReadonly") isReadonly: boolean = false;
  @Input("observation") observation: FourIndicationsObservation;
  @Input("department") department: Department;
  @Input("gloveUseMustBeRegistered") gloveUseMustBeRegistered: boolean;
  @Input("timeShouldBeRegistred") timeShouldBeRegistred: boolean;
  @Output("observasjonSlettetEvent") observasjonSlettetEvent: EventEmitter<FourIndicationsObservation> = new EventEmitter<FourIndicationsObservation>();

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
        this.hanskebrukTekst = this.ActivityTypeNotExecutedSelection[0].name;
      else if (this.observation.activity.gloveUsed === true)
        this.hanskebrukTekst = this.ActivityTypeNotExecutedSelection[1].name;
      else if (this.observation.activity.gloveUsed === false)
        this.hanskebrukTekst = this.ActivityTypeNotExecutedSelection[2].name;
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

  registrerAktivitet(activity: Activity) {
    if (this.observation.activity.activityType.code === ActivityTypeConstants.NotExecuted) {
      this.ikkeUtfortAktivitet = this.observation.activity;
    }
    if (activity.activityType.code === ActivityTypeConstants.NotExecuted) {
      this.showActivityTypeNotExecuted = true;
      if (this.ikkeUtfortAktivitet) {
        this.selectedActivityTypeNotExecutedSelectionId = this.valgtAktivitetTypeIkkeUtfort(this.ikkeUtfortAktivitet?.gloveUsed);
      }
      else {
        this.selectedActivityTypeNotExecutedSelectionId = ActivityTypeNotExecutedId.NotExecuted.toString();
      }
      activity.timeSpent = this.observation.activity.timeSpent;
      this.observation.activity = !this.ikkeUtfortAktivitet ? activity : this.ikkeUtfortAktivitet;
      this.ikkeUtfortAktivitet = null;
    }
    else {
      this.showActivityTypeNotExecuted = false;
      activity.timeSpent = !this.observation.activity.timeSpent ? 0 : this.observation.activity.timeSpent;
      activity.timeRecordingWasDone = activity.timeSpent > 0;
      this.observation.activity = activity;
      if (this.observation.activity.activityType.code === ActivityTypeConstants.Handwash) {
        this.vask = '';
        this.sprit = Activities.Alcohol;
      }
      else {
        this.sprit = '';
        this.vask = Activities.Wash;
      }
    }
  }

  valgtAktivitetTypeIkkeUtfort(gloveUsed: boolean): string {
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

  indikasjonsValgChanged(valgteIndikasjoner: IndicationType[]) {
    this.observation.indicationTypes = valgteIndikasjoner;
  }

  lagreObservasjon() {
    if (this.gloveUseMustBeRegistered && this.observation.activity.activityType.code === ActivityTypeConstants.NotExecuted) {
      this.observation = this.registrereAktivitetTypeIkkeUtfort(this.observation);
    }
    else if (!this.gloveUseMustBeRegistered && this.observation.activity.activityType.code === ActivityTypeConstants.NotExecuted) {
      this.observation.activity.timeSpent = 0;
    }
    this.sesjonService.endreObservasjon(this.observation);
    this.erRedigeringsmodus = false;
    this.ikkeUtfortAktivitet = null;
  }

  slettObservasjon() {
    this.sesjonService.slettObservasjon(this.observation);
    this.observasjonSlettetEvent.emit();
  }

  erAktivitetValgt(aktivitetTypeKode: string): boolean {
    return this.observation.activity.activityType.code === aktivitetTypeKode;
  }

  getAktivitetType(code: string) {
    return this.activityTypes?.find(x => x.code === code);
  }

  valgtAktivitetTypeIkkeUtfortEndret(valgtId) {
    this.selectedActivityTypeNotExecutedSelectionId = valgtId;
  }

  private registrereAktivitetTypeIkkeUtfort(observation: FourIndicationsObservation): FourIndicationsObservation {
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

  registrerKommentar(comment: string) {
    this.observation.comment = comment;
  }
}
