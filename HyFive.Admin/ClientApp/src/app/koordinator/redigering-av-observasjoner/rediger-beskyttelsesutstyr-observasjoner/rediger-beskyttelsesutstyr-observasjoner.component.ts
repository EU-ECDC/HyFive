import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {ObservasjonOversiktRapport} from "../../../models/api/ObservasjonOversiktRapport";
import {Avdeling} from "../../../models/api/Avdeling";
import {ObservasjonService} from "../../../services/data/observasjon.service";
import {ToastrService} from "ngx-toastr";
import {KeyEventService} from "../../../services/events/key-event.service";
import {Rolle} from "../../../models/api/Rolle";
import {BeskyttelsesutstyrObservasjon} from "../../../models/api/BeskyttelsesutstyrObservasjon";

@Component({
  selector: 'app-rediger-beskyttelsesutstyr-observasjoner',
  templateUrl: './rediger-beskyttelsesutstyr-observasjoner.component.html'
})
export class RedigerBeskyttelsesutstyrObservasjonerComponent implements OnInit {

  @Input() observasjoner: ObservasjonOversiktRapport[]
  @Input() sesjonId: string;
  @Input() avdeling: Avdeling;
  @Input() kanRedigere = false;

  @Output() observasjonOppdatertEvent = new EventEmitter();
  @Output() observasjonSlettetEvent = new EventEmitter();

  beskyttelsesutstyrObservasjonSomEndres: BeskyttelsesutstyrObservasjon;
  kanOppdatereBeskyttelsesutstyrObservasjon = false;

  registrertDato: Date = null;


  constructor(
    private observasjonService: ObservasjonService,
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
    this.beskyttelsesutstyrObservasjonSomEndres.kommentar = kommentar;
  }

  oppdaterRegistrerttidspunkt(dateString: string) {
    if (dateString) {
      this.registrertDato = new Date(dateString);
      this.beskyttelsesutstyrObservasjonSomEndres.registrerttidspunkt = this.registrertDato;
    }
  }

  oppdaterBeskyttelsesutstyrObservasjon() {

    this.observasjonService.oppdaterBeskyttelsesutstyrObservasjon(this.beskyttelsesutstyrObservasjonSomEndres).subscribe(
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

  slettBeskyttelsesutstyrObservasjon() {
    this.observasjonService.slettBeskyttelsesutstyrObservasjon(this.beskyttelsesutstyrObservasjonSomEndres.id, this.sesjonId).subscribe(
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

  velgRolle(rolle: Rolle) {
    this.beskyttelsesutstyrObservasjonSomEndres.rolle = rolle;
  }

  oppdaterObservasjonMedEndringerFraKort($event: BeskyttelsesutstyrObservasjon) {
    this.beskyttelsesutstyrObservasjonSomEndres = $event;
    this.kanOppdatereBeskyttelsesutstyrObservasjon = true;
  }
}
