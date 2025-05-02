import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FireIndikasjonerObservasjon } from '../../models/api/FireIndikasjonerObservasjon';
import { Department } from '../../models/api/Department';
import { Activity } from '../../models/api/Activity';
import { FireIndikasjonerSesjonService } from '../../services/data/fire-indikasjoner-sesjon.service';
import { faHandHoldingWater, faSave, faHandsWash, faTimesCircle, faTrashAlt } from '@fortawesome/free-solid-svg-icons';
import { Farger } from '../../utils/farger';
import { Dialogtekster } from '../../konstanter/dialogtekster';
import { IndikasjonType } from '../../models/api/IndikasjonType';
import { AktivitetTypeKonstanter } from '../../models/api/AktivitetTypeKonstanter';
import { ActivityType } from '../../models/api/ActivityType';
import { AktivitetService } from '../../services/data/aktivitet.service';
import { AktivitetTypeIkkeUtfort } from '../../models/api/AktivitetTypeIkkeUtfort';
import { AktivitetIkkeUtfortMapper } from '../../utils/AktivitetIkkeUtfortMapper';
import { Uuid } from '../../utils/uuid';
import { AktivitetTypeIkkeUtfortId } from '../../models/api/AktivitetTypeIkkeUtfortId';
import { Aktiviteter } from '../../konstanter/aktiviteter';

@Component({
  selector: 'app-rediger-fire-indikasjoner-observasjon',
  templateUrl: './rediger-fire-indikasjoner-observasjon.component.html'
})
export class RedigerFireIndikasjonerObservasjonComponent implements OnInit {

  erRedigeringsmodus: boolean = false;
  AktivitetTypeKonstanter = AktivitetTypeKonstanter;
  activity: Activity;
  fireIndikasjoner: IndikasjonType[];
  activityTypes: ActivityType[];
  Farger = Farger;
  dialogtekster = Dialogtekster;
  aktivitetTypeIkkeUtfortValg: AktivitetTypeIkkeUtfort[];
  valgtAktivitetTypeIkkeUtfortId: string;
  id: string = Uuid.generateUUID().substr(4);
  visAktivitetTypeIkkeUtfort: boolean = false;
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
    this.aktivitetTypeIkkeUtfortValg = AktivitetIkkeUtfortMapper.getNavnMap();
  }

  @Input("isReadonly") isReadonly: boolean = false;
  @Input("observasjon") observasjon: FireIndikasjonerObservasjon;
  @Input("department") department: Department;
  @Input("hanskebrukSkalRegistreres") hanskebrukSkalRegistreres: boolean;
  @Input("tidtakingSkalRegistreres") tidtakingSkalRegistreres: boolean;
  @Output("observasjonSlettetEvent") observasjonSlettetEvent: EventEmitter<FireIndikasjonerObservasjon> = new EventEmitter<FireIndikasjonerObservasjon>();

  ngOnInit(): void {
    if (this.observasjon.activity.activityType.code === AktivitetTypeKonstanter.IkkeUtfort) {
      this.visAktivitetTypeIkkeUtfort = true;
      this.valgtAktivitetTypeIkkeUtfortId = this.hentValgtAktivitetTypeIkkeUtfortId(this.observasjon.activity);
    }

    this.aktivitetService.getAktivitetTyper().subscribe((activityTypes) => {
      this.activityTypes = activityTypes;
    });

    if (this.isReadonly && this.observasjon.activity.activityType.code === AktivitetTypeKonstanter.IkkeUtfort) {
      if (this.observasjon.activity.gloveUsed === null)
        this.hanskebrukTekst = this.aktivitetTypeIkkeUtfortValg[0].name;
      else if (this.observasjon.activity.gloveUsed === true)
        this.hanskebrukTekst = this.aktivitetTypeIkkeUtfortValg[1].name;
      else if (this.observasjon.activity.gloveUsed === false)
        this.hanskebrukTekst = this.aktivitetTypeIkkeUtfortValg[2].name;
    }
  }

  hentValgtAktivitetTypeIkkeUtfortId(activity: Activity): string {
    if (activity.gloveUsed === null && activity.activityType.code === AktivitetTypeKonstanter.IkkeUtfort) {
      return AktivitetTypeIkkeUtfortId.IkkeUtfort.toString();
    }
    else if (activity.gloveUsed === true && activity.activityType.code === AktivitetTypeKonstanter.IkkeUtfort) {
      return AktivitetTypeIkkeUtfortId.HanskeBleBenyttet.toString();
    }
    else if (activity.gloveUsed === false && activity.activityType.code === AktivitetTypeKonstanter.IkkeUtfort) {
      return AktivitetTypeIkkeUtfortId.HanskeIkkeBleBenyttet.toString();
    }
  }

  registrerAktivitet(activity: Activity) {
    if (this.observasjon.activity.activityType.code === AktivitetTypeKonstanter.IkkeUtfort) {
      this.ikkeUtfortAktivitet = this.observasjon.activity;
    }
    if (activity.activityType.code === AktivitetTypeKonstanter.IkkeUtfort) {
      this.visAktivitetTypeIkkeUtfort = true;
      if (this.ikkeUtfortAktivitet) {
        this.valgtAktivitetTypeIkkeUtfortId = this.valgtAktivitetTypeIkkeUtfort(this.ikkeUtfortAktivitet?.gloveUsed);
      }
      else {
        this.valgtAktivitetTypeIkkeUtfortId = AktivitetTypeIkkeUtfortId.IkkeUtfort.toString();
      }
      activity.timeSpent = this.observasjon.activity.timeSpent;
      this.observasjon.activity = !this.ikkeUtfortAktivitet ? activity : this.ikkeUtfortAktivitet;
      this.ikkeUtfortAktivitet = null;
    }
    else {
      this.visAktivitetTypeIkkeUtfort = false;
      activity.timeSpent = !this.observasjon.activity.timeSpent ? 0 : this.observasjon.activity.timeSpent;
      activity.timeRecordingWasDone = activity.timeSpent > 0;
      this.observasjon.activity = activity;
      if (this.observasjon.activity.activityType.code === AktivitetTypeKonstanter.Handvask) {
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
      return AktivitetTypeIkkeUtfortId.HanskeBleBenyttet.toString();
    }
    else if (gloveUsed === false) {
      return AktivitetTypeIkkeUtfortId.HanskeIkkeBleBenyttet.toString();
    }
    else if (gloveUsed || gloveUsed === null) {
      return AktivitetTypeIkkeUtfortId.IkkeUtfort.toString();
    }
  }

  indikasjonsValgChanged(valgteIndikasjoner: IndikasjonType[]) {
    this.observasjon.indikasjonstyper = valgteIndikasjoner;
  }

  lagreObservasjon() {
    if (this.hanskebrukSkalRegistreres && this.observasjon.activity.activityType.code === AktivitetTypeKonstanter.IkkeUtfort) {
      this.observasjon = this.registrereAktivitetTypeIkkeUtfort(this.observasjon);
    }
    else if (!this.hanskebrukSkalRegistreres && this.observasjon.activity.activityType.code === AktivitetTypeKonstanter.IkkeUtfort) {
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
    this.valgtAktivitetTypeIkkeUtfortId = valgtId;
  }

  private registrereAktivitetTypeIkkeUtfort(observasjon: FireIndikasjonerObservasjon): FireIndikasjonerObservasjon {
    if ((!this.valgtAktivitetTypeIkkeUtfortId || this.valgtAktivitetTypeIkkeUtfortId === AktivitetTypeIkkeUtfortId.IkkeUtfort.toString())) {
      observasjon.activity.gloveUsed = null;
    }
    else if (this.valgtAktivitetTypeIkkeUtfortId === AktivitetTypeIkkeUtfortId.HanskeBleBenyttet.toString()) {
      observasjon.activity.gloveUsed = true;
    }
    else if (this.valgtAktivitetTypeIkkeUtfortId === AktivitetTypeIkkeUtfortId.HanskeIkkeBleBenyttet.toString()) {
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
