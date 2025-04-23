import { Component, OnInit, OnDestroy } from '@angular/core';
import { GloveWithoutIndicationType } from '../../../models/api/GloveWithoutIndicationType';
import { ToastrService } from 'ngx-toastr';
import { GloveWithoutIndicationTypeService } from '../../../services/data/gloveWithoutIndicationType.service';
import { KeyEventService } from '../../../services/events/key-event.service';

@Component({
  selector: 'app-redigering-av-hanskeutenindikasjontyper',
  templateUrl: './redigering-av-hanskeutenindikasjontyper.component.html'
})
export class RedigeringAvHanskeutenindikasjontyperComponent implements OnInit, OnDestroy {

  hanskeUtenIndikasjonTyper: GloveWithoutIndicationType[] = [];
  hanskeUtenIndikasjonTypeSomEndres: GloveWithoutIndicationType = null;

  constructor(
    private hanskeUtenIndikasjonTypeService: GloveWithoutIndicationTypeService,
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
    this.hanskeUtenIndikasjonTypeService.getGloveWithoutIndicationTypes().subscribe(
      (resultat) => this.hanskeUtenIndikasjonTyper = resultat,
      (error) => this.toastrService.error('Det oppstod en feil under lasting av GloveWithoutIndicationType: ' + error?.message, '', { disableTimeOut: true}),
    );
  }

  valgtHanskeUtenIndikasjonType(hanskeUtenIndikasjonType: GloveWithoutIndicationType): void {
    if (this.hanskeUtenIndikasjonTypeSomEndres?.id == hanskeUtenIndikasjonType.id) return;
    this.hanskeUtenIndikasjonTypeSomEndres = JSON.parse(JSON.stringify(hanskeUtenIndikasjonType));
  }

  updateGloveWithoutIndicationType(): void {
    this.hanskeUtenIndikasjonTypeService.updateGloveWithoutIndicationType(this.hanskeUtenIndikasjonTypeSomEndres).subscribe(
      (oppdatertHanskeUtenIndikasjonType) => {
        this.toastrService.success("GloveWithoutIndicationType oppdatert");
        this.lastHanskeUtenIndikasjonType();
      },
      error => this.toastrService.error('Det oppstod en feil under oppdatering av GloveWithoutIndicationType: ' + error?.error, '', { disableTimeOut: true}),
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
