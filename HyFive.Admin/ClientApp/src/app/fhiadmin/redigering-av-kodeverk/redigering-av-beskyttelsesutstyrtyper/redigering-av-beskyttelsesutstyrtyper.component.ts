import { Component, OnInit, OnDestroy } from '@angular/core';
import { BeskyttelsesutstyrtyperService } from 'src/app/services/data/beskyttelsesutstyrtyper.service';
import { BeskyttelsesutstyrType } from '../../../models/api/BeskyttelsesutstyrType';
import { ToastrService } from 'ngx-toastr';
import { KeyEventService } from '../../../services/events/key-event.service';

@Component({
  selector: 'app-redigering-av-beskyttelsesutstyrtyper',
  templateUrl: './redigering-av-beskyttelsesutstyrtyper.component.html'
})
export class RedigeringAvBeskyttelsesutstyrtyperComponent implements OnInit, OnDestroy {

  utstyrtyper: BeskyttelsesutstyrType[] = [];
  utstyrtypeSomEndres: BeskyttelsesutstyrType = null;
  visRedigerAvBeskyttelsesutstyrtyper: boolean;

  constructor(
    private beskyttelsesutstyrtyperService: BeskyttelsesutstyrtyperService,
    private toastrService: ToastrService,
    private keyEventService: KeyEventService
  ) { }

  ngOnInit(): void {
    this.keyEventService.escapeKeyEvent.subscribe((event: KeyboardEvent) => {
      if (this.visRedigerAvBeskyttelsesutstyrtyper)
        this.avbrytRedigering();
    });

    this.visRedigerAvBeskyttelsesutstyrtyper = true;
    this.lastBeskyttelsesutstyrtyper();
  }

  ngOnDestroy(): void {
    this.toastrService.clear();
  }

  lastBeskyttelsesutstyrtyper() {
    this.beskyttelsesutstyrtyperService.hentBeskyttelsesutstyrtyper().subscribe(
      (resultat) => this.utstyrtyper = resultat,
      (error) => this.toastrService.error('Det oppstod en feil under lasting av Beskyttelsesutstyrtyper: ' + error?.message, '', { disableTimeOut: true}),
    );
  }

  valgtUtstyrtype(utstyrtype: BeskyttelsesutstyrType): void {
    if (this.utstyrtypeSomEndres?.id == utstyrtype.id) return;
    this.utstyrtypeSomEndres = JSON.parse(JSON.stringify(utstyrtype));
  }

  oppdaterUtstyrtype(utstyrtype: BeskyttelsesutstyrType): void {
    this.beskyttelsesutstyrtyperService.oppdaterBeskyttelsesutstyrtyper(utstyrtype).subscribe(
      (oppdatertUtstyrtype) => {
        this.toastrService.success("Beskyttelsesutstyrtype oppdatert");
        this.lastBeskyttelsesutstyrtyper();
      },
      error => this.toastrService.error('Det oppstod en feil under oppdatering av Beskyttelsesutstyrtype: ' + error?.error, '', { disableTimeOut: true}),
      () => this.utstyrtypeSomEndres = null
    );
  }

  navigerTilFeilbruktyper(utstyrtype: BeskyttelsesutstyrType): void {
    this.visRedigerAvBeskyttelsesutstyrtyper = false;
    this.utstyrtypeSomEndres = utstyrtype;
  }

  navigerTilBeskyttelsesutstyrtyper(visRedigerAvBeskyttelsesutstyrtyper: boolean): void {
    this.visRedigerAvBeskyttelsesutstyrtyper = visRedigerAvBeskyttelsesutstyrtyper;
    this.utstyrtypeSomEndres = null;
  }

  avbrytRedigering($event: Event = null) {
    if($event){
      $event.stopPropagation();
      $event.preventDefault();
    }
    this.utstyrtypeSomEndres = null;
  }
}
