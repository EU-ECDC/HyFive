import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {ObservasjonOversiktRapport} from "../../../models/api/ObservasjonOversiktRapport";
import {Avdeling} from "../../../models/api/Avdeling";
import {ObservasjonService} from "../../../services/data/observasjon.service";
import {ToastrService} from "ngx-toastr";
import {KeyEventService} from "../../../services/events/key-event.service";
import {HanskeMedIndikasjonTypeService} from "../../../services/data/hanskemedindikasjontype.service";
import {HanskeUtenIndikasjonTypeService} from "../../../services/data/hanskeutenindikasjontype.service";
import {HandhygieneEtterHanskebrukTypeService} from "../../../services/data/handhygieneetterhanskebruktype.service";
import {HanskeMedIndikasjonType} from "../../../models/api/HanskeMedIndikasjonType";
import {HanskeUtenIndikasjonType} from "../../../models/api/HanskeUtenIndikasjonType";
import {Rolle} from "../../../models/api/Rolle";
import {HanskeObservasjon} from "../../../models/api/HanskeObservasjon";
import {HandhygieneEtterHanskebrukType} from "../../../models/api/HandhygieneEtterHanskebrukType";

@Component({
  selector: 'app-rediger-hanske-observasjoner',
  templateUrl: './rediger-hanske-observasjoner.component.html'
})
export class RedigerHanskeObservasjonerComponent implements OnInit{

  @Input() observasjoner: ObservasjonOversiktRapport[]
  @Input() sesjonId: string;
  @Input() avdeling: Avdeling;
  @Input() kanRedigere = false;

  @Output() observasjonOppdatertEvent = new EventEmitter();
  @Output() observasjonSlettetEvent = new EventEmitter();

  hanskeObservasjonSomEndres: HanskeObservasjon;
  valgtHygieneEtterHanskebrukKode: any;

  hanskeMedIndikasjonerValgt: boolean;

  hanskeMedIndikasjonTyper: HanskeMedIndikasjonType[];
  hanskeUtenIndikasjonTyper: HanskeUtenIndikasjonType[];
  handhygieneEtterHanskebrukTyper: HandhygieneEtterHanskebrukType[];

  constructor(
    private observasjonService: ObservasjonService,
    private toastrService: ToastrService,
    private keyEventService: KeyEventService,
    private hanskeMedIndikasjonService: HanskeMedIndikasjonTypeService,
    private hanskeUtenIndikasjonService: HanskeUtenIndikasjonTypeService,
    private handhygieneEtterHanskebrukService: HandhygieneEtterHanskebrukTypeService

  ) { }

  ngOnInit(): void {
    this.handhygieneEtterHanskebrukService.hentHandhygieneEtterHanskebrukTyper().subscribe((typer) => {
      this.handhygieneEtterHanskebrukTyper = typer;
    })

    this.hanskeMedIndikasjonService.hentHanskeMedIndikasjonTyper().subscribe((typer)=> {
      this.hanskeMedIndikasjonTyper = typer;
    })

    this.hanskeUtenIndikasjonService.hentHanskeUtenIndikasjonTyper().subscribe((typer)=> {
      this.hanskeUtenIndikasjonTyper = typer;
    })

    this.keyEventService.escapeKeyEvent.subscribe((event: KeyboardEvent) => {
      if (this.hanskeObservasjonSomEndres)
        this.hanskeObservasjonSomEndres = null;
    });
  }

  velgObservasjon(observasjon: ObservasjonOversiktRapport) {
    if(!this.kanRedigere){
      return;
    }

    this.hanskeObservasjonSomEndres = {
      benyttetHanske: observasjon.hanskeObservasjon.benyttetHanske,
      id: observasjon.id,
      sesjonId:  this.sesjonId,
      kommentar: observasjon.kommentar,
      rolle: observasjon.rolle,
      registrerttidspunkt: observasjon.registrerttidspunkt,
      handhygieneEtterHanskebrukType: observasjon.hanskeObservasjon.handhygieneEtterHanskebrukType,
    }
    this.hanskeMedIndikasjonTyper.map((h) =>{
      h.erValgt = observasjon.hanskeObservasjon.hanskeMedIndikasjonTyper.map(x => x.kode).indexOf(h.kode) !== -1;
      return h;
    });

    this.hanskeUtenIndikasjonTyper.map((h) =>{
      h.erValgt = observasjon.hanskeObservasjon.hanskeUtenIndikasjonTyper.map(x => x.kode).indexOf(h.kode) !== -1;
      return h;
    });

    this.valgtHygieneEtterHanskebrukKode = this.hanskeObservasjonSomEndres.handhygieneEtterHanskebrukType?.kode;
    this.hanskeMedIndikasjonerValgt = this.hanskeMedIndikasjonTyper.filter(h => h.erValgt).length > 0;
    if(this.hanskeMedIndikasjonerValgt){
      this.hanskeObservasjonSomEndres.hanskeUtenIndikasjonTyper = [];
    }
    else{
      // Impliserer at hanskeUtenIndikasjonTyper er valgt. Da skal hanske benyttes
      this.hanskeObservasjonSomEndres.hanskeMedIndikasjonTyper = [];
      this.hanskeObservasjonSomEndres.benyttetHanske = true;
    }
  }

  endretKommentar(kommentar: string) {
    this.hanskeObservasjonSomEndres.kommentar = kommentar;
  }

  oppdaterHanskeObservasjon() {
    if(this.hanskeObservasjonSomEndres.benyttetHanske){
      this.hanskeObservasjonSomEndres.handhygieneEtterHanskebrukType = this.handhygieneEtterHanskebrukTyper.find(x => x.kode === this.valgtHygieneEtterHanskebrukKode);
    }
    else {
      this.hanskeObservasjonSomEndres.handhygieneEtterHanskebrukType = null;
      this.valgtHygieneEtterHanskebrukKode = null;
    }

    if(this.hanskeMedIndikasjonerValgt){
      this.hanskeObservasjonSomEndres.hanskeMedIndikasjonTyper = this.hanskeMedIndikasjonTyper.filter(h => h.erValgt);
      this.hanskeObservasjonSomEndres.hanskeUtenIndikasjonTyper = [];
    }
    else {
      this.hanskeObservasjonSomEndres.hanskeUtenIndikasjonTyper = this.hanskeUtenIndikasjonTyper.filter(h => h.erValgt);
      this.hanskeObservasjonSomEndres.hanskeMedIndikasjonTyper = [];
    }

    this.observasjonService.oppdaterHanskeObservasjon(this.hanskeObservasjonSomEndres).subscribe(
      (erOppdatert) => {
        this.hanskeObservasjonSomEndres = null;
        this.toastrService.success('Observasjonen ble oppdatert');
        this.observasjonOppdatertEvent.emit();
      },
      (error) => {
        this.toastrService.error(error?.error ? error.error : error, 'Feil ved oppdatering av observasjon: ', { disableTimeOut: true});
      }
    );
  }

  slettHanskeObservasjon() {
    this.observasjonService.slettHanskeObservasjon(this.hanskeObservasjonSomEndres.id, this.sesjonId).subscribe(
      () => {
        this.hanskeObservasjonSomEndres = null;
        this.toastrService.success('Observasjonen ble slettet');
        this.observasjonSlettetEvent.emit();
      },
      (error) => {
        this.toastrService.error(error?.error ? error.error : error, 'Feil ved sletting av observasjon', { disableTimeOut: true});
      });
  }

  avbrytRedigeringAvObservasjon(event) {
    event.stopPropagation();
    this.hanskeObservasjonSomEndres = null;
    this.valgtHygieneEtterHanskebrukKode = null;
  }

  velgRolle(rolle: Rolle) {
    this.hanskeObservasjonSomEndres.rolle = rolle;
  }

  settBenyttetHanskeHvisAktuelt(hanskeMedIndikasjonerValgt: boolean) {
    if(hanskeMedIndikasjonerValgt == false){
      this.hanskeObservasjonSomEndres.benyttetHanske = true;
    }
  }
}
