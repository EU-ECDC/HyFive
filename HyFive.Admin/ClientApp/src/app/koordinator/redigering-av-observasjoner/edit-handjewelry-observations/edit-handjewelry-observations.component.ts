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
  selector: 'app-edit-handjewelry-observations',
  templateUrl: './edit-handjewelry-observations.component.html'
})
export class EditHandjewelryObservationsComponent implements OnInit {

  @Input() observasjoner: ObservasjonOversiktRapport[]
  @Input() sesjonId: string;
  @Input() avdeling: Department;
  @Input() kanRedigere = false;

  @Output() observasjonOppdatertEvent = new EventEmitter();
  @Output() observasjonSlettetEvent = new EventEmitter();

  handjewelryObservationAsChanged: BraceletObservation;
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
      if (this.handjewelryObservationAsChanged)
        this.handjewelryObservationAsChanged = null;
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

    this.handjewelryObservationAsChanged = {
      id: observasjon.id,
      sessionId:  this.sesjonId,
      handJewelry: observasjon.handsmykketyper,
      comment: observasjon.kommentar,
      role: observasjon.rolle,
      registrationTime: observasjon.registrerttidspunkt
    }
  }

  endretKommentar(kommentar: string) {
    this.handjewelryObservationAsChanged.comment = kommentar;
  }

  updateHandJewelryObservation() {
    var typer = this.handsmykkeValg.filter(h => h.isSelected).map(hsv => hsv.type)
    this.handjewelryObservationAsChanged.handJewelry = this.handsmykketyper.filter(h => typer.indexOf(h.code) !== -1)
    this.observationService.updateHandJewelryObservation(this.handjewelryObservationAsChanged).subscribe(
      (erOppdatert) => {
        this.handjewelryObservationAsChanged = null;
        this.toastrService.success('Observasjonen ble oppdatert');
        this.observasjonOppdatertEvent.emit();
      },
      (error) => {
        this.toastrService.error(error?.error ? error.error : error, 'Feil ved oppdatering av observasjon: ', { disableTimeOut: true});
      }
    );

  }

  deleteHandJewelryObservation() {
    this.observationService.deleteHandJewelryObservation(this.handjewelryObservationAsChanged.id, this.sesjonId).subscribe(
      () => {
        this.handjewelryObservationAsChanged = null;
        this.toastrService.success('Observasjonen ble slettet');
        this.observasjonSlettetEvent.emit();
      },
      (error) => {
        this.toastrService.error(error?.error ? error.error : error, 'Feil ved sletting av observasjon', { disableTimeOut: true});
      });
  }

  avbrytRedigeringAvObservasjon(event) {
    event.stopPropagation();
    this.handjewelryObservationAsChanged = null;
  }

  velgRolle(rolle: Role) {
    this.handjewelryObservationAsChanged.role = rolle;
  }
}
