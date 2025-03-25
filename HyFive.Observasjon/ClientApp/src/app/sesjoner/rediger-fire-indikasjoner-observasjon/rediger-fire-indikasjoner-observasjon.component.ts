import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FireIndikasjonerObservasjon } from '../../models/api/FireIndikasjonerObservasjon';
import { Avdeling } from '../../models/api/Avdeling';
import { Aktivitet } from '../../models/api/Aktivitet';
import { FireIndikasjonerSesjonService } from '../../services/data/fire-indikasjoner-sesjon.service';
import { faHandHoldingWater, faSave, faHandsWash, faTimesCircle, faTrashAlt } from '@fortawesome/free-solid-svg-icons';
import { Farger } from '../../utils/farger';
import { Dialogtekster } from '../../konstanter/dialogtekster';
import { IndikasjonType } from '../../models/api/IndikasjonType';
import { AktivitetTypeKonstanter } from '../../models/api/AktivitetTypeKonstanter';
import { AktivitetType } from '../../models/api/AktivitetType';
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
  aktivitet: Aktivitet;
  fireIndikasjoner: IndikasjonType[];
  aktivitetTyper: AktivitetType[];
  Farger = Farger;
  dialogtekster = Dialogtekster;
  aktivitetTypeIkkeUtfortValg: AktivitetTypeIkkeUtfort[];
  valgtAktivitetTypeIkkeUtfortId: string;
  id: string = Uuid.generateUUID().substr(4);
  visAktivitetTypeIkkeUtfort: boolean = false;
  hanskebrukTekst: string;
  ikkeUtfortAktivitet: Aktivitet;
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
  @Input("avdeling") avdeling: Avdeling;
  @Input("hanskebrukSkalRegistreres") hanskebrukSkalRegistreres: boolean;
  @Input("tidtakingSkalRegistreres") tidtakingSkalRegistreres: boolean;
  @Output("observasjonSlettetEvent") observasjonSlettetEvent: EventEmitter<FireIndikasjonerObservasjon> = new EventEmitter<FireIndikasjonerObservasjon>();

  ngOnInit(): void {
    if (this.observasjon.aktivitet.aktivitetType.kode === AktivitetTypeKonstanter.IkkeUtfort) {
      this.visAktivitetTypeIkkeUtfort = true;
      this.valgtAktivitetTypeIkkeUtfortId = this.hentValgtAktivitetTypeIkkeUtfortId(this.observasjon.aktivitet);
    }

    this.aktivitetService.getAktivitetTyper().subscribe((aktivitetTyper) => {
      this.aktivitetTyper = aktivitetTyper;
    });

    if (this.isReadonly && this.observasjon.aktivitet.aktivitetType.kode === AktivitetTypeKonstanter.IkkeUtfort) {
      if (this.observasjon.aktivitet.benyttetHanske === null)
        this.hanskebrukTekst = this.aktivitetTypeIkkeUtfortValg[0].navn;
      else if (this.observasjon.aktivitet.benyttetHanske === true)
        this.hanskebrukTekst = this.aktivitetTypeIkkeUtfortValg[1].navn;
      else if (this.observasjon.aktivitet.benyttetHanske === false)
        this.hanskebrukTekst = this.aktivitetTypeIkkeUtfortValg[2].navn;
    }
  }

  hentValgtAktivitetTypeIkkeUtfortId(aktivitet: Aktivitet): string {
    if (aktivitet.benyttetHanske === null && aktivitet.aktivitetType.kode === AktivitetTypeKonstanter.IkkeUtfort) {
      return AktivitetTypeIkkeUtfortId.IkkeUtfort.toString();
    }
    else if (aktivitet.benyttetHanske === true && aktivitet.aktivitetType.kode === AktivitetTypeKonstanter.IkkeUtfort) {
      return AktivitetTypeIkkeUtfortId.HanskeBleBenyttet.toString();
    }
    else if (aktivitet.benyttetHanske === false && aktivitet.aktivitetType.kode === AktivitetTypeKonstanter.IkkeUtfort) {
      return AktivitetTypeIkkeUtfortId.HanskeIkkeBleBenyttet.toString();
    }
  }

  registrerAktivitet(aktivitet: Aktivitet) {
    if (this.observasjon.aktivitet.aktivitetType.kode === AktivitetTypeKonstanter.IkkeUtfort) {
      this.ikkeUtfortAktivitet = this.observasjon.aktivitet;
    }
    if (aktivitet.aktivitetType.kode === AktivitetTypeKonstanter.IkkeUtfort) {
      this.visAktivitetTypeIkkeUtfort = true;
      if (this.ikkeUtfortAktivitet) {
        this.valgtAktivitetTypeIkkeUtfortId = this.valgtAktivitetTypeIkkeUtfort(this.ikkeUtfortAktivitet?.benyttetHanske);
      }
      else {
        this.valgtAktivitetTypeIkkeUtfortId = AktivitetTypeIkkeUtfortId.IkkeUtfort.toString();
      }
      aktivitet.sekunderBrukt = this.observasjon.aktivitet.sekunderBrukt;
      this.observasjon.aktivitet = !this.ikkeUtfortAktivitet ? aktivitet : this.ikkeUtfortAktivitet;
      this.ikkeUtfortAktivitet = null;
    }
    else {
      this.visAktivitetTypeIkkeUtfort = false;
      aktivitet.sekunderBrukt = !this.observasjon.aktivitet.sekunderBrukt ? 0 : this.observasjon.aktivitet.sekunderBrukt;
      aktivitet.tidtakingBleUtfort = aktivitet.sekunderBrukt > 0;
      this.observasjon.aktivitet = aktivitet;
      if (this.observasjon.aktivitet.aktivitetType.kode === AktivitetTypeKonstanter.Handvask) {
        this.vask = '';
        this.sprit = Aktiviteter.Sprit;
      }
      else {
        this.sprit = '';
        this.vask = Aktiviteter.Vask;
      }
    }
  }

  valgtAktivitetTypeIkkeUtfort(benyttetHanske: boolean): string {
    if (benyttetHanske === true) {
      return AktivitetTypeIkkeUtfortId.HanskeBleBenyttet.toString();
    }
    else if (benyttetHanske === false) {
      return AktivitetTypeIkkeUtfortId.HanskeIkkeBleBenyttet.toString();
    }
    else if (benyttetHanske || benyttetHanske === null) {
      return AktivitetTypeIkkeUtfortId.IkkeUtfort.toString();
    }
  }

  indikasjonsValgChanged(valgteIndikasjoner: IndikasjonType[]) {
    this.observasjon.indikasjonstyper = valgteIndikasjoner;
  }

  lagreObservasjon() {
    if (this.hanskebrukSkalRegistreres && this.observasjon.aktivitet.aktivitetType.kode === AktivitetTypeKonstanter.IkkeUtfort) {
      this.observasjon = this.registrereAktivitetTypeIkkeUtfort(this.observasjon);
    }
    else if (!this.hanskebrukSkalRegistreres && this.observasjon.aktivitet.aktivitetType.kode === AktivitetTypeKonstanter.IkkeUtfort) {
      this.observasjon.aktivitet.sekunderBrukt = 0;
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
    return this.observasjon.aktivitet.aktivitetType.kode === aktivitetTypeKode;
  }

  getAktivitetType(kode: string) {
    return this.aktivitetTyper?.find(x => x.kode === kode);
  }

  valgtAktivitetTypeIkkeUtfortEndret(valgtId) {
    this.valgtAktivitetTypeIkkeUtfortId = valgtId;
  }

  private registrereAktivitetTypeIkkeUtfort(observasjon: FireIndikasjonerObservasjon): FireIndikasjonerObservasjon {
    if ((!this.valgtAktivitetTypeIkkeUtfortId || this.valgtAktivitetTypeIkkeUtfortId === AktivitetTypeIkkeUtfortId.IkkeUtfort.toString())) {
      observasjon.aktivitet.benyttetHanske = null;
    }
    else if (this.valgtAktivitetTypeIkkeUtfortId === AktivitetTypeIkkeUtfortId.HanskeBleBenyttet.toString()) {
      observasjon.aktivitet.benyttetHanske = true;
    }
    else if (this.valgtAktivitetTypeIkkeUtfortId === AktivitetTypeIkkeUtfortId.HanskeIkkeBleBenyttet.toString()) {
      observasjon.aktivitet.benyttetHanske = false;
    }
    observasjon.aktivitet.tidtakingBleUtfort = false;
    observasjon.aktivitet.sekunderBrukt = 0;
    return observasjon;
  }

  registrerKommentar(kommentar: string) {
    this.observasjon.kommentar = kommentar;
  }
}
