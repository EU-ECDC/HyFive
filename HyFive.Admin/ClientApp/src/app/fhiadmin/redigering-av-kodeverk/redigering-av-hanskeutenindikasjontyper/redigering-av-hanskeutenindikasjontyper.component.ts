import { Component, OnInit, OnDestroy } from '@angular/core';
import { HanskeUtenIndikasjonType } from '../../../models/api/HanskeUtenIndikasjonType';
import { ToastrService } from 'ngx-toastr';
import { HanskeUtenIndikasjonTypeService } from '../../../services/data/hanskeutenindikasjontype.service';
import { KeyEventService } from '../../../services/events/key-event.service';

@Component({
  selector: 'app-redigering-av-hanskeutenindikasjontyper',
  templateUrl: './redigering-av-hanskeutenindikasjontyper.component.html'
})
export class RedigeringAvHanskeutenindikasjontyperComponent implements OnInit, OnDestroy {

  hanskeUtenIndikasjonTyper: HanskeUtenIndikasjonType[] = [];
  hanskeUtenIndikasjonTypeSomEndres: HanskeUtenIndikasjonType = null;

  constructor(
    private hanskeUtenIndikasjonTypeService: HanskeUtenIndikasjonTypeService,
    private toastrService: ToastrService,
    private keyEventService: KeyEventService
  ) { }

  ngOnInit(): void {
    this.keyEventService.escapeKeyEvent.subscribe((event: KeyboardEvent) => {
      this.avbrytRedigering();
    });

    this.lastHanskeUtenIndikasjonType();
  }

  ngOnDestroy(): void {
    this.toastrService.clear();
  }

  lastHanskeUtenIndikasjonType() {
    this.hanskeUtenIndikasjonTypeService.hentHanskeUtenIndikasjonTyper().subscribe(
      (resultat) => this.hanskeUtenIndikasjonTyper = resultat,
      (error) => this.toastrService.error('Det oppstod en feil under lasting av HanskeUtenIndikasjonType: ' + error?.message, '', { disableTimeOut: true}),
    );
  }

  valgtHanskeUtenIndikasjonType(hanskeUtenIndikasjonType: HanskeUtenIndikasjonType): void {
    if (this.hanskeUtenIndikasjonTypeSomEndres?.id == hanskeUtenIndikasjonType.id) return;
    this.hanskeUtenIndikasjonTypeSomEndres = JSON.parse(JSON.stringify(hanskeUtenIndikasjonType));
  }

  oppdaterHanskeUtenIndikasjonType(): void {
    this.hanskeUtenIndikasjonTypeService.oppdaterHanskeUtenIndikasjonType(this.hanskeUtenIndikasjonTypeSomEndres).subscribe(
      (oppdatertHanskeUtenIndikasjonType) => {
        this.toastrService.success("HanskeUtenIndikasjonType oppdatert");
        this.lastHanskeUtenIndikasjonType();
      },
      error => this.toastrService.error('Det oppstod en feil under oppdatering av HanskeUtenIndikasjonType: ' + error?.error, '', { disableTimeOut: true}),
      () => this.hanskeUtenIndikasjonTypeSomEndres = null
    );
  }

  avbrytRedigering($event: Event = null) {
    if($event){
      $event.stopPropagation();
      $event.preventDefault();
    }
    this.hanskeUtenIndikasjonTypeSomEndres = null;
  }
}
