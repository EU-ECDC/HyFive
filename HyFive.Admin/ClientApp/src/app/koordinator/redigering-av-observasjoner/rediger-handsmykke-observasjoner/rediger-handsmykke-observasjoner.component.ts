import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {ObservasjonOversiktRapport} from "../../../models/api/ObservasjonOversiktRapport";
import {HandsmykkeObservasjon} from "../../../models/api/HandsmykkeObservasjon";
import {Avdeling} from "../../../models/api/Avdeling";
import {KeyEventService} from "../../../services/events/key-event.service";
import {Rolle} from "../../../models/api/Rolle";
import {ObservasjonService} from "../../../services/data/observasjon.service";
import {ToastrService} from "ngx-toastr";
import {Handsmykkevalg} from "../../../../../../../HyFive.Observasjon/ClientApp/src/app/models/registrering/handsmykkevalg.model";
import {HandsmykketypeService} from "../../../services/data/handsmykketype.service";
import {HandsmykkeType} from "../../../models/api/HandsmykkeType";

@Component({
  selector: 'app-rediger-handsmykke-observasjoner',
  templateUrl: './rediger-handsmykke-observasjoner.component.html'
})
export class RedigerHandsmykkeObservasjonerComponent implements OnInit {

  @Input() observasjoner: ObservasjonOversiktRapport[]
  @Input() sesjonId: string;
  @Input() avdeling: Avdeling;
  @Input() kanRedigere = false;

  @Output() observasjonOppdatertEvent = new EventEmitter();
  @Output() observasjonSlettetEvent = new EventEmitter();

  handsmykkeObservasjonSomEndres: HandsmykkeObservasjon;
  handsmykkeValg: Handsmykkevalg[] = [];
  handsmykketyper: HandsmykkeType[] = [];

  constructor(
    private observasjonService: ObservasjonService,
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
        type: t.kode,
        disabled: false,
        erValgt: observasjon.handsmykketyper.map(ht => ht.kode).indexOf(t.kode) !== -1,
        navn: t.navn
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

  oppdaterHandsmykkeObservasjon() {
    var typer = this.handsmykkeValg.filter(h => h.erValgt).map(hsv => hsv.type)
    this.handsmykkeObservasjonSomEndres.handsmykker = this.handsmykketyper.filter(h => typer.indexOf(h.kode) !== -1)
    this.observasjonService.oppdaterHandsmykkeObservasjon(this.handsmykkeObservasjonSomEndres).subscribe(
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

  slettHandsmykkeObservasjon() {
    this.observasjonService.slettHandsmykkeObservasjon(this.handsmykkeObservasjonSomEndres.id, this.sesjonId).subscribe(
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

  velgRolle(rolle: Rolle) {
    this.handsmykkeObservasjonSomEndres.rolle = rolle;
  }
}
