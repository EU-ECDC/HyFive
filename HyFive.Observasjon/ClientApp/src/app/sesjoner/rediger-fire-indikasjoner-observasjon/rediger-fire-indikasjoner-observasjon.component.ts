import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FireIndikasjonerObservasjon } from '../../models/api/FireIndikasjonerObservasjon';
import { Department } from '../../models/api/Department';
import { Activity } from '../../models/api/Activity';
import { FireIndikasjonerSesjonService } from '../../services/data/fire-indikasjoner-sesjon.service';
import { faHandHoldingWater, faSave, faHandsWash, faTimesCircle, faTrashAlt } from '@fortawesome/free-solid-svg-icons';
import { Farger } from '../../utils/farger';
import { Dialogtekster } from '../../konstanter/dialogtekster';
import { IndikasjonType } from '../../models/api/IndikasjonType';
import { ActivityTypeConstants } from '../../models/api/ActivityTypeConstants';
import { ActivityType } from '../../models/api/ActivityType';
import { AktivitetService } from '../../services/data/aktivitet.service';
import { ActivityTypeNotExecuted } from '../../models/api/ActivityTypeNotExecuted';
import { ActivityTypeNotExecutedMapper } from '../../utils/ActivityTypeNotExecutedMapper';
import { Uuid } from '../../utils/uuid';
import { ActivityTypeNotExecutedId } from '../../models/api/ActivityTypeNotExecutedId';
import { Aktiviteter } from '../../konstanter/aktiviteter';

@Component({
  selector: 'app-rediger-fire-indikasjoner-observasjon',
  templateUrl: './rediger-fire-indikasjoner-observasjon.component.html'
})
export class RedigerFireIndikasjonerObservasjonComponent implements OnInit {

  erRedigeringsmodus: boolean = false;
  ActivityTypeConstants = ActivityTypeConstants;
  activity: Activity;
  fireIndikasjoner: IndikasjonType[];
  activityTypes: ActivityType[];
  Farger = Farger;
  dialogtekster = Dialogtekster;
  ActivityTypeNotExecutedSelection: ActivityTypeNotExecuted[];
  selectedActivityTypeNotExecutedSelectionId: string;
  id: string = Uuid.generateUUID().substr(4);
  showActivityTypeNotExecuted: boolean = false;
  hanskebrukTekst: string;
  ikkeUtfortAktivitet: Activity;
  sprit: string = Aktiviteter.Sprit;
  vask: string = Aktiviteter.Vask;

  faHandHoldingWater = faHandHoldingWater;
  faHandsWash = faHandsWash;
  faSave = faSave;
  faTrashAlt = faTrashAlt;
  faTimesCircle = faTimesCircle;

  constructor(
    private sesjonService: FireIndikasjonerSesjonService,
    private aktivitetService: AktivitetService
  ) {
    this.ActivityTypeNotExecutedSelection = ActivityTypeNotExecutedMapper.getNameMap();
  }

  @Input("isReadonly") isReadonly: boolean = false;
  @Input("observasjon") observasjon: FireIndikasjonerObservasjon;
  @Input("department") department: Department;
  @Input("hanskebrukSkalRegistreres") hanskebrukSkalRegistreres: boolean;
  @Input("tidtakingSkalRegistreres") tidtakingSkalRegistreres: boolean;
  @Output("observasjonSlettetEvent") observasjonSlettetEvent: EventEmitter<FireIndikasjonerObservasjon> = new EventEmitter<FireIndikasjonerObservasjon>();

  ngOnInit(): void {
    if (this.observasjon.activity.activityType.code === ActivityTypeConstants.NotExecuted) {
      this.showActivityTypeNotExecuted = true;
      this.selectedActivityTypeNotExecutedSelectionId = this.getSelectedActivityTypeNotExecutedId(this.observasjon.activity);
    }

    this.aktivitetService.getAktivitetTyper().subscribe((activityTypes) => {
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
        this.sprit = Aktiviteter.Sprit;
      }
      else {
        this.sprit = '';
        this.vask = Aktiviteter.Vask;
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

  indikasjonsValgChanged(valgteIndikasjoner: IndikasjonType[]) {
    this.observasjon.indikasjonstyper = valgteIndikasjoner;
  }

  lagreObservasjon() {
    if (this.hanskebrukSkalRegistreres && this.observasjon.activity.activityType.code === ActivityTypeConstants.NotExecuted) {
      this.observasjon = this.registrereAktivitetTypeIkkeUtfort(this.observasjon);
    }
    else if (!this.hanskebrukSkalRegistreres && this.observasjon.activity.activityType.code === ActivityTypeConstants.NotExecuted) {
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

  private registrereAktivitetTypeIkkeUtfort(observasjon: FireIndikasjonerObservasjon): FireIndikasjonerObservasjon {
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

  registrerKommentar(kommentar: string) {
    this.observasjon.kommentar = kommentar;
  }
}
