import { Component, OnInit, OnDestroy } from '@angular/core';
import { HanskeMedIndikasjonType } from '../../../models/api/HanskeMedIndikasjonType';
import { HanskeMedIndikasjonTypeService } from '../../../services/data/hanskemedindikasjontype.service';
import { ToastrService } from 'ngx-toastr';
import { KeyEventService } from '../../../services/events/key-event.service';

@Component({
  selector: 'app-redigering-av-hanskemedindikasjontyper',
  templateUrl: './redigering-av-hanskemedindikasjontyper.component.html'
})
export class RedigeringAvHanskemedindikasjontyperComponent implements OnInit, OnDestroy {

  hanskeMedIndikasjonTyper: HanskeMedIndikasjonType[] = [];
  hanskeMedIndikasjonTypeSomEndres: HanskeMedIndikasjonType = null;

  constructor(
    private hanskeMedIndikasjonTypeService: HanskeMedIndikasjonTypeService,
    private toastrService: ToastrService,
    private keyEventService: KeyEventService
  ) { }

  ngOnInit(): void {
    this.keyEventService.escapeKeyEvent.subscribe((event: KeyboardEvent) => {
      this.avbrytRedigering();
    });

    this.lastHanskeVedIndikasjonType();
  }
  
  ngOnDestroy(): void {
    this.toastrService.clear();
  }

  lastHanskeVedIndikasjonType() {
    this.hanskeMedIndikasjonTypeService.hentHanskeMedIndikasjonTyper().subscribe(
      (resultat) => this.hanskeMedIndikasjonTyper = resultat,
      (error) => this.toastrService.error('Det oppstod en feil under lasting av HanskeVedIndikasjonType: ' + error?.message, '', { disableTimeOut: true}),
    );
  }

  valgtHanskeMedIndikasjonType(hanskeMedIndikasjonType: HanskeMedIndikasjonType): void {
    if (this.hanskeMedIndikasjonTypeSomEndres?.id == hanskeMedIndikasjonType.id) return;
    this.hanskeMedIndikasjonTypeSomEndres = JSON.parse(JSON.stringify(hanskeMedIndikasjonType));
  }

  oppdaterHanskeMedIndikasjonType(): void {
    this.hanskeMedIndikasjonTypeService.oppdaterHanskeMedIndikasjonType(this.hanskeMedIndikasjonTypeSomEndres).subscribe(
      (oppdatertHanskeMedIndikasjonType) => {
        this.toastrService.success("HanskeVedIndikasjonType oppdatert");
        this.lastHanskeVedIndikasjonType();
      },
      error => this.toastrService.error('Det oppstod en feil under oppdatering av HanskeVedIndikasjonType: ' + error?.error, '', { disableTimeOut: true}),
      () => this.hanskeMedIndikasjonTypeSomEndres = null
    );
  }

  avbrytRedigering($event: Event = null) {
    if($event){
      $event.stopPropagation();
      $event.preventDefault();
    }
    this.hanskeMedIndikasjonTypeSomEndres = null;
  }
}
