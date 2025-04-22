import { Component, OnInit, OnDestroy } from '@angular/core';
import { GloveWithIndicationType } from '../../../models/api/GloveWithIndicationType';
import { HanskeMedIndikasjonTypeService } from '../../../services/data/hanskemedindikasjontype.service';
import { ToastrService } from 'ngx-toastr';
import { KeyEventService } from '../../../services/events/key-event.service';

@Component({
  selector: 'app-redigering-av-hanskemedindikasjontyper',
  templateUrl: './redigering-av-hanskemedindikasjontyper.component.html'
})
export class RedigeringAvHanskemedindikasjontyperComponent implements OnInit, OnDestroy {

  gloveWithIndicationTypes: GloveWithIndicationType[] = [];
  gloveWithIndicationTypeThatChanges: GloveWithIndicationType = null;

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
      (resultat) => this.gloveWithIndicationTypes = resultat,
      (error) => this.toastrService.error('Det oppstod en feil under lasting av HanskeVedIndikasjonType: ' + error?.message, '', { disableTimeOut: true}),
    );
  }

  valgtHanskeMedIndikasjonType(hanskeMedIndikasjonType: GloveWithIndicationType): void {
    if (this.gloveWithIndicationTypeThatChanges?.id == hanskeMedIndikasjonType.id) return;
    this.gloveWithIndicationTypeThatChanges = JSON.parse(JSON.stringify(hanskeMedIndikasjonType));
  }

  oppdaterHanskeMedIndikasjonType(): void {
    this.hanskeMedIndikasjonTypeService.oppdaterHanskeMedIndikasjonType(this.gloveWithIndicationTypeThatChanges).subscribe(
      (oppdatertHanskeMedIndikasjonType) => {
        this.toastrService.success("HanskeVedIndikasjonType oppdatert");
        this.lastHanskeVedIndikasjonType();
      },
      error => this.toastrService.error('Det oppstod en feil under oppdatering av HanskeVedIndikasjonType: ' + error?.error, '', { disableTimeOut: true}),
      () => this.gloveWithIndicationTypeThatChanges = null
    );
  }

  avbrytRedigering($event: Event = null) {
    if($event){
      $event.stopPropagation();
      $event.preventDefault();
    }
    this.gloveWithIndicationTypeThatChanges = null;
  }
}
