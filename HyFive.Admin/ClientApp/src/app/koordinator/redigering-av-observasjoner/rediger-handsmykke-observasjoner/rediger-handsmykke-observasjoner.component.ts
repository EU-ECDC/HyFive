import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {ObservasjonOversiktRapport} from "../../../models/api/ObservasjonOversiktRapport";
import {BraceletObservation} from "../../../models/api/BraceletObservation";
import {Department} from "../../../models/api/Department";
import {KeyEventService} from "../../../services/events/key-event.service";
import {Role} from "../../../models/api/Role";
import {ObservationService} from "../../../services/data/observation.service";
import {ToastrService} from "ngx-toastr";
import {HandJewelrySelection} from "../../../../../../../HyFive.Observasjon/ClientApp/src/app/models/registrering/handsmykkevalg.model";
import {HandsmykketypeService} from "../../../services/data/handsmykketype.service";
import {HandsmykkeType} from "../../../models/api/HandsmykkeType";

@Component({
  selector: 'app-rediger-handsmykke-observasjoner',
  templateUrl: './rediger-handsmykke-observasjoner.component.html'
})
export class RedigerHandsmykkeObservasjonerComponent implements OnInit {

  @Input() observasjoner: ObservasjonOversiktRapport[]
  @Input() sesjonId: string;
  @Input() avdeling: Department;
  @Input() kanRedigere = false;

  @Output() observasjonOppdatertEvent = new EventEmitter();
  @Output() observasjonSlettetEvent = new EventEmitter();

  handsmykkeObservasjonSomEndres: BraceletObservation;
  handsmykkeValg: HandJewelrySelection[] = [];
  handsmykketyper: HandsmykkeType[] = [];

  constructor(
    private observationService: ObservationService,
    private toastrService: ToastrService,
    private keyEventService: KeyEventService,
    private handsmykketypeService: HandsmykketypeService
    ) { }

  ngOnInit(): void {
    this.handsmykketypeService.hentHandsmykketyper().subscribe((typer) => {
      this.handsmykketyper = typer;
    })
    this.keyEventService.escapeKeyEvent.subscribe((event: KeyboardEvent) => {
      if (this.handsmykkeObservasjonSomEndres)
        this.handsmykkeObservasjonSomEndres = null;
    });
  }

  velgObservasjon(observasjon: ObservasjonOversiktRapport) {
    if(!this.kanRedigere){
      return;
    }

    this.handsmykkeValg = this.handsmykketyper.map((t) => {
      return {
        type: t.code,
        disabled: false,
        isSelected: observasjon.handsmykketyper.map(ht => ht.code).indexOf(t.code) !== -1,
        name: t.name
      };
    })

    this.handsmykkeObservasjonSomEndres = {
      id: observasjon.id,
      sesjonId:  this.sesjonId,
      handsmykker: observasjon.handsmykketyper,
      kommentar: observasjon.kommentar,
      rolle: observasjon.rolle,
      registrerttidspunkt: observasjon.registrerttidspunkt
    }
  }

  endretKommentar(kommentar: string) {
    this.handsmykkeObservasjonSomEndres.kommentar = kommentar;
  }

  updateHandJewelryObservation() {
    var typer = this.handsmykkeValg.filter(h => h.isSelected).map(hsv => hsv.type)
    this.handsmykkeObservasjonSomEndres.handsmykker = this.handsmykketyper.filter(h => typer.indexOf(h.code) !== -1)
    this.observationService.updateHandJewelryObservation(this.handsmykkeObservasjonSomEndres).subscribe(
      (erOppdatert) => {
        this.handsmykkeObservasjonSomEndres = null;
        this.toastrService.success('Observasjonen ble oppdatert');
        this.observasjonOppdatertEvent.emit();
      },
      (error) => {
        this.toastrService.error(error?.error ? error.error : error, 'Feil ved oppdatering av observasjon: ', { disableTimeOut: true});
      }
    );

  }

  deleteHandJewelryObservation() {
    this.observationService.deleteHandJewelryObservation(this.handsmykkeObservasjonSomEndres.id, this.sesjonId).subscribe(
      () => {
        this.handsmykkeObservasjonSomEndres = null;
        this.toastrService.success('Observasjonen ble slettet');
        this.observasjonSlettetEvent.emit();
      },
      (error) => {
        this.toastrService.error(error?.error ? error.error : error, 'Feil ved sletting av observasjon', { disableTimeOut: true});
      });
  }

  avbrytRedigeringAvObservasjon(event) {
    event.stopPropagation();
    this.handsmykkeObservasjonSomEndres = null;
  }

  velgRolle(rolle: Role) {
    this.handsmykkeObservasjonSomEndres.rolle = rolle;
  }
}
