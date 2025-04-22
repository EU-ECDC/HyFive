import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {ObservasjonOversiktRapport} from "../../../models/api/ObservasjonOversiktRapport";
import {Department} from "../../../models/api/Department";
import {ObservationService} from "../../../services/data/observation.service";
import {ToastrService} from "ngx-toastr";
import {KeyEventService} from "../../../services/events/key-event.service";
import {Role} from "../../../models/api/Role";
import {ProtectiveEquipmentObservation} from "../../../models/api/ProtectiveEquipmentObservation";

@Component({
  selector: 'app-rediger-beskyttelsesutstyr-observasjoner',
  templateUrl: './rediger-beskyttelsesutstyr-observasjoner.component.html'
})
export class RedigerBeskyttelsesutstyrObservasjonerComponent implements OnInit {

  @Input() observasjoner: ObservasjonOversiktRapport[]
  @Input() sesjonId: string;
  @Input() avdeling: Department;
  @Input() kanRedigere = false;

  @Output() observasjonOppdatertEvent = new EventEmitter();
  @Output() observasjonSlettetEvent = new EventEmitter();

  beskyttelsesutstyrObservasjonSomEndres: ProtectiveEquipmentObservation;
  kanOppdatereBeskyttelsesutstyrObservasjon = false;

  registrertDato: Date = null;


  constructor(
    private observationService: ObservationService,
    private toastrService: ToastrService,
    private keyEventService: KeyEventService
  ) { }

  ngOnInit(): void {

    this.keyEventService.escapeKeyEvent.subscribe((event: KeyboardEvent) => {
      if (this.beskyttelsesutstyrObservasjonSomEndres)
        this.avbrytRedigeringAvObservasjon(null)
    });
  }
  
  velgObservasjon(observasjon: ObservasjonOversiktRapport) {

    if(!this.kanRedigere){
      return;
    }

    this.avbrytRedigeringAvObservasjon(null)
    this.beskyttelsesutstyrObservasjonSomEndres = observasjon.beskyttelsesutstyrObservasjon
  }

  endretKommentar(kommentar: string) {
    this.beskyttelsesutstyrObservasjonSomEndres.comment = kommentar;
  }

  oppdaterRegistrerttidspunkt(dateString: string) {
    if (dateString) {
      this.registrertDato = new Date(dateString);
      this.beskyttelsesutstyrObservasjonSomEndres.registrationTime = this.registrertDato;
    }
  }

  updateProtectiveEquipmentObservation() {

    this.observationService.updateProtectiveEquipmentObservation(this.beskyttelsesutstyrObservasjonSomEndres).subscribe(
      (erOppdatert) => {
        this.beskyttelsesutstyrObservasjonSomEndres = null;
        this.toastrService.success('Observasjonen ble oppdatert');
        this.observasjonOppdatertEvent.emit();
      },
      (error) => {
        this.toastrService.error(error?.error ? error.error : error, 'Feil ved oppdatering av observasjon: ', { disableTimeOut: true});
      }
    );
  }

  deleteProtectiveEquipmentObservation() {
    this.observationService.deleteProtectiveEquipmentObservation(this.beskyttelsesutstyrObservasjonSomEndres.id, this.sesjonId).subscribe(
      () => {
        this.beskyttelsesutstyrObservasjonSomEndres = null;
        this.toastrService.success('Observasjonen ble slettet');
        this.observasjonSlettetEvent.emit();
      },
      (error) => {
        this.toastrService.error(error?.error ? error.error : error, 'Feil ved sletting av observasjon', { disableTimeOut: true});
      });
  }

  avbrytRedigeringAvObservasjon(event: Event) {
    if(event) {
      event.stopPropagation();
    }

    this.beskyttelsesutstyrObservasjonSomEndres = null;
    this.kanOppdatereBeskyttelsesutstyrObservasjon = false;
  }

  velgRolle(rolle: Role) {
    this.beskyttelsesutstyrObservasjonSomEndres.role = rolle;
  }

  oppdaterObservasjonMedEndringerFraKort($event: ProtectiveEquipmentObservation) {
    this.beskyttelsesutstyrObservasjonSomEndres = $event;
    this.kanOppdatereBeskyttelsesutstyrObservasjon = true;
  }
}
