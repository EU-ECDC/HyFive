import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { OverforingstatusTypeKonstanter } from '../../../models/api/OverforingstatusTypeKonstanter';
import { SesjonType } from '../../../models/api/SesjonType';
import { FireIndikasjonerObservasjon } from '../../../models/api/FireIndikasjonerObservasjon';
import { Rolle } from '../../../models/api/Rolle';
import { IndikasjonType } from '../../../models/api/IndikasjonType';
import { AktivitetType } from '../../../models/api/AktivitetType';
import { AktivitetTypeKonstanter } from '../../../models/api/AktivitetTypeKonstanter';
import { ObservasjonService } from '../../../services/data/observasjon.service';
import { ToastrService } from 'ngx-toastr';
import {HandsmykkeObservasjon} from "../../../models/api/HandsmykkeObservasjon";
import {Avdeling} from "../../../models/api/Avdeling";
import {KeyEventService} from "../../../services/events/key-event.service";

@Component({
  selector: 'app-rediger-fire-indikasjoner-observasjoner',
  templateUrl: './rediger-fire-indikasjoner-observasjoner.component.html'
})
export class RedigerFireIndikasjonerObservasjonerComponent implements OnInit {

  @Input() observasjoner: FireIndikasjonerObservasjon[]
  @Input() sesjonId: string;
  @Input() avdeling: Avdeling;
  @Input() kanRedigere = false;
  @Output() observasjonOppdatertEvent = new EventEmitter();
  @Output() observasjonSlettetEvent = new EventEmitter();

  fireIndikasjonerObservasjonSomEndres: FireIndikasjonerObservasjon = null;
  handsmykkeObservasjonSomEndres: HandsmykkeObservasjon = null;

  sekunderBrukt: number;

  kanLagres = true;
  overforingstatusTypeKonstanter = OverforingstatusTypeKonstanter;
  SesjonType = SesjonType;

  constructor(
    private observasjonService: ObservasjonService,
    private toastrService: ToastrService,
    private keyEventService: KeyEventService) { }

  ngOnInit(): void {
    this.keyEventService.escapeKeyEvent.subscribe((event: KeyboardEvent) => {
      if (this.fireIndikasjonerObservasjonSomEndres)
        this.fireIndikasjonerObservasjonSomEndres= null;
    });
  }

  velgObservasjon(observasjon: FireIndikasjonerObservasjon) {
    if(!this.kanRedigere){
      return;
    }

    this.fireIndikasjonerObservasjonSomEndres = JSON.parse(JSON.stringify(observasjon));
    this.fireIndikasjonerObservasjonSomEndres.sesjonId = this.sesjonId;
  }

  velgRolle(rolle: Rolle) {
    this.fireIndikasjonerObservasjonSomEndres.rolle = rolle;
  }

  indikasjonsValgChanged(valgteIndikasjoner: IndikasjonType[]) {
    this.fireIndikasjonerObservasjonSomEndres.indikasjonstyper = valgteIndikasjoner;
  }

  velgAktivitet(aktivitetType: AktivitetType) {
    this.fireIndikasjonerObservasjonSomEndres.aktivitet.aktivitetType = aktivitetType;
  }

  endretSekunderBrukt(sekunderBrukt: number) {
    this.fireIndikasjonerObservasjonSomEndres.aktivitet.sekunderBrukt = sekunderBrukt;
  }

  endretKommentar(kommentar: string) {
    this.fireIndikasjonerObservasjonSomEndres.kommentar = kommentar;
  }

  oppdaterFireIndikasjonerObservasjon() {
    if (this.fireIndikasjonerObservasjonSomEndres.aktivitet.aktivitetType.kode === AktivitetTypeKonstanter.IkkeUtfort
      || this.fireIndikasjonerObservasjonSomEndres.aktivitet.aktivitetType.kode === AktivitetTypeKonstanter.IkkeRegistrert) {
      this.fireIndikasjonerObservasjonSomEndres.aktivitet.sekunderBrukt = 0;
      this.fireIndikasjonerObservasjonSomEndres.aktivitet.tidtakingBleUtfort = false;
    }
    else {
      this.fireIndikasjonerObservasjonSomEndres.aktivitet.benyttetHanske = null;
      if (this.fireIndikasjonerObservasjonSomEndres.aktivitet.sekunderBrukt <= 0){
        this.fireIndikasjonerObservasjonSomEndres.aktivitet.sekunderBrukt = 0;
        this.fireIndikasjonerObservasjonSomEndres.aktivitet.tidtakingBleUtfort = false;
      }
      else {
        this.fireIndikasjonerObservasjonSomEndres.aktivitet.tidtakingBleUtfort = true;
      }
    }

    if (this.kanLagres) {
      this.observasjonService.oppdaterFireIndikasjonerObservasjon(this.fireIndikasjonerObservasjonSomEndres).subscribe(
        (erOppdatert) => {
          this.fireIndikasjonerObservasjonSomEndres = null;
          this.toastrService.success('Observasjonen ble oppdatert');
          this.observasjonOppdatertEvent.emit();
        },
        (error) => {
          this.toastrService.error(error?.error ? error.error : error, 'Feil ved oppdatering av observasjon', { disableTimeOut: true});
        }
      );
    }
  }

  slettFireIndikasjonerObservasjon() {
    this.observasjonService.slettFireIndikasjonerObservasjon(this.fireIndikasjonerObservasjonSomEndres.id, this.sesjonId).subscribe(
      () => {
        this.fireIndikasjonerObservasjonSomEndres = null;
        this.toastrService.success('Observasjonen ble slettet');
        this.observasjonSlettetEvent.emit();
      },
      (error) => {
        this.toastrService.error(error?.error ? error.error : error,'Feil ved sletting av observasjon', { disableTimeOut: true});
      });
  }

  avbrytRedigeringAvObservasjon(event) {
    event.stopPropagation();
    this.fireIndikasjonerObservasjonSomEndres = null;
  }
}
