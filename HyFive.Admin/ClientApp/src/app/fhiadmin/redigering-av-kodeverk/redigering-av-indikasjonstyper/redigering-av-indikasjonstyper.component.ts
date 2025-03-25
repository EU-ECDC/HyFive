import { Component, OnInit, OnDestroy } from '@angular/core';
import { IndikasjonstyperService } from '../../../services/data/indikasjonstyper.service';
import { IndikasjonType } from '../../../models/api/IndikasjonType';
import { ToastrService } from 'ngx-toastr';
import { KeyEventService } from '../../../services/events/key-event.service';

@Component({
  selector: 'app-redigering-av-indikasjonstyper',
  templateUrl: './redigering-av-indikasjonstyper.component.html'
})
export class RedigeringAvIndikasjonstyperComponent implements OnInit, OnDestroy {

  indikasjonstyper: IndikasjonType[] = [];
  indikasjonstypeSomEndres: IndikasjonType = null;

  constructor(
    private indikasjonstyperService: IndikasjonstyperService,
    private toastrService: ToastrService,
    private keyEventService: KeyEventService
  ) { }

  ngOnInit(): void {
    this.keyEventService.escapeKeyEvent.subscribe((event: KeyboardEvent) => {
      this.avbrytRedigering();
    });

    this.lastIndikasjonstyper();
  }

  ngOnDestroy(): void {
    this.toastrService.clear();
  }
  
  lastIndikasjonstyper() {
    this.indikasjonstyperService.hentIndikasjonstyper().subscribe(
      (resultat) => this.indikasjonstyper = resultat,
      (error) => this.toastrService.error('Det oppstod en feil under lasting av Indikasjonstyper: ' + error?.message, '', { disableTimeOut: true}),
    );
  }

  valgtIndikasjonstype(indikasjonstype: IndikasjonType): void {
    if (this.indikasjonstypeSomEndres?.id == indikasjonstype.id) return;
    this.indikasjonstypeSomEndres = JSON.parse(JSON.stringify(indikasjonstype));
  }

  oppdaterIndikasjonstype(indikasjonstype: IndikasjonType): void {
    indikasjonstype.nummer = indikasjonstype.nummer.toString();
    this.indikasjonstyperService.oppdaterIndikasjonstype(indikasjonstype).subscribe(
      (oppdatertIndikasjonstype) => {
        this.toastrService.success("Indikasjonstype oppdatert");
        this.lastIndikasjonstyper();
      },
      error => this.toastrService.error('Det oppstod en feil under oppdatering av Indikasjonstype: ' + error?.error, '', { disableTimeOut: true}),
      () => this.indikasjonstypeSomEndres = null
    );
  }

  avbrytRedigering($event: Event = null) {
    if($event){
      $event.stopPropagation();
      $event.preventDefault();
    }
    this.indikasjonstypeSomEndres = null;
  }
}
