import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { TransferstatusTypeConstants } from '../../../models/api/TransferstatusTypeConstants';
import { SessionType } from '../../../models/api/SessionType';
import { FourIndicationsObservation } from '../../../models/api/FourIndicationsObservation';
import { Role } from '../../../models/api/Role';
import { IndikasjonType } from '../../../models/api/IndikasjonType';
import { AktivitetType } from '../../../models/api/AktivitetType';
import { AktivitetTypeKonstanter } from '../../../models/api/AktivitetTypeKonstanter';
import { ObservationService } from '../../../services/data/observation.service';
import { ToastrService } from 'ngx-toastr';
import {BraceletObservation} from "../../../models/api/BraceletObservation";
import {Department} from "../../../models/api/Department";
import {KeyEventService} from "../../../services/events/key-event.service";

@Component({
  selector: 'app-rediger-fire-indikasjoner-observasjoner',
  templateUrl: './rediger-fire-indikasjoner-observasjoner.component.html'
})
export class RedigerFourIndicationsObservationerComponent implements OnInit {

  @Input() observasjoner: FourIndicationsObservation[]
  @Input() sesjonId: string;
  @Input() avdeling: Department;
  @Input() kanRedigere = false;
  @Output() observasjonOppdatertEvent = new EventEmitter();
  @Output() observasjonSlettetEvent = new EventEmitter();

  fourIndicationsObservationSomEndres: FourIndicationsObservation = null;
  handsmykkeObservasjonSomEndres: BraceletObservation = null;

  sekunderBrukt: number;

  kanLagres = true;
  transferstatusTypeConstants = TransferstatusTypeConstants;
  SessionType = SessionType;

  constructor(
    private observationService: ObservationService,
    private toastrService: ToastrService,
    private keyEventService: KeyEventService) { }

  ngOnInit(): void {
    this.keyEventService.escapeKeyEvent.subscribe((event: KeyboardEvent) => {
      if (this.fourIndicationsObservationSomEndres)
        this.fourIndicationsObservationSomEndres= null;
    });
  }

  velgObservasjon(observasjon: FourIndicationsObservation) {
    if(!this.kanRedigere){
      return;
    }

    this.fourIndicationsObservationSomEndres = JSON.parse(JSON.stringify(observasjon));
    this.fourIndicationsObservationSomEndres.sesjonId = this.sesjonId;
  }

  velgRolle(rolle: Role) {
    this.fourIndicationsObservationSomEndres.rolle = rolle;
  }

  indikasjonsValgChanged(valgteIndikasjoner: IndikasjonType[]) {
    this.fourIndicationsObservationSomEndres.indicationTypes = valgteIndikasjoner;
  }

  velgAktivitet(aktivitetType: AktivitetType) {
    this.fourIndicationsObservationSomEndres.activity.aktivitetType = aktivitetType;
  }

  endretSekunderBrukt(sekunderBrukt: number) {
    this.fourIndicationsObservationSomEndres.activity.sekunderBrukt = sekunderBrukt;
  }

  endretKommentar(kommentar: string) {
    this.fourIndicationsObservationSomEndres.kommentar = kommentar;
  }

  updateFourIndicationsObservation() {
    if (this.fourIndicationsObservationSomEndres.activity.aktivitetType.code === AktivitetTypeKonstanter.IkkeUtfort
      || this.fourIndicationsObservationSomEndres.activity.aktivitetType.code === AktivitetTypeKonstanter.IkkeRegistrert) {
      this.fourIndicationsObservationSomEndres.activity.sekunderBrukt = 0;
      this.fourIndicationsObservationSomEndres.activity.tidtakingBleUtfort = false;
    }
    else {
      this.fourIndicationsObservationSomEndres.activity.benyttetHanske = null;
      if (this.fourIndicationsObservationSomEndres.activity.sekunderBrukt <= 0){
        this.fourIndicationsObservationSomEndres.activity.sekunderBrukt = 0;
        this.fourIndicationsObservationSomEndres.activity.tidtakingBleUtfort = false;
      }
      else {
        this.fourIndicationsObservationSomEndres.activity.tidtakingBleUtfort = true;
      }
    }

    if (this.kanLagres) {
      this.observationService.updateFourIndicationsObservation(this.fourIndicationsObservationSomEndres).subscribe(
        (erOppdatert) => {
          this.fourIndicationsObservationSomEndres = null;
          this.toastrService.success('Observasjonen ble oppdatert');
          this.observasjonOppdatertEvent.emit();
        },
        (error) => {
          this.toastrService.error(error?.error ? error.error : error, 'Feil ved oppdatering av observasjon', { disableTimeOut: true});
        }
      );
    }
  }

  deleteFourIndicationsObservation() {
    this.observationService.deleteFourIndicationsObservation(this.fourIndicationsObservationSomEndres.id, this.sesjonId).subscribe(
      () => {
        this.fourIndicationsObservationSomEndres = null;
        this.toastrService.success('Observasjonen ble slettet');
        this.observasjonSlettetEvent.emit();
      },
      (error) => {
        this.toastrService.error(error?.error ? error.error : error,'Feil ved sletting av observasjon', { disableTimeOut: true});
      });
  }

  avbrytRedigeringAvObservasjon(event) {
    event.stopPropagation();
    this.fourIndicationsObservationSomEndres = null;
  }
}
