import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FourIndicationsObservation } from '../../models/api/FourIndicationsObservation';
import { Department } from '../../models/api/Department';
import { Activity } from '../../models/api/Activity';
import { FireIndikasjonerSesjonService } from '../../services/data/fire-indikasjoner-sesjon.service';
import { faHandHoldingWater, faSave, faHandsWash, faTimesCircle, faTrashAlt } from '@fortawesome/free-solid-svg-icons';
import { Farger } from '../../utils/farger';
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
  Farger = Farger;
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
    private sesjonService: FireIndikasjonerSesjonService,
    private activityService: ActivityService
  ) {
    this.ActivityTypeNotExecutedSelection = ActivityTypeNotExecutedMapper.getNameMap();
  }

  @Input("isReadonly") isReadonly: boolean = false;
  @Input("observasjon") observasjon: FourIndicationsObservation;
  @Input("department") department: Department;
  @Input("gloveUseMustBeRegistered") gloveUseMustBeRegistered: boolean;
  @Input("timeShouldBeRegistred") timeShouldBeRegistred: boolean;
  @Output("observasjonSlettetEvent") observasjonSlettetEvent: EventEmitter<FourIndicationsObservation> = new EventEmitter<FourIndicationsObservation>();

  ngOnInit(): void {
    if (this.observasjon.activity.activityType.code === ActivityTypeConstants.NotExecuted) {
      this.showActivityTypeNotExecuted = true;
      this.selectedActivityTypeNotExecutedSelectionId = this.getSelectedActivityTypeNotExecutedId(this.observasjon.activity);
    }

    this.activityService.getActivityTypes().subscribe((activityTypes) => {
      this.activityTypes = activityTypes;
    });

    if (this.isReadonly && this.observasjon.activity.activityType.code === ActivityTypeConstants.NotExecuted) {
      if (this.observasjon.activity.gloveUsed === null)
        this.hanskebrukTekst = this.ActivityTypeNotExecutedSelection[0].name;
      else if (this.observasjon.activity.gloveUsed === true)
        this.hanskebrukTekst = this.ActivityTypeNotExecutedSelection[1].name;
      else if (this.observasjon.activity.gloveUsed === false)
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
    if (this.observasjon.activity.activityType.code === ActivityTypeConstants.NotExecuted) {
      this.ikkeUtfortAktivitet = this.observasjon.activity;
    }
    if (activity.activityType.code === ActivityTypeConstants.NotExecuted) {
      this.showActivityTypeNotExecuted = true;
      if (this.ikkeUtfortAktivitet) {
        this.selectedActivityTypeNotExecutedSelectionId = this.valgtAktivitetTypeIkkeUtfort(this.ikkeUtfortAktivitet?.gloveUsed);
      }
      else {
        this.selectedActivityTypeNotExecutedSelectionId = ActivityTypeNotExecutedId.NotExecuted.toString();
      }
      activity.timeSpent = this.observasjon.activity.timeSpent;
      this.observasjon.activity = !this.ikkeUtfortAktivitet ? activity : this.ikkeUtfortAktivitet;
      this.ikkeUtfortAktivitet = null;
    }
    else {
      this.showActivityTypeNotExecuted = false;
      activity.timeSpent = !this.observasjon.activity.timeSpent ? 0 : this.observasjon.activity.timeSpent;
      activity.timeRecordingWasDone = activity.timeSpent > 0;
      this.observasjon.activity = activity;
      if (this.observasjon.activity.activityType.code === ActivityTypeConstants.Handwash) {
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
    this.observasjon.indicationTypes = valgteIndikasjoner;
  }

  lagreObservasjon() {
    if (this.gloveUseMustBeRegistered && this.observasjon.activity.activityType.code === ActivityTypeConstants.NotExecuted) {
      this.observasjon = this.registrereAktivitetTypeIkkeUtfort(this.observasjon);
    }
    else if (!this.gloveUseMustBeRegistered && this.observasjon.activity.activityType.code === ActivityTypeConstants.NotExecuted) {
      this.observasjon.activity.timeSpent = 0;
    }
    this.sesjonService.endreObservasjon(this.observasjon);
    this.erRedigeringsmodus = false;
    this.ikkeUtfortAktivitet = null;
  }

  slettObservasjon() {
    this.sesjonService.slettObservasjon(this.observasjon);
    this.observasjonSlettetEvent.emit();
  }

  erAktivitetValgt(aktivitetTypeKode: string): boolean {
    return this.observasjon.activity.activityType.code === aktivitetTypeKode;
  }

  getAktivitetType(code: string) {
    return this.activityTypes?.find(x => x.code === code);
  }

  valgtAktivitetTypeIkkeUtfortEndret(valgtId) {
    this.selectedActivityTypeNotExecutedSelectionId = valgtId;
  }

  private registrereAktivitetTypeIkkeUtfort(observasjon: FourIndicationsObservation): FourIndicationsObservation {
    if ((!this.selectedActivityTypeNotExecutedSelectionId || this.selectedActivityTypeNotExecutedSelectionId === ActivityTypeNotExecutedId.NotExecuted.toString())) {
      observasjon.activity.gloveUsed = null;
    }
    else if (this.selectedActivityTypeNotExecutedSelectionId === ActivityTypeNotExecutedId.GloveWasUsed.toString()) {
      observasjon.activity.gloveUsed = true;
    }
    else if (this.selectedActivityTypeNotExecutedSelectionId === ActivityTypeNotExecutedId.GloveWasNotUsed.toString()) {
      observasjon.activity.gloveUsed = false;
    }
    observasjon.activity.timeRecordingWasDone = false;
    observasjon.activity.timeSpent = 0;
    return observasjon;
  }

  registrerKommentar(comment: string) {
    this.observasjon.comment = comment;
  }
}
