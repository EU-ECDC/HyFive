import { Component, OnInit, OnDestroy } from '@angular/core';
import { IndicationTypesService } from '../../../services/data/indicationTypes.service';
import { IndicationType } from '../../../models/api/IndicationType';
import { ToastrService } from 'ngx-toastr';
import { KeyEventService } from '../../../services/events/key-event.service';

@Component({
  selector: 'app-redigering-av-indikasjonstyper',
  templateUrl: './redigering-av-indikasjonstyper.component.html'
})
export class RedigeringAvIndikasjonstyperComponent implements OnInit, OnDestroy {

  indicationtypes: IndicationType[] = [];
  indikasjonstypeSomEndres: IndicationType = null;

  constructor(
    private indikasjonstyperService: IndicationTypesService,
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
    this.indikasjonstyperService.getIndicationTypes().subscribe(
      (resultat) => this.indicationtypes = resultat,
      (error) => this.toastrService.error('Det oppstod en feil under lasting av Indikasjonstyper: ' + error?.message, '', { disableTimeOut: true}),
    );
  }

  valgtIndikasjonstype(indicationtype: IndicationType): void {
    if (this.indikasjonstypeSomEndres?.id == indicationtype.id) return;
    this.indikasjonstypeSomEndres = JSON.parse(JSON.stringify(indicationtype));
  }

  oppdaterIndikasjonstype(indicationtype: IndicationType): void {
    indicationtype.number = indicationtype.number.toString();
    this.indikasjonstyperService.updateIndicationTypes(indicationtype).subscribe(
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
