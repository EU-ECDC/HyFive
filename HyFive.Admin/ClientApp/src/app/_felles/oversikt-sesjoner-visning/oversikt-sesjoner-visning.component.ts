import { DatePipe } from '@angular/common';
import { Component, EventEmitter, Input, OnInit, Output, OnDestroy } from '@angular/core';
import { faPaperPlane } from '@fortawesome/free-solid-svg-icons';
import { OverforingstatusTypeKonstanter } from '../../models/api/OverforingstatusTypeKonstanter';
import { SesjonOversiktRapport } from '../../models/api/SesjonOversiktRapport';
import { SesjonType } from '../../models/api/SesjonType';
import { ObservasjonService } from '../../services/data/observasjon.service';
import { ToastrService } from 'ngx-toastr';
import {SesjonService} from '../../services/data/sesjon.service';

@Component({
  selector: 'app-oversikt-sesjoner-visning',
  templateUrl: './oversikt-sesjoner-visning.component.html'
})
export class OversiktSesjonerVisningComponent implements OnInit, OnDestroy {

  faPaperPlane = faPaperPlane;

  @Input() sesjoner: SesjonOversiktRapport[] = [];
  @Input() isOverforingOversikt = false;

  @Output() overforEvent = new EventEmitter();
  @Output() observasjonOppdatertEvent = new EventEmitter();
  @Output() observasjonSlettetEvent = new EventEmitter();

  erOverfortTilFhi = false;
  overforingstatusTypeKonstanter = OverforingstatusTypeKonstanter;
  SesjonType = SesjonType;

  constructor(
    private observasjonService: ObservasjonService,
    private sesjonService: SesjonService,
    private toastrService: ToastrService,
    private datePipe: DatePipe) { }

  ngOnInit(): void {
  }

  ngOnDestroy(): void {
    this.toastrService.clear();
  }

  isOverfortTilFhi(kode) {
    return kode === OverforingstatusTypeKonstanter.OverfortTilFhi;
  }

  overfor(sesjonId, event) {
    event.stopPropagation();
    this.overforEvent.emit(sesjonId);
  }

  velgOverforTilFhi(sesjonId, event) {
    event.stopPropagation();
    this.sesjoner.find(s => s.id === sesjonId).erValgt = event.target.checked; 
  }

  slettSesjon(sesjonOversiktRapport: SesjonOversiktRapport) {
    const feilmelding = `En feil skjedde under sletting av sesjon med id ${sesjonOversiktRapport.id}`;
    this.sesjonService.slettSesjon(sesjonOversiktRapport.id, sesjonOversiktRapport.avdeling.institusjonId).subscribe(
      (erSlettet) => {
        if (erSlettet){
          this.toastrService.success(`Sesjon med id ${sesjonOversiktRapport.id} ble slettet`);
          this.sesjoner = this.sesjoner.filter((s) => s.id !== sesjonOversiktRapport.id);
        }
        else {
          this.toastrService.error(feilmelding, '', { disableTimeOut: true});
        }
      },
      (error) => {
        this.toastrService.error(feilmelding + ' : ' + error?.error ? error.error : error, '',  { disableTimeOut: true});
      }
    );
  }

  hentSlettemelding(sesjonOversiktRapport: SesjonOversiktRapport) {
    return `Du er i ferd med å slette sesjon registrert av ${sesjonOversiktRapport.observatorNavn},
    opprettet ${this.datePipe.transform(sesjonOversiktRapport.opprettettidspunkt, 'dd.MM.yyyy HH:mm')} med ${sesjonOversiktRapport.observasjoner?.length} tilhørende observasjon${sesjonOversiktRapport.observasjoner?.length > 1 ? 'er' : ''}.
    Er du sikker på at du vil slette denne sesjonen?`;
  }
}
