import { Component, OnInit, Output, EventEmitter, Input, OnDestroy } from '@angular/core';
import { FeilbrukType } from '../../../../models/api/FeilbrukType';
import { BeskyttelsesutstyrType } from '../../../../models/api/BeskyttelsesutstyrType';
import { BeskyttelsesutstyrtyperService } from '../../../../services/data/beskyttelsesutstyrtyper.service';
import { faChevronLeft } from '@fortawesome/free-solid-svg-icons';
import { ToastrService } from 'ngx-toastr';
import { OpprettFeilbrukTypeRequest } from '../../../../models/api/OpprettFeilbrukTypeRequest';
import { KeyEventService } from '../../../../services/events/key-event.service';

@Component({
  selector: 'app-redigering-av-feilbruktyper',
  templateUrl: './redigering-av-feilbruktyper.component.html'
})
export class RedigeringAvFeilbruktyperComponent implements OnInit, OnDestroy {

  nyFeilbruktype: OpprettFeilbrukTypeRequest = this.tomRequest();
  feilbruktyper: FeilbrukType[] = [];
  feilbruktypeSomEndres: FeilbrukType = null;

  faChevronLeft = faChevronLeft;

  @Input() utstyrtype: BeskyttelsesutstyrType;
  @Output() visRedigeringAvBeskyttelsesutstyrtyperEvent: EventEmitter<boolean> = new EventEmitter<boolean>();

  constructor(
    private beskyttelsesutstyrtyperService: BeskyttelsesutstyrtyperService,
    private toastrService: ToastrService,
    private keyEventService: KeyEventService
  ) { }

  ngOnInit(): void {
    this.keyEventService.escapeKeyEvent.subscribe((event: KeyboardEvent) => {
      this.avbrytRedigering();
    });

    this.lastFeilbruktyper();
  }

  
  ngOnDestroy(): void {
    this.toastrService.clear();
  }

  lastFeilbruktyper() {
    this.beskyttelsesutstyrtyperService.hentFeilbruktyper(this.utstyrtype.id).subscribe(
      (feilbruktyper) => this.feilbruktyper = feilbruktyper,
      (error) => this.toastrService.error('Det oppstod en feil under lasting av Feilbruktyper: ' + error?.message, '', { disableTimeOut: true}),
    );
  }

  tomRequest(): OpprettFeilbrukTypeRequest {
    return {
      navn: null
    };
  }

  opprettFeilbruktype(): void {
    this.beskyttelsesutstyrtyperService.opprettFeilbruktype(this.utstyrtype.id, this.nyFeilbruktype).subscribe(
      (opprettetFeilbruktype) => this.toastrService.success(`Feilbruktype opprettet.`),
      error => this.toastrService.error(`En feil skjedde under opprettelse av feilbruktype ${this.nyFeilbruktype.navn}. Feil: "${error.error}"`, '', { disableTimeOut: true}),
      () => { this.nyFeilbruktype = this.tomRequest(); this.lastFeilbruktyper(); }
    );
  }

  valgtFeilbruktype(feilbruktype: FeilbrukType) {
    if (this.feilbruktypeSomEndres?.id == feilbruktype.id) return;
    this.feilbruktypeSomEndres = JSON.parse(JSON.stringify(feilbruktype));
  }

  oppdaterFeilbruktype(feilbruktype: FeilbrukType): void {
    this.beskyttelsesutstyrtyperService.oppdaterFeilbruktype(this.utstyrtype.id, feilbruktype).subscribe(
      (result) => {
        this.toastrService.success('Feilbruktype ble oppdatert');
        this.lastFeilbruktyper();
      },
      (error) => {
        this.toastrService.error('En feil skjedde under oppdatering av feilbruktype: ' + error?.error, '', { disableTimeOut: true});
      },
      () => this.feilbruktypeSomEndres = null
    );
  }

  navigerTilBeskyttelsesutsyrtyper(): void {
    this.visRedigeringAvBeskyttelsesutstyrtyperEvent.emit(true);
  }

  avbrytRedigering($event: Event = null) {
    if($event){
      $event.stopPropagation();
      $event.preventDefault();
    }
    this.feilbruktypeSomEndres = null;
  }
}
